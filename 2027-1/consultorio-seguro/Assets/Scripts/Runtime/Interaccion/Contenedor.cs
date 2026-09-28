using UnityEngine;

namespace ConsultorioSeguro
{
    // Recipiente o estación donde se deposita un residuo.
    public class Contenedor : MonoBehaviour, IInteractuable
    {
        [SerializeField] Destino destino;

        public Destino Destino => destino;

        public bool PuedeInteractuar(Interactor interactor) => true;

        public string Indicacion(Interactor interactor) =>
            interactor.Sostenido != null ? $"Depositar en: {destino.Nombre()}" : $"Ver: {destino.Nombre()}";

        public void Interactuar(Interactor interactor)
        {
            if (interactor.Sostenido == null)
            {
                GestorSimulacion.Instancia?.Informar(destino.Nombre(), destino.Descripcion());
                return;
            }

            Residuo residuo = interactor.Soltar();
            residuo.Escenario.Clasificar(residuo, destino);
        }
    }
}
