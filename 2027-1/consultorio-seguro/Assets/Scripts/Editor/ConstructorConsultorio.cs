using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Editor
{
    // Genera Assets/Escenas/Consultorio.unity: una maqueta hecha con primitivas
    // del consultorio, con los recipientes, las cinco acciones (cada una en su
    // área, con su zona y su hoja de práctica), el jugador y la interfaz. El
    // edificio, la recepción y la calle están en ConstructorConsultorio.Entorno.cs.
    // Las primitivas son provisionales; el equipo las sustituye por modelos de
    // Assets/Terceros conservando los componentes (Residuo, Contenedor, Paso...).
    public static partial class ConstructorConsultorio
    {
        const string RutaEscena = "Assets/Escenas/Consultorio.unity";
        const string CarpetaResiduos = "Assets/Datos/Residuos";
        const string CarpetaEscenarios = "Assets/Datos/Escenarios";
        const string CarpetaMateriales = "Assets/Materiales/Provisionales";

        const string CreditosEquipo =
            "<b>Consultorio Seguro</b>\n" +
            "Prototipo de simulador para el manejo de Residuos Peligrosos Biológico-Infecciosos en un consultorio.\n" +
            "Optativa III: Simuladores Virtuales · 7NM78 · UPIICSA, IPN · 2027-1\n\n" +
            "<b>Equipo 7</b>\n" +
            "Alatorre Fuentes Eduardo\nCruz Cruz Guillermo\nFlores Roa Jorge Alejandro\n" +
            "González Calzada Maximiliano\nNeri Mondragón Mónica\nSoto Soto Héctor\n\n" +
            "Profesora: Bustamante Tranquilino Rocío";

        // Medidas interiores del consultorio, en metros. El piso está en y = 0 y la
        // puerta que da a la recepción en z negativo; la calle queda más allá.
        const float Ancho = 7f;
        const float Fondo = 6f;
        const float Alto = 2.8f;

        // En la banqueta de enfrente, mirando la clínica al otro lado del paso peatonal.
        static readonly Vector3 PosicionInicio = new(0, 0, -18.3f);
        const float RotacionInicio = 0;
        // Superficies donde quedan los residuos de cada acción (centro de la cara superior).
        static readonly Vector3 CentroCarrito = new(-1.8f, 0.85f, -1f);
        static readonly Vector3 CentroCharola = new(Ancho / 2 - 0.35f, 0.96f, -0.1f);
        static readonly Vector3 CentroEstante = new(-1.6f, 1f, 2.75f);
        static readonly Vector3 CentroMesa = new(1f, 0.85f, 0.1f);
        static readonly Vector3 CentroMesaRayosX = new(2.75f, 0.8f, 2.5f);

        static readonly Color Acero = new(0.72f, 0.74f, 0.78f);
        static readonly Color Blanco = new(0.9f, 0.91f, 0.92f);
        static readonly Color Plastico = new(0.78f, 0.9f, 0.98f);
        static readonly Color FondoTablero = new(0.13f, 0.17f, 0.22f);

        // Área de cada acción: dónde se para el usuario (acceso), el volumen que
        // detecta su llegada y dónde está su hoja de práctica.
        struct Area
        {
            public Vector3 acceso;
            public Vector3 centroZona;
            public Vector2 tamanoZona;
            public Vector3 tablero;
            public float rotacionTablero;
            public bool conAtril;
        }

        static Area AreaDe(AccionSimulacion accion) => accion switch
        {
            AccionSimulacion.Clasificacion => new Area
            {
                acceso = new Vector3(-1.8f, 0, -1.75f), centroZona = new Vector3(-1.8f, 0, -1f), tamanoZona = new Vector2(2f, 2f),
                tablero = new Vector3(-1.8f, 0, -0.45f), conAtril = true,
            },
            AccionSimulacion.Esterilizacion => new Area
            {
                acceso = new Vector3(2.3f, 0, -0.6f), centroZona = new Vector3(2.6f, 0, -1f), tamanoZona = new Vector2(1.8f, 3f),
                tablero = new Vector3(Ancho / 2 - 0.01f, 1.65f, -0.35f), rotacionTablero = 90,
            },
            AccionSimulacion.MuestraMateriales => new Area
            {
                acceso = new Vector3(-1.6f, 0, 1.9f), centroZona = new Vector3(-1.6f, 0, 1.9f), tamanoZona = new Vector2(2.2f, 1.8f),
                tablero = new Vector3(-1.6f, 1.75f, Fondo / 2 - 0.01f),
            },
            AccionSimulacion.Procedimiento => new Area
            {
                acceso = new Vector3(0.9f, 0, -0.5f), centroZona = new Vector3(0.5f, 0, 0.2f), tamanoZona = new Vector2(2.6f, 2.6f),
                tablero = new Vector3(1f, 0, 0.65f), conAtril = true,
            },
            _ => new Area
            {
                acceso = new Vector3(2f, 0, 2.1f), centroZona = new Vector3(2f, 0, 2.25f), tamanoZona = new Vector2(2.4f, 1.5f),
                tablero = new Vector3(2.15f, 1.85f, Fondo / 2 - 0.01f),
            },
        };

        static Dictionary<string, Material> materiales;
        static Sprite spriteRedondeado;

        [MenuItem("Consultorio Seguro/Construir consultorio")]
        static void ConstruirDesdeMenu()
        {
            if (File.Exists(RutaEscena) && !EditorUtility.DisplayDialog("Construir consultorio",
                    "La escena Consultorio ya existe y se reemplazará por la maqueta original; se perderán los cambios hechos en ella.\n\n" +
                    "Los datos de Assets/Datos (residuos, escenarios, atribuciones) se conservan.",
                    "Reemplazar", "Cancelar"))
                return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            if (RecursosTmpListos)
                Construir();
            else
                ImportarRecursosTmp(Construir);
        }

        // Desde la terminal, en dos pasos porque la importación de paquetes es asíncrona:
        // Unity -batchmode -projectPath . -executeMethod ConsultorioSeguro.Editor.ConstructorConsultorio.ImportarRecursosTmpPorLotes
        // Unity -batchmode -quit -projectPath . -executeMethod ConsultorioSeguro.Editor.ConstructorConsultorio.Construir
        public static void ImportarRecursosTmpPorLotes()
        {
            if (RecursosTmpListos)
                EditorApplication.Exit(0);
            else
                ImportarRecursosTmp(() => EditorApplication.Exit(RecursosTmpListos ? 0 : 1));
        }

        public static void Construir()
        {
            if (!RecursosTmpListos)
                throw new InvalidOperationException(
                    "Faltan los TMP Essential Resources. Impórtalos con Window > TextMeshPro > Import TMP Essential Resources o con ImportarRecursosTmpPorLotes.");

            materiales = new Dictionary<string, Material>();
            spriteRedondeado = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            CatalogoAtribuciones catalogo = HerramientasAtribucion.ActualizarCatalogo();

            Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            ConfigurarIluminacion();

            ConstruirEdificio(Raiz("Edificio"));
            ConstruirRecepcion(Raiz("Recepción"));
            ConstruirEntornoUrbano(Raiz("Entorno urbano"));
            ConstruirRecipientes(Raiz("Recipientes"));
            Transform muebles = Raiz("Mobiliario");
            ConstruirUnidadDental(muebles);
            Transform cabezaPaciente = ConstruirPaciente(muebles);
            ConstruirCarrito(muebles);
            ConstruirMesaInstrumental(muebles);
            ConstruirEstante(muebles);
            ConstruirAreaEsterilizacion(muebles);
            EquipoRayosX rayosX = ConstruirEquipoRayosX(muebles);

            Transform raizEscenarios = Raiz("Escenarios");
            List<Escenario> escenarios = new();
            foreach (CatalogoInicial.Escenario datos in CatalogoInicial.Escenarios)
            {
                Escenario escenario = ConstruirEscenario(datos, raizEscenarios);
                if (datos.accion == AccionSimulacion.Procedimiento)
                    ConfigurarProcedimiento(escenario, cabezaPaciente);
                else if (datos.accion == AccionSimulacion.Radiografia)
                    ConfigurarRadiografia(escenario, rayosX);
                escenarios.Add(escenario);
            }

            (ControladorPrimeraPersona jugador, Interactor interactor) = ConstruirJugador();

            GameObject simulacion = new("Simulacion");
            GestorSimulacion gestor = simulacion.AddComponent<GestorSimulacion>();
            Asignar(gestor, "escenarios", escenarios.Cast<Object>().ToList());
            Asignar(gestor, "jugador", jugador);
            Asignar(gestor, "interactor", interactor);

            AudioSource fuente = simulacion.AddComponent<AudioSource>();
            fuente.playOnAwake = false;
            Asignar(simulacion.AddComponent<SonidosRetroalimentacion>(), "gestor", gestor);

            ConstruirInterfaz(gestor, interactor, catalogo);

            AplicarAcabados();
            AplicarModelos();
            DistribuirSalas();
            AfinarInterior();
            AsegurarCarpeta(Path.GetDirectoryName(RutaEscena));
            EditorSceneManager.SaveScene(escena, RutaEscena);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(RutaEscena, true) };
            AssetDatabase.SaveAssets();
            Debug.Log($"Consultorio construido en {RutaEscena} con {escenarios.Count} escenarios.");
        }

        // === Recursos ===

        static bool RecursosTmpListos => Resources.Load<TMP_Settings>("TMP Settings") != null;

        static void ImportarRecursosTmp(Action alTerminar)
        {
            AssetDatabase.ImportPackageCallback completado = null;
            AssetDatabase.ImportPackageFailedCallback fallido = null;
            completado = _ =>
            {
                AssetDatabase.importPackageCompleted -= completado;
                AssetDatabase.importPackageFailed -= fallido;
                AssetDatabase.Refresh();
                alTerminar();
            };
            fallido = (_, error) =>
            {
                AssetDatabase.importPackageCompleted -= completado;
                AssetDatabase.importPackageFailed -= fallido;
                Debug.LogError($"No se pudieron importar los TMP Essential Resources: {error}");
                if (Application.isBatchMode)
                    EditorApplication.Exit(1);
            };

            AssetDatabase.importPackageCompleted += completado;
            AssetDatabase.importPackageFailed += fallido;
            string carpetaUgui = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.ugui").resolvedPath;
            AssetDatabase.ImportPackage(Path.Combine(carpetaUgui, "Package Resources", "TMP Essential Resources.unitypackage"), false);
        }

        public static void AsegurarCarpeta(string ruta)
        {
            ruta = ruta.Replace('\\', '/');
            if (AssetDatabase.IsValidFolder(ruta))
                return;

            string padre = Path.GetDirectoryName(ruta).Replace('\\', '/');
            AsegurarCarpeta(padre);
            AssetDatabase.CreateFolder(padre, Path.GetFileName(ruta));
        }

        static T ObtenerOCrear<T>(string ruta, Action<T> inicializar) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(ruta);
            if (asset != null)
                return asset;

            AsegurarCarpeta(Path.GetDirectoryName(ruta));
            asset = ScriptableObject.CreateInstance<T>();
            inicializar(asset);
            AssetDatabase.CreateAsset(asset, ruta);
            return asset;
        }

        static Material Mat(Color color, float metalico = 0, float suavidad = 0.35f, bool emisivo = false)
        {
            string nombre = $"{(emisivo ? "Luz" : "Mat")}_{ColorUtility.ToHtmlStringRGB(color)}";
            if (materiales.TryGetValue(nombre, out Material existente))
                return existente;

            string ruta = $"{CarpetaMateriales}/{nombre}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(ruta);
            if (material == null)
            {
                AsegurarCarpeta(CarpetaMateriales);
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                material.SetColor("_BaseColor", color);
                material.SetFloat("_Metallic", metalico);
                material.SetFloat("_Smoothness", suavidad);
                if (emisivo)
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", color * 2f);
                    material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                }
                AssetDatabase.CreateAsset(material, ruta);
            }

            materiales[nombre] = material;
            return material;
        }

        static Material Metal => Mat(Acero, 0.8f, 0.6f);

        // Las charolas contrastan con el instrumental metálico para que las agujas se vean.
        static Material Charola => Mat(new Color(0.3f, 0.45f, 0.55f), 0, 0.4f);

        // === Geometría ===

        static Transform Raiz(string nombre) => new GameObject(nombre).transform;

        static GameObject Primitiva(PrimitiveType tipo, string nombre, Transform padre, Vector3 posicion, Vector3 escala,
            Material material, Vector3 rotacion = default)
        {
            GameObject objeto = GameObject.CreatePrimitive(tipo);
            objeto.name = nombre;
            objeto.transform.SetParent(padre, false);
            objeto.transform.localPosition = posicion;
            objeto.transform.localRotation = Quaternion.Euler(rotacion);
            objeto.transform.localScale = escala;
            objeto.GetComponent<Renderer>().sharedMaterial = material;
            return objeto;
        }

        static GameObject Caja(string nombre, Transform padre, Vector3 posicion, Vector3 escala, Material material,
            Vector3 rotacion = default) =>
            Primitiva(PrimitiveType.Cube, nombre, padre, posicion, escala, material, rotacion);

        static Transform Grupo(string nombre, Transform padre, Vector3 posicion, Vector3 rotacion = default)
        {
            Transform grupo = new GameObject(nombre).transform;
            grupo.SetParent(padre, false);
            grupo.localPosition = posicion;
            grupo.localRotation = Quaternion.Euler(rotacion);
            return grupo;
        }

        // Los objetos pequeños (agujas, hojas de bisturí) son difíciles de apuntar:
        // su colisionador mide al menos 10 cm por lado.
        static void AgrandarColisionador(GameObject objeto, float minimo = 0.1f)
        {
            Object.DestroyImmediate(objeto.GetComponent<Collider>());
            Bounds limites = objeto.GetComponent<MeshFilter>().sharedMesh.bounds;
            Vector3 escala = objeto.transform.lossyScale;
            Vector3 tamano = limites.size;
            for (int eje = 0; eje < 3; eje++)
                if (tamano[eje] * escala[eje] < minimo)
                    tamano[eje] = minimo / escala[eje];

            BoxCollider colisionador = objeto.AddComponent<BoxCollider>();
            colisionador.center = limites.center;
            colisionador.size = tamano;
        }

        static void Informativo(GameObject objeto, string nombre, string descripcion)
        {
            ObjetoInformativo informativo = objeto.AddComponent<ObjetoInformativo>();
            Asignar(informativo, "nombre", nombre);
            Asignar(informativo, "descripcion", descripcion);
        }

        // Placa con texto, de 80 x 60 cm si no se indica otro tamaño. La cara legible mira
        // hacia -z local, así que rotacionY apunta +z hacia la pared: izquierda -90,
        // derecha 90, fondo 0, fachada 0.
        static (Transform grupo, TextMeshPro texto) Placa(Transform padre, string nombre, Vector3 posicion,
            float rotacionY, Color fondo, Vector2 tamano = default)
        {
            if (tamano == default)
                tamano = new Vector2(0.8f, 0.6f);

            Transform grupo = Grupo(nombre, padre, posicion, new Vector3(0, rotacionY, 0));
            Caja("Placa", grupo, Vector3.zero, new Vector3(tamano.x, tamano.y, 0.02f), Mat(fondo));

            GameObject objetoTexto = new("Texto", typeof(RectTransform));
            objetoTexto.transform.SetParent(grupo, false);
            objetoTexto.transform.localPosition = new Vector3(0, 0, -0.012f);

            TextMeshPro texto = objetoTexto.AddComponent<TextMeshPro>();
            texto.font = TMP_Settings.defaultFontAsset;
            texto.rectTransform.sizeDelta = tamano - new Vector2(0.08f, 0.08f);
            texto.enableAutoSizing = true;
            texto.fontSizeMin = 0.1f;
            // En TextMeshPro 3D una unidad de tamaño mide ~0.1 m de alto; el tope deja que el texto llene la placa.
            texto.fontSizeMax = 8 * tamano.y;
            texto.alignment = TextAlignmentOptions.Center;
            texto.color = fondo.grayscale > 0.6f ? new Color(0.1f, 0.1f, 0.1f) : Color.white;
            return (grupo, texto);
        }

        static void Letrero(Transform padre, string nombre, Vector3 posicion, float rotacionY, string titulo,
            string cuerpo, Color fondo, Vector2 tamano = default)
        {
            (_, TextMeshPro texto) = Placa(padre, nombre, posicion, rotacionY, fondo, tamano);
            texto.text = string.IsNullOrEmpty(cuerpo)
                ? $"<b>{titulo.ToUpperInvariant()}</b>"
                : $"<b>{titulo.ToUpperInvariant()}</b>\n<size=55%>{cuerpo}</size>";
        }

        static void ConfigurarIluminacion()
        {
            RenderSettings.skybox = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Skybox.mat");
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.72f, 0.76f, 0.82f);
            RenderSettings.ambientEquatorColor = new Color(0.56f, 0.57f, 0.58f);
            RenderSettings.ambientGroundColor = new Color(0.3f, 0.3f, 0.31f);

            // Niebla lejana para ocultar el borde del mundo; no alcanza el interior.
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 35;
            RenderSettings.fogEndDistance = 110;
            RenderSettings.fogColor = new Color(0.74f, 0.8f, 0.87f);

            // El sol ilumina la fachada (que mira a -z); con sombras para que el techo oscurezca el interior.
            Light sol = new GameObject("Sol").AddComponent<Light>();
            sol.type = LightType.Directional;
            sol.intensity = 1.2f;
            sol.color = new Color(1f, 0.96f, 0.9f);
            sol.shadows = LightShadows.Soft;
            sol.transform.rotation = Quaternion.Euler(45, 30, 0);
        }

        static void ConstruirRecipientes(Transform raiz)
        {
            (Destino destino, float z)[] recipientes =
            {
                (Destino.BolsaRoja, -1.5f),
                (Destino.BolsaAmarilla, -0.6f),
                (Destino.Punzocortantes, 0.3f),
                (Destino.BasuraComun, 1.2f),
            };

            const float x = -Ancho / 2 + 0.4f;
            foreach ((Destino destino, float z) in recipientes)
            {
                // El de punzocortantes va sobre un soporte a la altura de la mano.
                if (destino == Destino.Punzocortantes)
                    Caja("Soporte", raiz, new Vector3(x, 0.4f, z), new Vector3(0.35f, 0.8f, 0.35f), Mat(new Color(0.5f, 0.52f, 0.55f)));
                Recipiente(raiz, destino, new Vector3(x, destino == Destino.Punzocortantes ? 0.8f : 0, z));
                Letrero(raiz, $"Letrero {destino.Nombre()}", new Vector3(-Ancho / 2 + 0.01f, 1.6f, z), -90,
                    destino.Nombre(), destino.Descripcion(), destino.Color());
            }
        }

        static void Recipiente(Transform padre, Destino destino, Vector3 posicion)
        {
            Transform recipiente = Grupo(destino.Nombre(), padre, posicion);
            Color color = destino.Color();
            Color borde = color * 0.7f;
            borde.a = 1;

            if (destino == Destino.Punzocortantes)
            {
                Caja("Recipiente rígido", recipiente, new Vector3(0, 0.14f, 0), new Vector3(0.24f, 0.28f, 0.18f), Mat(color));
                Caja("Tapa", recipiente, new Vector3(0, 0.295f, 0), new Vector3(0.26f, 0.03f, 0.2f), Mat(borde));
            }
            else
            {
                Caja("Bote", recipiente, new Vector3(0, 0.3f, 0), new Vector3(0.42f, 0.6f, 0.42f), Mat(color));
                Caja("Borde de la bolsa", recipiente, new Vector3(0, 0.62f, 0), new Vector3(0.45f, 0.05f, 0.45f), Mat(borde));
            }

            Asignar(recipiente.gameObject.AddComponent<Contenedor>(), "destino", destino);
        }

        static void ConstruirUnidadDental(Transform padre)
        {
            Transform unidad = Grupo("Unidad dental", padre, new Vector3(0, 0, 0.6f));
            Material tapiz = Mat(new Color(0.3f, 0.58f, 0.66f), 0, 0.5f);
            Caja("Base", unidad, new Vector3(0, 0.22f, 0), new Vector3(0.45f, 0.44f, 0.6f), Mat(Blanco));
            Caja("Asiento", unidad, new Vector3(0, 0.5f, -0.1f), new Vector3(0.6f, 0.12f, 0.7f), tapiz);
            Caja("Piernas", unidad, new Vector3(0, 0.45f, -0.75f), new Vector3(0.55f, 0.1f, 0.6f), tapiz);
            Caja("Respaldo", unidad, new Vector3(0, 0.72f, 0.55f), new Vector3(0.6f, 0.1f, 0.75f), tapiz, new Vector3(-30, 0, 0));
            Caja("Cabecera", unidad, new Vector3(0, 0.97f, 0.98f), new Vector3(0.3f, 0.08f, 0.25f), tapiz, new Vector3(-30, 0, 0));
            Caja("Poste de lámpara", unidad, new Vector3(-0.55f, 0.9f, 0.9f), new Vector3(0.05f, 1.8f, 0.05f), Mat(Blanco));
            Caja("Brazo de lámpara", unidad, new Vector3(-0.3f, 1.8f, 0.9f), new Vector3(0.5f, 0.05f, 0.05f), Mat(Blanco));
            Caja("Lámpara", unidad, new Vector3(0, 1.75f, 0.9f), new Vector3(0.25f, 0.08f, 0.12f), Mat(new Color(1f, 0.97f, 0.88f), 0, 0.5f, true));

            Informativo(unidad.gameObject, "Unidad dental",
                "Sillón donde se atiende al paciente. Sus superficies de contacto se cubren con barreras que se cambian entre pacientes.");
        }

        static Transform ConstruirPaciente(Transform padre)
        {
            Transform paciente = Grupo("Paciente (maniquí)", padre, Vector3.zero);
            Material piel = Mat(new Color(0.85f, 0.7f, 0.6f));
            Material ropa = Mat(new Color(0.35f, 0.4f, 0.5f));

            GameObject torso = Primitiva(PrimitiveType.Capsule, "Torso", paciente, new Vector3(0, 0.86f, 1.1f), new Vector3(0.4f, 0.35f, 0.25f), ropa, new Vector3(60, 0, 0));
            Primitiva(PrimitiveType.Capsule, "Piernas", paciente, new Vector3(0, 0.68f, -0.2f), new Vector3(0.3f, 0.45f, 0.2f), ropa, new Vector3(90, 0, 0));
            GameObject cabeza = Primitiva(PrimitiveType.Sphere, "Cabeza", paciente, new Vector3(0, 1.1f, 1.5f), Vector3.one * 0.22f, piel);

            Informativo(torso, "Paciente (maniquí)",
                "Representa al paciente durante los procedimientos. El simulador no trabaja con pacientes reales.");
            return cabeza.transform;
        }

        static void ConstruirCarrito(Transform padre)
        {
            Transform carrito = Grupo("Carrito de curación", padre, new Vector3(CentroCarrito.x, 0, CentroCarrito.z));
            Caja("Gabinete", carrito, new Vector3(0, 0.4f, 0), new Vector3(0.8f, 0.8f, 0.5f), Mat(new Color(0.72f, 0.8f, 0.86f)));
            Caja("Charola", carrito, new Vector3(0, 0.825f, 0), new Vector3(0.84f, 0.05f, 0.54f), Charola);
            Informativo(carrito.gameObject, "Carrito de curación",
                "Aquí quedan los residuos al terminar una consulta, antes de separarlos en sus recipientes.");
        }

        static void ConstruirMesaInstrumental(Transform padre)
        {
            Transform mesa = Grupo("Mesa de instrumental", padre, new Vector3(CentroMesa.x, 0, CentroMesa.z));
            Caja("Gabinete", mesa, new Vector3(0, 0.4f, 0), new Vector3(0.8f, 0.8f, 0.5f), Mat(Blanco));
            Caja("Charola", mesa, new Vector3(0, 0.825f, 0), new Vector3(0.84f, 0.05f, 0.54f), Charola);
            Informativo(mesa.gameObject, "Mesa de instrumental",
                "Aquí quedan el instrumental y los consumibles después de cada procedimiento.");
        }

        static void ConstruirEstante(Transform padre)
        {
            Transform estante = Grupo("Estante de materiales", padre, new Vector3(CentroEstante.x, 0, CentroEstante.z));
            Caja("Mueble", estante, new Vector3(0, 0.5f, 0), new Vector3(1.3f, 1f, 0.4f), Mat(new Color(0.6f, 0.47f, 0.33f)));
            Informativo(estante.gameObject, "Estante de materiales",
                "Consumibles e instrumental del consultorio. Antes de usar un material, piensa qué residuo generará.");
        }

        static void ConstruirAreaEsterilizacion(Transform padre)
        {
            const float x = Ancho / 2 - 0.35f;
            Transform mostrador = Grupo("Mostrador de esterilización", padre, new Vector3(x, 0, -1f));
            Caja("Gabinete", mostrador, new Vector3(0, 0.45f, 0), new Vector3(0.6f, 0.9f, 2.4f), Mat(Blanco));
            Caja("Cubierta", mostrador, new Vector3(0, 0.92f, 0), new Vector3(0.62f, 0.04f, 2.42f), Metal);

            GameObject charola = Caja("Charola de instrumental sucio", mostrador, new Vector3(0, CentroCharola.y - 0.01f, CentroCharola.z + 1f),
                new Vector3(0.4f, 0.02f, 0.6f), Charola);
            Informativo(charola, "Charola de instrumental sucio",
                "Instrumental usado que espera su lavado. Lo reutilizable se esteriliza; lo desechable no.");

            GameObject tarja = Caja("Tarja", mostrador, new Vector3(0, 0.945f, 0.25f), new Vector3(0.4f, 0.05f, 0.5f), Mat(new Color(0.5f, 0.52f, 0.55f), 0.8f, 0.6f));
            Informativo(tarja, "Tarja de lavado",
                "El instrumental reutilizable se lava aquí, con guantes gruesos, antes de empaquetarlo y esterilizarlo.");

            // Un segundo recipiente de punzocortantes para no cruzar el consultorio con una aguja en la mano.
            Recipiente(padre, Destino.Punzocortantes, new Vector3(x, 0.94f, -1.3f));

            Transform autoclave = Grupo("Autoclave", padre, new Vector3(x, 0.94f, -1.85f));
            Caja("Cuerpo", autoclave, new Vector3(0, 0.175f, 0), new Vector3(0.45f, 0.35f, 0.5f), Mat(new Color(0.92f, 0.92f, 0.9f)));
            Primitiva(PrimitiveType.Cylinder, "Puerta", autoclave, new Vector3(-0.23f, 0.175f, 0), new Vector3(0.26f, 0.01f, 0.26f), Metal, new Vector3(0, 0, 90));
            Asignar(autoclave.gameObject.AddComponent<Contenedor>(), "destino", Destino.Esterilizacion);

            Letrero(padre, "Letrero área de esterilización", new Vector3(Ancho / 2 - 0.01f, 1.65f, -1.55f), 90,
                Destino.Esterilizacion.Nombre(), Destino.Esterilizacion.Descripcion(), Destino.Esterilizacion.Color());
        }

        class EquipoRayosX
        {
            public GameObject interruptor;
            public GameObject luzEncendido;
            public GameObject cabezal;
            public GameObject barreraColocada;
            public GameObject segmento;
            public TransicionTransform brazo;
            public GameObject disparador;
        }

        static EquipoRayosX ConstruirEquipoRayosX(Transform padre)
        {
            EquipoRayosX equipo = new();
            Transform raiz = Grupo("Equipo de rayos X", padre, Vector3.zero);
            const float z = Fondo / 2;

            Caja("Soporte de pared", raiz, new Vector3(0.9f, 1.75f, z - 0.05f), new Vector3(0.3f, 0.3f, 0.1f), Mat(Blanco));

            // Plegado contra la pared; el Paso "Posicionar el brazo" lo gira hacia el paciente.
            Transform pivote = Grupo("Brazo", raiz, new Vector3(0.9f, 1.75f, z - 0.15f), new Vector3(0, 90, 0));
            equipo.brazo = pivote.gameObject.AddComponent<TransicionTransform>();
            Asignar(equipo.brazo, "posicionFinal", pivote.localPosition);
            Asignar(equipo.brazo, "rotacionFinal", new Vector3(0, 30, 0));
            Asignar(equipo.brazo, "duracion", 1.5f);

            equipo.segmento = Caja("Segmento", pivote, new Vector3(0, 0, -0.55f), new Vector3(0.08f, 0.08f, 1.1f), Mat(Blanco));
            equipo.cabezal = Caja("Cabezal", pivote, new Vector3(0, -0.14f, -1.1f), new Vector3(0.2f, 0.2f, 0.25f), Mat(new Color(0.95f, 0.95f, 0.93f)));
            Primitiva(PrimitiveType.Cylinder, "Cono", equipo.cabezal.transform, new Vector3(0, -0.7f, 0), new Vector3(0.5f, 0.4f, 0.4f), Mat(new Color(0.3f, 0.3f, 0.32f)));
            equipo.barreraColocada = Caja("Barrera colocada", equipo.cabezal.transform, Vector3.zero, new Vector3(1.15f, 1.15f, 1.15f), Mat(Plastico, 0, 0.8f));
            Object.DestroyImmediate(equipo.barreraColocada.GetComponent<Collider>());

            equipo.interruptor = Caja("Interruptor", raiz, new Vector3(2f, 1.3f, z - 0.03f), new Vector3(0.25f, 0.35f, 0.05f), Mat(new Color(0.25f, 0.26f, 0.28f)));
            equipo.luzEncendido = Primitiva(PrimitiveType.Sphere, "Luz de encendido", equipo.interruptor.transform, new Vector3(0, 0.3f, -0.6f), new Vector3(0.16f, 0.12f, 0.8f), Mat(new Color(0.2f, 0.9f, 0.3f), 0, 0.5f, true));
            Object.DestroyImmediate(equipo.luzEncendido.GetComponent<Collider>());

            equipo.disparador = Caja("Disparador", raiz, new Vector3(2.3f, 1.3f, z - 0.03f), new Vector3(0.08f, 0.14f, 0.05f), Mat(new Color(0.25f, 0.26f, 0.28f)));
            Primitiva(PrimitiveType.Sphere, "Botón", equipo.disparador.transform, new Vector3(0, 0.2f, -0.6f), new Vector3(0.45f, 0.25f, 0.7f), Mat(new Color(0.8f, 0.15f, 0.12f)));

            Transform mesita = Grupo("Mesita del equipo de rayos X", raiz, new Vector3(CentroMesaRayosX.x, 0, CentroMesaRayosX.z));
            Caja("Cuerpo", mesita, new Vector3(0, 0.39f, 0), new Vector3(0.5f, 0.78f, 0.4f), Mat(Blanco));
            Caja("Cubierta", mesita, new Vector3(0, 0.79f, 0), new Vector3(0.52f, 0.02f, 0.42f), Charola);
            Informativo(mesita.gameObject, "Mesita del equipo de rayos X",
                "Aquí se dejan las barreras y fundas nuevas, y las usadas al retirarlas.");
            return equipo;
        }

        static (ControladorPrimeraPersona, Interactor) ConstruirJugador()
        {
            GameObject jugador = new("Jugador");
            jugador.transform.SetPositionAndRotation(PosicionInicio, Quaternion.Euler(0, RotacionInicio, 0));

            CharacterController controlador = jugador.AddComponent<CharacterController>();
            controlador.height = 1.75f;
            controlador.radius = 0.3f;
            controlador.center = new Vector3(0, 0.875f, 0);
            // Con el valor por defecto (1 mm) los pasos cortos de fotogramas muy rápidos se ignoran y el jugador no avanza.
            controlador.minMoveDistance = 0;

            GameObject objetoCamara = new("Camara") { tag = "MainCamera" };
            objetoCamara.transform.SetParent(jugador.transform, false);
            objetoCamara.transform.localPosition = new Vector3(0, 1.6f, 0);
            Camera camara = objetoCamara.AddComponent<Camera>();
            camara.nearClipPlane = 0.02f;
            camara.fieldOfView = 70;
            objetoCamara.AddComponent<AudioListener>();

            Transform sujecion = Grupo("PuntoSujecion", objetoCamara.transform, new Vector3(0.2f, -0.2f, 0.45f));

            ControladorPrimeraPersona controladorJugador = jugador.AddComponent<ControladorPrimeraPersona>();
            Asignar(controladorJugador, "camara", objetoCamara.transform);

            Interactor interactor = jugador.AddComponent<Interactor>();
            Asignar(interactor, "camara", camara);
            Asignar(interactor, "puntoSujecion", sujecion);
            return (controladorJugador, interactor);
        }

        // === Escenarios ===

        static Escenario ConstruirEscenario(CatalogoInicial.Escenario datos, Transform padre)
        {
            DatosEscenario datosEscenario = ObtenerOCrear<DatosEscenario>($"{CarpetaEscenarios}/{datos.archivo}.asset", d =>
            {
                d.accion = datos.accion;
                d.nombre = datos.nombre;
                d.instrucciones = datos.instrucciones;
            });

            GameObject objeto = new($"Escenario {datos.archivo}");
            objeto.transform.SetParent(padre, false);
            Escenario escenario = objeto.AddComponent<Escenario>();
            Asignar(escenario, "datos", datosEscenario);

            foreach (CatalogoInicial.Residuo residuo in datos.residuos)
            {
                DatosResiduo datosResiduo = ObtenerOCrear<DatosResiduo>($"{CarpetaResiduos}/{datos.archivo}/{residuo.archivo}.asset", d =>
                {
                    d.nombre = residuo.nombre;
                    d.destinoCorrecto = residuo.destino;
                    d.explicacion = residuo.explicacion;
                });
                ConstruirResiduo(residuo, datos.superficie, datosResiduo, objeto.transform);
            }

            Area area = AreaDe(datos.accion);
            ConstruirZona(area, objeto.transform);
            ConstruirTablero(escenario, area, objeto.transform);
            return escenario;
        }

        // El objeto Zona se coloca donde se para el usuario; el volumen cubre toda el área.
        static void ConstruirZona(Area area, Transform padre)
        {
            GameObject zona = new("Zona");
            zona.transform.SetParent(padre, false);
            zona.transform.localPosition = area.acceso;

            BoxCollider volumen = zona.AddComponent<BoxCollider>();
            volumen.isTrigger = true;
            volumen.center = area.centroZona - area.acceso + Vector3.up * Alto / 2;
            volumen.size = new Vector3(area.tamanoZona.x, Alto, area.tamanoZona.y);
            zona.AddComponent<ZonaEscenario>();
        }

        static void ConstruirTablero(Escenario escenario, Area area, Transform padre)
        {
            Vector3 posicion = area.tablero;
            if (area.conAtril)
            {
                Caja("Atril", padre, new Vector3(posicion.x, 0.6f, posicion.z + 0.03f), new Vector3(0.05f, 1.2f, 0.05f), Metal);
                posicion.y = 1.45f;
            }

            (Transform grupo, TextMeshPro texto) = Placa(padre, "Hoja de práctica", posicion, area.rotacionTablero, FondoTablero);
            TableroPractica tablero = grupo.gameObject.AddComponent<TableroPractica>();
            Asignar(tablero, "escenario", escenario);
            Asignar(tablero, "texto", texto);
        }

        static void ConstruirResiduo(CatalogoInicial.Residuo residuo, CatalogoInicial.Superficie superficie, DatosResiduo datos, Transform padre)
        {
            // Un cilindro de Unity mide 2 unidades de alto; cubos y esferas, 1.
            float mitadAltura = residuo.forma == PrimitiveType.Cylinder ? residuo.escala.y : residuo.escala.y / 2;
            Vector3 posicion = Lugar(superficie, residuo.lugar) + Vector3.up * mitadAltura;

            Material material = residuo.color == Acero ? Metal : Mat(residuo.color);
            GameObject objeto = Primitiva(residuo.forma, residuo.nombre, padre, posicion, residuo.escala, material);
            AgrandarColisionador(objeto);
            Asignar(objeto.AddComponent<Residuo>(), "datos", datos);
        }

        // Ocho lugares en el carrito y la mesa (4 x 2), seis en la charola (2 x 3) y el estante, tres en la mesita.
        static Vector3 Lugar(CatalogoInicial.Superficie superficie, int lugar) => superficie switch
        {
            CatalogoInicial.Superficie.Carrito => CentroCarrito + Rejilla(lugar),
            CatalogoInicial.Superficie.CharolaEsterilizacion => CentroCharola + new Vector3(lugar % 2 == 0 ? -0.1f : 0.1f, 0, -0.2f + 0.2f * (lugar / 2)),
            CatalogoInicial.Superficie.Estante => CentroEstante + new Vector3(-0.5f + 0.2f * lugar, 0, 0),
            CatalogoInicial.Superficie.MesaRayosX => CentroMesaRayosX + new Vector3(-0.15f + 0.15f * lugar, 0, 0),
            _ => CentroMesa + Rejilla(lugar),
        };

        static Vector3 Rejilla(int lugar) => new(-0.3f + 0.2f * (lugar % 4), 0, lugar < 4 ? -0.12f : 0.12f);

        static void ConfigurarProcedimiento(Escenario escenario, Transform cabezaPaciente)
        {
            Transform raiz = escenario.transform;
            GameObject jeringa = Caja("Jeringa con anestesia", raiz, Lugar(CatalogoInicial.Superficie.MesaInstrumental, 6) + Vector3.up * 0.01f,
                new Vector3(0.12f, 0.02f, 0.02f), Metal);
            AgrandarColisionador(jeringa);
            GameObject gasas = Caja("Paquete de gasas", raiz, Lugar(CatalogoInicial.Superficie.MesaInstrumental, 7) + Vector3.up * 0.015f,
                new Vector3(0.09f, 0.03f, 0.09f), Mat(Color.white));
            AgrandarColisionador(gasas);

            Paso anestesia = CrearPaso(jeringa, "Aplicar la anestesia",
                "Se aplicó la anestesia. La aguja ya tuvo contacto con el paciente, así que será un residuo punzocortante.");
            UnityEventTools.AddBoolPersistentListener(anestesia.AlCompletar, jeringa.SetActive, false);
            UnityEventTools.AddBoolPersistentListener(anestesia.AlReiniciar, jeringa.SetActive, true);

            Paso extraccion = CrearPaso(cabezaPaciente.gameObject, "Extraer la pieza dental",
                "Pieza extraída. Ahora contén el sangrado con una gasa del paquete de la mesa.");
            Paso gasa = CrearPaso(gasas, "Colocar una gasa",
                "Procedimiento terminado. Clasifica los residuos que quedaron en la mesa de instrumental.");

            SecuenciaPasos secuencia = raiz.gameObject.AddComponent<SecuenciaPasos>();
            Asignar(secuencia, "pasos", new List<Object> { anestesia, extraccion, gasa });
            Asignar(escenario, "secuencia", secuencia);
        }

        static void ConfigurarRadiografia(Escenario escenario, EquipoRayosX equipo)
        {
            Paso encender = CrearPaso(equipo.interruptor, "Encender el equipo",
                "El equipo está encendido y se prepara. Espera a que termine antes de continuar.", 3f);
            UnityEventTools.AddBoolPersistentListener(encender.AlCompletar, equipo.luzEncendido.SetActive, true);
            UnityEventTools.AddBoolPersistentListener(encender.AlReiniciar, equipo.luzEncendido.SetActive, false);

            Paso barreras = CrearPaso(equipo.cabezal, "Colocar las barreras de protección",
                "Barreras colocadas en el cabezal y el sensor. Evitan que el equipo se contamine entre pacientes.");
            UnityEventTools.AddBoolPersistentListener(barreras.AlCompletar, equipo.barreraColocada.SetActive, true);
            UnityEventTools.AddBoolPersistentListener(barreras.AlReiniciar, equipo.barreraColocada.SetActive, false);

            Paso posicionar = CrearPaso(equipo.segmento, "Posicionar el brazo", "Brazo colocado frente al paciente.");
            UnityEventTools.AddVoidPersistentListener(posicionar.AlCompletar, equipo.brazo.Aplicar);
            UnityEventTools.AddVoidPersistentListener(posicionar.AlReiniciar, equipo.brazo.Revertir);

            Paso tomar = CrearPaso(equipo.disparador, "Tomar la radiografía",
                "Toma realizada. Las barreras que retiraste quedaron en la mesita junto al equipo: clasifícalas.");

            SecuenciaPasos secuencia = escenario.gameObject.AddComponent<SecuenciaPasos>();
            Asignar(secuencia, "pasos", new List<Object> { encender, barreras, posicionar, tomar });
            UnityEventTools.AddBoolPersistentListener(secuencia.AlTerminar, equipo.barreraColocada.SetActive, false);
            Asignar(escenario, "secuencia", secuencia);

            equipo.luzEncendido.SetActive(false);
            equipo.barreraColocada.SetActive(false);
        }

        static Paso CrearPaso(GameObject objeto, string accion, string mensaje, float espera = 0)
        {
            Paso paso = objeto.AddComponent<Paso>();
            Asignar(paso, "accion", accion);
            Asignar(paso, "mensaje", mensaje);
            Asignar(paso, "espera", espera);
            return paso;
        }

        // === Interfaz ===

        static readonly Color FondoPanel = new(0.07f, 0.09f, 0.12f, 0.94f);
        static readonly Color FondoHud = new(0, 0, 0, 0.55f);
        static readonly Color ColorBoton = new(0.16f, 0.42f, 0.62f);
        static readonly Color TextoSecundario = new(0.75f, 0.8f, 0.86f);

        static void ConstruirInterfaz(GestorSimulacion gestor, Interactor interactor, CatalogoAtribuciones catalogo)
        {
            GameObject eventos = new("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventos.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();

            GameObject objetoCanvas = new("Interfaz", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            objetoCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler escalador = objetoCanvas.GetComponent<CanvasScaler>();
            escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            escalador.referenceResolution = new Vector2(1920, 1080);
            escalador.matchWidthOrHeight = 0.5f;

            InterfazSimulador interfaz = objetoCanvas.AddComponent<InterfazSimulador>();
            Transform raiz = objetoCanvas.transform;

            // --- HUD ---
            RectTransform hud = Rect("HUD", raiz);
            Estirar(hud);

            Image reticula = Imagen(Rect("Reticula", hud), AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"), new Color(1, 1, 1, 0.85f));
            Anclar(reticula.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(10, 10));

            RectTransform marcador = PanelAjustable("Marcador", hud, FondoHud, 16, 4);
            Anclar(marcador, new Vector2(0, 1), new Vector2(24, -24), new Vector2(680, 0));
            TMP_Text textoEscenario = Texto("TextoEscenario", marcador, "", 30, TextAlignmentOptions.Left, FontStyles.Bold);
            TMP_Text textoMarcador = Texto("TextoMarcador", marcador, "", 24, TextAlignmentOptions.Left);

            RectTransform indicacion = PanelAjustable("Indicacion", hud, FondoHud, 14, 0);
            indicacion.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Anclar(indicacion, new Vector2(0.5f, 0), new Vector2(0, 150), Vector2.zero);
            TMP_Text textoIndicacion = Texto("TextoIndicacion", indicacion, "", 28, TextAlignmentOptions.Center);
            textoIndicacion.textWrappingMode = TextWrappingModes.NoWrap;

            // --- Mensaje de retroalimentación ---
            RectTransform mensaje = PanelAjustable("Mensaje", raiz, FondoPanel, 24, 8);
            Anclar(mensaje, new Vector2(0.5f, 1), new Vector2(0, -30), new Vector2(1000, 0));
            TMP_Text tituloMensaje = Texto("TituloMensaje", mensaje, "", 32, TextAlignmentOptions.Left, FontStyles.Bold);
            TMP_Text textoMensaje = Texto("TextoMensaje", mensaje, "", 24, TextAlignmentOptions.Left);

            // --- Menú principal ---
            RectTransform menu = PanelCentral("PanelMenu", raiz, 1000);
            Texto("Titulo", menu, "Consultorio Seguro", 60, TextAlignmentOptions.Center, FontStyles.Bold);
            Texto("Subtitulo", menu, "Simulador para el manejo de Residuos Peligrosos Biológico-Infecciosos\nNOM-087-ECOL-SSA1-2002",
                24, TextAlignmentOptions.Center, FontStyles.Normal, TextoSecundario);
            Texto("Descripcion", menu,
                "Recorre el consultorio: cada área tiene una práctica. Acércate a una para ver sus instrucciones; " +
                "la hoja de práctica de cada área lleva tu marcador y te deja repetirla.",
                26, TextAlignmentOptions.Left);
            Texto("Controles", menu, "WASD: caminar · Ratón: mirar · Shift: correr · E: tomar, depositar o interactuar · Esc: pausa",
                22, TextAlignmentOptions.Center, FontStyles.Normal, TextoSecundario);
            Boton("BotonEntrar", menu, "Entrar al consultorio", 76, gestor.Entrar);
            RectTransform filaMenu = Fila("BotonesMenu", menu);
            Boton("BotonCreditos", filaMenu, "Créditos", 64, interfaz.AbrirCreditos);
            Boton("BotonSalir", filaMenu, "Salir", 64, gestor.Salir);

            // --- Pausa ---
            RectTransform pausa = PanelCentral("PanelPausa", raiz, 640);
            Texto("Titulo", pausa, "Pausa", 48, TextAlignmentOptions.Center, FontStyles.Bold);
            Boton("BotonReanudar", pausa, "Reanudar", 68, gestor.Reanudar);
            Button reiniciarPractica = Boton("BotonReiniciarPractica", pausa, "Reiniciar la práctica actual", 68, gestor.ReiniciarEnFoco);
            Boton("BotonMenu", pausa, "Menú principal", 68, gestor.IrAlMenu);

            // --- Créditos ---
            RectTransform creditos = Rect("PanelCreditos", raiz);
            Imagen(creditos, spriteRedondeado, FondoPanel);
            Anclar(creditos, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1100, 900));
            Columna(creditos, 18, 40);
            Texto("Titulo", creditos, "Créditos", 48, TextAlignmentOptions.Center, FontStyles.Bold);
            TMP_Text textoCreditos = ListaDesplazable(creditos);
            Boton("BotonCerrar", creditos, "Cerrar", 68, interfaz.CerrarCreditos);

            Asignar(interfaz, "gestor", gestor);
            Asignar(interfaz, "interactor", interactor);
            Asignar(interfaz, "catalogo", catalogo);
            Asignar(interfaz, "creditosEquipo", CreditosEquipo);
            Asignar(interfaz, "hud", hud.gameObject);
            Asignar(interfaz, "marcador", marcador.gameObject);
            Asignar(interfaz, "textoEscenario", textoEscenario);
            Asignar(interfaz, "textoMarcador", textoMarcador);
            Asignar(interfaz, "indicacion", indicacion.gameObject);
            Asignar(interfaz, "textoIndicacion", textoIndicacion);
            Asignar(interfaz, "mensaje", mensaje.gameObject);
            Asignar(interfaz, "fondoMensaje", mensaje.GetComponent<Image>());
            Asignar(interfaz, "textoTituloMensaje", tituloMensaje);
            Asignar(interfaz, "textoMensaje", textoMensaje);
            Asignar(interfaz, "panelMenu", menu.gameObject);
            Asignar(interfaz, "panelPausa", pausa.gameObject);
            Asignar(interfaz, "botonReiniciarPractica", reiniciarPractica);
            Asignar(interfaz, "panelCreditos", creditos.gameObject);
            Asignar(interfaz, "textoCreditos", textoCreditos);

            // Estado inicial en el editor: solo el menú visible.
            foreach (RectTransform panel in new[] { hud, mensaje, pausa, creditos })
                panel.gameObject.SetActive(false);
        }

        static RectTransform Rect(string nombre, Transform padre)
        {
            RectTransform rect = new GameObject(nombre, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(padre, false);
            return rect;
        }

        static void Estirar(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        // Ancla y pivote en el mismo punto relativo del padre.
        static void Anclar(RectTransform rect, Vector2 ancla, Vector2 posicion, Vector2 tamano)
        {
            rect.anchorMin = ancla;
            rect.anchorMax = ancla;
            rect.pivot = ancla;
            rect.anchoredPosition = posicion;
            rect.sizeDelta = tamano;
        }

        static Image Imagen(RectTransform rect, Sprite sprite, Color color)
        {
            Image imagen = rect.gameObject.AddComponent<Image>();
            imagen.sprite = sprite;
            imagen.type = Image.Type.Sliced;
            imagen.color = color;
            imagen.raycastTarget = false;
            return imagen;
        }

        static void Columna(RectTransform rect, float espacio, int margen = 0)
        {
            VerticalLayoutGroup columna = rect.gameObject.AddComponent<VerticalLayoutGroup>();
            columna.spacing = espacio;
            columna.padding = new RectOffset(margen, margen, margen, margen);
            columna.childControlWidth = true;
            columna.childControlHeight = true;
            columna.childForceExpandWidth = true;
            columna.childForceExpandHeight = false;
        }

        static RectTransform Fila(string nombre, Transform padre)
        {
            RectTransform fila = Rect(nombre, padre);
            HorizontalLayoutGroup grupo = fila.gameObject.AddComponent<HorizontalLayoutGroup>();
            grupo.spacing = 16;
            grupo.childControlWidth = true;
            grupo.childControlHeight = true;
            grupo.childForceExpandWidth = true;
            grupo.childForceExpandHeight = false;
            return fila;
        }

        // Panel con fondo cuya altura se ajusta a su contenido.
        static RectTransform PanelAjustable(string nombre, Transform padre, Color fondo, int margen, float espacio)
        {
            RectTransform panel = Rect(nombre, padre);
            Imagen(panel, spriteRedondeado, fondo);
            Columna(panel, espacio, margen);
            panel.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return panel;
        }

        static RectTransform PanelCentral(string nombre, Transform padre, float ancho)
        {
            RectTransform panel = PanelAjustable(nombre, padre, FondoPanel, 40, 18);
            Anclar(panel, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(ancho, 0));
            panel.GetComponent<Image>().raycastTarget = true;
            return panel;
        }

        static TextMeshProUGUI Texto(string nombre, Transform padre, string contenido, float tamano,
            TextAlignmentOptions alineacion, FontStyles estilo = FontStyles.Normal, Color? color = null)
        {
            TextMeshProUGUI texto = Rect(nombre, padre).gameObject.AddComponent<TextMeshProUGUI>();
            texto.font = TMP_Settings.defaultFontAsset;
            texto.text = contenido;
            texto.fontSize = tamano;
            texto.alignment = alineacion;
            texto.fontStyle = estilo;
            texto.color = color ?? Color.white;
            texto.textWrappingMode = TextWrappingModes.Normal;
            texto.raycastTarget = false;
            return texto;
        }

        static Button Boton(string nombre, Transform padre, string etiqueta, float alto, UnityAction accion)
        {
            RectTransform rect = Rect(nombre, padre);
            Image fondo = Imagen(rect, spriteRedondeado, Color.white);
            fondo.raycastTarget = true;

            Button boton = rect.gameObject.AddComponent<Button>();
            boton.targetGraphic = fondo;
            ColorBlock colores = boton.colors;
            colores.normalColor = ColorBoton;
            colores.highlightedColor = Color.Lerp(ColorBoton, Color.white, 0.25f);
            colores.pressedColor = Color.Lerp(ColorBoton, Color.black, 0.25f);
            colores.selectedColor = colores.highlightedColor;
            boton.colors = colores;

            LayoutElement elemento = rect.gameObject.AddComponent<LayoutElement>();
            elemento.minHeight = alto;
            elemento.preferredHeight = alto;

            TextMeshProUGUI texto = Texto("Etiqueta", rect, etiqueta, 28, TextAlignmentOptions.Center);
            Estirar(texto.rectTransform);
            texto.margin = new Vector4(16, 4, 16, 4);

            if (accion != null)
                UnityEventTools.AddPersistentListener(boton.onClick, accion);
            return boton;
        }

        static TMP_Text ListaDesplazable(Transform padre)
        {
            RectTransform area = Rect("Desplazamiento", padre);
            area.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;
            ScrollRect desplazamiento = area.gameObject.AddComponent<ScrollRect>();
            desplazamiento.horizontal = false;
            desplazamiento.movementType = ScrollRect.MovementType.Clamped;
            desplazamiento.scrollSensitivity = 30;

            RectTransform ventana = Rect("Ventana", area);
            Estirar(ventana);
            ventana.gameObject.AddComponent<RectMask2D>();

            TextMeshProUGUI texto = Texto("TextoCreditos", ventana, "", 24, TextAlignmentOptions.TopLeft);
            texto.raycastTarget = true;
            texto.gameObject.AddComponent<EnlacesTexto>();
            texto.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            RectTransform contenido = texto.rectTransform;
            contenido.anchorMin = new Vector2(0, 1);
            contenido.anchorMax = new Vector2(1, 1);
            contenido.pivot = new Vector2(0.5f, 1);
            contenido.sizeDelta = Vector2.zero;

            desplazamiento.viewport = ventana;
            desplazamiento.content = contenido;
            return texto;
        }

        // === Serialización ===

        // Asigna campos [SerializeField] privados como lo haría el Inspector.
        static void Asignar(Object objeto, string campo, object valor)
        {
            SerializedObject serializado = new(objeto);
            SerializedProperty propiedad = serializado.FindProperty(campo)
                ?? throw new ArgumentException($"{objeto.GetType().Name} no tiene el campo serializado '{campo}'.");

            switch (valor)
            {
                case Object referencia:
                    propiedad.objectReferenceValue = referencia;
                    break;
                case IList<Object> lista:
                    propiedad.arraySize = lista.Count;
                    for (int i = 0; i < lista.Count; i++)
                        propiedad.GetArrayElementAtIndex(i).objectReferenceValue = lista[i];
                    break;
                case string texto:
                    propiedad.stringValue = texto;
                    break;
                case float numero:
                    propiedad.floatValue = numero;
                    break;
                case Vector3 vector:
                    propiedad.vector3Value = vector;
                    break;
                case Enum enumeracion:
                    propiedad.intValue = Convert.ToInt32(enumeracion);
                    break;
                default:
                    throw new ArgumentException($"Tipo no soportado para '{campo}': {valor?.GetType().Name}");
            }

            serializado.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
