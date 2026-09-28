using UnityEngine;

namespace ConsultorioSeguro
{
    // Sonidos de acierto, error, paso completado y fin de escenario.
    // Si no se asigna un clip se genera un tono sencillo.
    [RequireComponent(typeof(AudioSource))]
    public class SonidosRetroalimentacion : MonoBehaviour
    {
        const int Frecuencia = 44100;

        [SerializeField] GestorSimulacion gestor;

        [Header("Opcionales: si se dejan vacíos se usa un tono generado")]
        [SerializeField] AudioClip correcto;
        [SerializeField] AudioClip incorrecto;
        [SerializeField] AudioClip paso;
        [SerializeField] AudioClip fin;

        AudioSource fuente;

        void Awake()
        {
            fuente = GetComponent<AudioSource>();
            if (correcto == null)
                correcto = Tono("correcto", (660, 0.1f), (880, 0.18f));
            if (incorrecto == null)
                incorrecto = Tono("incorrecto", (220, 0.12f), (180, 0.25f));
            if (paso == null)
                paso = Tono("paso", (520, 0.08f));
            if (fin == null)
                fin = Tono("fin", (523, 0.12f), (659, 0.12f), (784, 0.3f));
        }

        void OnEnable()
        {
            gestor.ResiduoClasificado += AlClasificar;
            gestor.PasoCompletado += AlCompletarPaso;
            gestor.EscenarioTerminado += AlTerminar;
        }

        void OnDisable()
        {
            gestor.ResiduoClasificado -= AlClasificar;
            gestor.PasoCompletado -= AlCompletarPaso;
            gestor.EscenarioTerminado -= AlTerminar;
        }

        void AlClasificar(Residuo residuo, ResultadoClasificacion resultado) =>
            fuente.PlayOneShot(resultado.Correcto ? correcto : incorrecto);

        void AlCompletarPaso(Paso _) => fuente.PlayOneShot(paso);

        void AlTerminar(Escenario _) => fuente.PlayOneShot(fin);

        static AudioClip Tono(string nombre, params (float frecuencia, float duracion)[] notas)
        {
            int total = 0;
            foreach ((float _, float duracion) in notas)
                total += Mathf.CeilToInt(duracion * Frecuencia);

            float[] muestras = new float[total];
            int inicio = 0;
            foreach ((float frecuencia, float duracion) in notas)
            {
                int largo = Mathf.CeilToInt(duracion * Frecuencia);
                for (int i = 0; i < largo; i++)
                {
                    // Envolvente corta al inicio y al final para evitar chasquidos.
                    float envolvente = Mathf.Min(1, Mathf.Min(i, largo - i) / (0.01f * Frecuencia));
                    muestras[inicio + i] = 0.25f * envolvente * Mathf.Sin(2 * Mathf.PI * frecuencia * i / Frecuencia);
                }
                inicio += largo;
            }

            AudioClip clip = AudioClip.Create(nombre, total, 1, Frecuencia, false);
            clip.SetData(muestras, 0);
            return clip;
        }
    }
}
