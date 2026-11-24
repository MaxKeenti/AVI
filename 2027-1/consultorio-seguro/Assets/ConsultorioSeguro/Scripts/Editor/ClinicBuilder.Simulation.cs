using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace ConsultorioSeguroNuevo.Editor
{
    // Modelos que dan sentido a las acciones de simulación. Se añaden a la escena existente sin
    // reconstruirla, de modo que los ajustes manuales se conservan; «Crear escena nueva» también los incluye.
    public static partial class ClinicBuilder
    {
        public const string PatientName = "Paciente simulado";
        static readonly string[] PatientPieces =
            { "torso", "cuello", "cabeza", "brazo derecho", "brazo izquierdo", "pierna derecha", "pie derecho", "pierna izquierda", "pie izquierdo" };

        [MenuItem("Consultorio Seguro/Añadir modelos de simulación")]
        public static void AddSimulationModels()
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.path != ScenePath) scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            mats.Clear(); meshes.Clear(); usedAssets.Clear();
            SetupFont();
            ApplySimulationModels();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            // Conserva el registro de modelos de la escena y añade los nuevos.
            const string list = "Verificacion/modelos-utilizados.txt";
            var lines = File.Exists(list) ? File.ReadAllLines(list).ToList() : new List<string>();
            File.WriteAllLines(list, lines.Concat(usedAssets).Where(l => !string.IsNullOrWhiteSpace(l)).Distinct().OrderBy(l => l));
            Debug.Log("CONSULTORIO_SIMULACION · Modelos de simulación añadidos a la escena.");
        }

        static void ApplySimulationModels()
        {
            foreach (var room in Object.FindObjectsByType<PracticeRoom>())
            {
                var chair = Deep(room.transform, "dental-chair-reclined");
                if (chair) AddPatient(chair);
            }
        }

        static Transform Deep(Transform root, string name) =>
            root ? root.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == name) : null;

        static Bounds RendererBounds(Transform root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;
            foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
            return bounds;
        }

        static IEnumerable<Vector3> WorldVertices(Transform root, bool enabledOnly)
        {
            foreach (var filter in root.GetComponentsInChildren<MeshFilter>())
            {
                var renderer = filter.GetComponent<MeshRenderer>();
                if (!filter.sharedMesh || (enabledOnly && (!renderer || !renderer.enabled))) continue;
                var matrix = filter.transform.localToWorldMatrix;
                foreach (var vertex in filter.sharedMesh.vertices) yield return matrix.MultiplyPoint3x4(vertex);
            }
        }

        // --- Paciente ---

        static void AddPatient(Transform chair)
        {
            var previous = chair.Find(PatientName);
            if (previous) Object.DestroyImmediate(previous.gameObject);
            var tailored = Deep(chair, "Tapicería anatómica y detalles");
            var headrest = Deep(tailored, "Reposacabezas anatómico");
            var legrest = Deep(tailored, "Apoyo de piernas");
            var back = Deep(chair, "chair-back");
            if (!headrest || !legrest || !back) return;
            Vector3 head = RendererBounds(headrest).center, feet = RendererBounds(legrest).center;
            Vector3 along = feet - head; along.y = 0; along.Normalize();

            var clothes = Material("Ropa de paciente", new Color(.21f, .33f, .43f), .22f);
            var skin = Material("Piel de paciente", new Color(.67f, .47f, .35f), .32f);
            var shoes = Material("Calzado de paciente", new Color(.13f, .13f, .14f), .35f);
            var patient = Group(PatientName, chair);
            // Las piezas OBJ comparten un marco: la cabeza hacia -Z y los pies hacia +Z.
            patient.rotation = Quaternion.LookRotation(along, Vector3.up);
            var pieces = new Transform[PatientPieces.Length];
            for (int i = 0; i < PatientPieces.Length; i++)
            {
                string path = $"display-mannequin/paciente-{i}.obj";
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Terceros/" + path);
                var piece = (GameObject)PrefabUtility.InstantiatePrefab(prefab, patient);
                piece.name = "Paciente · " + PatientPieces[i];
                piece.transform.localPosition = Vector3.zero;
                piece.transform.localRotation = Quaternion.identity;
                var material = i == 0 || i == 5 || i == 7 ? clothes : i == 6 || i == 8 ? shoes : skin;
                foreach (var renderer in piece.GetComponentsInChildren<Renderer>())
                {
                    renderer.sharedMaterials = Enumerable.Repeat(material, renderer.sharedMaterials.Length).ToArray();
                    renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                    renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;
                }
                pieces[i] = piece.transform;
                usedAssets.Add(path);
            }
            // La cadera (entre torso y piernas) se coloca sobre la articulación del respaldo.
            Vector3 hip = patient.TransformPoint(new Vector3(0, .80f, .60f));
            Vector3 joint = back.position + along * .06f;
            patient.position += new Vector3(joint.x - hip.x, 0, joint.z - hip.z);
            hip = patient.TransformPoint(new Vector3(0, .80f, .60f));

            var support = SupportGrid(tailored, chair.GetChild(0));
            var lower = new[] { pieces[5], pieces[6], pieces[7], pieces[8] };
            var upper = new[] { pieces[0], pieces[1], pieces[2], pieces[3], pieces[4] };
            // Cada tramo se inclina para seguir la línea de la tapicería que tiene debajo:
            // piernas sobre asiento y apoyo de piernas, torso sobre el respaldo.
            Vector3 right = patient.right;
            float legAngle = MatchSlope(lower.Take(1).Concat(lower.Skip(2).Take(1)), hip, along, right, support, .25f, .85f);
            foreach (var piece in lower) piece.RotateAround(hip, right, legAngle);
            float torsoAngle = MatchSlope(upper.Take(1), hip, along, right, support, -.15f, -.55f);
            foreach (var piece in upper) piece.RotateAround(hip, right, torsoAngle);
            // Altura: el cuerpo toca la tapicería. Se usa un percentil alto en lugar del máximo porque la malla
            // adaptada conserva un saliente bajo la rodilla de la postura sentada original, que queda dentro del cojín.
            var lowest = new Dictionary<Vector2Int, float>();
            foreach (var p in WorldVertices(patient, false))
            {
                var key = SupportKey(p);
                lowest[key] = lowest.TryGetValue(key, out float y) ? Mathf.Min(y, p.y) : p.y;
            }
            var overlap = lowest.Where(pair => support.ContainsKey(pair.Key)).Select(pair => support[pair.Key] - pair.Value)
                .OrderBy(value => value).ToArray();
            if (overlap.Length > 0)
                patient.position += Vector3.up * (overlap[Mathf.Min(overlap.Length - 1, Mathf.FloorToInt(overlap.Length * .9f))] + .006f);
        }

        /// <summary>
        /// Ángulo, en grados sobre <paramref name="right"/>, que alinea la cara inferior de las piezas con la
        /// superficie de apoyo entre las distancias <paramref name="near"/> y <paramref name="far"/> desde la cadera.
        /// </summary>
        static float MatchSlope(IEnumerable<Transform> pieces, Vector3 hip, Vector3 along, Vector3 right,
            Dictionary<Vector2Int, float> support, float near, float far)
        {
            var vertices = pieces.SelectMany(piece => WorldVertices(piece, false)).ToArray();
            // Cara inferior en una franja ancha (las mallas son de pocos polígonos), ignorando salientes aislados.
            float Bottom(float distance)
            {
                var band = vertices.Where(v => Mathf.Abs(Vector3.Dot(v - hip, along) - distance) < .16f)
                    .Select(v => v.y).OrderBy(y => y).ToArray();
                return band.Length == 0 ? float.NaN : band[Mathf.FloorToInt(band.Length * .1f)];
            }
            float Top(float distance)
            {
                Vector3 center = hip + along * distance;
                float best = float.NaN;
                foreach (float side in new[] { -.12f, -.06f, 0, .06f, .12f })
                    if (support.TryGetValue(SupportKey(center + right * side), out float top))
                        best = float.IsNaN(best) ? top : Mathf.Max(best, top);
                return best;
            }
            float span = far - near;
            float body = Mathf.Atan((Bottom(far) - Bottom(near)) / span) * Mathf.Rad2Deg;
            float seat = Mathf.Atan((Top(far) - Top(near)) / span) * Mathf.Rad2Deg;
            if (float.IsNaN(body) || float.IsNaN(seat)) return 0;
            // Un giro positivo sobre el eje lateral reduce la pendiente en la dirección «along» de todo el tramo.
            return Mathf.Clamp(body - seat, -40, 40);
        }

        const float SupportCell = .035f;
        static Vector2Int SupportKey(Vector3 p) => new Vector2Int(Mathf.FloorToInt(p.x / SupportCell), Mathf.FloorToInt(p.z / SupportCell));

        static Dictionary<Vector2Int, float> SupportGrid(params Transform[] roots)
        {
            var support = new Dictionary<Vector2Int, float>();
            foreach (var root in roots)
                foreach (var p in WorldVertices(root, true))
                {
                    var key = SupportKey(p);
                    support[key] = support.TryGetValue(key, out float top) ? Mathf.Max(top, p.y) : p.y;
                }
            return support;
        }

    }
}
