using UnityEngine;

namespace ConsultorioSeguro
{
    // Recipientes de la NOM-087-ECOL-SSA1-2002 más los destinos que no son RPBI.
    public enum Destino
    {
        BolsaRoja,
        BolsaAmarilla,
        RecipienteHermeticoRojo,
        RecipienteHermeticoAmarillo,
        Punzocortantes,
        BasuraComun,
        Esterilizacion,
    }

    public static class DestinoExtensiones
    {
        public static string Nombre(this Destino destino) => destino switch
        {
            Destino.BolsaRoja => "Bolsa roja",
            Destino.BolsaAmarilla => "Bolsa amarilla",
            Destino.RecipienteHermeticoRojo => "Recipiente hermético rojo",
            Destino.RecipienteHermeticoAmarillo => "Recipiente hermético amarillo",
            Destino.Punzocortantes => "Recipiente de punzocortantes",
            Destino.BasuraComun => "Basura común",
            Destino.Esterilizacion => "Área de esterilización",
            _ => destino.ToString(),
        };

        // Texto de los letreros que acompañan a cada recipiente.
        public static string Descripcion(this Destino destino) => destino switch
        {
            Destino.BolsaRoja =>
                "RPBI sólidos: sangre, cultivos y residuos no anatómicos, como material de curación empapado, saturado o goteando sangre.",
            Destino.BolsaAmarilla =>
                "Patológicos sólidos: tejidos, órganos y partes que se extirpan o remueven durante una intervención.",
            Destino.RecipienteHermeticoRojo =>
                "RPBI líquidos: sangre líquida y residuos no anatómicos líquidos.",
            Destino.RecipienteHermeticoAmarillo =>
                "Patológicos líquidos: fluidos de tejidos y órganos, excepto orina y excremento.",
            Destino.Punzocortantes =>
                "Recipiente rígido: agujas, hojas de bisturí, lancetas y otros punzocortantes usados con el paciente.",
            Destino.BasuraComun =>
                "Residuos sin sangre ni fluidos del listado de la norma: envolturas, papel, barreras y consumibles sin contaminar.",
            Destino.Esterilizacion =>
                "Instrumental reutilizable: se lava, se empaqueta y se esteriliza para volver a usarse.",
            _ => string.Empty,
        };

        public static Color Color(this Destino destino) => destino switch
        {
            Destino.BolsaRoja or Destino.RecipienteHermeticoRojo or Destino.Punzocortantes =>
                new Color(0.78f, 0.1f, 0.12f),
            Destino.BolsaAmarilla or Destino.RecipienteHermeticoAmarillo =>
                new Color(0.95f, 0.8f, 0.1f),
            Destino.BasuraComun => new Color(0.35f, 0.37f, 0.4f),
            Destino.Esterilizacion => new Color(0.2f, 0.45f, 0.75f),
            _ => UnityEngine.Color.white,
        };

        public static bool EsRpbi(this Destino destino) =>
            destino is not (Destino.BasuraComun or Destino.Esterilizacion);
    }
}
