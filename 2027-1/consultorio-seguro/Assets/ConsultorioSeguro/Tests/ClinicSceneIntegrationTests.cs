using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ConsultorioSeguroNuevo.Tests
{
    // Esta prueba carga la escena entregable y usa su geometría, colisiones y puntos de interacción.
    // La navegación aplica Move al CharacterController real; no teletransporta al visitante.
    public sealed class ClinicSceneIntegrationTests
    {
        const string SceneName = "ConsultorioSeguro";
        const float Grid = .25f;
        ClinicGame game;
        ClinicPlayer player;
        CharacterController controller;
        Scene scene;
        readonly Dictionary<Vector2Int, bool> clearance = new Dictionary<Vector2Int, bool>();
        readonly Dictionary<(Vector2Int, Vector2Int), bool> edges = new Dictionary<(Vector2Int, Vector2Int), bool>();
        static readonly Vector2Int[] Directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        [UnitySetUp]
        public IEnumerator LoadClinic()
        {
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(SceneName),
                "La escena entregable debe estar creada e incluida en los ajustes de compilación.");
            Time.timeScale = 1;
            yield return SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            yield return null;
            scene = SceneManager.GetActiveScene();
            game = UnityEngine.Object.FindFirstObjectByType<ClinicGame>();
            Assert.IsNotNull(game);
            player = game.player;
            controller = player.GetComponent<CharacterController>();
            Assert.AreEqual(5, game.rooms.Length);
            Assert.AreEqual(ClinicScreen.Menu, game.Screen);
            Assert.That(player.eye.transform.localPosition.y, Is.InRange(1.6f, 1.75f));
            // Evita que movimientos físicos del ratón afecten una prueba reproducible.
            player.enabled = false;
            clearance.Clear(); edges.Clear();
            Physics.SyncTransforms();
        }

        [UnityTearDown]
        public IEnumerator UnloadClinic()
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None;
            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.SetActiveScene(SceneManager.CreateScene("Escena vacía de verificación"));
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        [UnityTest, Timeout(240000)]
        public IEnumerator WalkEveryRouteAndCompleteEveryPracticeThroughVisibleTargets()
        {
            CaptureRuntime("13-inicio");
            ClickVisibleButton("Créditos y fuentes");
            Assert.AreEqual(ClinicScreen.Credits, game.Screen);
            var creditsScroll = game.GetComponentsInChildren<ScrollRect>().Single();
            var sourceCredits = Resources.Load<TextAsset>("Creditos");
            string expectedCredits = sourceCredits ? sourceCredits.text : game.credits;
            var creditsBlocks = creditsScroll.content.GetComponentsInChildren<Text>();
            Assert.AreEqual(expectedCredits, string.Concat(creditsBlocks.Select(block => block.text)),
                "La división del texto conserva la totalidad de las fuentes y licencias.");
            Assert.IsTrue(creditsBlocks.All(block => block.text.Length <= 6000));
            Canvas.ForceUpdateCanvases();
            ClickVisibleButton("← Volver");
            ClickVisibleButton("Manual de controles");
            Assert.AreEqual(ClinicScreen.Manual, game.Screen);
            Assert.IsFalse(VisibleButtons().Any(button => button.name == "Reanudar desde ayuda"));
            yield return null;
            Canvas.ForceUpdateCanvases();
            CaptureRuntime("14-manual");
            var scroll = game.GetComponentsInChildren<ScrollRect>().Single();
            var viewportGraphic = scroll.viewport.GetComponent<Graphic>();
            Assert.IsTrue(viewportGraphic && viewportGraphic.raycastTarget,
                "El manual necesita una superficie que reciba la rueda del ratón.");
            var wheel = new PointerEventData(EventSystem.current) { scrollDelta = new Vector2(0, -5) };
            ExecuteEvents.ExecuteHierarchy(scroll.viewport.gameObject, wheel, ExecuteEvents.scrollHandler);
            Assert.Less(scroll.verticalNormalizedPosition, 1, "La rueda debe desplazar el texto del manual.");
            ClickVisibleButton("← Volver");
            Assert.AreEqual(ClinicScreen.Menu, game.Screen);
            ClickVisibleButton("Entrar al consultorio");
            Assert.IsTrue(game.Playing);

            yield return WalkTo(new Vector3(0, 0, -5.5f), "Entrada principal");
            yield return WalkTo(new Vector3(4.25f, 0, -1.5f), "Frente del registro");
            yield return WalkTo(new Vector3(5.25f, 0, 1.5f), "Circulación detrás de recepción");
            yield return WalkTo(new Vector3(-1.75f, 0, -3.5f), "Acceso a la sala de espera");
            yield return WalkTo(new Vector3(0, 0, 4.75f), "Pasillo central");

            var destinations = UnityEngine.Object.FindObjectsByType<ClinicDestination>(FindObjectsSortMode.None);
            var manuals = UnityEngine.Object.FindObjectsByType<ClinicManual>(FindObjectsSortMode.None);
            foreach (var room in game.rooms.OrderBy(room => room.roomId))
            {
                int side = room.roomId % 2 == 1 ? -1 : 1;
                float doorway = 4.85f + (room.roomId - 1) / 2 * 6;
                yield return WalkTo(new Vector3(0, 0, doorway), "Pasillo frente a " + room.title);
                yield return WalkTo(new Vector3(side * 2.25f, 0, doorway), "Umbral de " + room.title);
                game.RefreshRoom();
                Assert.AreSame(room, game.CurrentRoom, "El visitante debe entrar físicamente en la sala.");

                var manual = manuals.Single(candidate => candidate.roomId == room.roomId);
                yield return ApproachAndAim(manual);
                ActivateTarget(manual);
                Assert.AreEqual(ClinicScreen.Manual, game.Screen);
                Assert.AreEqual(0, Time.timeScale);
                ClickVisibleButton("← Volver");
                Assert.AreEqual(ClinicScreen.Pause, game.Screen);
                if (room.roomId == 1) CaptureRuntime("15-pausa");
                ClickVisibleButton("Reanudar");
                Assert.IsTrue(game.Playing);

                foreach (var step in room.steps)
                {
                    yield return ApproachAndAim(step);
                    ActivateTarget(step);
                    float deadline = Time.realtimeSinceStartup + step.duration + 5;
                    while (!step.Completed && Time.realtimeSinceStartup < deadline) yield return null;
                    Assert.IsTrue(step.Completed, room.title + ": " + step.actionText);
                }
                Assert.IsTrue(room.StepsComplete);
                foreach (var item in room.Items)
                {
                    yield return ApproachAndAim(item);
                    Assert.IsTrue(game.ShouldOutline(item), "El material cercano debe tener asistencia antes de tomarlo.");
                    if (room.roomId == 1 && item == room.Items[0])
                    {
                        yield return null;
                        CaptureRuntime("16-silueta-dorada");
                    }
                    ActivateTarget(item);
                    Assert.AreSame(item, game.HeldItem);
                    if (room.roomId == 1 && item == room.Items[0])
                    {
                        yield return null;
                        CaptureRuntime("17-material-en-mano");
                    }
                    Assert.IsFalse(room.Items.Any(candidate => game.ShouldOutline(candidate)),
                        "Llevar un objeto oculta los contornos de todos los instrumentos.");
                    var destination = destinations.Single(candidate => candidate.roomId == room.roomId
                        && candidate.destination == item.correctDestination);
                    yield return ApproachAndAim(destination);
                    ActivateTarget(destination);
                    Assert.IsTrue(item.Completed, room.title + ": " + item.displayName);
                    Assert.IsNull(game.HeldItem);
                    Assert.IsFalse(game.ShouldOutline(item));
                }
                Assert.IsTrue(room.Completed);
                Assert.AreEqual(room.Items.Length * 10, room.Score);
                foreach (var destination in destinations.Where(candidate => candidate.roomId == room.roomId))
                {
                    yield return ApproachAndAim(destination);
                    ActivateTarget(destination);
                    Assert.IsNull(game.HeldItem);
                    Assert.AreEqual(room.Items.Length * 10, room.Score, "Consultar un recipiente no altera la puntuación.");
                }
                TestContext.Progress.WriteLine("VERIFICADO · " + room.title + ": manual, " + room.steps.Length
                    + " pasos y " + room.Items.Length + " materiales; interacción por raycast y circulación con colisiones.");
                yield return WalkTo(new Vector3(side * 2.25f, 0, doorway), "Salida de " + room.title);
                yield return WalkTo(new Vector3(0, 0, doorway), "Retorno al pasillo");
                game.RefreshRoom();
                Assert.IsNull(game.CurrentRoom);
            }
            Assert.AreEqual(5, game.CompletedRooms);
            yield return WalkTo(new Vector3(0, 0, 16.85f), "Acceso al área de personal");
            yield return WalkTo(new Vector3(2.25f, 0, 16.85f), "Umbral del área de personal");
            yield return WalkTo(new Vector3(4.25f, 0, 19.25f), "Circulación del área de personal y almacén");
            yield return WalkTo(new Vector3(2.25f, 0, 19.5f), "Manual del área de personal");
            var staffManual = manuals.Single(candidate => candidate.roomId == 0);
            Vector3 staffManualPoint = staffManual.GetComponent<Collider>().bounds.center;
            player.eye.transform.rotation = Quaternion.LookRotation(staffManualPoint - player.eye.transform.position);
            game.RefreshRoom(); game.RefreshTarget();
            Assert.IsNull(game.CurrentRoom, "El área de personal no constituye una sexta práctica.");
            ActivateTarget(staffManual);
            Assert.AreEqual(ClinicScreen.Manual, game.Screen);
            ClickVisibleButton("← Volver");
            Assert.AreEqual(ClinicScreen.Pause, game.Screen);
            ClickVisibleButton("Reanudar");
            Assert.IsTrue(game.Playing);
            yield return WalkTo(new Vector3(0, 0, 16.85f), "Regreso del área de personal");
            yield return WalkTo(new Vector3(0, 0, -9), "Salida a la banqueta");
            Assert.That(player.transform.position.y, Is.InRange(-.12f, .2f));
            game.Pause();
            CaptureRuntime("18-cinco-practicas-completadas");
            ClickVisibleButton("Manual de controles");
            ClickVisibleButton("Reanudar desde ayuda");
            Assert.IsTrue(game.Playing);
            TestContext.Progress.WriteLine("VERIFICADO · Ruta completa: banqueta, entrada, registro frontal y posterior, espera, cinco salas, personal y salida.");
        }

        void CaptureRuntime(string name)
        {
            string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Capturas"));
            Directory.CreateDirectory(directory);
            var camera = player.eye;
            var canvas = game.GetComponentInChildren<Canvas>(true);
            var originalMode = canvas.renderMode;
            var originalCamera = canvas.worldCamera;
            float originalDistance = canvas.planeDistance;
            var originalTarget = camera.targetTexture;
            var originalActive = RenderTexture.active;
            var output = new RenderTexture(1800, 1125, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
                { antiAliasing = 4 };
            var pixels = new Texture2D(1800, 1125, TextureFormat.RGB24, false);
            try
            {
                // Camera.Render no incluye ScreenSpaceOverlay: conserva la interfaz real y la renderiza
                // temporalmente delante de la misma cámara del visitante, a la resolución documentada.
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = .08f;
                output.Create(); camera.targetTexture = output;
                var scaler = canvas.GetComponent<CanvasScaler>();
                bool scalerEnabled = scaler.enabled;
                scaler.enabled = false; scaler.enabled = scalerEnabled;
                Canvas.ForceUpdateCanvases();
                foreach (var text in canvas.GetComponentsInChildren<Text>(true)) text.SetAllDirty();
                Canvas.ForceUpdateCanvases();
                foreach (var outline in UnityEngine.Object.FindObjectsByType<ClinicOutline>(FindObjectsSortMode.None)) outline.Refresh();
                camera.Render(); camera.Render();
                RenderTexture.active = output;
                pixels.ReadPixels(new Rect(0, 0, 1800, 1125), 0, 0); pixels.Apply();
                File.WriteAllBytes(Path.Combine(directory, name + ".png"), pixels.EncodeToPNG());
                string provenance = Path.Combine(directory, "ORIGEN-RUNTIME.txt");
                if (name == "13-inicio") File.WriteAllText(provenance,
                    "Capturas reales del modo de ejecución de Unity " + Application.unityVersion + ".\n"
                    + "Escena: Assets/ConsultorioSeguro/Escenas/ConsultorioSeguro.unity.\n"
                    + "Cámara del visitante, altura local de ojos 1,65 m. Resolución: 1800 × 1125.\n"
                    + "Generadas durante ClinicSceneIntegrationTests mediante Camera.Render y la interfaz real, "
                    + "cambiada temporalmente de ScreenSpaceOverlay a ScreenSpaceCamera para capturarla.\n"
                    + "El recorrido y las interacciones de la prueba son automatizados con CharacterController.Move "
                    + "y raycasts sobre los colliders reales; no equivalen a una revisión manual.\n");
                File.AppendAllText(provenance, name + ".png · UTC " + DateTime.UtcNow.ToString("O")
                    + " · cámara " + camera.transform.position + " · pantalla " + game.Screen + "\n");
            }
            finally
            {
                RenderTexture.active = originalActive; camera.targetTexture = originalTarget;
                canvas.renderMode = originalMode; canvas.worldCamera = originalCamera; canvas.planeDistance = originalDistance;
                output.Release(); UnityEngine.Object.Destroy(output); UnityEngine.Object.Destroy(pixels);
                Canvas.ForceUpdateCanvases();
            }
        }

        IEnumerable<Button> VisibleButtons() => game.GetComponentsInChildren<Button>(true)
            .Where(button => button.gameObject.activeInHierarchy && button.interactable);

        void ClickVisibleButton(string name)
        {
            var button = VisibleButtons().Single(candidate => candidate.name == name);
            button.onClick.Invoke();
        }

        IEnumerator WalkTo(Vector3 destination, string description)
        {
            var path = FindPath(node => (World(node) - new Vector3(destination.x, .05f, destination.z)).sqrMagnitude < .14f * .14f);
            Assert.IsNotNull(path, "No existe una ruta con el ancho del visitante: " + description);
            yield return WalkPath(path, description);
            Assert.Less(Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z),
                new Vector2(destination.x, destination.z)), .20f, description);
        }

        IEnumerator ApproachAndAim(ClinicInteractable target)
        {
            var path = FindPath(node => CanAimSafelyFrom(World(node), target));
            Assert.IsNotNull(path, "No hay posición alcanzable con visión y distancia de interacción para "
                + target.name + " (sala " + target.roomId + ").");
            string planned = DescribeAim(World(path[path.Count - 1]), target);
            yield return WalkPath(path, target.name);
            Physics.SyncTransforms();
            Assert.IsTrue(CanAimFrom(player.transform.position, target, out Vector3 aim),
                "El objetivo dejó de ser visible a altura de ojos: " + target.name
                + "; visitante " + player.transform.position + "; destino de ruta " + World(path[path.Count - 1])
                + "\nPlaneado: " + planned + "\nReal: " + DescribeAim(player.transform.position, target));
            player.eye.transform.rotation = Quaternion.LookRotation(aim - player.eye.transform.position);
            game.RefreshRoom(); game.RefreshTarget();
            Assert.AreSame(target, game.Target, "El primer collider apuntado debe pertenecer a " + target.name);
            Assert.IsFalse(string.IsNullOrWhiteSpace(game.Prompt));
            Assert.IsTrue(target.CanInteract(game), game.Prompt);
        }

        void ActivateTarget(ClinicInteractable expected)
        {
            game.RefreshTarget();
            Assert.AreSame(expected, game.Target);
            Assert.IsTrue(game.Target.CanInteract(game));
            game.Target.Interact(game);
        }

        string DescribeAim(Vector3 feet, ClinicInteractable target)
        {
            feet.y = player.transform.position.y;
            Vector3 eye = feet + Vector3.up * player.eye.transform.localPosition.y;
            string result = "ojos=" + eye.ToString("F4") + " cámara local=" + player.eye.transform.localPosition.ToString("F4");
            foreach (var collider in target.GetComponentsInChildren<Collider>())
            {
                Vector3 point = collider.bounds.center;
                bool hit = Physics.Raycast(eye, (point - eye).normalized, out RaycastHit contact,
                    game.interactionRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
                result += " centro=" + point.ToString("F4") + " impacto=" + (hit ? contact.collider.name : "ninguno");
            }
            return result;
        }

        bool CanAimSafelyFrom(Vector3 feet, ClinicInteractable target)
        {
            // Evita elegir una visual que roce una esquina. El controlador termina cada avance
            // a menos de 2,5 cm; la visual debe resistir ese margen en ambas direcciones.
            return CanAimFrom(feet, target, out _)
                && CanAimFrom(feet + Vector3.right * .04f, target, out _)
                && CanAimFrom(feet - Vector3.right * .04f, target, out _)
                && CanAimFrom(feet + Vector3.forward * .04f, target, out _)
                && CanAimFrom(feet - Vector3.forward * .04f, target, out _);
        }

        bool CanAimFrom(Vector3 feet, ClinicInteractable target, out Vector3 aim)
        {
            feet.y = player.transform.position.y;
            Vector3 eye = feet + Vector3.up * player.eye.transform.localPosition.y;
            aim = default;
            var room = game.rooms.First(candidate => candidate.roomId == target.roomId);
            if (!room.Contains(eye)) return false;
            foreach (var collider in target.GetComponentsInChildren<Collider>())
            {
                if (!collider.enabled || collider.isTrigger) continue;
                Vector3 point = collider.bounds.center;
                Vector3 direction = point - eye;
                if (direction.sqrMagnitude > (game.interactionRange - .10f) * (game.interactionRange - .10f)) continue;
                if (Physics.Raycast(eye, direction.normalized, out RaycastHit hit, game.interactionRange,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
                    && hit.collider.GetComponentInParent<ClinicInteractable>() == target)
                { aim = point; return true; }
            }
            return false;
        }

        List<Vector2Int> FindPath(Func<Vector2Int, bool> isDestination)
        {
            Physics.SyncTransforms();
            Vector2Int start = Node(player.transform.position);
            var parents = new Dictionary<Vector2Int, Vector2Int> { [start] = start };
            var queue = new Queue<Vector2Int>(); queue.Enqueue(start);
            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                if (isDestination(current))
                {
                    var path = new List<Vector2Int>();
                    while (current != start) { path.Add(current); current = parents[current]; }
                    path.Add(start); path.Reverse(); return path;
                }
                foreach (var direction in Directions)
                {
                    Vector2Int next = current + direction;
                    if (parents.ContainsKey(next) || !Clear(next) || !Connected(current, next)) continue;
                    parents.Add(next, current); queue.Enqueue(next);
                }
            }
            return null;
        }

        bool Clear(Vector2Int node)
        {
            if (node.x < -28 || node.x > 28 || node.y < -37 || node.y > 82) return false;
            if (clearance.TryGetValue(node, out bool free)) return free;
            Capsule(World(node), out Vector3 low, out Vector3 high, out float radius);
            free = !Physics.OverlapCapsule(low, high, radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
                .Any(collider => collider != controller);
            clearance[node] = free; return free;
        }

        bool Connected(Vector2Int from, Vector2Int to)
        {
            var key = (from, to);
            if (edges.TryGetValue(key, out bool connected)) return connected;
            Capsule(World(from), out Vector3 low, out Vector3 high, out float radius);
            Vector3 delta = World(to) - World(from);
            connected = !Physics.CapsuleCastAll(low, high, radius, delta.normalized, delta.magnitude,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore).Any(hit => hit.collider != controller);
            edges[key] = connected; edges[(to, from)] = connected; return connected;
        }

        void Capsule(Vector3 position, out Vector3 low, out Vector3 high, out float radius)
        {
            radius = controller.radius + .02f;
            float half = controller.height * .5f - controller.radius;
            low = position + controller.center - Vector3.up * half;
            high = position + controller.center + Vector3.up * half;
        }

        IEnumerator WalkPath(List<Vector2Int> path, string description)
        {
            int moves = 0;
            foreach (var node in path)
            {
                Vector3 destination = World(node);
                int budget = 12;
                while (budget-- > 0)
                {
                    Vector3 delta = destination - player.transform.position; delta.y = 0;
                    if (delta.magnitude < .025f) break;
                    controller.Move(Vector3.ClampMagnitude(delta, .075f) + Vector3.down * .035f);
                    if (++moves % 24 == 0) { game.RefreshRoom(); yield return null; }
                }
                Vector3 remaining = destination - player.transform.position; remaining.y = 0;
                Assert.Less(remaining.magnitude, .055f,
                    "Colisión que bloquea la circulación hacia " + description + " en " + destination
                    + "; visitante en " + player.transform.position);
            }
            game.RefreshRoom();
            yield return null;
        }

        static Vector2Int Node(Vector3 position) => new Vector2Int(Mathf.RoundToInt(position.x / Grid), Mathf.RoundToInt(position.z / Grid));
        static Vector3 World(Vector2Int node) => new Vector3(node.x * Grid, .05f, node.y * Grid);
    }
}
