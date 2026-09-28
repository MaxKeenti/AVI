using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Editor
{
    // Edificio de la clínica (recepción y consultorio) y la calle que lo rodea.
    // Coordenadas en metros: el consultorio ocupa z de -3 a 3, la recepción de
    // -7 a -3, la fachada da a la banqueta en z = -7.2 y la calle corre a lo largo de x.
    public static partial class ConstructorConsultorio
    {
        const float FondoRecepcion = 4f;
        const float AltoEdificio = 3.3f;
        const float GrosorMuro = 0.2f;
        const float ZFachada = -Fondo / 2 - FondoRecepcion - GrosorMuro;

        // Calle: banqueta de la clínica, arroyo vehicular y banqueta de enfrente.
        const float ZOrillaCalle = -10f;
        const float ZOrillaEnfrente = -17f;
        const float ZFachadasEnfrente = -19.8f;
        const float LimiteX = 28.5f;

        static readonly Color ColorFachada = new(0.87f, 0.8f, 0.68f);
        static readonly Color ColorCornisa = new(0.18f, 0.45f, 0.5f);
        static readonly Color Vidrio = new(0.22f, 0.3f, 0.38f);

        // === Edificio ===

        static void ConstruirEdificio(Transform raiz)
        {
            const float zRecepcion = -Fondo / 2 - FondoRecepcion;
            Material pared = Mat(new Color(0.9f, 0.93f, 0.95f));

            CajaEntre("Piso", raiz, new Vector3(-Ancho / 2, -0.25f, zRecepcion), new Vector3(Ancho / 2, 0, Fondo / 2),
                Mat(new Color(0.78f, 0.79f, 0.78f), 0, 0.5f));
            CajaEntre("Techo", raiz, new Vector3(-Ancho / 2, Alto, zRecepcion), new Vector3(Ancho / 2, Alto + 0.15f, Fondo / 2), Mat(Color.white));

            CajaEntre("Muro fondo", raiz, new Vector3(-Ancho / 2 - GrosorMuro, -0.25f, Fondo / 2),
                new Vector3(Ancho / 2 + GrosorMuro, AltoEdificio, Fondo / 2 + GrosorMuro), pared);
            CajaEntre("Muro izquierdo", raiz, new Vector3(-Ancho / 2 - GrosorMuro, -0.25f, ZFachada),
                new Vector3(-Ancho / 2, AltoEdificio, Fondo / 2), pared);
            CajaEntre("Muro derecho", raiz, new Vector3(Ancho / 2, -0.25f, ZFachada),
                new Vector3(Ancho / 2 + GrosorMuro, AltoEdificio, Fondo / 2), pared);
            MuroConPuerta("Fachada", raiz, Ancho / 2 + GrosorMuro, ZFachada, zRecepcion, AltoEdificio, 1.2f, 2.2f, pared);
            MuroConPuerta("Muro del consultorio", raiz, Ancho / 2, -Fondo / 2 - 0.1f, -Fondo / 2, Alto, 1f, 2.1f, pared);

            // Puerta del consultorio abierta hacia la recepción.
            CajaEntre("Puerta del consultorio", raiz, new Vector3(0.52f, 0.02f, -Fondo / 2 - 1.05f),
                new Vector3(0.56f, 2.08f, -Fondo / 2 - 0.1f), Mat(new Color(0.55f, 0.4f, 0.28f)));

            LamparaDeTecho(raiz, new Vector3(-1.5f, Alto - 0.02f, 0));
            LamparaDeTecho(raiz, new Vector3(1.5f, Alto - 0.02f, 0.5f));
            LamparaDeTecho(raiz, new Vector3(0, Alto - 0.02f, zRecepcion + FondoRecepcion / 2));

            // Reflejos del propio cuarto; sin ellas el piso reflejaría el cielo.
            SondaDeReflejos(raiz, "Sonda consultorio", new Vector3(0, Alto / 2, 0), new Vector3(Ancho, Alto, Fondo));
            SondaDeReflejos(raiz, "Sonda recepción", new Vector3(0, Alto / 2, zRecepcion + FondoRecepcion / 2),
                new Vector3(Ancho, Alto, FondoRecepcion));

            ConstruirFachada(raiz);

            CrearZonaMensaje("Mensaje de bienvenida", raiz, new Vector3(-Ancho / 2, 0, zRecepcion), new Vector3(Ancho / 2, Alto, -Fondo / 2 - 0.4f),
                "Clínica dental",
                "Bienvenido. En esta clínica los residuos se separan según la NOM-087-ECOL-SSA1-2002. " +
                "El consultorio está al fondo, pasando la recepción: ahí cada área tiene una práctica.");
            CrearZonaMensaje("Mensaje del consultorio", raiz, new Vector3(-1f, 0, -Fondo / 2), new Vector3(1f, Alto, -Fondo / 2 + 1f),
                "Consultorio",
                "Cada área del consultorio tiene una práctica y una hoja con tu avance. Acércate a una para ver sus instrucciones. " +
                "Los recipientes de residuos están en la pared izquierda.");
        }

        // Muro centrado en x = 0 con un vano de puerta al centro.
        static void MuroConPuerta(string nombre, Transform padre, float medioAncho, float zMin, float zMax, float alto,
            float anchoPuerta, float altoPuerta, Material material)
        {
            Transform muro = Grupo(nombre, padre, Vector3.zero);
            float x = anchoPuerta / 2;
            CajaEntre("Izquierda", muro, new Vector3(-medioAncho, -0.25f, zMin), new Vector3(-x, alto, zMax), material);
            CajaEntre("Derecha", muro, new Vector3(x, -0.25f, zMin), new Vector3(medioAncho, alto, zMax), material);
            CajaEntre("Dintel", muro, new Vector3(-x, altoPuerta, zMin), new Vector3(x, alto, zMax), material);
        }

        static void ConstruirFachada(Transform raiz)
        {
            Transform fachada = Grupo("Fachada exterior", raiz, Vector3.zero);
            const float z = ZFachada;
            const float x = Ancho / 2 + GrosorMuro;
            Material revestimiento = Mat(ColorFachada);
            Material cornisa = Mat(ColorCornisa);

            CajaEntre("Revestimiento izquierdo", fachada, new Vector3(-x, -0.25f, z - 0.02f), new Vector3(-0.6f, 2.9f, z), revestimiento);
            CajaEntre("Revestimiento derecho", fachada, new Vector3(0.6f, -0.25f, z - 0.02f), new Vector3(x, 2.9f, z), revestimiento);
            CajaEntre("Revestimiento del dintel", fachada, new Vector3(-0.6f, 2.2f, z - 0.02f), new Vector3(0.6f, 2.9f, z), revestimiento);
            CajaEntre("Cornisa", fachada, new Vector3(-x - 0.05f, 2.9f, z - 0.06f), new Vector3(x + 0.05f, AltoEdificio + 0.05f, z), cornisa);

            CajaEntre("Marco izquierdo", fachada, new Vector3(-0.7f, 0, z - 0.05f), new Vector3(-0.6f, 2.3f, z), cornisa);
            CajaEntre("Marco derecho", fachada, new Vector3(0.6f, 0, z - 0.05f), new Vector3(0.7f, 2.3f, z), cornisa);
            CajaEntre("Marco superior", fachada, new Vector3(-0.7f, 2.2f, z - 0.05f), new Vector3(0.7f, 2.3f, z), cornisa);
            CajaEntre("Marquesina", fachada, new Vector3(-1f, 2.3f, z - 1f), new Vector3(1f, 2.36f, z), Mat(Blanco));
            SinColision(CajaEntre("Tapete", fachada, new Vector3(-0.7f, 0, z - 0.9f), new Vector3(0.7f, 0.01f, z - 0.1f),
                Mat(new Color(0.2f, 0.2f, 0.22f))));

            CajaEntre("Ventana", fachada, new Vector3(1.4f, 0.9f, z - 0.04f), new Vector3(3.1f, 2f, z), Mat(Vidrio, 0, 0.9f));
            Letrero(fachada, "Letrero de la clínica", new Vector3(0, 3.125f, z - 0.075f), 0,
                "Clínica dental", "", new Color(0.96f, 0.96f, 0.94f), new Vector2(3.4f, 0.38f));
            Letrero(fachada, "Lona institucional", new Vector3(-2.2f, 1.55f, z - 0.03f), 0,
                "Consultorio Seguro", "Prototipo de simulador para el manejo de RPBI\nUPIICSA · IPN · Equipo 7",
                new Color(0.42f, 0.11f, 0.27f), new Vector2(1.9f, 1.2f));
        }

        static void LamparaDeTecho(Transform padre, Vector3 posicion)
        {
            Caja("Lámpara de techo", padre, posicion, new Vector3(1.2f, 0.03f, 0.3f), Mat(Color.white, 0, 0.5f, true));
            Light luz = new GameObject("Luz de techo").AddComponent<Light>();
            luz.transform.SetParent(padre, false);
            luz.transform.localPosition = posicion + Vector3.down * 0.5f;
            luz.type = LightType.Point;
            luz.range = 6f;
            luz.intensity = 2f;
            luz.shadows = LightShadows.None;
        }

        static void SondaDeReflejos(Transform padre, string nombre, Vector3 centro, Vector3 tamano)
        {
            ReflectionProbe sonda = new GameObject(nombre).AddComponent<ReflectionProbe>();
            sonda.transform.SetParent(padre, false);
            sonda.transform.localPosition = centro;
            sonda.size = tamano;
            sonda.mode = ReflectionProbeMode.Realtime;
            sonda.refreshMode = ReflectionProbeRefreshMode.OnAwake;
            sonda.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;
        }

        // === Recepción ===

        static void ConstruirRecepcion(Transform raiz)
        {
            const float zMuro = -Fondo / 2 - 0.11f;

            Transform mostrador = Grupo("Mostrador de recepción", raiz, new Vector3(2.4f, 0, -5.2f));
            Caja("Cuerpo", mostrador, new Vector3(0, 0.525f, 0), new Vector3(1.8f, 1.05f, 0.5f), Mat(ColorCornisa));
            Caja("Cubierta", mostrador, new Vector3(0, 1.07f, 0), new Vector3(1.9f, 0.04f, 0.6f), Mat(Blanco));
            Caja("Monitor", mostrador, new Vector3(0.3f, 1.27f, 0.1f), new Vector3(0.45f, 0.3f, 0.04f), Mat(new Color(0.12f, 0.12f, 0.14f)));
            Informativo(mostrador.gameObject, "Recepción", "Aquí se registra a los pacientes antes de pasar al consultorio.");

            Transform sala = Grupo("Sala de espera", raiz, Vector3.zero);
            foreach (float z in new[] { -5.8f, -5.2f, -4.6f })
            {
                Transform silla = Grupo("Silla", sala, new Vector3(-Ancho / 2 + 0.3f, 0, z));
                Caja("Base", silla, new Vector3(0, 0.21f, 0), new Vector3(0.35f, 0.42f, 0.35f), Mat(new Color(0.25f, 0.26f, 0.28f)));
                Caja("Asiento", silla, new Vector3(0, 0.45f, 0), new Vector3(0.45f, 0.06f, 0.45f), Mat(ColorCornisa));
                Caja("Respaldo", silla, new Vector3(-0.2f, 0.72f, 0), new Vector3(0.05f, 0.5f, 0.45f), Mat(ColorCornisa));
            }
            Informativo(sala.gameObject, "Sala de espera", "Los pacientes esperan aquí su turno.");

            Primitiva(PrimitiveType.Cylinder, "Maceta", raiz, new Vector3(-3.1f, 0.2f, -6.6f), new Vector3(0.4f, 0.2f, 0.4f), Mat(new Color(0.6f, 0.35f, 0.25f)));
            SinColision(Primitiva(PrimitiveType.Sphere, "Planta", raiz, new Vector3(-3.1f, 0.75f, -6.6f), Vector3.one * 0.7f, Mat(new Color(0.25f, 0.5f, 0.28f))));

            Transform lavabo = Grupo("Lavabo", raiz, new Vector3(-Ancho / 2 + 0.25f, 0, -3.7f));
            Caja("Pedestal", lavabo, new Vector3(0, 0.4f, 0), new Vector3(0.2f, 0.8f, 0.2f), Mat(Blanco));
            Caja("Tarja", lavabo, new Vector3(0, 0.85f, 0), new Vector3(0.4f, 0.12f, 0.5f), Mat(Blanco));
            Caja("Espejo", lavabo, new Vector3(-0.22f, 1.45f, 0), new Vector3(0.02f, 0.6f, 0.45f), Mat(new Color(0.75f, 0.82f, 0.86f), 0.9f, 0.95f));
            Informativo(lavabo.gameObject, "Lavabo",
                "La higiene de manos es la primera medida de bioseguridad: antes y después de cada paciente y después de manejar residuos.");

            Letrero(raiz, "Letrero de higiene de manos", new Vector3(-Ancho / 2 + 0.01f, 1.6f, -4.45f), -90, "Higiene de manos",
                "Lávate las manos antes y después de atender a cada paciente y después de manejar residuos.", new Color(0.2f, 0.5f, 0.7f));
            Letrero(raiz, "Póster de RPBI", new Vector3(-2f, 1.55f, zMuro), 0, "Manejo de RPBI",
                "En este consultorio los residuos se separan según la NOM-087-ECOL-SSA1-2002. Pasa al consultorio para practicar.",
                Destino.BolsaRoja.Color(), new Vector2(1f, 0.75f));
            Letrero(raiz, "Póster de colores", new Vector3(2f, 1.55f, zMuro), 0, "Código de colores",
                "Rojo: RPBI y punzocortantes\nAmarillo: patológicos\nGris: basura común", new Color(0.25f, 0.27f, 0.3f), new Vector2(1f, 0.75f));
            Letrero(raiz, "Letrero del consultorio", new Vector3(0, 2.45f, zMuro), 0, "Consultorio", "",
                ColorCornisa, new Vector2(1f, 0.3f));
        }

        // === Calle ===

        static void ConstruirEntornoUrbano(Transform raiz)
        {
            Material asfalto = Mat(new Color(0.22f, 0.22f, 0.24f), 0, 0.2f);
            Material banqueta = Mat(new Color(0.68f, 0.68f, 0.66f));
            Material pintura = Mat(new Color(0.92f, 0.92f, 0.9f));

            CajaEntre("Terreno", raiz, new Vector3(-45, -0.35f, -40), new Vector3(45, -0.25f, 20), Mat(new Color(0.45f, 0.5f, 0.42f)));
            CajaEntre("Calle", raiz, new Vector3(-LimiteX, -0.25f, ZOrillaEnfrente), new Vector3(LimiteX, -0.15f, ZOrillaCalle), asfalto);
            CajaEntre("Banqueta", raiz, new Vector3(-LimiteX, -0.25f, ZOrillaCalle), new Vector3(LimiteX, 0, ZFachada), banqueta);
            CajaEntre("Banqueta de enfrente", raiz, new Vector3(-LimiteX, -0.25f, ZFachadasEnfrente), new Vector3(LimiteX, 0, ZOrillaEnfrente), banqueta);

            Transform senalamiento = Grupo("Señalamiento", raiz, Vector3.zero);
            const float zCentro = (ZOrillaCalle + ZOrillaEnfrente) / 2;
            for (float x = -26; x <= 26; x += 4)
                if (Mathf.Abs(x) > 3)
                    SinColision(CajaEntre("Línea", senalamiento, new Vector3(x - 1, -0.15f, zCentro - 0.06f), new Vector3(x + 1, -0.145f, zCentro + 0.06f), pintura));
            for (float z = ZOrillaEnfrente + 0.4f; z < ZOrillaCalle; z += 0.9f)
                SinColision(CajaEntre("Paso peatonal", senalamiento, new Vector3(-1.5f, -0.15f, z - 0.22f), new Vector3(1.5f, -0.145f, z + 0.22f), pintura));

            // Vecinos junto a la clínica (fachada hacia -z) y enfrente (fachada hacia +z).
            Transform vecinos = Grupo("Edificios vecinos", raiz, Vector3.zero);
            (float xMin, float xMax, float altura, Color color)[] deEsteLado =
            {
                (-28, -19.2f, 6, new Color(0.78f, 0.62f, 0.52f)),
                (-19, -11.2f, 10, new Color(0.62f, 0.66f, 0.72f)),
                (-11, -3.7f, 7, new Color(0.85f, 0.78f, 0.55f)),
                (3.7f, 12, 9, new Color(0.7f, 0.55f, 0.55f)),
                (12.2f, 20, 6.5f, new Color(0.6f, 0.72f, 0.64f)),
                (20.2f, 28, 11, new Color(0.8f, 0.8f, 0.78f)),
            };
            (float xMin, float xMax, float altura, Color color)[] deEnfrente =
            {
                (-28, -17, 5, new Color(0.72f, 0.7f, 0.62f)),
                (-16.8f, -8, 12, new Color(0.58f, 0.6f, 0.66f)),
                (-7.8f, 1, 8, new Color(0.82f, 0.68f, 0.5f)),
                (1.2f, 10, 6, new Color(0.66f, 0.74f, 0.8f)),
                (10.2f, 19, 10, new Color(0.76f, 0.6f, 0.62f)),
                (19.2f, 28, 7, new Color(0.7f, 0.74f, 0.6f)),
            };
            for (int i = 0; i < deEsteLado.Length; i++)
                EdificioVecino(vecinos, i, deEsteLado[i].xMin, deEsteLado[i].xMax, ZFachada, 4, deEsteLado[i].altura, deEsteLado[i].color);
            for (int i = 0; i < deEnfrente.Length; i++)
                EdificioVecino(vecinos, i + deEsteLado.Length, deEnfrente[i].xMin, deEnfrente[i].xMax, ZFachadasEnfrente, -30, deEnfrente[i].altura, deEnfrente[i].color);

            Transform mobiliario = Grupo("Mobiliario urbano", raiz, Vector3.zero);
            foreach (float x in new[] { -22f, -14f, -6f, 6f, 14f, 22f })
                Arbol(mobiliario, new Vector3(x, 0, -9.5f));
            foreach (float x in new[] { -20f, -8f, 4f, 16f })
                Arbol(mobiliario, new Vector3(x, 0, -17.5f));
            foreach (float x in new[] { -18f, -10f, 10f, 18f })
                Farol(mobiliario, new Vector3(x, 0, -9.7f), -1);
            foreach (float x in new[] { -14f, -2f, 10f, 22f })
                Farol(mobiliario, new Vector3(x, 0, -17.3f), 1);

            Auto(mobiliario, new Vector3(-9, -0.15f, -11), new Color(0.7f, 0.12f, 0.12f), 0);
            Auto(mobiliario, new Vector3(9, -0.15f, -11), new Color(0.85f, 0.85f, 0.88f), 0);
            Auto(mobiliario, new Vector3(-15, -0.15f, -16), new Color(0.15f, 0.3f, 0.6f), 180);
            Auto(mobiliario, new Vector3(5, -0.15f, -16), new Color(0.4f, 0.42f, 0.45f), 180);

            Transform banca = Grupo("Banca", mobiliario, new Vector3(-3f, 0, -9.4f));
            Caja("Asiento", banca, new Vector3(0, 0.45f, 0), new Vector3(1.6f, 0.06f, 0.45f), Mat(new Color(0.55f, 0.4f, 0.28f)));
            Caja("Respaldo", banca, new Vector3(0, 0.72f, 0.2f), new Vector3(1.6f, 0.4f, 0.05f), Mat(new Color(0.55f, 0.4f, 0.28f)));
            foreach (float x in new[] { -0.7f, 0.7f })
                Caja("Pata", banca, new Vector3(x, 0.21f, 0), new Vector3(0.06f, 0.42f, 0.4f), Mat(new Color(0.2f, 0.2f, 0.22f)));

            Transform bote = Grupo("Bote de basura municipal", mobiliario, new Vector3(3f, 0, -9.5f));
            Primitiva(PrimitiveType.Cylinder, "Cuerpo", bote, new Vector3(0, 0.45f, 0), new Vector3(0.5f, 0.45f, 0.5f), Mat(new Color(0.2f, 0.45f, 0.3f)));
            Primitiva(PrimitiveType.Cylinder, "Tapa", bote, new Vector3(0, 0.92f, 0), new Vector3(0.55f, 0.03f, 0.55f), Mat(new Color(0.15f, 0.16f, 0.17f)));
            Informativo(bote.gameObject, "Bote de basura municipal",
                "Aquí solo va basura común. Los RPBI nunca se tiran en la calle ni con la basura municipal: " +
                "los recoge una empresa autorizada.");

            // Límites invisibles en los extremos de la calle.
            Limite("Límite oeste", raiz, new Vector3(-LimiteX - 0.5f, -1, -31), new Vector3(-LimiteX, 8, 5));
            Limite("Límite este", raiz, new Vector3(LimiteX, -1, -31), new Vector3(LimiteX + 0.5f, 8, 5));
        }

        static void EdificioVecino(Transform padre, int indice, float xMin, float xMax, float zFrente, float zFondo, float altura, Color color)
        {
            Transform edificio = Grupo($"Edificio vecino {indice + 1}", padre, Vector3.zero);
            CajaEntre("Volumen", edificio, new Vector3(xMin, -0.25f, zFrente), new Vector3(xMax, altura, zFondo), Mat(color));

            // Signo hacia la calle: los de este lado la tienen en z menor, los de enfrente en z mayor.
            float haciaCalle = zFondo > zFrente ? -1 : 1;
            Material vidrio = Mat(Vidrio, 0, 0.85f);
            Color[] toldos = { new(0.7f, 0.15f, 0.12f), new(0.15f, 0.45f, 0.3f), new(0.15f, 0.3f, 0.6f), new(0.85f, 0.5f, 0.15f) };

            SinColision(Ventana(edificio, xMin + 0.8f, xMax - 0.8f, 0.3f, 2.5f, zFrente, haciaCalle, vidrio));
            SinColision(CajaEntre("Toldo", edificio, new Vector3(xMin + 0.6f, 2.65f, zFrente), new Vector3(xMax - 0.6f, 2.75f, zFrente + haciaCalle * 0.9f),
                Mat(toldos[indice % toldos.Length])));

            for (float y = 3.6f; y + 1.4f < altura - 0.4f; y += 3f)
                for (float x = xMin + 1.2f; x + 1f <= xMax - 0.8f; x += 2.2f)
                    SinColision(Ventana(edificio, x, x + 1f, y, y + 1.4f, zFrente, haciaCalle, vidrio));
        }

        static GameObject Ventana(Transform padre, float xMin, float xMax, float yMin, float yMax, float zFachada, float haciaCalle, Material vidrio) =>
            CajaEntre("Ventana", padre, new Vector3(xMin, yMin, zFachada), new Vector3(xMax, yMax, zFachada + haciaCalle * 0.05f), vidrio);

        static void Arbol(Transform padre, Vector3 posicion)
        {
            Transform arbol = Grupo("Árbol", padre, posicion);
            Primitiva(PrimitiveType.Cylinder, "Tronco", arbol, new Vector3(0, 1.1f, 0), new Vector3(0.25f, 1.1f, 0.25f), Mat(new Color(0.4f, 0.3f, 0.2f)));
            SinColision(Primitiva(PrimitiveType.Sphere, "Copa", arbol, new Vector3(0, 2.9f, 0), new Vector3(1.9f, 1.7f, 1.9f), Mat(new Color(0.25f, 0.48f, 0.25f))));
        }

        // haciaCalle: -1 si la calle está en z menor, 1 si está en z mayor.
        static void Farol(Transform padre, Vector3 posicion, float haciaCalle)
        {
            Transform farol = Grupo("Farol", padre, posicion);
            Material metal = Mat(new Color(0.3f, 0.32f, 0.34f), 0.6f, 0.5f);
            Primitiva(PrimitiveType.Cylinder, "Poste", farol, new Vector3(0, 2.25f, 0), new Vector3(0.12f, 2.25f, 0.12f), metal);
            SinColision(Caja("Brazo", farol, new Vector3(0, 4.45f, 0.6f * haciaCalle), new Vector3(0.08f, 0.08f, 1.2f), metal));
            SinColision(Caja("Lámpara", farol, new Vector3(0, 4.38f, 1.15f * haciaCalle), new Vector3(0.3f, 0.1f, 0.45f), Mat(new Color(0.95f, 0.93f, 0.85f))));
        }

        static void Auto(Transform padre, Vector3 posicion, Color color, float rotacionY)
        {
            Transform auto = Grupo("Auto", padre, posicion, new Vector3(0, rotacionY, 0));
            Caja("Carrocería", auto, new Vector3(0, 0.6f, 0), new Vector3(4.2f, 0.65f, 1.8f), Mat(color, 0.3f, 0.7f));
            Caja("Cabina", auto, new Vector3(-0.2f, 1.2f, 0), new Vector3(2.2f, 0.55f, 1.6f), Mat(Vidrio, 0, 0.85f));
            foreach (float x in new[] { -1.35f, 1.35f })
                foreach (float z in new[] { -0.85f, 0.85f })
                    SinColision(Primitiva(PrimitiveType.Cylinder, "Llanta", auto, new Vector3(x, 0.32f, z), new Vector3(0.64f, 0.1f, 0.64f),
                        Mat(new Color(0.1f, 0.1f, 0.11f)), new Vector3(90, 0, 0)));
        }

        // === Ayudantes ===

        // Caja definida por dos esquinas opuestas, en cualquier orden.
        static GameObject CajaEntre(string nombre, Transform padre, Vector3 a, Vector3 b, Material material) =>
            Caja(nombre, padre, (a + b) / 2, Vector3.Max(a, b) - Vector3.Min(a, b), material);

        static GameObject SinColision(GameObject objeto)
        {
            Object.DestroyImmediate(objeto.GetComponent<Collider>());
            return objeto;
        }

        static void Limite(string nombre, Transform padre, Vector3 a, Vector3 b)
        {
            GameObject limite = new(nombre);
            limite.transform.SetParent(padre, false);
            limite.transform.localPosition = (a + b) / 2;
            limite.AddComponent<BoxCollider>().size = Vector3.Max(a, b) - Vector3.Min(a, b);
        }

        static void CrearZonaMensaje(string nombre, Transform padre, Vector3 a, Vector3 b, string titulo, string texto)
        {
            GameObject zona = new(nombre);
            zona.transform.SetParent(padre, false);
            zona.transform.localPosition = (a + b) / 2;
            BoxCollider volumen = zona.AddComponent<BoxCollider>();
            volumen.isTrigger = true;
            volumen.size = Vector3.Max(a, b) - Vector3.Min(a, b);

            ZonaMensaje mensaje = zona.AddComponent<ZonaMensaje>();
            Asignar(mensaje, "titulo", titulo);
            Asignar(mensaje, "texto", texto);
        }
    }
}
