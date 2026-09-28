using System;

namespace ConsultorioSeguro
{
    public class Puntuacion
    {
        readonly int puntosPorAcierto;
        readonly int penalizacionPorError;

        public int Aciertos { get; private set; }
        public int Errores { get; private set; }

        // Nunca baja de cero para no desanimar a quien apenas empieza.
        public int Puntos => Math.Max(0, Aciertos * puntosPorAcierto - Errores * penalizacionPorError);

        public int PuntosMaximos(int totalResiduos) => totalResiduos * puntosPorAcierto;

        public Puntuacion(int puntosPorAcierto, int penalizacionPorError)
        {
            this.puntosPorAcierto = puntosPorAcierto;
            this.penalizacionPorError = penalizacionPorError;
        }

        public void Registrar(bool correcto)
        {
            if (correcto)
                Aciertos++;
            else
                Errores++;
        }

        public void Reiniciar()
        {
            Aciertos = 0;
            Errores = 0;
        }
    }
}
