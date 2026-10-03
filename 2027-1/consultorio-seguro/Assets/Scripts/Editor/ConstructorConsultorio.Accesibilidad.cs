using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Editor
{
    public static partial class ConstructorConsultorio
    {
        static void AmueblarEsperaDeEntrada(Transform padre)
        {
            var espera = Grupo("Espera junto a entrada", padre, Vector3.zero);
            foreach (float z in new[] { -5.45f, -4.35f, -3.25f })
            {
                var silla = ColocarModelo(espera, "chairModernFrameCushion", new Vector3(2.96f, 0, z), .95f, 90);
                var colision = silla.gameObject.AddComponent<BoxCollider>();
                colision.center = Vector3.up * .45f;
                colision.size = new Vector3(.65f, .9f, .65f);
            }
            var mesa = ColocarModelo(espera, "sideTable", new Vector3(2.9f, 0, -2.15f), .52f, 0);
            var caja = mesa.gameObject.AddComponent<BoxCollider>();
            caja.center = Vector3.up * .26f; caja.size = new Vector3(.65f, .52f, .65f);
            ColocarModelo(espera, "pottedPlant", new Vector3(2.85f, 0, -6.35f), 1.25f, 0);
            Informativo(espera.gameObject, "Espera de pacientes", "Toma asiento después de registrarte. Mantén libre el acceso central a recepción y a las salas.");
            var bienvenida = GameObject.Find("Recepción ampliada y guías/Bienvenida recepción").transform;
            bienvenida.position = new Vector3(3.46f, 1.85f, -4.2f);
            bienvenida.localScale = Vector3.one * .8f;
        }

        static void ConfigurarContornos()
        {
            foreach (var residuo in Object.FindObjectsByType<Residuo>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                residuo.gameObject.AddComponent<ResaltadoInstrumento>();
            foreach (var paso in Object.FindObjectsByType<Paso>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                // En los pasos se resaltan los objetos pequeños, no el mobiliario ni el paciente.
                if (paso.name == "Jeringa con anestesia" || paso.name == "Paquete de gasas")
                    paso.gameObject.AddComponent<ResaltadoInstrumento>();
            }
            foreach (string perfil in new[] { "PC", "Mobile" })
            {
                var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>($"Assets/Settings/{perfil}_Renderer.asset");
                var efecto = renderer.rendererFeatures.OfType<ContornoInstrumentos>().FirstOrDefault();
                if (efecto == null)
                {
                    efecto = ScriptableObject.CreateInstance<ContornoInstrumentos>();
                    efecto.name = "Contorno de instrumentos";
                    AssetDatabase.AddObjectToAsset(efecto, renderer);
                    renderer.rendererFeatures.Add(efecto);
                }
                Asignar(efecto, "sombreado", Shader.Find("Hidden/Consultorio/ContornoInstrumentos"));
                efecto.Create();
                var serializado = new SerializedObject(renderer);
                var mapa = serializado.FindProperty("m_RendererFeatureMap");
                mapa.arraySize = renderer.rendererFeatures.Count;
                for (int i = 0; i < renderer.rendererFeatures.Count; i++)
                {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(renderer.rendererFeatures[i], out string _, out long id);
                    mapa.GetArrayElementAtIndex(i).longValue = id;
                }
                serializado.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(efecto);
                renderer.SetDirty();
                EditorUtility.SetDirty(renderer);
            }
        }
    }
}
