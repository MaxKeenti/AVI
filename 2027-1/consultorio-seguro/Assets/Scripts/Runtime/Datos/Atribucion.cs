using UnityEngine;

namespace ConsultorioSeguro
{
    // Fuente de un recurso de terceros (modelo, textura, sonido).
    // Cada carpeta dentro de Assets/Terceros/ debe tener una.
    [CreateAssetMenu(menuName = "Consultorio Seguro/Atribución", fileName = "Atribucion")]
    public class Atribucion : ScriptableObject
    {
        [Tooltip("Nombre del recurso tal como aparece en el sitio.")]
        public string titulo;

        public string autor;

        [Tooltip("Página desde la que se descargó el recurso.")]
        public string url;

        [Tooltip("Por ejemplo: CC0 1.0, CC BY 4.0, Licencia estándar de la Unity Asset Store.")]
        public string licencia;

        [Tooltip("Fecha de consulta en formato AAAA-MM-DD, necesaria para citar en APA.")]
        public string fechaConsulta;

        [Tooltip("Cambios que hizo el equipo: escala, texturas, recortes, etc. Algunas licencias (CC BY) piden indicarlo.")]
        [TextArea(2, 4)]
        public string modificaciones;
    }
}
