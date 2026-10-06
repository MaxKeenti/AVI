using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace ConsultorioSeguroNuevo.Tests
{
    public sealed class RuntimeInteractionTests
    {
        GameObject root;
        ClinicGame game;
        ClinicPlayer player;
        PracticeRoom room;
        ClinicItem first, second;
        ClinicDestination wrongBin, correctBin;

        [UnitySetUp]
        public IEnumerator Prepare()
        {
            root = new GameObject("Prueba de interacciones");
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.transform.SetParent(root.transform);
            floor.transform.position = new Vector3(0, -.2f, 0);
            floor.transform.localScale = new Vector3(30, .4f, 30);
            var playerObject = Child("Visitante");
            var controller = playerObject.AddComponent<CharacterController>();
            controller.height = 1.8f; controller.center = Vector3.up * .9f;
            controller.radius = .27f;
            player = playerObject.AddComponent<ClinicPlayer>();
            var roomObject = Child("Sala de prueba");
            room = roomObject.AddComponent<PracticeRoom>(); room.roomId = 901;
            room.title = "Clasificación"; room.instructions = "Lee las condiciones de cada caso.";
            var volume = roomObject.AddComponent<BoxCollider>(); volume.isTrigger = true;
            volume.center = new Vector3(0, 1.5f, 0); volume.size = new Vector3(8, 3, 8);
            room.roomVolume = volume;
            first = Item("Gasa saturada de sangre", new Vector3(-.2f, 1.1f, 1), WasteDestination.RedBag);
            second = Item("Envoltura limpia", new Vector3(.3f, 1.1f, 1), WasteDestination.General);
            wrongBin = Child("Residuos comunes").AddComponent<ClinicDestination>();
            wrongBin.roomId = room.roomId; wrongBin.destination = WasteDestination.General;
            correctBin = Child("Bolsa roja").AddComponent<ClinicDestination>();
            correctBin.roomId = room.roomId; correctBin.destination = WasteDestination.RedBag;
            game = Child("Gestión").AddComponent<ClinicGame>(); game.player = player;
            game.rooms = new[] { room };
            yield return null;
            game.StartVisit(); game.RefreshRoom();
        }

        [UnityTearDown]
        public IEnumerator Clean()
        {
            Object.Destroy(root);
            yield return null;
            Time.timeScale = 1;
        }

        [UnityTest]
        public IEnumerator AssistanceHidesForEveryItemWhileHoldingOrPaused()
        {
            Assert.IsTrue(game.ShouldOutline(first));
            Assert.IsTrue(game.ShouldOutline(second));
            game.PickUp(first);
            Assert.IsFalse(game.ShouldOutline(first));
            Assert.IsFalse(game.ShouldOutline(second), "Llevar un objeto debe ocultar todos los contornos de asistencia.");
            game.ReturnHeld();
            Assert.IsTrue(game.ShouldOutline(second));
            game.Pause();
            Assert.IsFalse(game.ShouldOutline(first));
            game.Resume(); game.RefreshRoom();
            Assert.IsTrue(game.ShouldOutline(first));
            first.Complete();
            Assert.IsFalse(game.ShouldOutline(first));
            player.Teleport(new Vector3(0, 0, 5)); game.RefreshRoom();
            Assert.IsNull(game.CurrentRoom);
            Assert.IsFalse(game.ShouldOutline(second), "Un objeto cercano no debe verse resaltado desde fuera de su sala.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator WrongAnswerRestoresPoseAndCorrectAnswerCompletesOnce()
        {
            var position = first.transform.position;
            var scale = first.transform.lossyScale;
            game.PickUp(first); game.Deposit(wrongBin);
            Assert.IsNull(game.HeldItem);
            Assert.IsFalse(first.Completed);
            Assert.AreEqual(position, first.transform.position);
            Assert.AreEqual(scale, first.transform.lossyScale);
            Assert.IsTrue(first.GetComponent<Collider>().enabled);
            Assert.AreEqual(1, room.Mistakes);
            Assert.AreEqual(0, room.Score);
            game.PickUp(first); game.Deposit(correctBin);
            Assert.IsTrue(first.Completed);
            Assert.AreEqual(1, room.Correct);
            game.PickUp(first); game.Deposit(correctBin);
            Assert.IsNull(game.HeldItem);
            Assert.AreEqual(1, room.Correct, "No se puede puntuar dos veces el mismo residuo.");
            game.PickUp(second); game.Deposit(wrongBin);
            Assert.IsTrue(room.Completed);
            Assert.AreEqual(15, room.Score);
            game.RestartCurrentRoom();
            Assert.IsFalse(room.Completed);
            Assert.AreEqual(0, room.Correct + room.Mistakes);
            Assert.IsTrue(first.gameObject.activeSelf && second.gameObject.activeSelf);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ManualAndCreditsPreserveTheirReturnRouteAndPauseSimulation()
        {
            game.MainMenu(); game.OpenManual();
            Assert.AreEqual(ClinicScreen.Manual, game.Screen);
            Assert.AreEqual(0, Time.timeScale);
            game.Back(); Assert.AreEqual(ClinicScreen.Menu, game.Screen);
            game.StartVisit(); game.OpenManual();
            Assert.IsFalse(game.Playing);
            Assert.AreEqual(CursorLockMode.None, Cursor.lockState);
            game.Back(); Assert.AreEqual(ClinicScreen.Pause, game.Screen);
            game.OpenCredits(); game.Back(); Assert.AreEqual(ClinicScreen.Pause, game.Screen);
            game.Resume(); Assert.AreEqual(1, Time.timeScale);
            Assert.IsTrue(game.Playing);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OrderedStepsCannotBeSkippedAndResetCancelsAnActiveProcess()
        {
            var stepOne = Child("Limpiar").AddComponent<PracticeStation>();
            stepOne.roomId = room.roomId; stepOne.stepIndex = 0; stepOne.duration = .2f;
            var stepTwo = Child("Empacar").AddComponent<PracticeStation>();
            stepTwo.roomId = room.roomId; stepTwo.stepIndex = 1;
            room.steps = new[] { stepOne, stepTwo }; room.ResetPractice();
            Assert.IsFalse(first.CanInteract(game), "Los residuos se clasifican al completar la secuencia.");
            stepTwo.Interact(game); Assert.IsFalse(stepTwo.Completed);
            stepOne.Interact(game); Assert.IsTrue(room.Busy);
            game.Pause();
            yield return new WaitForSecondsRealtime(.3f);
            Assert.IsTrue(room.Busy, "El proceso debe detenerse mientras se consulta el manual o se pausa.");
            game.RestartCurrentRoom();
            yield return new WaitForSeconds(.3f);
            Assert.AreEqual(0, room.NextStep);
            Assert.IsFalse(room.Busy);
            stepOne.Interact(game);
            yield return new WaitForSeconds(.3f);
            Assert.AreEqual(1, room.NextStep);
            stepTwo.Interact(game);
            Assert.IsTrue(room.StepsComplete);
            Assert.IsTrue(first.CanInteract(game));
        }

        [Test]
        public void ManualShortcutWorksAtStartDuringVisitAndWhilePaused()
        {
            var previousFocusBehavior = InputSystem.settings.backgroundBehavior;
            var previousUpdateMode = InputSystem.settings.updateMode;
            var previousEditorBehavior = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
            var keyboard = InputSystem.AddDevice<Keyboard>();
            keyboard.MakeCurrent();
            game.enabled = false;
            try
            {
                // El modo por lotes no tiene foco de ventana. Procesa los mismos eventos y Update de producción
                // de forma síncrona, sin depender del orden entre la corrutina de NUnit y el bucle de entrada.
                game.MainMenu(); PressManualKey(keyboard);
                Assert.AreEqual(ClinicScreen.Manual, game.Screen);
                PressManualKey(keyboard);
                Assert.AreEqual(ClinicScreen.Menu, game.Screen);
                game.StartVisit(); PressManualKey(keyboard);
                Assert.AreEqual(ClinicScreen.Manual, game.Screen);
                Assert.AreEqual(0, Time.timeScale);
                game.Back();
                Assert.AreEqual(ClinicScreen.Pause, game.Screen);
                PressManualKey(keyboard);
                Assert.AreEqual(ClinicScreen.Manual, game.Screen);
                game.Back();
                Assert.AreEqual(ClinicScreen.Pause, game.Screen);
            }
            finally
            {
                InputSystem.RemoveDevice(keyboard);
                InputSystem.settings.updateMode = previousUpdateMode;
                InputSystem.settings.backgroundBehavior = previousFocusBehavior;
                InputSystem.settings.editorInputBehaviorInPlayMode = previousEditorBehavior;
                game.enabled = true;
            }
        }

        void PressManualKey(Keyboard keyboard)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            InputSystem.Update();
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.M));
            InputSystem.Update();
            keyboard.MakeCurrent();
            Assert.IsTrue(keyboard.mKey.wasPressedThisFrame, "El evento M debe llegar al sistema de entrada. "
                + "Dispositivo habilitado: " + keyboard.enabled + "; pulsado: " + keyboard.mKey.isPressed
                + "; actualizado: " + keyboard.wasUpdatedThisFrame);
            game.SendMessage("Update", SendMessageOptions.RequireReceiver);
        }

        [UnityTest]
        public IEnumerator RotatedRoomUsesItsActualVolumeAndAssistanceHasADistanceLimit()
        {
            room.roomVolume.size = new Vector3(2, 3, 8);
            room.transform.rotation = Quaternion.Euler(0, 45, 0);
            Vector3 inside = room.transform.TransformPoint(new Vector3(.5f, 1.6f, 2));
            Vector3 outside = room.transform.TransformPoint(new Vector3(1.2f, 1.6f, 0));
            Assert.IsTrue(room.Contains(inside)); Assert.IsFalse(room.Contains(outside));
            game.outlineRange = .25f;
            game.RefreshRoom();
            Assert.IsFalse(game.ShouldOutline(first));
            yield return null;
        }

        GameObject Child(string name)
        {
            var child = new GameObject(name); child.transform.SetParent(root.transform); return child;
        }
        ClinicItem Item(string name, Vector3 position, WasteDestination destination)
        {
            var itemObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            itemObject.name = name; itemObject.transform.SetParent(root.transform);
            itemObject.transform.position = position; itemObject.transform.localScale = Vector3.one * .12f;
            var item = itemObject.AddComponent<ClinicItem>();
            item.roomId = room.roomId; item.displayName = name; item.correctDestination = destination;
            item.description = "Lee las condiciones del caso antes de elegir un destino.";
            return item;
        }
    }
}
