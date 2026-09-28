using UnityEngine;

namespace ConsultorioSeguro.Editor
{
    // Contenido inicial de las cinco acciones. El constructor lo convierte en
    // assets de Assets/Datos solo si todavía no existen, así que después de la
    // primera construcción se edita desde el Inspector y no aquí.
    // Las clasificaciones marcadas con "validar" deben revisarse con el especialista.
    static class CatalogoInicial
    {
        // Dónde están los residuos de cada acción; cada una tiene su propia área.
        public enum Superficie
        {
            Carrito,
            CharolaEsterilizacion,
            Estante,
            MesaInstrumental,
            MesaRayosX,
        }

        public class Residuo
        {
            public string archivo;
            public string nombre;
            public Destino destino;
            public string explicacion;
            public PrimitiveType forma = PrimitiveType.Cube;
            public Vector3 escala;
            public Color color;
            public int lugar;
        }

        public class Escenario
        {
            public string archivo;
            public AccionSimulacion accion;
            public string nombre;
            public string instrucciones;
            public Superficie superficie;
            public Residuo[] residuos;
        }

        static readonly Color Acero = new(0.72f, 0.74f, 0.78f);
        static readonly Color Sangre = new(0.62f, 0.07f, 0.08f);
        static readonly Color Papel = new(0.88f, 0.92f, 0.96f);
        static readonly Color Nitrilo = new(0.42f, 0.45f, 0.85f);
        static readonly Color Plastico = new(0.78f, 0.9f, 0.98f);

        public static readonly Escenario[] Escenarios =
        {
            new()
            {
                archivo = "1-Clasificacion",
                accion = AccionSimulacion.Clasificacion,
                nombre = "Clasificación por el tipo de desecho",
                instrucciones =
                    "En el carrito de curación quedaron los residuos de la última consulta. " +
                    "Toma cada uno y deposítalo en el recipiente que le corresponde según la NOM-087-ECOL-SSA1-2002.\n\n" +
                    "Los recipientes están junto a la pared izquierda. Si tienes dudas, lee sus letreros.",
                superficie = Superficie.Carrito,
                residuos = new Residuo[]
                {
                    new()
                    {
                        archivo = "gasa-empapada", nombre = "Gasa empapada de sangre", destino = Destino.BolsaRoja,
                        explicacion = "Es material de curación empapado de sangre: la norma lo clasifica como residuo no anatómico y va en la bolsa roja.",
                        escala = new(0.08f, 0.015f, 0.08f), color = Sangre, lugar = 0,
                    },
                    new()
                    {
                        archivo = "aguja-hipodermica", nombre = "Aguja hipodérmica usada", destino = Destino.Punzocortantes,
                        explicacion = "Las agujas que tuvieron contacto con el paciente son punzocortantes. Van en el recipiente rígido, sin reencapuchar.",
                        escala = new(0.005f, 0.005f, 0.08f), color = Acero, lugar = 1,
                    },
                    new()
                    {
                        archivo = "hoja-bisturi", nombre = "Hoja de bisturí usada", destino = Destino.Punzocortantes,
                        explicacion = "Las hojas de bisturí son punzocortantes. Siempre van en el recipiente rígido para evitar cortes y pinchazos.",
                        escala = new(0.045f, 0.003f, 0.014f), color = Acero, lugar = 2,
                    },
                    new()
                    {
                        archivo = "envoltura-papel", nombre = "Envoltura de papel del instrumental", destino = Destino.BasuraComun,
                        explicacion = "La envoltura no tuvo contacto con sangre ni fluidos del paciente, así que no es RPBI: va en la basura común.",
                        escala = new(0.14f, 0.004f, 0.09f), color = Papel, lugar = 3,
                    },
                    new()
                    {
                        // validar: criterio de guantes sin sangre visible.
                        archivo = "guantes-sin-sangre", nombre = "Guantes de exploración sin sangre visible", destino = Destino.BasuraComun,
                        explicacion = "La norma solo incluye materiales empapados, saturados o goteando sangre. Sin sangre, los guantes van en la basura común.",
                        escala = new(0.1f, 0.02f, 0.13f), color = Nitrilo, lugar = 4,
                    },
                    new()
                    {
                        archivo = "cubrebocas", nombre = "Cubrebocas usado sin sangre", destino = Destino.BasuraComun,
                        explicacion = "Sin sangre ni fluidos del listado de la norma, el cubrebocas no es RPBI y va en la basura común.",
                        escala = new(0.15f, 0.01f, 0.08f), color = Plastico, lugar = 5,
                    },
                },
            },
            new()
            {
                archivo = "2-Esterilizacion",
                accion = AccionSimulacion.Esterilizacion,
                nombre = "Esterilización de instrumentos de trabajo",
                instrucciones =
                    "En la charola junto a la tarja está el instrumental sucio de la última consulta. " +
                    "Lo que se reutiliza va a la autoclave; lo que es de un solo uso se desecha en su recipiente.\n\n" +
                    "Recuerda: los punzocortantes desechables nunca se esterilizan.",
                superficie = Superficie.CharolaEsterilizacion,
                residuos = new Residuo[]
                {
                    new()
                    {
                        archivo = "espejo-dental", nombre = "Espejo dental", destino = Destino.Esterilizacion,
                        explicacion = "Es instrumental metálico reutilizable: se lava, se empaqueta y se esteriliza en autoclave.",
                        escala = new(0.15f, 0.012f, 0.015f), color = Acero, lugar = 0,
                    },
                    new()
                    {
                        archivo = "forceps", nombre = "Fórceps de extracción", destino = Destino.Esterilizacion,
                        explicacion = "Es instrumental quirúrgico reutilizable. Después de lavarlo se esteriliza para la siguiente intervención.",
                        escala = new(0.17f, 0.02f, 0.035f), color = Acero, lugar = 1,
                    },
                    new()
                    {
                        archivo = "explorador", nombre = "Explorador dental", destino = Destino.Esterilizacion,
                        explicacion = "Aunque tiene punta, es un instrumento reutilizable: no se desecha, se esteriliza.",
                        escala = new(0.15f, 0.008f, 0.008f), color = Acero, lugar = 2,
                    },
                    new()
                    {
                        archivo = "aguja-dental", nombre = "Aguja dental desechable usada", destino = Destino.Punzocortantes,
                        explicacion = "Es de un solo uso y ya tuvo contacto con el paciente. Va en el recipiente de punzocortantes, no se esteriliza.",
                        escala = new(0.005f, 0.005f, 0.06f), color = Acero, lugar = 3,
                    },
                    new()
                    {
                        // validar: hay limas reutilizables un número limitado de veces.
                        archivo = "lima-endodontica", nombre = "Lima endodóntica desechable usada", destino = Destino.Punzocortantes,
                        explicacion = "Las limas desechables son punzocortantes. Al terminar su uso van en el recipiente rígido.",
                        escala = new(0.035f, 0.006f, 0.006f), color = new(0.9f, 0.75f, 0.2f), lugar = 4,
                    },
                },
            },
            new()
            {
                archivo = "3-MuestraMateriales",
                accion = AccionSimulacion.MuestraMateriales,
                nombre = "Muestra de materiales",
                instrucciones =
                    "En este estante están los materiales del consultorio. Imagina que ya se usaron con un paciente " +
                    "y decide a qué recipiente irá cada uno, o si se esteriliza.\n\n" +
                    "Así aprenderás a reconocer qué materiales generan RPBI antes de usarlos.",
                superficie = Superficie.Estante,
                residuos = new Residuo[]
                {
                    new()
                    {
                        archivo = "rollos-algodon", nombre = "Rollos de algodón saturados de sangre", destino = Destino.BolsaRoja,
                        explicacion = "Material de curación saturado de sangre: es residuo no anatómico y va en la bolsa roja.",
                        escala = new(0.05f, 0.016f, 0.016f), color = new(0.85f, 0.55f, 0.55f),
                        lugar = 0,
                    },
                    new()
                    {
                        // validar: criterio de saliva sin sangre visible.
                        archivo = "vaso-desechable", nombre = "Vaso desechable del paciente", destino = Destino.BasuraComun,
                        explicacion = "Solo tuvo contacto con agua y saliva. La saliva sin sangre no está en el listado de RPBI de la norma, así que va en la basura común.",
                        forma = PrimitiveType.Cylinder, escala = new(0.07f, 0.05f, 0.07f), color = Color.white,
                        lugar = 1,
                    },
                    new()
                    {
                        archivo = "babero", nombre = "Babero desechable sin sangre", destino = Destino.BasuraComun,
                        explicacion = "Protege la ropa del paciente. Si no se empapó de sangre no es RPBI y va en la basura común.",
                        escala = new(0.18f, 0.01f, 0.14f), color = new(0.5f, 0.78f, 0.9f),
                        lugar = 2,
                    },
                    new()
                    {
                        archivo = "jeringa-carpule", nombre = "Jeringa carpule metálica", destino = Destino.Esterilizacion,
                        explicacion = "Es metálica y reutilizable: la aguja y el cartucho se desechan, pero la jeringa se esteriliza.",
                        escala = new(0.14f, 0.02f, 0.02f), color = Acero,
                        lugar = 3,
                    },
                    new()
                    {
                        archivo = "aguja-sutura", nombre = "Aguja de sutura usada", destino = Destino.Punzocortantes,
                        explicacion = "La norma menciona expresamente las agujas de sutura entre los punzocortantes: van en el recipiente rígido.",
                        escala = new(0.025f, 0.004f, 0.025f), color = Acero,
                        lugar = 4,
                    },
                    new()
                    {
                        archivo = "pelicula-barrera", nombre = "Película plástica de barrera sin sangre", destino = Destino.BasuraComun,
                        explicacion = "Las barreras que solo cubrieron superficies van en la basura común. Si se contaminan con sangre, van en la bolsa roja.",
                        escala = new(0.12f, 0.004f, 0.12f), color = Plastico,
                        lugar = 5,
                    },
                },
            },
            new()
            {
                archivo = "4-Procedimiento",
                accion = AccionSimulacion.Procedimiento,
                nombre = "Retiro de una pieza dental",
                instrucciones =
                    "Representa la extracción de una pieza dental para ver qué residuos genera. Sigue los pasos en orden: " +
                    "aplica la anestesia con la jeringa de la mesa de instrumental, extrae la pieza en el paciente y coloca una gasa.\n\n" +
                    "Al terminar, clasifica los residuos que quedaron en la mesa. " +
                    "El procedimiento no es una guía clínica; solo da contexto a los residuos.",
                superficie = Superficie.MesaInstrumental,
                residuos = new Residuo[]
                {
                    new()
                    {
                        // validar: pieza dental como residuo patológico.
                        archivo = "pieza-dental", nombre = "Pieza dental extraída", destino = Destino.BolsaAmarilla,
                        explicacion = "Es una parte que se remueve durante una intervención; la norma la considera residuo patológico y va en la bolsa amarilla.",
                        forma = PrimitiveType.Sphere, escala = new(0.022f, 0.028f, 0.022f), color = new(0.95f, 0.93f, 0.84f),
                        lugar = 0,
                    },
                    new()
                    {
                        archivo = "gasa-extraccion", nombre = "Gasa con sangre de la extracción", destino = Destino.BolsaRoja,
                        explicacion = "Material de curación empapado de sangre: residuo no anatómico, bolsa roja.",
                        escala = new(0.08f, 0.015f, 0.08f), color = Sangre, lugar = 1,
                    },
                    new()
                    {
                        archivo = "aguja-anestesia", nombre = "Aguja de anestesia usada", destino = Destino.Punzocortantes,
                        explicacion = "Tuvo contacto con el paciente al aplicar la anestesia: es punzocortante y va en el recipiente rígido.",
                        escala = new(0.005f, 0.005f, 0.06f), color = Acero, lugar = 6,
                    },
                    new()
                    {
                        archivo = "forceps-extraccion", nombre = "Fórceps de extracción usado", destino = Destino.Esterilizacion,
                        explicacion = "Aunque tiene sangre, es instrumental reutilizable: se lava y se esteriliza, no se desecha.",
                        escala = new(0.17f, 0.02f, 0.035f), color = Acero, lugar = 2,
                    },
                    new()
                    {
                        // validar: guantes empapados de sangre.
                        archivo = "guantes-con-sangre", nombre = "Guantes empapados de sangre", destino = Destino.BolsaRoja,
                        explicacion = "Al estar empapados de sangre dejan de ser basura común y van en la bolsa roja.",
                        escala = new(0.1f, 0.02f, 0.13f), color = new(0.5f, 0.25f, 0.5f), lugar = 3,
                    },
                },
            },
            new()
            {
                archivo = "5-Radiografia",
                accion = AccionSimulacion.Radiografia,
                nombre = "Manejo del equipo de rayos X",
                instrucciones =
                    "Prepara el equipo de rayos X para tomar una radiografía. Sigue la secuencia: enciende el equipo en el " +
                    "interruptor de la pared, espera a que esté listo, coloca las barreras de protección en el cabezal, " +
                    "posiciona el brazo y toma la radiografía con el disparador.\n\n" +
                    "Después clasifica las barreras que retiraste; quedarán en la mesita junto al equipo.",
                superficie = Superficie.MesaRayosX,
                residuos = new Residuo[]
                {
                    new()
                    {
                        archivo = "barrera-cabezal", nombre = "Barrera plástica del cabezal", destino = Destino.BasuraComun,
                        explicacion = "Solo cubrió el equipo y no tiene sangre: va en la basura común.",
                        escala = new(0.14f, 0.006f, 0.14f), color = Plastico, lugar = 0,
                    },
                    new()
                    {
                        // validar: criterio de saliva sin sangre visible.
                        archivo = "funda-sensor", nombre = "Funda del sensor con saliva", destino = Destino.BasuraComun,
                        explicacion = "La saliva sin sangre no está en el listado de RPBI de la norma, así que la funda va en la basura común. Si tuviera sangre, iría en la bolsa roja.",
                        escala = new(0.05f, 0.008f, 0.04f), color = Plastico, lugar = 1,
                    },
                    new()
                    {
                        archivo = "guantes-radiografia", nombre = "Guantes de la toma sin sangre", destino = Destino.BasuraComun,
                        explicacion = "Sin sangre ni fluidos del listado de la norma, los guantes van en la basura común.",
                        escala = new(0.1f, 0.02f, 0.13f), color = Nitrilo, lugar = 2,
                    },
                },
            },
        };
    }
}
