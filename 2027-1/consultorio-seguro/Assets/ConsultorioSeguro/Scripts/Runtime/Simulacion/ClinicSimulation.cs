using System.Collections;
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
        // Grados que recorren los brazos en sus clips y nombre de la articulación del respaldo.
        public const float ArmSwing = 40, LampSwing = 40;
        public const string BackrestJoint = "Respaldo", PatientHead = "Paciente · cabeza";
        // Ciclo del autoclave: la puerta cierra a 0.9-1.5 s, el ciclo dura hasta 3.0 s y abre a 3.0-3.6 s.
        public const string AutoclaveController = "Autoclave", AutoclaveDoorName = "Puerta del autoclave",
            AutoclaveMouthName = "Boca del autoclave";
        public const float AutoclaveCycleSeconds = 3.7f, AutoclaveClosed = 1.5f, AutoclaveOpens = 3f;
        static readonly string[] PatientUpperBody =
            { "Paciente · torso", "Paciente · cuello", PatientHead, "Paciente · brazo derecho", "Paciente · brazo izquierdo" };
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
        public Transform Backrest { get; private set; }
        public AnimatedPart AutoclaveDoor { get; private set; }
        public const string ForcepsName = "Pinzas de recuperación";
        public GlovedHand Hand { get; private set; }
        public Transform Forceps { get; private set; }
        public IReadOnlyList<ClinicGloveDispenser> Dispensers => dispensers;
        readonly List<ClinicGloveDispenser> dispensers = new List<ClinicGloveDispenser>();
        ClinicItem gripped;
        public Transform AutoclaveTray { get; private set; }
        public Vector3 AutoclaveOpening { get; private set; }
        public bool AutoclaveRunning { get; private set; }
        Renderer autoclaveBody;
        int[] autoclaveLightSlots = new int[0];
        Vector3 autoclaveOutward, trayPosition, trayScale;
        Quaternion trayRotation;
        Transform trayParent;
        Coroutine autoclaveCycle;
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
                Hand = GlovedHand.Build(game.player.HoldPoint);
                Forceps = game.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == ForcepsName);
                if (Forceps) { Forceps.SetParent(game.player.HoldPoint, false); Forceps.gameObject.SetActive(false); }
            }
            SetUpDispensers();
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
                    if (autoclaveCycle != null) StopCoroutine(autoclaveCycle);
                    autoclaveCycle = StartCoroutine(RunAutoclave());
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
                    ResetAutoclave();
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

        // --- Protección: dispensadores, mano enguantada y pinzas ---

        void SetUpDispensers()
        {
            foreach (var model in FindObjectsByType<Transform>(FindObjectsInactive.Exclude)
                         .Where(t => t.name == "glove-and-mask-dispenser" && (!t.parent || t.parent.name != t.name)))
            {
                if (model.GetComponent<ClinicGloveDispenser>()) continue;
                Bounds bounds = WorldBounds(model);
                var room = RoomAt(bounds.center);
                var dispenser = model.gameObject.AddComponent<ClinicGloveDispenser>();
                dispenser.roomId = room ? room.roomId : 0;
                var collider = model.gameObject.AddComponent<BoxCollider>();
                collider.center = model.InverseTransformPoint(bounds.center);
                Vector3 size = model.InverseTransformVector(bounds.size);
                collider.size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
                dispensers.Add(dispenser);
            }
        }

        void UpdateGrip()
        {
            var held = game.HeldItem;
            if (held == gripped) return;
            gripped = held;
            if (!held)
            {
                if (Hand) Hand.Release();
                if (Forceps) Forceps.gameObject.SetActive(false);
                return;
            }
            // Límites del objeto en el espacio del punto de sujeción.
            var point = game.player.HoldPoint;
            Bounds local = new Bounds(Vector3.zero, Vector3.one * .02f);
            foreach (var renderer in held.GetComponentsInChildren<Renderer>())
            {
                if (renderer.name == "Silueta de interacción") continue;
                var b = renderer.bounds;
                for (int c = 0; c < 8; c++)
                    local.Encapsulate(point.InverseTransformPoint(b.center + Vector3.Scale(b.extents,
                        new Vector3((c & 1) == 0 ? -1 : 1, (c & 2) == 0 ? -1 : 1, (c & 4) == 0 ? -1 : 1))));
            }
            bool forceps = game.HeldWithForceps && Forceps;
            if (Hand) Hand.Grip(local, forceps);
            if (Forceps)
            {
                Forceps.gameObject.SetActive(forceps);
                Forceps.localPosition = new Vector3(0, -.01f, -.01f);
                Forceps.localRotation = Quaternion.identity;
            }
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
                SetUpAutoclave(sterilization, autoclave, toRoom);
            }

            var procedures = RoomById(Procedures);
            var chair = FindIn(procedures, "dental-chair-reclined");
            // Objetivo de la lámpara: la boca del paciente con el sillón reclinado (pose guardada en la escena).
            var procedureHead = FindIn(procedures, PatientHead);
            Vector3? mouth = procedureHead ? WorldBounds(procedureHead).center + Vector3.up * .07f : (Vector3?)null;
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
                SetUpBackrest(chair);
            }
            var lamp = FindIn(procedures, "operating-light");
            var lampArm = FindIn(lamp, "light-arm");
            var lens = FindIn(lamp, "light-head-lens");
            if (lampArm && lens && mouth.HasValue)
            {
                // El brazo de la lámpara gira sobre su eje y se coloca sobre la boca del paciente.
                Vector3 lensPosition = WorldBounds(lens).center;
                OperatingLamp = AnimatedPart.SwingToward("Lámpara operatoria", LampController, lampArm, lensPosition, mouth.Value,
                    LampSwing, out float swing);
                Quaternion turn = Quaternion.AngleAxis(swing, Vector3.up);
                Vector3 pivot = OperatingLamp.transform.position;
                Vector3 parked = WorldBounds(lens).center + Vector3.down * .03f;
                Vector3 working = pivot + turn * (parked - pivot);
                // La luz se orienta como quedará al terminar el giro.
                AddLampLight(OperatingLamp, parked, Quaternion.Inverse(turn) * Quaternion.LookRotation(mouth.Value - working));
            }
            else if (lamp)
            {
                Bounds bounds = WorldBounds(lamp);
                Vector3 target = mouth ?? (chair ? WorldBounds(chair).center + Vector3.up * .3f : bounds.center + lamp.forward);
                Vector3 head = new Vector3(bounds.center.x, bounds.max.y - .15f, bounds.center.z);
                OperatingLamp = AnimatedPart.Create("Lámpara operatoria", LampController, lamp, head, Quaternion.identity);
                AddLampLight(OperatingLamp, head, Quaternion.LookRotation(target - head));
            }

            var radiography = RoomById(Radiography);
            var arm = FindIn(radiography, "intraoral-xray-arm");
            var articulated = FindIn(arm, "xray-arm");
            var radiographyHead = FindIn(radiography, PatientHead);
            if (articulated && radiographyHead)
            {
                // Solo el brazo articulado gira; la base queda fija en la columna.
                var collimator = FindIn(articulated, "xray-arm/dark");
                Vector3 tip = collimator ? WorldBounds(collimator).center : WorldBounds(articulated).center;
                XrayArm = AnimatedPart.SwingToward("Brazo de rayos X", ArmController, articulated, tip,
                    WorldBounds(radiographyHead).center, ArmSwing, out _);
            }
            else if (arm) XrayArm = AnimatedPart.Wrap("Brazo de rayos X", ArmController, arm.position, arm.rotation, arm);
        }

        // --- Autoclave: puerta articulada, charola que entra a la cámara y ciclo con indicadores ---

        void SetUpAutoclave(PracticeRoom room, Transform autoclave, Vector3 toRoom)
        {
            var door = FindIn(autoclave, AutoclaveDoorName);
            var mouth = FindIn(autoclave, AutoclaveMouthName);
            autoclaveOutward = toRoom.sqrMagnitude > 0 ? toRoom.normalized : -autoclave.forward;
            if (door && mouth)
            {
                // La puerta abierta es una lámina perpendicular al frente: la bisagra está en su extremo más cercano a la cámara.
                Bounds bounds = WorldBounds(door);
                AutoclaveOpening = mouth.position;
                Vector3 axis = bounds.size.x > bounds.size.z ? Vector3.right : Vector3.forward;
                float reach = Vector3.Dot(bounds.extents, axis);
                Vector3 a = bounds.center - axis * reach, b = bounds.center + axis * reach;
                Vector3 hinge = FlatDistance(a, AutoclaveOpening) < FlatDistance(b, AutoclaveOpening) ? a : b;
                AutoclaveDoor = AnimatedPart.SwingToward("Puerta del autoclave", AutoclaveController, hinge, new[] { door },
                    bounds.center, AutoclaveOpening, 90, out _);
            }
            // Indicadores del panel: submallas con los materiales 14 y 15 del modelo.
            autoclaveBody = autoclave.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(r => r.name != AutoclaveDoorName
                && r.sharedMaterials.Length > 1);
            if (autoclaveBody)
                autoclaveLightSlots = Enumerable.Range(0, autoclaveBody.sharedMaterials.Length)
                    .Where(i => autoclaveBody.sharedMaterials[i] && (autoclaveBody.sharedMaterials[i].name.EndsWith("_14")
                        || autoclaveBody.sharedMaterials[i].name.EndsWith("_15"))).ToArray();
            AutoclaveTray = FindIn(room, "instrument-tray");
            if (AutoclaveTray)
            {
                trayParent = AutoclaveTray.parent;
                trayPosition = AutoclaveTray.localPosition;
                trayRotation = AutoclaveTray.localRotation;
                trayScale = AutoclaveTray.localScale;
            }
            // El paso dura lo mismo que el ciclo representado.
            var step = room.steps.FirstOrDefault(candidate => candidate && candidate.stepIndex == 2);
            if (step) step.duration = Mathf.Max(step.duration, AutoclaveCycleSeconds);
        }

        static float FlatDistance(Vector3 a, Vector3 b) => new Vector2(a.x - b.x, a.z - b.z).magnitude;

        IEnumerator RunAutoclave()
        {
            AutoclaveRunning = true;
            if (AutoclaveDoor) AutoclaveDoor.Trigger("Ciclo");
            if (AutoclaveTray && AutoclaveDoor)
            {
                // La charola entra a la cámara antes de que cierre la puerta.
                Vector3 start = AutoclaveTray.position;
                Quaternion startRotation = AutoclaveTray.rotation;
                Quaternion inward = Quaternion.LookRotation(-autoclaveOutward, Vector3.up);
                Bounds tray = WorldBounds(AutoclaveTray);
                float drop = tray.center.y - AutoclaveTray.position.y;
                Vector3 front = AutoclaveOpening + autoclaveOutward * .32f + Vector3.down * drop;
                Vector3 inside = AutoclaveOpening - autoclaveOutward * .05f + Vector3.down * drop;
                for (float t = 0; t < .5f; t += Time.deltaTime)
                {
                    float k = Mathf.SmoothStep(0, 1, t / .5f);
                    AutoclaveTray.SetPositionAndRotation(Vector3.Lerp(start, front, k) + Vector3.up * Mathf.Sin(k * Mathf.PI) * .08f,
                        Quaternion.Slerp(startRotation, inward, k));
                    yield return null;
                }
                for (float t = 0; t < .35f; t += Time.deltaTime)
                {
                    AutoclaveTray.position = Vector3.Lerp(front, inside, Mathf.SmoothStep(0, 1, t / .35f));
                    yield return null;
                }
                AutoclaveTray.SetPositionAndRotation(inside, inward);
            }
            yield return new WaitForSeconds(Mathf.Max(0, AutoclaveClosed - .85f));
            // Ciclo con la puerta cerrada: vapor por el respiradero e indicadores en ámbar.
            SetAutoclaveLights(new Color(1f, .62f, .12f));
            if (AutoclaveSteam) AutoclaveSteam.Play();
            if (steamSound) steamSound.Play();
            yield return new WaitForSeconds(AutoclaveOpens - AutoclaveClosed);
            StopSteam();
            SetAutoclaveLights(new Color(.3f, .86f, .42f));
            yield return new WaitForSeconds(.08f);
            SimulationFx.SteamBurst(AutoclaveOpening + autoclaveOutward * .1f, autoclaveOutward);
            SimulationFx.Play("Vapor", AutoclaveOpening, .45f);
            AutoclaveRunning = false;
            autoclaveCycle = null;
        }

        public Color? AutoclaveLight { get; private set; }

        void SetAutoclaveLights(Color? color)
        {
            AutoclaveLight = color;
            if (!autoclaveBody) return;
            foreach (int slot in autoclaveLightSlots)
            {
                block.Clear();
                if (color.HasValue)
                {
                    block.SetColor(BaseColor, color.Value);
                    block.SetColor(LegacyColor, color.Value);
                }
                autoclaveBody.SetPropertyBlock(block, slot);
            }
            block.Clear();
        }

        void ResetAutoclave()
        {
            if (autoclaveCycle != null) StopCoroutine(autoclaveCycle);
            autoclaveCycle = null;
            AutoclaveRunning = false;
            StopSteam();
            if (AutoclaveDoor) AutoclaveDoor.ResetPose();
            SetAutoclaveLights(null);
            if (AutoclaveTray)
            {
                AutoclaveTray.SetParent(trayParent, false);
                AutoclaveTray.localPosition = trayPosition;
                AutoclaveTray.localRotation = trayRotation;
                AutoclaveTray.localScale = trayScale;
            }
        }

        // El respaldo, el reposacabezas y la parte superior del paciente giran juntos sobre la articulación del sillón.
        void SetUpBackrest(Transform chair)
        {
            var back = FindIn(chair, "chair-back");
            if (!back || !DentalChair) return;
            var parts = new[] { "Respaldo lumbar", "Reposacabezas anatómico" }.Concat(PatientUpperBody)
                .Select(name => FindIn(chair, name)).Where(part => part).Prepend(back).ToArray();
            var headrest = FindIn(chair, "Reposacabezas anatómico");
            Vector3 toHead = (headrest ? WorldBounds(headrest).center : WorldBounds(back).center) - back.position;
            toHead.y = 0;
            // El eje local -Z apunta a la cabeza: el giro positivo en X levanta el respaldo.
            Backrest = AnimatedPart.Nest(DentalChair.Axis, BackrestJoint, back.position,
                Quaternion.LookRotation(-toHead.normalized, Vector3.up), parts);
            // El Animator se enlazó antes de existir la articulación: se vuelve a enlazar.
            DentalChair.ResetPose();
        }

        static void AddLampLight(AnimatedPart lamp, Vector3 position, Quaternion rotation)
        {
            var light = new GameObject("Luz").AddComponent<Light>();
            light.transform.SetParent(lamp.Axis, false);
            light.transform.SetPositionAndRotation(position, rotation);
            light.type = LightType.Spot;
            light.range = 3.5f;
            light.spotAngle = 38;
            light.innerSpotAngle = 20;
            light.color = new Color(1f, .96f, .9f);
            light.intensity = 0;
            light.shadows = LightShadows.None;
            // El Animator se enlazó antes de existir la luz: se vuelve a enlazar para animar su intensidad.
            lamp.ResetPose();
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

        void LateUpdate()
        {
            if (game) UpdateGrip();
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
