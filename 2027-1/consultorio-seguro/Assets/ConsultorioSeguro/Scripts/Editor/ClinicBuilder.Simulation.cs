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
                var autoclave = Deep(room.transform, "autoclave");
                if (autoclave) SplitAutoclaveDoor(autoclave);
            }
            foreach (var destination in Object.FindObjectsByType<ClinicDestination>()) UpgradeBin(destination.transform);
            var game = Object.FindAnyObjectByType<ClinicGame>();
            if (game) AddForceps(game.transform);
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

        // --- Pinzas para recoger punzocortantes ---

        // Las pinzas quedan inactivas bajo la simulación; al jugar se colocan en la mano. Su eje largo
        // apunta a +Z y la punta queda en el origen, donde se sostiene el objeto.
        static void AddForceps(Transform game)
        {
            if (Deep(game, ClinicSimulation.ForcepsName)) return;
            var holder = Group(ClinicSimulation.ForcepsName, game);
            var model = Model("field-medicine/forceps.glb", holder, Vector3.zero, .03f, 0, false);
            Bounds Measure()
            {
                var renderers = model.GetComponentsInChildren<Renderer>();
                Bounds b = new Bounds(holder.InverseTransformPoint(renderers[0].bounds.center), Vector3.zero);
                foreach (var r in renderers)
                    foreach (var corner in Corners(r.bounds)) b.Encapsulate(holder.InverseTransformPoint(corner));
                return b;
            }
            Bounds bounds = Measure();
            if (bounds.size.x >= bounds.size.y && bounds.size.x >= bounds.size.z) model.localRotation = Quaternion.Euler(0, 90, 0) * model.localRotation;
            else if (bounds.size.y >= bounds.size.z) model.localRotation = Quaternion.Euler(90, 0, 0) * model.localRotation;
            bounds = Measure();
            model.localScale *= .17f / Mathf.Max(bounds.size.z, 1e-4f);
            bounds = Measure();
            model.localPosition -= new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
            holder.gameObject.SetActive(false);
        }

        // --- Recipientes ---

        // Sustituye la caja provisional por el bote de pedal real (o el contenedor rígido de punzocortantes),
        // con el color de su destino. Las piezas anteriores se ocultan, no se borran, y el colisionador se ajusta.
        static void UpgradeBin(Transform bin)
        {
            var body = bin.Find("Cuerpo");
            if (!body) return;
            bool sharps = bin.Find("Abertura segura");
            var color = body.GetComponent<Renderer>().sharedMaterial;
            var model = bin.Find(BinFeedback.ModelName);
            if (!model)
            {
                string path = sharps ? "dental-practice/sharps-bin.glb" : "dental-practice/pedal-waste-bin.glb";
                // El modelo mira hacia +Z; el frente del recipiente (rótulo y pedal) está en -Z.
                model = Model(path, bin, Vector3.zero, sharps ? .50f : .70f, 180, false);
                model.name = BinFeedback.ModelName;
                var trim = Material("Detalle de recipiente", new Color(.16f, .18f, .18f), .4f);
                // Bote de acero con tapa del color del destino; contenedor rígido rojo para punzocortantes.
                foreach (var renderer in model.GetComponentsInChildren<Renderer>())
                    renderer.sharedMaterials = renderer.sharedMaterials.Select(material =>
                    {
                        string n = material ? material.name.ToLowerInvariant() : "";
                        if (n.Contains("yellow") || (n.Contains("white") && !sharps)) return color;
                        return n.Contains("teal") ? trim : material;
                    }).ToArray();
                foreach (var name in new[] { "Tapa", "Pedal", "Abertura segura" })
                {
                    var old = bin.Find(name);
                    if (old) old.gameObject.SetActive(false);
                }
                body.GetComponent<Renderer>().enabled = false;
            }

            Bounds Local(IEnumerable<Renderer> renderers)
            {
                Bounds result = default;
                bool first = true;
                foreach (var renderer in renderers)
                    foreach (var corner in Corners(renderer.bounds))
                    {
                        Vector3 p = bin.InverseTransformPoint(corner);
                        if (first) { result = new Bounds(p, Vector3.zero); first = false; } else result.Encapsulate(p);
                    }
                return result;
            }
            var parts = model.GetComponentsInChildren<Renderer>();
            Bounds all = Local(parts);
            // Cuerpo del bote (sin tapa, base ni pedal) para el rótulo y la bolsa.
            var shellParts = parts.Where(r => r.GetComponentsInParent<Transform>().Any(t => t.name == "pedal-waste-bin_1")).ToArray();
            Bounds shell = Local(shellParts.Length > 0 ? shellParts : parts);
            var rimParts = parts.Where(r => r.GetComponentsInParent<Transform>().Any(t => t.name == "pedal-waste-bin_2")).ToArray();
            float rim = rimParts.Length > 0 ? Local(rimParts).max.y : shell.max.y;

            var collider = body.GetComponent<BoxCollider>();
            if (collider)
            {
                collider.center = all.center - body.localPosition;
                collider.size = all.size;
            }
            // El rótulo se apoya en el frente del cuerpo.
            var sign = bin.Cast<Transform>().FirstOrDefault(t => t.name == bin.name);
            if (sign)
            {
                float fit = Mathf.Min(1, shell.size.x * .95f / .45f);
                sign.localScale = new Vector3(fit, fit, 1);
                sign.localPosition = new Vector3(shell.center.x, shell.min.y + shell.size.y * (sharps ? .45f : .6f), shell.min.z - .006f);
            }
            var previousBag = bin.Find("Bolsa interior");
            if (previousBag) Object.DestroyImmediate(previousBag.gameObject);
            if (!sharps)
            {
                // Bolsa del color del destino, visible al abrir la tapa.
                var bag = Material("Bolsa · " + bin.name, Color.Lerp(color.GetColor("_BaseColor"), Color.black, .25f), .55f);
                float diameter = Mathf.Min(shell.size.x, shell.size.z) * .86f;
                Cylinder("Bolsa interior", bin, new Vector3(shell.center.x, rim + .002f, shell.center.z),
                    new Vector3(diameter, .002f, diameter), bag);
            }
        }

        static IEnumerable<Vector3> Corners(Bounds b)
        {
            for (int c = 0; c < 8; c++)
                yield return b.center + Vector3.Scale(b.extents, new Vector3((c & 1) == 0 ? -1 : 1, (c & 2) == 0 ? -1 : 1, (c & 4) == 0 ? -1 : 1));
        }

        // --- Autoclave ---

        // El OBJ del autoclave es una sola malla con una submalla por material. La puerta (materiales 18-22)
        // se separa en su propio objeto para que gire sobre su bisagra; se marca también la boca de la cámara.
        static void SplitAutoclaveDoor(Transform autoclave)
        {
            // La fuente es siempre el OBJ original: volver a ejecutar el paso regenera las mallas sin duplicar objetos.
            var model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Terceros/dental-practice/autoclave.obj");
            var original = model ? model.GetComponentInChildren<MeshRenderer>() : null;
            if (!original) return;
            var source = original.GetComponent<MeshFilter>().sharedMesh;
            var names = original.sharedMaterials.Select(material => material ? material.name : "").ToArray();
            bool IsDoor(int i) => new[] { "_18", "_19", "_20", "_21", "_22" }.Any(suffix => names[i].EndsWith(suffix));
            var doorSlots = Enumerable.Range(0, source.subMeshCount).Where(IsDoor).ToArray();
            var bodySlots = Enumerable.Range(0, source.subMeshCount).Where(i => !IsDoor(i)).ToArray();
            int mouthSlot = Enumerable.Range(0, source.subMeshCount).FirstOrDefault(i => names[i].EndsWith("_6"));
            if (doorSlots.Length == 0) return;

            Mesh Subset(string name, int[] slots)
            {
                string path = Generated + "/" + name + ".asset";
                var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                bool created = !mesh;
                if (created) mesh = new Mesh();
                mesh.Clear();
                mesh.name = name;
                // Solo los vértices que usa cada parte: así sus límites corresponden a su geometría.
                var remap = new Dictionary<int, int>();
                var sourceVertices = source.vertices;
                var sourceNormals = source.normals;
                var sourceUv = source.uv;
                var vertices = new List<Vector3>();
                var normals = new List<Vector3>();
                var uv = new List<Vector2>();
                var triangles = slots.Select(slot => source.GetTriangles(slot).Select(index =>
                {
                    if (!remap.TryGetValue(index, out int mapped))
                    {
                        mapped = remap[index] = vertices.Count;
                        vertices.Add(sourceVertices[index]);
                        if (sourceNormals.Length > index) normals.Add(sourceNormals[index]);
                        if (sourceUv.Length > index) uv.Add(sourceUv[index]);
                    }
                    return mapped;
                }).ToArray()).ToArray();
                mesh.indexFormat = vertices.Count > 65535 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
                mesh.SetVertices(vertices);
                if (normals.Count == vertices.Count) mesh.SetNormals(normals);
                if (uv.Count == vertices.Count) mesh.SetUVs(0, uv);
                mesh.subMeshCount = slots.Length;
                for (int i = 0; i < slots.Length; i++) mesh.SetTriangles(triangles[i], i);
                mesh.RecalculateBounds();
                if (created) AssetDatabase.CreateAsset(mesh, path);
                else EditorUtility.SetDirty(mesh);
                return mesh;
            }
            var body = Subset("Autoclave · cuerpo", bodySlots);
            var door = Subset("Autoclave · puerta", doorSlots);
            if (Deep(autoclave, ClinicSimulation.AutoclaveDoorName)) return;
            var filter = autoclave.GetComponentsInChildren<MeshFilter>().FirstOrDefault(f => f.sharedMesh == source);
            if (!filter) return;
            var renderer = filter.GetComponent<MeshRenderer>();
            var materials = renderer.sharedMaterials;
            filter.sharedMesh = body;
            renderer.sharedMaterials = bodySlots.Select(i => materials[i]).ToArray();
            var doorObject = new GameObject(ClinicSimulation.AutoclaveDoorName, typeof(MeshFilter), typeof(MeshRenderer));
            doorObject.transform.SetParent(filter.transform.parent, false);
            doorObject.transform.localPosition = filter.transform.localPosition;
            doorObject.transform.localRotation = filter.transform.localRotation;
            doorObject.transform.localScale = filter.transform.localScale;
            doorObject.GetComponent<MeshFilter>().sharedMesh = door;
            var doorRenderer = doorObject.GetComponent<MeshRenderer>();
            doorRenderer.sharedMaterials = doorSlots.Select(i => materials[i]).ToArray();
            doorRenderer.lightProbeUsage = renderer.lightProbeUsage;
            doorRenderer.reflectionProbeUsage = renderer.reflectionProbeUsage;
            // Centro de la boca de la cámara, a partir de los vértices de su aro.
            var ring = source.GetTriangles(mouthSlot).Select(index => source.vertices[index]).ToArray();
            Vector3 center = ring.Aggregate(Vector3.zero, (sum, v) => sum + v) / Mathf.Max(1, ring.Length);
            var mouth = Group(ClinicSimulation.AutoclaveMouthName, filter.transform.parent);
            mouth.position = filter.transform.TransformPoint(center);
        }

        // --- Paciente ---

        static void AddPatient(Transform chair)
        {
            // Para volver a ajustar al paciente, se borra «Paciente simulado» y se ejecuta de nuevo el menú.
            if (chair.Find(PatientName)) return;
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
