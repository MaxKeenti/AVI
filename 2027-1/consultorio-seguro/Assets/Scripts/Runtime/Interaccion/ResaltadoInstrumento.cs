using System.Collections.Generic;
using UnityEngine;

namespace ConsultorioSeguro
{
    // Conserva la silueta real: no cambia el tamaño ni el material del instrumento.
    [DisallowMultipleComponent]
    public sealed class ResaltadoInstrumento : MonoBehaviour
    {
        public static readonly HashSet<ResaltadoInstrumento> Activos = new();
        public Renderer[] Mallas { get; private set; }
        Residuo residuo;
        Paso paso;
        Collider zona;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void LimpiarRegistro() => Activos.Clear();

        void Awake()
        {
            Mallas = GetComponentsInChildren<Renderer>(true);
            residuo = GetComponent<Residuo>();
            paso = GetComponent<Paso>();
            var escenario = GetComponentInParent<Escenario>();
            zona = escenario != null ? escenario.GetComponentInChildren<ZonaEscenario>().GetComponent<Collider>() : null;
        }

        void OnEnable() => Activos.Add(this);
        void OnDisable() => Activos.Remove(this);

        public bool DebeMostrar(Camera camara, Interactor interactor)
        {
            if (!isActiveAndEnabled || GestorSimulacion.Instancia == null ||
                GestorSimulacion.Instancia.Estado != EstadoSimulacion.Jugando || interactor == null)
                return false;
            if (residuo != null && (residuo.Clasificado || interactor.Sostenido == residuo)) return false;
            if (paso != null && !paso.PuedeInteractuar(interactor)) return false;
            // El halo atraviesa la charola, pero no revela instrumentos de otra sala.
            if (zona == null || !zona.bounds.Contains(camara.transform.position)) return false;
            return (transform.position - camara.transform.position).sqrMagnitude <= 4.5f * 4.5f;
        }

        public bool Enfocado(Interactor interactor) =>
            (residuo != null && interactor.Objetivo == residuo) || (paso != null && interactor.Objetivo == paso);
    }
}
