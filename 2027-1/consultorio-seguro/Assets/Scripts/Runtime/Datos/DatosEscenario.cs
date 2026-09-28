using UnityEngine;

namespace ConsultorioSeguro
{
    [CreateAssetMenu(menuName = "Consultorio Seguro/Escenario", fileName = "Escenario")]
    public class DatosEscenario : ScriptableObject
    {
        public AccionSimulacion accion;
        public string nombre;

        [TextArea(4, 10)]
        public string instrucciones;

        [Min(0)] public int puntosPorAcierto = 10;
        [Min(0)] public int penalizacionPorError = 5;
    }
}
