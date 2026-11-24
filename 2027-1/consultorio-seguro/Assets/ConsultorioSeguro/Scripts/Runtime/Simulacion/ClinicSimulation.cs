using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    /// <summary>
    /// Conecta las cinco acciones de simulación con la escena guardada al iniciar el juego:
    /// física de materiales, movimiento de equipos, partículas, efectos de manipulación y animaciones.
    /// Nada se guarda en la escena: los ajustes manuales del editor se conservan.
    /// </summary>
    public sealed class ClinicSimulation : MonoBehaviour
    {
        // Controladores de Resources/Simulacion y sus parámetros.
        public const string PanelController = "Panel", ArmController = "Brazo", ChairController = "Sillon", LampController = "Lampara";
        const int Sterilization = 2, Procedures = 4, Radiography = 5;

        ClinicGame game;
        readonly Dictionary<ClinicDestination, BinFeedback> bins = new Dictionary<ClinicDestination, BinFeedback>();
        readonly Dictionary<PracticeStation, StationVisual> stations = new Dictionary<PracticeStation, StationVisual>();
        readonly Dictionary<int, ClinicFaucet> faucetByRoom = new Dictionary<int, ClinicFaucet>();
        readonly List<ClinicFaucet> faucets = new List<ClinicFaucet>();
        MaterialPropertyBlock block;
        static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        static readonly int LegacyColor = Shader.PropertyToID("_Color");

        public HeldItemHandling Handling { get; private set; }
        public AnimatedPart XrayArm { get; private set; }
        public AnimatedPart DentalChair { get; private set; }
        public AnimatedPart OperatingLamp { get; private set; }
        public ParticleSystem AutoclaveSteam { get; private set; }
        public IReadOnlyList<ClinicFaucet> Faucets => faucets;
        public BinFeedback FeedbackFor(ClinicDestination destination) => destination && bins.TryGetValue(destination, out var bin) ? bin : null;
        public AnimatedPart PanelFor(PracticeStation station) => station && stations.TryGetValue(station, out var visual) ? visual.Panel : null;
        public Renderer IndicatorFor(PracticeStation station) => station && stations.TryGetValue(station, out var visual) ? visual.Indicator : null;
        AudioSource steamSound;

        sealed class StationVisual
        {
            public AnimatedPart Panel;
            public Renderer Indicator;
            public PracticeRoom Room;
        }

        public void Initialize(ClinicGame owner, ClinicItem[] items)
        {
            game = owner;
            block = new MaterialPropertyBlock();
            if (game.player)
            {
                Handling = game.player.HoldPoint.gameObject.AddComponent<HeldItemHandling>();
                Handling.Initialize(game, game.player);
            }
            foreach (var item in items)
            {
                if (!item) continue;
                var physics = item.GetComponent<ItemPhysics>();
                if (!physics) physics = item.gameObject.AddComponent<ItemPhysics>();
                physics.Initialize(game, item, RoomById(item.roomId));
            }
            foreach (var destination in FindObjectsByType<ClinicDestination>(FindObjectsInactive.Include))
            {
                var feedback = destination.gameObject.AddComponent<BinFeedback>();
                feedback.Initialize(game, destination);
                bins[destination] = feedback;
            }
            foreach (var room in game.rooms)
                if (room) foreach (var station in room.steps) if (station) SetUpStation(station, room);
            SetUpFaucets();
            SetUpEquipment();
            game.ItemDeposited += OnDeposited;
            game.RoomCompleted += OnRoomCompleted;
        }

        void OnDestroy()
        {
            if (!game) return;
            game.ItemDeposited -= OnDeposited;
            game.RoomCompleted -= OnRoomCompleted;
        }

        PracticeRoom RoomById(int id) => game.rooms.FirstOrDefault(room => room && room.roomId == id);
        PracticeRoom RoomAt(Vector3 point) => game.rooms.FirstOrDefault(room => room && room.Contains(point));

        static Transform FindIn(Component root, string name) =>
            root ? root.GetComponentsInChildren<Transform>(true).FirstOrDefault(child => child.name == name) : null;

        static Bounds WorldBounds(Transform root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>().Where(r => !(r is ParticleSystemRenderer)).ToArray();
            if (renderers.Length == 0) return new Bounds(root.position, Vector3.zero);
            Bounds bounds = renderers[0].bounds;
            foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
            return bounds;
        }

        // --- Pasos: panel animado, indicador de estado y efectos sincronizados ---

        void SetUpStation(PracticeStation station, PracticeRoom room)
        {
            var visual = new StationVisual { Room = room };
            visual.Panel = AnimatedPart.Wrap(station.name, PanelController, station.transform.position, station.transform.rotation, station.transform);
            var light = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            light.name = "Indicador de paso";
            Destroy(light.GetComponent<Collider>());
            light.transform.SetParent(station.transform, false);
            light.transform.localPosition = new Vector3(.235f, .088f, -.018f);
            light.transform.localScale = Vector3.one * .026f;
            var indicator = light.GetComponent<Renderer>();
            var material = Resources.Load<Material>(SimulationFx.ResourceFolder + "Indicador");
            if (material) indicator.sharedMaterial = material;
            indicator.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            visual.Indicator = indicator;
            stations[station] = visual;
            station.Began += OnStepBegan;
            station.onComplete.AddListener(() => OnStepCompleted(station));
            station.onReset.AddListener(() => OnStepReset(station));
        }

        void OnStepBegan(PracticeStation station)
        {
            switch (station.roomId, station.stepIndex)
            {
                case (Sterilization, 1):
                    if (faucetByRoom.TryGetValue(Sterilization, out var faucet)) faucet.SetRunning(true, 3.5f);
                    break;
                case (Sterilization, 2):
                    if (AutoclaveSteam) AutoclaveSteam.Play();
                    if (steamSound) steamSound.Play();
                    break;
                case (Procedures, 1):
                    if (OperatingLamp) OperatingLamp.SetBool("Encendida", true);
                    break;
                case (Radiography, 1):
                    if (XrayArm) XrayArm.Trigger("Toma");
                    break;
            }
        }

        void OnStepCompleted(PracticeStation station)
        {
            if (stations.TryGetValue(station, out var visual) && visual.Panel) visual.Panel.Trigger("Completar");
            SimulationFx.StepDone(station.transform.TransformPoint(new Vector3(0, 0, -.03f)), station.transform.rotation * Quaternion.Euler(0, 180, 0));
            SimulationFx.Play("Acierto", station.transform.position, .25f);
            switch (station.roomId, station.stepIndex)
            {
                case (Sterilization, 2):
                    StopSteam();
                    break;
                case (Procedures, 0):
                    if (DentalChair) DentalChair.SetBool("Elevado", true);
                    break;
                case (Procedures, 2):
                    if (OperatingLamp) OperatingLamp.SetBool("Encendida", false);
                    if (DentalChair) DentalChair.SetBool("Elevado", false);
                    break;
            }
        }

        void OnStepReset(PracticeStation station)
        {
            if (stations.TryGetValue(station, out var visual) && visual.Panel) visual.Panel.ResetPose();
            if (station.stepIndex != 0) return;
            switch (station.roomId)
            {
                case Sterilization:
                    StopSteam();
                    if (faucetByRoom.TryGetValue(Sterilization, out var faucet)) faucet.SetRunning(false);
                    break;
                case Procedures:
                    if (DentalChair) DentalChair.ResetPose();
                    if (OperatingLamp) OperatingLamp.ResetPose();
                    break;
                case Radiography:
                    if (XrayArm) XrayArm.ResetPose();
                    break;
            }
        }

        void StopSteam()
        {
            if (AutoclaveSteam) AutoclaveSteam.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            if (steamSound) steamSound.Stop();
        }

        // --- Grifos ---

        void SetUpFaucets()
        {
            foreach (var arm in FindObjectsByType<Transform>(FindObjectsInactive.Exclude)
                         .Where(t => t.name == "Grifo horizontal"))
            {
                // El grifo es un cilindro: su eje local Y va del codo a la boca y mide 2·escala.
                Vector3 half = arm.up * arm.lossyScale.y;
                Vector3 spout = arm.position + half;
                var zone = new GameObject("Zona del grifo");
                zone.transform.SetParent(arm.parent, true);
                zone.transform.SetPositionAndRotation(arm.position, Quaternion.LookRotation(arm.up, Vector3.up));
                var collider = zone.AddComponent<BoxCollider>();
                collider.size = new Vector3(.08f, .08f, arm.lossyScale.y * 2 + .04f);
                var faucet = zone.AddComponent<ClinicFaucet>();
                var room = RoomAt(spout + Vector3.up * .3f);
                faucet.roomId = room ? room.roomId : 0;
                faucet.Initialize(spout + Vector3.down * .05f);
                faucets.Add(faucet);
                if (room && !faucetByRoom.ContainsKey(room.roomId)) faucetByRoom[room.roomId] = faucet;
            }
        }

        // --- Equipos con movimiento ---

        void SetUpEquipment()
        {
            var sterilization = RoomById(Sterilization);
            var autoclave = FindIn(sterilization, "autoclave");
            if (autoclave)
            {
                Bounds bounds = WorldBounds(autoclave);
                Vector3 toRoom = sterilization.roomVolume ? sterilization.roomVolume.bounds.center - bounds.center : -autoclave.forward;
                toRoom.y = 0;
                Vector3 vent = new Vector3(bounds.center.x, bounds.max.y + .01f, bounds.center.z) + toRoom.normalized * bounds.extents.z * .5f;
                AutoclaveSteam = SimulationFx.Steam(autoclave, vent);
                steamSound = SimulationFx.Loop("Vapor", AutoclaveSteam.transform, .3f);
            }

            var procedures = RoomById(Procedures);
            var chair = FindIn(procedures, "dental-chair-reclined");
            if (chair)
            {
                var original = chair.parent;
                DentalChair = AnimatedPart.Wrap("Sillón dental", ChairController, chair.position, chair.rotation, chair);
                // El mando de pie y su cable permanecen en el piso al elevar el sillón.
                foreach (var name in new[] { "Mando de pie", "Cable del mando" })
                {
                    var floorPart = FindIn(chair, name);
                    if (floorPart) floorPart.SetParent(original, true);
                }
            }
            var lamp = FindIn(procedures, "operating-light");
            if (lamp)
            {
                Bounds bounds = WorldBounds(lamp);
                Vector3 target = chair ? WorldBounds(chair).center + Vector3.up * .3f : bounds.center + lamp.forward;
                Vector3 head = new Vector3(bounds.center.x, bounds.max.y - .15f, bounds.center.z);
                head = Vector3.Lerp(head, new Vector3(target.x, head.y, target.z), .35f);
                OperatingLamp = AnimatedPart.Create("Lámpara operatoria", LampController, lamp, head,
                    Quaternion.LookRotation(target - head));
                var light = new GameObject("Luz").AddComponent<Light>();
                light.transform.SetParent(OperatingLamp.Axis, false);
                light.type = LightType.Spot;
                light.range = 3.5f;
                light.spotAngle = 42;
                light.innerSpotAngle = 22;
                light.color = new Color(1f, .96f, .9f);
                light.intensity = 0;
                light.shadows = LightShadows.None;
                // El Animator se enlazó antes de existir la luz: se vuelve a enlazar para animar su intensidad.
                OperatingLamp.ResetPose();
            }

            var radiography = RoomById(Radiography);
            var arm = FindIn(radiography, "intraoral-xray-arm");
            if (arm) XrayArm = AnimatedPart.Wrap("Brazo de rayos X", ArmController, arm.position, arm.rotation, arm);
        }

        // --- Retroalimentación de clasificación ---

        void OnDeposited(ClinicItem item, ClinicDestination destination, bool correct, Vector3 handPosition, Quaternion handRotation)
        {
            var feedback = FeedbackFor(destination);
            if (feedback) feedback.OnDeposited(item, correct, handPosition, handRotation);
        }

        void OnRoomCompleted(PracticeRoom room)
        {
            if (!game.player) return;
            var eye = game.player.eye.transform;
            Vector3 forward = eye.forward;
            forward.y = 0;
            Vector3 position = eye.position + (forward.sqrMagnitude > .01f ? forward.normalized : Vector3.forward) * 1.4f + Vector3.down * .5f;
            SimulationFx.Confetti(position);
            SimulationFx.Play("Acierto", position, .8f);
        }

        void Update()
        {
            if (block == null) return;
            float pulse = .65f + .35f * Mathf.Sin(Time.time * 5);
            foreach (var pair in stations)
            {
                var station = pair.Key;
                var visual = pair.Value;
                if (!station || !visual.Indicator) continue;
                Color color;
                if (station.Completed) color = new Color(.32f, .82f, .44f);
                else if (station.Working) color = Color.Lerp(new Color(.5f, .34f, .08f), new Color(1f, .72f, .26f), pulse);
                else if (visual.Room && visual.Room.CurrentStep == station) color = Color.Lerp(new Color(.85f, .6f, .2f), new Color(1f, .78f, .35f), pulse);
                else color = new Color(.55f, .57f, .56f);
                visual.Indicator.GetPropertyBlock(block);
                block.SetColor(BaseColor, color);
                block.SetColor(LegacyColor, color);
                visual.Indicator.SetPropertyBlock(block);
            }
        }
    }
}
