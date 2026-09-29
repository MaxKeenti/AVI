using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Editor
{
    public static partial class ConstructorConsultorio
    {
        const string CarpetaKenney = "Assets/Terceros/kenney-furniture-kit";

        [MenuItem("Consultorio Seguro/Actualizar modelos de terceros")]
        public static void ActualizarModelos()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Construir();
            HerramientasAtribucion.ActualizarCatalogo();
            HerramientasAtribucion.Exportar();
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
        }

        public static void ActualizarModelosYCapturar()
        {
            ActualizarModelos();
            CapturarAcabados();
        }

        static void AplicarModelos()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>($"{CarpetaKenney}/chairModernFrameCushion.obj") == null)
                throw new InvalidOperationException("Falta Furniture Kit: restaura Assets/Terceros/kenney-furniture-kit.");

            string ruta = $"{CarpetaKenney}/Atribucion.asset";
            Atribucion credito = AssetDatabase.LoadAssetAtPath<Atribucion>(ruta);
            if (credito == null)
            {
                credito = ScriptableObject.CreateInstance<Atribucion>();
                AssetDatabase.CreateAsset(credito, ruta);
            }
            credito.titulo = "Furniture Kit (mobiliario interior y urbano)";
            credito.autor = "Kenney";
            credito.url = "https://kenney.nl/assets/furniture-kit";
            credito.licencia = "CC0 1.0 Universal";
            credito.fechaConsulta = "2026-09-28";
            credito.modificaciones = "Selección de chairModernFrameCushion, pottedPlant, computerScreen, bathroomSink, bench y trashcan. Escala, orientación y posición; materiales adaptados a URP y a la paleta del consultorio. Geometría original sin modificar.";
            EditorUtility.SetDirty(credito);

            GameObject anterior = GameObject.Find("Modelos de terceros");
            if (anterior != null) Object.DestroyImmediate(anterior);
            Transform raiz = Raiz("Modelos de terceros");
            Transform sala = GameObject.Find("Recepción/Sala de espera").transform;
            foreach (Transform silla in sala)
            {
                OcultarMallas(silla.gameObject);
                ColocarModelo(raiz, "chairModernFrameCushion", silla.position, 0.95f, -90);
            }
            OcultarMallas(GameObject.Find("Recepción/Maceta"));
            OcultarMallas(GameObject.Find("Recepción/Planta"));
            ColocarModelo(raiz, "pottedPlant", new Vector3(-3.1f, 0, -6.6f), 1.05f, 0);
            OcultarMallas(GameObject.Find("Recepción/Mostrador de recepción/Monitor"));
            ColocarModelo(raiz, "computerScreen", new Vector3(2.7f, 0.9f, -5.1f), 0.34f, 0);
            OcultarMallas(GameObject.Find("Recepción/Lavabo/Pedestal"));
            OcultarMallas(GameObject.Find("Recepción/Lavabo/Tarja"));
            ColocarModelo(raiz, "bathroomSink", new Vector3(-3.22f, 0, -3.7f), 0.95f, 90);
            AplicarModelosClinicos(raiz);
            HerramientasAtribucion.ActualizarCatalogo();
        }

        static void OcultarMallas(GameObject objeto)
        {
            if (objeto == null) throw new InvalidOperationException("No se encontró el objeto original para sustituir su aspecto.");
            // Se conservan objetos, colisionadores y componentes de interacción.
            foreach (Renderer renderer in objeto.GetComponentsInChildren<Renderer>()) renderer.enabled = false;
        }

        static void ColocarModelo(Transform raiz, string nombre, Vector3 posicion, float alto, float giro)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>($"{CarpetaKenney}/{nombre}.obj");
            if (asset == null) throw new InvalidOperationException($"Falta el modelo {nombre}.");
            Transform pivote = Grupo(nombre, raiz, Vector3.zero);
            GameObject modelo = (GameObject)PrefabUtility.InstantiatePrefab(asset, pivote);
            Renderer[] renderers = modelo.GetComponentsInChildren<Renderer>();
            Bounds limites = renderers[0].bounds;
            foreach (Renderer renderer in renderers.Skip(1)) limites.Encapsulate(renderer.bounds);
            float escala = alto / limites.size.y;
            modelo.transform.localScale *= escala;
            modelo.transform.localPosition = new Vector3(-limites.center.x, -limites.min.y, -limites.center.z) * escala;
            foreach (Renderer renderer in renderers)
                renderer.sharedMaterials = renderer.sharedMaterials.Select(MaterialKenney).ToArray();
            pivote.localPosition = posicion;
            pivote.localRotation = Quaternion.Euler(0, giro, 0);
        }

        static Material MaterialKenney(Material original)
        {
            string nombre = original != null ? original.name.Replace(" (Instance)", "") : "Blanco";
            Color color = original != null && original.HasProperty("_Color") ? original.color : Color.white;
            float suavidad = 0.3f;
            switch (nombre)
            {
                case "carpetBlue": color = new Color(0.29f, 0.46f, 0.44f); suavidad = 0.18f; break;
                case "plant": color = new Color(0.2f, 0.43f, 0.29f); suavidad = 0.16f; break;
                case "wood": color = new Color(0.64f, 0.47f, 0.33f); break;
                case "woodDark": color = new Color(0.3f, 0.24f, 0.18f); break;
                case "metalDark": color = new Color(0.12f, 0.17f, 0.19f); break;
            }
            Material material = Acabado("Modelo_" + nombre + "_" + ColorUtility.ToHtmlStringRGB(color), color, suavidad);
            if (nombre.StartsWith("metal"))
            {
                material.SetFloat("_Metallic", 0.65f);
                material.SetFloat("_Smoothness", 0.55f);
            }
            return material;
        }
    }
}
