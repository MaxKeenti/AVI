namespace ConsultorioSeguroNuevo
{
    public sealed class ClinicManual : ClinicInteractable
    {
        public override bool CanOutline(ClinicGame game) => false;
        public override bool CanInteract(ClinicGame game) => game.Playing;
        public override string Prompt(ClinicGame game) => "E · Abrir el manual de controles";
        public override void Interact(ClinicGame game)
        {
            if (CanInteract(game)) game.OpenManual();
        }
    }
}
