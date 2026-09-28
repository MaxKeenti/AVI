using UnityEngine;

namespace ConsultorioSeguro
{
    [CreateAssetMenu(menuName = "Consultorio Seguro/Residuo", fileName = "Residuo")]
    public class DatosResiduo : ScriptableObject
    {
        public string nombre;

        public Destino destinoCorrecto;

        [Tooltip("Por qué va en ese destino. Se muestra al clasificar, acierte o no el usuario.")]
        [TextArea(3, 6)]
        public string explicacion;
    }
}
