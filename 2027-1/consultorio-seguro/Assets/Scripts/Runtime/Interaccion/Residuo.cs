using UnityEngine;

namespace ConsultorioSeguro
{
    // Objeto que el usuario toma y deposita. Debe estar dentro de un Escenario.
    [DisallowMultipleComponent]
    public class Residuo : MonoBehaviour, IInteractuable
    {
        [SerializeField] DatosResiduo datos;

        Transform padreInicial;
        Vector3 posicionInicial;
        Quaternion rotacionInicial;
        Vector3 escalaInicial;
        Collider[] colisionadores;

        public DatosResiduo Datos => datos;
        public Escenario Escenario { get; private set; }
        public bool Clasificado { get; private set; }

        public void Inicializar(Escenario escenario)
        {
            Escenario = escenario;
            padreInicial = transform.parent;
            posicionInicial = transform.localPosition;
            rotacionInicial = transform.localRotation;
            escalaInicial = transform.localScale;
            colisionadores = GetComponentsInChildren<Collider>(true);
        }

        public bool PuedeInteractuar(Interactor interactor) =>
            Escenario != null && !Clasificado && interactor.Sostenido == null;

        public string Indicacion(Interactor interactor) =>
            PuedeInteractuar(interactor) ? $"Tomar: {datos.nombre}" : datos.nombre;

        public void Interactuar(Interactor interactor)
        {
            interactor.Tomar(this);
            GestorSimulacion.Instancia?.Enfocar(Escenario);
        }

        public void Sujetar(Transform punto)
        {
            transform.SetParent(punto, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            ActivarColisiones(false);
        }

        public void Devolver()
        {
            transform.SetParent(padreInicial, false);
            transform.localPosition = posicionInicial;
            transform.localRotation = rotacionInicial;
            transform.localScale = escalaInicial;
            ActivarColisiones(true);
        }

        public void MarcarClasificado()
        {
            Clasificado = true;
            Devolver();
            gameObject.SetActive(false);
        }

        public void Restablecer(bool visible)
        {
            Clasificado = false;
            Devolver();
            gameObject.SetActive(visible);
        }

        public void Mostrar() => gameObject.SetActive(!Clasificado);

        void ActivarColisiones(bool activas)
        {
            foreach (Collider colisionador in colisionadores)
                colisionador.enabled = activas;
        }
    }
}
