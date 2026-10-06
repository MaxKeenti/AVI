using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    public abstract class ClinicInteractable : MonoBehaviour
    {
        public int roomId;
        public bool showOutline = true;
        public abstract string Prompt(ClinicGame game);
        public abstract bool CanInteract(ClinicGame game);
        public abstract void Interact(ClinicGame game);
        public virtual bool CanOutline(ClinicGame game) => showOutline && CanInteract(game);
    }
}
