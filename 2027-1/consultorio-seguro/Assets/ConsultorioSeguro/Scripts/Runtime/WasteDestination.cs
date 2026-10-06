namespace ConsultorioSeguroNuevo
{
    public enum WasteDestination { General, RedBag, Sharps, Sterilization, ChemicalRecovery, PathologicalYellow }

    public static class WasteInformation
    {
        public static string Label(WasteDestination destination) => destination switch
        {
            WasteDestination.General => "Residuos comunes",
            WasteDestination.RedBag => "Bolsa roja · RPBI no anatómicos",
            WasteDestination.Sharps => "Recipiente rígido rojo · Punzocortantes",
            WasteDestination.Sterilization => "Área de reprocesamiento",
            WasteDestination.ChemicalRecovery => "Recolección de residuos químicos",
            WasteDestination.PathologicalYellow => "Bolsa amarilla · Residuos patológicos",
            _ => "Destino sin definir"
        };

        public static string Explanation(WasteDestination destination) => destination switch
        {
            WasteDestination.General => "En el caso descrito, el material no cumple los criterios de RPBI. Sigue también el programa local de separación de residuos.",
            WasteDestination.RedBag => "Las gasas y otros materiales empapados, saturados o goteando sangre se separan como RPBI no anatómicos. Una mancha aislada no equivale a estar saturado.",
            WasteDestination.Sharps => "Deposita agujas y hojas de bisturí usadas directamente en un recipiente rígido rojo autorizado. No reencapuches ni manipules las puntas.",
            WasteDestination.Sterilization => "El instrumento reutilizable se transporta de forma segura al reprocesamiento: limpieza, secado, inspección, empaque y esterilización validada según su fabricante.",
            WasteDestination.ChemicalRecovery => "Los líquidos de revelado, fijado y otros residuos químicos requieren su ruta específica. No se vierten al drenaje ni se mezclan con RPBI.",
            WasteDestination.PathologicalYellow => "La pieza extraída de este ejercicio, sin conservador ni amalgama, es un residuo patológico sólido y se coloca en bolsa amarilla.",
            _ => "Consulta el protocolo de tu institución."
        };
    }
}
