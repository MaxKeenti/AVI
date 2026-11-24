using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace ConsultorioSeguroNuevo.Tests
{
    // Comprueba los modelos de simulación en la escena entregable: que existan, que se articulen
    // sobre las piezas correctas y que se muevan hacia el paciente.
    public sealed class SimulationSceneTests
    {
        ClinicGame game;
        Scene scene;

        [UnitySetUp]
        public IEnumerator LoadClinic()
        {
            Time.timeScale = 1;
            yield return SceneManager.LoadSceneAsync("ConsultorioSeguro", LoadSceneMode.Single);
            yield return null;
            scene = SceneManager.GetActiveScene();
            game = Object.FindAnyObjectByType<ClinicGame>();
            yield return null;
            game.StartVisit();
        }

        [UnityTearDown]
        public IEnumerator UnloadClinic()
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None;
            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.SetActiveScene(SceneManager.CreateScene("Escena vacía de simulación"));
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        PracticeRoom Room(int id) => game.rooms.First(room => room.roomId == id);
        static Transform Find(Component root, string name) =>
            root.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == name);
        static Vector3 Center(Transform root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers.Skip(1)) bounds.Encapsulate(r.bounds);
            return bounds.center;
        }

        void Enter(PracticeRoom room)
        {
            var center = room.roomVolume.bounds.center;
            game.player.Teleport(new Vector3(center.x, .05f, center.z));
            game.RefreshRoom();
            Assert.AreSame(room, game.CurrentRoom);
        }

        [Test]
        public void PatientsLieOnTheProcedureAndRadiographyChairs()
        {
            foreach (int id in new[] { 4, 5 })
            {
                var room = Room(id);
                var head = Find(room, ClinicSimulation.PatientHead);
                Assert.IsNotNull(head, room.title + ": falta el paciente");
                Assert.AreEqual(9, room.GetComponentsInChildren<Transform>(true).Count(t => t.name.StartsWith("Paciente · ")));
                var headrest = Find(room, "Reposacabezas anatómico");
                Vector3 offset = Center(head) - Center(headrest);
                Assert.Less(new Vector2(offset.x, offset.z).magnitude, .35f, room.title + ": la cabeza descansa sobre el reposacabezas");
                Assert.That(offset.y, Is.InRange(-.05f, .35f), room.title + ": la cabeza no atraviesa ni flota sobre el reposacabezas");
            }
        }

        [UnityTest]
        public IEnumerator ChairReclinesWithThePatientAndRises()
        {
            var sim = game.Simulation;
            Assert.IsNotNull(sim.Backrest, "El respaldo se articula sobre chair-back");
            Assert.IsNotNull(sim.Backrest.Find(ClinicSimulation.PatientHead), "La cabeza del paciente acompaña al respaldo");
            Assert.AreEqual(25, sim.Backrest.localEulerAngles.x, 1, "Al iniciar, el paciente está sentado");
            var room = Room(4);
            Enter(room);
            room.steps[0].Interact(game);
            yield return new WaitForSeconds(2.1f);
            Assert.Less(Mathf.DeltaAngle(0, sim.Backrest.localEulerAngles.x), 1, "Preparar el área reclina el sillón");
            Assert.AreEqual(.14f, sim.DentalChair.Axis.localPosition.y, .01f);
        }

        [UnityTest]
        public IEnumerator XrayArmSwingsTowardThePatientWhileItsBaseStaysFixed()
        {
            var room = Room(5);
            var arm = Find(room, "intraoral-xray-arm");
            var basePart = Find(arm, "intraoral-xray-arm_0");
            var collimator = Find(arm, "xray-arm/dark");
            Vector3 head = Center(Find(room, ClinicSimulation.PatientHead));
            Vector3 baseBefore = basePart.position;
            float before = Vector3.Distance(Center(collimator), head);
            game.Simulation.XrayArm.Trigger("Toma");
            yield return new WaitForSeconds(1.1f);
            Assert.AreEqual(baseBefore, basePart.position, "La base del equipo permanece en la columna");
            Assert.Less(Vector3.Distance(Center(collimator), head), before - .1f, "El colimador se acerca a la cabeza del paciente");
        }

        [UnityTest]
        public IEnumerator AutoclaveLoadsTheTrayClosesForTheCycleAndReopens()
        {
            var sim = game.Simulation;
            var room = Room(2);
            Assert.IsNotNull(sim.AutoclaveDoor, "La puerta del autoclave se articula");
            var door = sim.AutoclaveDoor.Axis;
            Vector3 mouth = sim.AutoclaveOpening;
            float FlatDistance(Vector3 a, Vector3 b) => new Vector2(a.x - b.x, a.z - b.z).magnitude;
            float openDistance = FlatDistance(Center(door), mouth);
            Assert.Greater(openDistance, .2f, "Al iniciar, la puerta está abierta a un lado");
            Enter(room);
            room.steps[0].Interact(game);
            room.steps[1].Interact(game);
            room.steps[2].Interact(game);
            Assert.GreaterOrEqual(room.steps[2].duration, ClinicSimulation.AutoclaveCycleSeconds, "El paso dura todo el ciclo");
            yield return new WaitForSeconds(2f);
            Assert.Less(FlatDistance(Center(door), mouth), .12f, "Durante el ciclo la puerta cubre la cámara");
            Assert.Less(FlatDistance(Center(sim.AutoclaveTray), mouth), .2f, "La charola está dentro de la cámara");
            Assert.IsTrue(sim.AutoclaveSteam.isEmitting, "El respiradero libera vapor durante el ciclo");
            Assert.Greater(sim.AutoclaveLight.Value.r, sim.AutoclaveLight.Value.b, "Indicador en ámbar durante el ciclo");
            yield return new WaitForSeconds(2.2f);
            Assert.IsTrue(room.steps[2].Completed);
            Assert.Greater(FlatDistance(Center(door), mouth), openDistance - .05f, "Al terminar, la puerta vuelve a abrirse");
            Assert.Greater(sim.AutoclaveLight.Value.g, sim.AutoclaveLight.Value.r, "Indicador en verde al terminar");
            game.RestartCurrentRoom();
            yield return null;
            Assert.Greater(FlatDistance(Center(sim.AutoclaveTray), mouth), .4f, "Reiniciar devuelve la charola a la encimera");
            Assert.IsNull(sim.AutoclaveLight);
        }

        [UnityTest]
        public IEnumerator PedalBinsOpenTheirRealLidAndSharpsContainersStayClosed()
        {
            var destinations = Object.FindObjectsByType<ClinicDestination>();
            Assert.AreEqual(17, destinations.Length);
            foreach (var destination in destinations)
            {
                var feedback = game.Simulation.FeedbackFor(destination);
                Assert.IsNotNull(destination.transform.Find(BinFeedback.ModelName), destination.name + ": falta el modelo del recipiente");
                if (destination.destination == WasteDestination.Sharps)
                {
                    Assert.IsTrue(feedback.FixedOpening, "El contenedor rígido no tiene tapa abatible");
                    Assert.IsNull(feedback.Lid);
                    continue;
                }
                Assert.IsNotNull(feedback.Lid, destination.name + ": la tapa del bote se articula");
                Assert.IsNotNull(feedback.Pedal, destination.name + ": el pedal se articula");
                Assert.IsNotNull(destination.transform.Find("Bolsa interior"), destination.name + ": falta la bolsa");
                Assert.IsFalse(destination.transform.Find("Tapa").gameObject.activeSelf, "La tapa provisional queda oculta");
            }
            var room = Room(1);
            Enter(room);
            var red = destinations.First(d => d.roomId == 1 && d.destination == WasteDestination.RedBag);
            var item = room.Items.First(i => i.correctDestination == WasteDestination.RedBag);
            var bin = game.Simulation.FeedbackFor(red);
            game.PickUp(item);
            game.Deposit(red);
            yield return new WaitForSeconds(.4f);
            Assert.IsTrue(bin.LidOpen);
            Assert.Greater(bin.Lid.Axis.localEulerAngles.x, 60, "La tapa gira sobre la bisagra del modelo");
            Assert.Less(Mathf.DeltaAngle(0, bin.Pedal.localEulerAngles.x), -10, "El pedal se hunde al abrir");
            yield return new WaitForSeconds(1.2f);
            Assert.IsFalse(bin.LidOpen);
            Assert.AreEqual(0, Mathf.DeltaAngle(0, bin.Pedal.localEulerAngles.x), .5f);
        }

        [UnityTest]
        public IEnumerator OperatingLampSwingsOverThePatientAndLightsTheMouth()
        {
            var room = Room(4);
            var lens = Find(room, "light-head-lens");
            Vector3 head = Center(Find(room, ClinicSimulation.PatientHead));
            Enter(room);
            room.steps[0].Interact(game);
            room.steps[1].Interact(game);
            yield return new WaitForSeconds(2.2f);
            Vector3 lensPosition = Center(lens);
            Assert.Less(new Vector2(lensPosition.x - head.x, lensPosition.z - head.z).magnitude, .45f, "El cabezal queda sobre la cabeza");
            var light = game.Simulation.OperatingLamp.GetComponentInChildren<Light>();
            Assert.Greater(light.intensity, 2.5f);
            Vector3 toHead = (Center(Find(room, ClinicSimulation.PatientHead)) - light.transform.position).normalized;
            Assert.Less(Vector3.Angle(light.transform.forward, toHead), light.spotAngle * .5f, "El haz alcanza la cabeza del paciente");
        }
    }
}
