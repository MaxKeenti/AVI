using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConsultorioSeguro.Editor
{
    public static partial class ConstructorConsultorio
    {
        const string CarpetaAcabados = "Assets/Materiales/Acabados";

        // Regenera la escena desde el constructor, incluidos los desplazamientos de las salas.
        [MenuItem("Consultorio Seguro/Actualizar acabados visuales")]
        public static void ActualizarAcabados()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Construir();
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
        }

        public static void ActualizarYCapturar()
        {
            ActualizarAcabados();
            CapturarAcabados();
        }

        static Material Acabado(string nombre, Color color, float suavidad, bool textura = false, bool juntas = false)
        {
            AsegurarCarpeta(CarpetaAcabados);
            string ruta = $"{CarpetaAcabados}/{nombre}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(ruta);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, ruta);
            }
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", suavidad);
            if (!textura) material.SetTexture("_BaseMap", null);
            if (textura)
            {
                string rutaTextura = $"{CarpetaAcabados}/{nombre}.png";
                if (!File.Exists(rutaTextura))
                {
                    const int n = 256;
                    Texture2D mapa = new(n, n, TextureFormat.RGB24, false);
                    Color[] pixeles = new Color[n * n];
                    System.Random azar = new(87);
                    for (int y = 0; y < n; y++)
                    for (int x = 0; x < n; x++)
                    {
                        float valor = 0.94f + (float)azar.NextDouble() * 0.06f;
                        if (juntas && (x < 2 || y < 2)) valor = 0.65f;
                        pixeles[y * n + x] = new Color(valor, valor, valor);
                    }
                    mapa.SetPixels(pixeles);
                    mapa.Apply();
                    File.WriteAllBytes(rutaTextura, mapa.EncodeToPNG());
                    Object.DestroyImmediate(mapa);
                    AssetDatabase.ImportAsset(rutaTextura);
                    TextureImporter importador = (TextureImporter)AssetImporter.GetAtPath(rutaTextura);
                    importador.wrapMode = TextureWrapMode.Repeat;
                    importador.anisoLevel = 8;
                    importador.SaveAndReimport();
                }
                material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(rutaTextura));
            }
            EditorUtility.SetDirty(material);
            return material;
        }

        static void Revestir(string ruta, Material material)
        {
            GameObject objeto = GameObject.Find(ruta);
            if (objeto != null && objeto.TryGetComponent(out Renderer renderer)) renderer.sharedMaterial = material;
        }

        static void Detalle(Transform padre, string nombre, Vector3 posicion, Vector3 escala, Material material)
        {
            SinColision(Caja(nombre, padre, posicion, escala, material));
        }

        static void AplicarAcabados()
        {
            GameObject anterior = GameObject.Find("Acabados arquitectónicos");
            if (anterior != null) Object.DestroyImmediate(anterior);
            Transform raiz = Raiz("Acabados arquitectónicos");
            Material porcelanato = Acabado("Porcelanato marfil", new Color(0.79f, 0.81f, 0.79f), 0.32f, true, true);
            porcelanato.SetTextureScale("_BaseMap", new Vector2(7, 10));
            Material yeso = Acabado("Yeso cálido", new Color(0.88f, 0.87f, 0.82f), 0.12f);
            Material verde = Acabado("Verde salvia", new Color(0.29f, 0.46f, 0.44f), 0.24f);
            Material roble = Acabado("Roble miel", new Color(0.61f, 0.43f, 0.27f), 0.22f, true);
            Material oscuro = Acabado("Aluminio grafito", new Color(0.12f, 0.18f, 0.2f), 0.45f);
            Material blanco = Acabado("Cerámica blanca", new Color(0.94f, 0.95f, 0.92f), 0.45f);
            Material asfalto = Acabado("Asfalto granular", new Color(0.24f, 0.26f, 0.28f), 0.08f, true);
            asfalto.SetTextureScale("_BaseMap", new Vector2(40, 6));
            Revestir("Edificio/Piso", porcelanato);
            Revestir("Entorno urbano/Calle", asfalto);
            foreach (string pared in new[] { "Muro fondo", "Muro izquierdo", "Muro derecho" }) Revestir("Edificio/" + pared, yeso);
            Revestir("Recepción/Mostrador de recepción/Cuerpo", verde);
            Revestir("Recepción/Mostrador de recepción/Cubierta", blanco);
            foreach (float x in new[] { -3.48f, 3.48f })
            {
                Detalle(raiz, "Zoclo lateral", new Vector3(x, 0.065f, -2), new Vector3(0.025f, 0.13f, 10), oscuro);
                Detalle(raiz, "Protector sanitario", new Vector3(x, 0.96f, 0), new Vector3(0.026f, 0.15f, 6), verde);
                Detalle(raiz, "Cornisa interior", new Vector3(x, 2.69f, -2), new Vector3(0.08f, 0.12f, 10), blanco);
            }
            Detalle(raiz, "Zoclo posterior", new Vector3(0, 0.065f, 2.98f), new Vector3(7, 0.13f, 0.025f), oscuro);
            Detalle(raiz, "Friso posterior", new Vector3(0, 0.96f, 2.98f), new Vector3(7, 0.15f, 0.025f), verde);
            for (int i = 0; i < 19; i++)
                Detalle(raiz, "Listón de recepción", new Vector3(1.55f + i * 0.094f, 0.54f, -5.462f), new Vector3(0.035f, 0.94f, 0.035f), roble);
            // Panel de acento bajo los carteles existentes; no ocupa la circulación.
            Detalle(raiz, "Revestimiento sala de espera", new Vector3(-3.47f, 0.53f, -5.3f), new Vector3(0.025f, 1.04f, 2.9f), verde);
            for (int i = 0; i < 8; i++)
                Detalle(raiz, "Junta del panel", new Vector3(-3.45f, 0.53f, -6.65f + i * 0.38f), new Vector3(0.012f, 0.95f, 0.012f), oscuro);
            // Frentes y tiradores que dan escala al mobiliario, sin nuevos colisionadores.
            foreach (Vector3 centro in new[] { CentroCarrito, CentroMesa })
            {
                for (int i = 0; i < 3; i++)
                {
                    float y = 0.19f + i * 0.23f;
                    Detalle(raiz, "Frente de cajón", new Vector3(centro.x, y, centro.z - 0.257f), new Vector3(0.75f, 0.21f, 0.014f), blanco);
                    Detalle(raiz, "Tirador de cajón", new Vector3(centro.x, y + 0.055f, centro.z - 0.28f), new Vector3(0.23f, 0.018f, 0.025f), oscuro);
                }
                Detalle(raiz, "Zócalo de gabinete", new Vector3(centro.x, 0.035f, centro.z - 0.253f), new Vector3(0.76f, 0.07f, 0.012f), oscuro);
            }
            for (int i = 0; i < 4; i++)
            {
                float z = -1.88f + i * 0.58f;
                Detalle(raiz, "Puerta esterilización", new Vector3(2.842f, 0.47f, z), new Vector3(0.016f, 0.78f, 0.555f), blanco);
                Detalle(raiz, "Tirador esterilización", new Vector3(2.816f, 0.72f, z), new Vector3(0.03f, 0.018f, 0.2f), oscuro);
            }
            for (int i = 0; i < 2; i++)
            {
                float x = -1.92f + i * 0.64f;
                Detalle(raiz, "Puerta de materiales", new Vector3(x, 0.53f, 2.54f), new Vector3(0.61f, 0.9f, 0.018f), blanco);
                Detalle(raiz, "Tirador de materiales", new Vector3(x + (i == 0 ? 0.23f : -0.23f), 0.75f, 2.514f), new Vector3(0.018f, 0.16f, 0.03f), oscuro);
            }
            // Marcos con profundidad para la ventana exterior.
            foreach (float x in new[] { 1.38f, 2.25f, 3.12f })
                Detalle(raiz, "Montante ventana", new Vector3(x, 1.45f, -7.255f), new Vector3(0.045f, 1.17f, 0.06f), oscuro);
            foreach (float y in new[] { 0.88f, 2.02f })
                Detalle(raiz, "Marco ventana", new Vector3(2.25f, y, -7.255f), new Vector3(1.78f, 0.045f, 0.06f), oscuro);
            for (int i = 0; i < 9; i++)
                Detalle(raiz, "Lama marquesina", new Vector3(-0.92f + i * 0.23f, 2.285f, -7.7f), new Vector3(0.07f, 0.05f, 0.9f), roble);
            // Marcos de luminarias: una fuente suave por panel, sin geometría que tape su luz.
            foreach (Vector3 p in new[] { new Vector3(-1.5f, 2.785f, 0), new Vector3(1.5f, 2.785f, 0.5f), new Vector3(0, 2.785f, -5) })
                Detalle(raiz, "Bastidor luminaria", p, new Vector3(1.28f, 0.025f, 0.38f), oscuro);
            RenderSettings.ambientSkyColor = new Color(0.57f, 0.65f, 0.72f);
            RenderSettings.ambientEquatorColor = new Color(0.43f, 0.47f, 0.48f);
            RenderSettings.ambientGroundColor = new Color(0.26f, 0.28f, 0.29f);
            foreach (Light luz in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (luz.type == LightType.Directional)
                {
                    luz.color = new Color(1, 0.92f, 0.79f);
                    luz.intensity = 1.35f;
                    luz.shadowBias = 0.035f;
                    luz.shadowNormalBias = 0.25f;
                    luz.transform.rotation = Quaternion.Euler(38, -28, 0);
                }
                else if (luz.name == "Luz de techo")
                {
                    luz.color = new Color(1, 0.96f, 0.89f);
                    luz.intensity = 2.1f;
                    luz.shadows = LightShadows.Soft;
                    luz.shadowBias = 0.025f;
                    luz.shadowNormalBias = 0.15f;
                }
            }
            foreach (Camera camara in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)) camara.allowMSAA = true;
            EditorUtility.SetDirty(porcelanato);
            EditorUtility.SetDirty(asfalto);
        }

        // Capturas reproducibles para revisar los acabados sin modificar la cámara del jugador.
        public static void CapturarAcabados()
        {
            EditorSceneManager.OpenScene(RutaEscena);
            GameObject interfaz = GameObject.Find("Interfaz");
            if (interfaz != null) interfaz.SetActive(false);
            Camera camara = new GameObject("Cámara de revisión").AddComponent<Camera>();
            camara.nearClipPlane = 0.05f;
            camara.fieldOfView = 68;
            Directory.CreateDirectory("Capturas");
            Capturar(camara, "barrio", new Vector3(-48, 40, -13.5f), new Vector3(0, 8, -13.5f));
            Capturar(camara, "avenida", new Vector3(-37, 1.7f, -13.5f), new Vector3(0, 9, -7));
            Capturar(camara, "torre-clinica", new Vector3(0, 2, -19), new Vector3(0, 8, -4));
            Capturar(camara, "fachada", new Vector3(0, 1.7f, -13), new Vector3(0, 1.6f, -7));
            Capturar(camara, "recepcion", new Vector3(0, 1.65f, -6.8f), new Vector3(-.3f, 1.3f, .5f));
            Capturar(camara, "sala-espera", new Vector3(-0.8f, 1.65f, -6.6f), new Vector3(-3.1f, 1f, -4.9f));
            Capturar(camara,"radiografia",new Vector3(-2.8f,1.65f,19.2f),new Vector3(-6.2f,1.2f,22.7f));
            Capturar(camara,"clasificacion",new Vector3(-2.6f,1.65f,6.8f),new Vector3(-4.8f,1.45f,7.55f));
            Capturar(camara,"materiales",new Vector3(-4.4f,1.65f,15.1f),new Vector3(-4.6f,1.75f,17.49f));
            Capturar(camara,"pasillo",new Vector3(0,1.65f,2),new Vector3(0,1.6f,20));
            Capturar(camara,"esterilizacion",new Vector3(3,1.65f,6),new Vector3(7,1,7.2f));
            Capturar(camara, "consultorio", new Vector3(3.1f, 1.7f, 11.9f), new Vector3(4.2f, 1.1f, 15.3f));
            var ocultos=Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Where(r=>r.enabled && (r.bounds.max.y>3 || r.name.StartsWith("Techo") || r.name=="Lámpara de techo")).ToArray();
            foreach(var r in ocultos) r.enabled=false;
            camara.orthographic=true; camara.orthographicSize=10.5f;
            Capturar(camara,"distribucion",new Vector3(0,35,8.5f),new Vector3(0,0,8.5f));
            foreach(var r in ocultos) r.enabled=true;
            if(interfaz!=null) interfaz.SetActive(true);
            Object.DestroyImmediate(camara.gameObject);
        }

        static void Capturar(Camera camara, string nombre, Vector3 posicion, Vector3 objetivo)
        {
            camara.transform.position = posicion;
            camara.transform.LookAt(objetivo);
            if(nombre=="distribucion") camara.transform.rotation=Quaternion.Euler(90,0,-90);
            RenderTexture destino = new(1600, 1000, 24) { antiAliasing = 4 };
            camara.targetTexture = destino;
            bool compilacionAsincrona = ShaderUtil.allowAsyncCompilation;
            ShaderUtil.allowAsyncCompilation = false;
            try
            {
                // La primera renderización inicializa los recursos de URP en modo por lotes.
                camara.Render();
                camara.Render();
            }
            finally { ShaderUtil.allowAsyncCompilation = compilacionAsincrona; }
            RenderTexture.active = destino;
            Texture2D imagen = new(1600, 1000, TextureFormat.RGB24, false);
            imagen.ReadPixels(new Rect(0, 0, 1600, 1000), 0, 0);
            imagen.Apply();
            File.WriteAllBytes($"Capturas/{nombre}.png", imagen.EncodeToPNG());
            camara.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(imagen);
            Object.DestroyImmediate(destino);
        }
    }
}
