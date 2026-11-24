using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ConsultorioSeguroNuevo.Tests
{
    // Comprueba las cinco acciones de simulación en una sala mínima con los mismos nombres de la escena:
    // física, movimiento, partículas, efectos de manipulación y animaciones con Animator.
    public sealed class SimulationActionsTests
    {
        GameObject root;
        ClinicGame game;
        ClinicPlayer player;
        PracticeRoom room, radiography;
        ClinicItem first, second;
        ClinicDestination correctBin, wrongBin;
        PracticeStation[] steps;
        Transform chair;

        [UnitySetUp]
        public IEnumerator Prepare()
        {
            root = new GameObject("Prueba de simulación");
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Piso";
            floor.transform.SetParent(root.transform);
            floor.transform.position = new Vector3(0, -.2f, 0);
            floor.transform.localScale = new Vector3(60, .4f, 30);

            var visitor = Child("Visitante", root.transform);
            var controller = visitor.AddComponent<CharacterController>();
            controller.height = 1.8f; controller.center = Vector3.up * .9f; controller.radius = .27f;
            player = visitor.AddComponent<ClinicPlayer>();

            room = Room(4, "Procedimientos", Vector3.zero);
            radiography = Room(5, "Radiografía", new Vector3(20, 0, 0));
            steps = Enumerable.Range(0, 3).Select(index =>
            {
                var step = Child("PASO " + (index + 1), room.transform).AddComponent<PracticeStation>();
                step.transform.position = new Vector3(-2.5f + index * .7f, 1.2f, 2.5f);
                step.roomId = room.roomId; step.stepIndex = index; step.actionText = "Paso " + (index + 1);
                return step;
            }).ToArray();
            chair = Box("dental-chair-reclined", room.transform, new Vector3(2.5f, .5f, 2.5f), new Vector3(.6f, 1, 1.6f)).transform;
            Box("operating-light", room.transform, new Vector3(2.2f, 1, 3.2f), new Vector3(.2f, 2, .2f));
            Box("intraoral-xray-arm", radiography.transform, new Vector3(20, 1.4f, 2), new Vector3(.4f, .2f, .4f));
            var sink = Child("Gabinete con lavabo", room.transform);
            sink.transform.position = new Vector3(-3, 1.19f, -2.5f);
            var spout = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spout.name = "Grifo horizontal";
            Object.Destroy(spout.GetComponent<Collider>());
            spout.transform.SetParent(sink.transform, false);
            spout.transform.localScale = new Vector3(.044f, .1f, .044f);
            spout.transform.up = Vector3.forward;

            first = Item("Gasa saturada de sangre", new Vector3(-.2f, 1.1f, 1), WasteDestination.RedBag);
            second = Item("Vaso sin sangre visible", new Vector3(.3f, 1.1f, 1), WasteDestination.General);
            // El recipiente correcto queda justo delante de los ojos: la cruz apunta a su cuerpo.
            correctBin = Bin("BOLSA ROJA", WasteDestination.RedBag, new Vector3(0, 1.2f, 1.6f));
            wrongBin = Bin("COMUNES", WasteDestination.General, new Vector3(2, 0, 1.6f));

            game = Child("Gestión", root.transform).AddComponent<ClinicGame>();
            game.player = player;
            game.requireGloves = false;
            game.rooms = new[] { room, radiography };
            // Los colisionadores se movieron tras crearse: se sincronizan antes de los raycasts.
            Physics.SyncTransforms();
            yield return null;
            game.StartVisit();
            game.RefreshRoom();
        }

        [UnityTearDown]
        public IEnumerator Clean()
        {
            Object.Destroy(root);
            foreach (var proxy in Object.FindObjectsByType<DepositProxy>()) Object.Destroy(proxy.gameObject);
            yield return null;
            Time.timeScale = 1;
        }

        // --- 1. Física ---

        [UnityTest]
        public IEnumerator Physics_DroppedItemFallsUnderGravityAndCanBeTakenAgain()
        {
            CompleteSteps();
            game.PickUp(first);
            yield return null;
            float handHeight = first.transform.position.y;
            game.DropHeld(0);
            Assert.IsNull(game.HeldItem);
            Assert.IsTrue(first.Loose);
            Assert.IsFalse(first.Body.isKinematic, "Al soltarlo, PhysX controla el objeto.");
            Assert.IsTrue(first.GetComponent<Collider>().enabled);
            yield return new WaitForSeconds(1.2f);
            Assert.Less(first.transform.position.y, handHeight - .8f, "El objeto debe caer por gravedad.");
            Assert.Greater(first.transform.position.y, -.05f, "El piso debe detener el objeto.");
            Assert.IsTrue(first.CanInteract(game), "El material suelto se puede tomar de nuevo.");
            game.PickUp(first);
            Assert.AreSame(first, game.HeldItem);
            Assert.IsTrue(first.Body.isKinematic);
            game.ReturnHeld();
            Assert.AreEqual(new Vector3(-.2f, 1.1f, 1), first.transform.position);
        }

        [UnityTest]
        public IEnumerator Physics_ThrownItemLeavingTheRoomReturnsToItsPlace()
        {
            CompleteSteps();
            game.PickUp(second);
            // Lanza hacia +X, lejos del recipiente: la parábola termina fuera del volumen de la sala.
            player.eye.transform.rotation = Quaternion.Euler(-30, 90, 0);
            game.DropHeld(1);
            Assert.IsTrue(second.Loose);
            float deadline = Time.time + 3;
            while (second.Loose && Time.time < deadline) yield return null;
            Assert.IsFalse(second.Loose, "El lanzamiento sale de la sala y el material regresa solo.");
            Assert.AreEqual(new Vector3(.3f, 1.1f, 1), second.transform.position);
            Assert.IsTrue(second.Body.isKinematic);
        }

        [UnityTest]
        public IEnumerator Physics_RestartingThePracticeCollectsLooseItems()
        {
            CompleteSteps();
            game.PickUp(first);
            game.DropHeld(0);
            yield return new WaitForSeconds(.3f);
            game.RestartCurrentRoom();
            Assert.IsFalse(first.Loose);
            Assert.IsTrue(first.Body.isKinematic);
            Assert.AreEqual(new Vector3(-.2f, 1.1f, 1), first.transform.position);
        }

        // --- 2. Movimiento ---

        [UnityTest]
        public IEnumerator Movement_CrouchLowersTheEyesSmoothlyAndStandsUpAgain()
        {
            float standing = player.eye.transform.localPosition.y;
            player.crouchRequested = true;
            yield return null;
            Assert.Greater(player.eye.transform.localPosition.y, player.crouchEyeHeight, "Agacharse es gradual.");
            yield return new WaitForSeconds(.5f);
            Assert.IsTrue(player.Crouching);
            Assert.AreEqual(player.crouchEyeHeight, player.eye.transform.localPosition.y, .01f);
            player.crouchRequested = false;
            yield return new WaitForSeconds(.5f);
            Assert.AreEqual(standing, player.eye.transform.localPosition.y, .01f);
        }

        [UnityTest]
        public IEnumerator Movement_DentalChairRisesAndLowersWithTheProcedureSteps()
        {
            var lift = game.Simulation.DentalChair;
            Assert.IsNotNull(lift);
            Assert.AreSame(lift.Axis, chair.parent, "El sillón cuelga del eje animado.");
            float floorHeight = chair.position.y;
            steps[0].Interact(game);
            yield return new WaitForSeconds(1.7f);
            Assert.AreEqual(floorHeight + .14f, chair.position.y, .01f, "El sillón sube a altura de trabajo.");
            steps[1].Interact(game);
            steps[2].Interact(game);
            yield return new WaitForSeconds(1.7f);
            Assert.AreEqual(floorHeight, chair.position.y, .01f, "El sillón baja al terminar.");
        }

        [UnityTest]
        public IEnumerator Movement_XrayArmSwingsDuringTheShotAndReturns()
        {
            var arm = game.Simulation.XrayArm;
            Assert.IsNotNull(arm);
            arm.Trigger("Toma");
            yield return new WaitForSeconds(1.1f);
            Assert.AreEqual(ClinicSimulation.ArmSwing, arm.Axis.localEulerAngles.y, 2, "El brazo gira hacia el paciente.");
            yield return new WaitForSeconds(1.5f);
            Assert.Less(Quaternion.Angle(Quaternion.identity, arm.Axis.localRotation), 1, "El brazo vuelve al reposo.");
        }

        // --- 3. Partículas ---

        [UnityTest]
        public IEnumerator Particles_FaucetStreamsWaterAndClosesAgain()
        {
            var faucet = game.Simulation.Faucets.Single(candidate => candidate.transform.IsChildOf(root.transform));
            Assert.AreEqual(room.roomId, faucet.roomId);
            faucet.Interact(game);
            Assert.IsTrue(faucet.Running);
            yield return new WaitForSeconds(.3f);
            Assert.Greater(faucet.Stream.particleCount, 10, "El chorro de agua debe emitir partículas.");
            faucet.Interact(game);
            Assert.IsFalse(faucet.Running);
            yield return new WaitForSeconds(.8f);
            Assert.AreEqual(0, faucet.Stream.particleCount, "Al cerrar, el agua termina de caer.");
        }

        [UnityTest]
        public IEnumerator Particles_ClassificationAndCompletionEmitBursts()
        {
            CompleteSteps();
            game.PickUp(first);
            game.Deposit(correctBin);
            yield return new WaitForSeconds(BinFeedback.FlightTime + .1f);
            Assert.IsTrue(LiveBurst("Partículas · Acierto"), "Un acierto produce destellos en la boca del recipiente.");
            game.PickUp(second);
            game.Deposit(correctBin);
            yield return new WaitForSeconds(BinFeedback.FlightTime * .8f + .1f);
            Assert.IsTrue(LiveBurst("Partículas · Clasificación revisada"), "Un error produce una nube roja.");
            game.PickUp(second);
            game.Deposit(wrongBin);
            Assert.IsTrue(room.Completed);
            yield return null;
            Assert.IsTrue(LiveBurst("Partículas · Práctica completada"), "Completar la práctica lanza confeti.");
        }

        // --- 4. Efectos de manipulación ---

        [UnityTest]
        public IEnumerator Manipulation_LidOpensWhileAimingWithAnItemAndClosesAfter()
        {
            CompleteSteps();
            var feedback = game.Simulation.FeedbackFor(correctBin);
            Assert.IsNotNull(feedback.Lid, "La tapa se envuelve con su Animator.");
            player.eye.transform.rotation = Quaternion.identity;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.AreSame(correctBin, game.Target);
            Assert.IsFalse(feedback.LidOpen, "Sin material en mano, la tapa permanece cerrada.");
            game.PickUp(first);
            yield return new WaitForSeconds(.6f);
            Assert.IsTrue(feedback.LidOpen);
            Assert.Greater(feedback.Lid.Axis.localEulerAngles.x, 60, "La tapa gira sobre su bisagra posterior.");
            game.ReturnHeld();
            yield return new WaitForSeconds(.6f);
            Assert.IsFalse(feedback.LidOpen);
            Assert.Less(Quaternion.Angle(Quaternion.identity, feedback.Lid.Axis.localRotation), 2);
        }

        [UnityTest]
        public IEnumerator Manipulation_ItemTravelsToTheHandAndCanBeInspected()
        {
            CompleteSteps();
            var hold = player.HoldPoint;
            Vector3 rest = hold.localPosition;
            game.PickUp(first);
            yield return null;
            Assert.Greater(Vector3.Distance(hold.localPosition, rest), .1f, "El objeto parte de la mesa hacia la mano.");
            yield return new WaitForSeconds(.8f);
            Assert.Less(Vector3.Distance(hold.localPosition, rest), .02f, "El resorte lo lleva a la mano.");
            game.Simulation.Handling.Rotate(90, 0);
            yield return null;
            Assert.AreEqual(90, Quaternion.Angle(Quaternion.identity, hold.localRotation), 3, "La inspección gira el objeto.");
            game.ReturnHeld();
            game.PickUp(first);
            Assert.AreEqual(0, Quaternion.Angle(Quaternion.identity, game.Simulation.Handling.InspectRotation), .01f,
                "Cada vez que se toma, el objeto empieza sin girar.");
        }

        [UnityTest]
        public IEnumerator Manipulation_DepositFliesIntoTheBinWithoutChangingTheScore()
        {
            CompleteSteps();
            game.PickUp(first);
            game.Deposit(correctBin);
            Assert.IsTrue(first.Completed, "La puntuación se registra al instante.");
            Assert.AreEqual(1, room.Correct);
            var proxy = Object.FindObjectsByType<DepositProxy>().Single();
            Assert.IsNull(proxy.GetComponentInChildren<Collider>(), "La copia visual no participa en colisiones.");
            Assert.IsNull(proxy.GetComponentInChildren<ClinicInteractable>());
            Vector3 mouth = game.Simulation.FeedbackFor(correctBin).Mouth;
            yield return new WaitForSeconds(BinFeedback.FlightTime - .05f);
            Assert.Less(Vector3.Distance(proxy.transform.position, mouth), .15f, "La trayectoria balística llega a la boca.");
            yield return new WaitForSeconds(.4f);
            Assert.IsTrue(proxy == null, "La copia desaparece dentro del recipiente.");
        }

        // --- 5. Animaciones ---

        [Test]
        public void Animations_ControllersAreGeneratedWithTheirParameters()
        {
            AssertController(BinFeedback.LidController, "Abierta", AnimatorControllerParameterType.Bool, 3);
            AssertController(ClinicSimulation.ChairController, "Elevado", AnimatorControllerParameterType.Bool, 3);
            AssertController(ClinicSimulation.LampController, "Encendida", AnimatorControllerParameterType.Bool, 3);
            AssertController(ClinicSimulation.ArmController, "Toma", AnimatorControllerParameterType.Trigger, 2);
            AssertController(ClinicSimulation.PanelController, "Completar", AnimatorControllerParameterType.Trigger, 2);
        }

        [UnityTest]
        public IEnumerator Animations_StepPanelFlipsAndItsIndicatorTurnsGreen()
        {
            var panel = game.Simulation.PanelFor(steps[0]);
            var indicator = game.Simulation.IndicatorFor(steps[0]);
            Assert.IsNotNull(panel.Animator);
            steps[0].Interact(game);
            yield return new WaitForSeconds(.45f);
            Assert.IsTrue(panel.Animator.GetCurrentAnimatorStateInfo(0).IsName("Completado"));
            Assert.IsTrue(game.Simulation.CompletedBackFor(steps[0]).activeSelf, "El reverso muestra «COMPLETADO»");
            Assert.Greater(Quaternion.Angle(Quaternion.identity, panel.Axis.localRotation), 20, "El panel gira al completarse.");
            var block = new MaterialPropertyBlock();
            indicator.GetPropertyBlock(block);
            Color color = block.GetColor("_BaseColor");
            Assert.Greater(color.g, color.r + .3f, "El indicador del paso completado es verde.");
            yield return new WaitForSeconds(.8f);
            Assert.AreEqual(180, Quaternion.Angle(Quaternion.identity, panel.Axis.localRotation), 1, "Queda volteado hasta reiniciar");
            game.RestartCurrentRoom();
            yield return null;
            Assert.Less(Quaternion.Angle(Quaternion.identity, panel.Axis.localRotation), 1);
            Assert.IsFalse(game.Simulation.CompletedBackFor(steps[0]).activeSelf);
        }

        [UnityTest]
        public IEnumerator Animations_OperatingLampLightsUpDuringTheExamination()
        {
            var lamp = game.Simulation.OperatingLamp;
            var light = lamp.Axis.GetComponentInChildren<Light>();
            Assert.AreEqual(0, light.intensity);
            steps[0].Interact(game);
            steps[1].Interact(game);
            yield return new WaitForSeconds(1.8f);
            Assert.AreEqual(2.8f, light.intensity, .05f, "La lámpara se enciende con la exploración.");
            Assert.AreEqual(ClinicSimulation.LampSwing, lamp.Axis.localEulerAngles.y, 2, "El brazo de la lámpara se coloca sobre el paciente.");
            game.RestartCurrentRoom();
            yield return null;
            Assert.AreEqual(0, light.intensity, .01f, "Reiniciar apaga la lámpara.");
        }

        // --- Utilidades ---

        void CompleteSteps()
        {
            foreach (var step in steps) step.Interact(game);
            Assert.IsTrue(room.StepsComplete);
        }

        static bool LiveBurst(string name) => Object.FindObjectsByType<ParticleSystem>()
            .Any(system => system.name == name && system.particleCount > 0);

        static void AssertController(string name, string parameter, AnimatorControllerParameterType type, int clips)
        {
            var controller = AnimatedPart.LoadController(name);
            Assert.IsNotNull(controller, "Falta Resources/Simulacion/" + name + ".controller");
            Assert.AreEqual(clips, controller.animationClips.Distinct().Count(), name);
            var probe = new GameObject("Sonda " + name).AddComponent<Animator>();
            try
            {
                probe.runtimeAnimatorController = controller;
                Assert.IsTrue(probe.parameters.Any(p => p.name == parameter && p.type == type), name + " · " + parameter);
            }
            finally { Object.DestroyImmediate(probe.gameObject); }
        }

        GameObject Child(string name, Transform parent)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent);
            return child;
        }

        GameObject Box(string name, Transform parent, Vector3 position, Vector3 size)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(parent);
            box.transform.position = position;
            box.transform.localScale = size;
            return box;
        }

        PracticeRoom Room(int id, string title, Vector3 center)
        {
            var roomObject = Child("Sala " + title, root.transform);
            roomObject.transform.position = center;
            var practice = roomObject.AddComponent<PracticeRoom>();
            practice.roomId = id; practice.title = title; practice.instructions = "Sala de prueba.";
            var volume = roomObject.AddComponent<BoxCollider>();
            volume.isTrigger = true; volume.center = new Vector3(0, 1.5f, 0); volume.size = new Vector3(8, 3, 8);
            practice.roomVolume = volume;
            return practice;
        }

        ClinicItem Item(string name, Vector3 position, WasteDestination destination)
        {
            var itemObject = Box(name, root.transform, position, Vector3.one * .12f);
            var item = itemObject.AddComponent<ClinicItem>();
            item.roomId = room.roomId; item.displayName = name; item.correctDestination = destination;
            item.description = "Caso de prueba.";
            return item;
        }

        ClinicDestination Bin(string label, WasteDestination destination, Vector3 position)
        {
            var bin = Child(label, room.transform);
            bin.transform.position = position;
            Box("Cuerpo", bin.transform, position + new Vector3(0, .33f, 0), new Vector3(.49f, .64f, .42f));
            Box("Tapa", bin.transform, position + new Vector3(0, .67f, 0), new Vector3(.515f, .065f, .445f));
            var target = bin.AddComponent<ClinicDestination>();
            target.roomId = room.roomId; target.destination = destination;
            return target;
        }
    }
}
