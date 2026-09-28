using UnityEngine;

namespace ConsultorioSeguro
{
    // Mobiliario o equipo que muestra su nombre al mirarlo y una descripción al interactuar.
    public class ObjetoInformativo : MonoBehaviour, IInteractuable
    {
        [SerializeField] string nombre;
        [SerializeField, TextArea(2, 5)] string descripcion;

        public bool PuedeInteractuar(Interactor interactor) => !string.IsNullOrWhiteSpace(descripcion);

        public string Indicacion(Interactor interactor) =>
            PuedeInteractuar(interactor) ? $"Ver: {nombre}" : nombre;

        public void Interactuar(Interactor interactor) =>
            GestorSimulacion.Instancia?.Informar(nombre, descripcion);
    }
}
