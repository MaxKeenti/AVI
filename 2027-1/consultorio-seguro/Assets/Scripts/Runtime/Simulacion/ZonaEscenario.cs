using UnityEngine;

namespace ConsultorioSeguro
{
    // Volumen que rodea un área del consultorio. Al entrar, el usuario ve las
    // instrucciones de esa acción y el marcador cambia a ella.
    [RequireComponent(typeof(Collider))]
    public class ZonaEscenario : MonoBehaviour
    {
        Escenario escenario;

        public Escenario Escenario => escenario;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void Awake()
        {
            escenario = GetComponentInParent<Escenario>();
            GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider otro)
        {
            if (otro.GetComponent<ControladorPrimeraPersona>() != null)
                GestorSimulacion.Instancia?.EntrarZona(escenario);
        }
    }
}
