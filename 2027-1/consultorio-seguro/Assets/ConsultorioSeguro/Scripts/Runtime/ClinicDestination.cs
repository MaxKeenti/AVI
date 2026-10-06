using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    public sealed class ClinicDestination : ClinicInteractable
    {
        public WasteDestination destination;
        public override bool CanOutline(ClinicGame game) => false;
        public override bool CanInteract(ClinicGame game) => game.Playing && game.CurrentRoom
            && game.CurrentRoom.roomId == roomId;
        public override string Prompt(ClinicGame game) => (game.HeldItem ? "E · Depositar en: " : "E · Consultar: ")
            + WasteInformation.Label(destination);
        public override void Interact(ClinicGame game)
        {
            if (!CanInteract(game)) return;
            if (game.HeldItem) game.Deposit(this);
            else game.ShowMessage(WasteInformation.Label(destination), WasteInformation.Explanation(destination));
        }
    }
}
