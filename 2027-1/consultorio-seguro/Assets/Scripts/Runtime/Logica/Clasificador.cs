namespace ConsultorioSeguro
{
    public readonly struct ResultadoClasificacion
    {
        public readonly bool Correcto;
        public readonly string Titulo;
        public readonly string Mensaje;

        public ResultadoClasificacion(bool correcto, string titulo, string mensaje)
        {
            Correcto = correcto;
            Titulo = titulo;
            Mensaje = mensaje;
        }
    }

    public static class Clasificador
    {
        public static ResultadoClasificacion Evaluar(DatosResiduo residuo, Destino destino)
        {
            bool correcto = residuo.destinoCorrecto == destino;
            string mensaje = correcto
                ? $"{residuo.nombre} va en: {destino.Nombre()}."
                : $"{residuo.nombre} no va en: {destino.Nombre()}.";

            if (!string.IsNullOrWhiteSpace(residuo.explicacion))
                mensaje += "\n" + residuo.explicacion;

            return new ResultadoClasificacion(correcto, correcto ? "¡Correcto!" : "Incorrecto", mensaje);
        }
    }
}
