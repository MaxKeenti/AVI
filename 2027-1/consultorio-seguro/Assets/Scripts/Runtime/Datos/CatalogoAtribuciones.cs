using System.Collections.Generic;
using UnityEngine;

namespace ConsultorioSeguro
{
    // Lista que lee la pantalla de créditos. El editor la llena solo con todas
    // las Atribucion del proyecto; no hace falta editarla a mano.
    [CreateAssetMenu(menuName = "Consultorio Seguro/Catálogo de atribuciones", fileName = "CatalogoAtribuciones")]
    public class CatalogoAtribuciones : ScriptableObject
    {
        public List<Atribucion> atribuciones = new();
    }
}
