namespace ConsultorioSeguro
{
    // Todo lo que el Interactor puede señalar con la mirada.
    public interface IInteractuable
    {
        bool PuedeInteractuar(Interactor interactor);

        // Texto que se muestra al mirarlo. Vacío o null = se ignora.
        string Indicacion(Interactor interactor);

        void Interactuar(Interactor interactor);
    }
}
