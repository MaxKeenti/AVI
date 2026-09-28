using UnityEngine;
using UnityEngine.Events;

namespace ConsultorioSeguro
{
    // Una acción dentro de una SecuenciaPasos: encender un equipo, colocar
    // barreras, extraer una pieza... Solo responde cuando es su turno.
    public class Paso : MonoBehaviour, IInteractuable
    {
        [SerializeField] string accion = "Realizar paso";

        [Tooltip("Se muestra al completar el paso.")]
        [SerializeField, TextArea(2, 4)] string mensaje;

        [Tooltip("Segundos antes de que el siguiente paso esté disponible, por ejemplo, el calentamiento del equipo.")]
        [SerializeField, Min(0)] float espera;

        [SerializeField] UnityEvent alCompletar;

        [Tooltip("Debe deshacer lo que hace alCompletar para poder repetir la acción.")]
        [SerializeField] UnityEvent alReiniciar;

        public SecuenciaPasos Secuencia { get; set; }
        public string Accion => accion;
        public string Mensaje => mensaje;
        public float Espera => espera;
        public bool Completado { get; private set; }
        public UnityEvent AlCompletar => alCompletar;
        public UnityEvent AlReiniciar => alReiniciar;

        public bool PuedeInteractuar(Interactor interactor) =>
            Secuencia != null && Secuencia.EsPasoActual(this) && interactor.Sostenido == null;

        public string Indicacion(Interactor interactor)
        {
            if (Secuencia == null || !Secuencia.Activa)
                return null;
            if (Completado)
                return $"{accion} (hecho)";
            if (PuedeInteractuar(interactor))
                return accion;
            if (Secuencia.Esperando)
                return "Espera a que termine el paso anterior";
            if (Secuencia.PasoActual == this)
                return $"{accion} (primero deja el residuo que llevas)";
            return $"{accion} (antes: {Secuencia.PasoActual.accion})";
        }

        public void Interactuar(Interactor interactor) => Secuencia.Completar(this);

        public void MarcarCompletado()
        {
            Completado = true;
            alCompletar.Invoke();
        }

        public void Reiniciar()
        {
            Completado = false;
            alReiniciar.Invoke();
        }
    }
}
