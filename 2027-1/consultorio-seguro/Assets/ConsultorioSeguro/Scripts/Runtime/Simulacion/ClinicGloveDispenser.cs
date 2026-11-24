namespace ConsultorioSeguroNuevo
{
    /// <summary>Dispensador de guantes de cada sala: ponerse guantes habilita la manipulación de materiales.</summary>
    public sealed class ClinicGloveDispenser : ClinicInteractable
    {
        public override bool CanOutline(ClinicGame game) => false;
        public override bool CanInteract(ClinicGame game) => game.Playing && !game.HeldItem && !game.GlovesOn;
        public override string Prompt(ClinicGame game) =>
            game.GlovesOn ? "Guantes puestos" : game.HeldItem ? "" : "E · Ponerse guantes";
        public override void Interact(ClinicGame game)
        {
            if (CanInteract(game)) game.PutOnGloves();
        }
    }
}
