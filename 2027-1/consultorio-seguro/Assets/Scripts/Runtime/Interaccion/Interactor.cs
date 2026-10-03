using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ConsultorioSeguro
{
    // Lanza un rayo desde la cámara, muestra qué se puede hacer con lo que se
    // mira y sostiene el residuo que el usuario tomó.
    public class Interactor : MonoBehaviour
    {
        [SerializeField] Camera camara;
        [SerializeField] Transform puntoSujecion;
        [SerializeField, Min(0.5f)] float alcance = 2.5f;
        [SerializeField] LayerMask capas = Physics.DefaultRaycastLayers;

        InputAction accionInteractuar;
        string tecla;

        public Residuo Sostenido { get; private set; }
        public IInteractuable Objetivo { get; private set; }
        public string Indicacion { get; private set; } = string.Empty;

        public event Action<string> IndicacionCambiada;

        void Awake()
        {
            accionInteractuar = InputSystem.actions.FindAction("Player/Interact", true);
            tecla = accionInteractuar.GetBindingDisplayString(InputBinding.MaskByGroup("Keyboard&Mouse"));
        }

        void OnDisable()
        {
            Objetivo = null;
            CambiarIndicacion(string.Empty);
        }

        void Update()
        {
            IInteractuable objetivo = BuscarObjetivo();
            string texto = objetivo?.Indicacion(this);
            if (string.IsNullOrEmpty(texto))
                objetivo = null;

            Objetivo = objetivo;
            bool puede = objetivo != null && objetivo.PuedeInteractuar(this);

            if (objetivo != null)
                CambiarIndicacion(puede ? $"[{tecla}] {texto}" : texto);
            else if (Sostenido != null)
                CambiarIndicacion($"[{tecla}] Devolver a su lugar: {Sostenido.Datos.nombre}");
            else
                CambiarIndicacion(string.Empty);

            if (!accionInteractuar.WasPressedThisFrame())
                return;

            if (puede)
                objetivo.Interactuar(this);
            else if (objetivo == null)
                Liberar();
        }

        IInteractuable BuscarObjetivo()
        {
            Transform origen = camara.transform;
            if (!Physics.Raycast(origen.position, origen.forward, out RaycastHit impacto, alcance, capas,
                    QueryTriggerInteraction.Ignore))
                return null;

            return impacto.collider.GetComponentInParent<IInteractuable>();
        }

        public void Tomar(Residuo residuo)
        {
            if (Sostenido != null)
                return;

            Sostenido = residuo;
            residuo.Sujetar(puntoSujecion);
        }

        // Entrega el residuo sostenido sin regresarlo a su lugar.
        public Residuo Soltar()
        {
            Residuo residuo = Sostenido;
            Sostenido = null;
            return residuo;
        }

        // Regresa el residuo sostenido al lugar donde estaba.
        public void Liberar()
        {
            if (Sostenido == null)
                return;

            Sostenido.Devolver();
            Sostenido = null;
        }

        void CambiarIndicacion(string texto)
        {
            if (texto == Indicacion)
                return;

            Indicacion = texto;
            IndicacionCambiada?.Invoke(texto);
        }
    }
}
