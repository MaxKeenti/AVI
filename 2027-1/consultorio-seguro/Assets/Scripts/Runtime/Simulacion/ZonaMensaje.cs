using UnityEngine;

namespace ConsultorioSeguro
{
    // Volumen que muestra un mensaje la primera vez que el usuario entra, por
    // ejemplo, la bienvenida al cruzar la puerta de la clínica.
    [RequireComponent(typeof(Collider))]
    public class ZonaMensaje : MonoBehaviour
    {
        [SerializeField] string titulo;
        [SerializeField, TextArea(2, 5)] string texto;

        bool mostrado;

        public string Titulo => titulo;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void Awake() => GetComponent<Collider>().isTrigger = true;

        void OnTriggerEnter(Collider otro)
        {
            if (mostrado || otro.GetComponent<ControladorPrimeraPersona>() == null)
                return;

            mostrado = true;
            GestorSimulacion.Instancia?.Informar(titulo, texto);
        }
    }
}
