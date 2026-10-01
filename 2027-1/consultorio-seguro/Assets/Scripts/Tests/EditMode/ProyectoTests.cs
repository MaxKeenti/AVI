using System.Linq;
using ConsultorioSeguro.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConsultorioSeguro.Tests
{
    // Revisa que los datos, las atribuciones y la escena estén completos.
    public class ProyectoTests
    {
        const string RutaEscena = "Assets/Escenas/Consultorio.unity";

        static T[] Cargar<T>() where T : Object =>
            AssetDatabase.FindAssets("t:" + typeof(T).Name)
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

        [Test]
        public void LasPlacasMiranHaciaLasZonasDeLectura()
        {
            EditorSceneManager.OpenScene(RutaEscena,OpenSceneMode.Single);
            foreach(var tablero in Object.FindObjectsByType<TableroPractica>(FindObjectsSortMode.None))
            {
                var accion=tablero.GetComponentInParent<Escenario>().Datos.accion;
                var t=tablero.transform;
                Vector3 lectura;
                if(accion==AccionSimulacion.Clasificacion || accion==AccionSimulacion.Procedimiento)
                {
                    lectura=accion==AccionSimulacion.Clasificacion?new Vector3(-1.25f,t.position.y,6.8f):new Vector3(1.25f,t.position.y,13.3f);
                    Assert.NotNull(t.Find("Texto reverso"));
                    Assert.Less(Vector3.Dot(t.Find("Texto").forward,t.Find("Texto reverso").forward),-.99f);
                }
                else
                {
                    var normal=accion==AccionSimulacion.Esterilizacion?Vector3.left:Vector3.back;
                    lectura=t.position+normal*2;
                }
                Assert.Greater(Vector3.Dot(-t.forward,(lectura-t.position).normalized),.99f,accion.ToString());
                Physics.SyncTransforms();
                Assert.IsTrue(Physics.Raycast(t.position-t.forward,t.forward,out RaycastHit impacto,1.2f,~0,QueryTriggerInteraction.Ignore));
                Assert.AreSame(tablero,impacto.collider.GetComponentInParent<TableroPractica>(),"Placa tapada: "+accion);
            }
            foreach(var t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
            {
                if(t.name=="Manual de sala") Assert.Greater(Vector3.Dot(-t.forward,Vector3.forward),.99f);
                if(t.name.StartsWith("Etiqueta "))
                {
                    float x=t.position.x;
                    Vector3 hacia=Mathf.Abs(x)<3?Vector3.right:x<0?Vector3.right:Vector3.left;
                    Assert.Greater(Vector3.Dot(-t.forward,hacia),.99f,t.name);
                }
            }
        }

        [Test]
        public void LosResiduosTienenNombreYExplicacion()
        {
            DatosResiduo[] residuos = Cargar<DatosResiduo>();
            Assert.IsNotEmpty(residuos);
            foreach (DatosResiduo residuo in residuos)
            {
                Assert.IsNotEmpty(residuo.nombre, AssetDatabase.GetAssetPath(residuo));
                Assert.IsNotEmpty(residuo.explicacion, AssetDatabase.GetAssetPath(residuo));
            }
        }

        [Test]
        public void HayUnEscenarioPorAccion()
        {
            DatosEscenario[] escenarios = Cargar<DatosEscenario>();
            foreach (AccionSimulacion accion in System.Enum.GetValues(typeof(AccionSimulacion)))
                Assert.AreEqual(1, escenarios.Count(e => e.accion == accion), accion.ToString());

            foreach (DatosEscenario escenario in escenarios)
            {
                Assert.IsNotEmpty(escenario.nombre, AssetDatabase.GetAssetPath(escenario));
                Assert.IsNotEmpty(escenario.instrucciones, AssetDatabase.GetAssetPath(escenario));
            }
        }

        [Test]
        public void TodosLosRecursosDeTercerosTienenAtribucion()
        {
            var problemas = HerramientasAtribucion.Problemas();
            Assert.IsEmpty(problemas, string.Join("\n", problemas));
        }

        [Test]
        public void LaEscenaEstaCompleta()
        {
            Scene escena = EditorSceneManager.OpenScene(RutaEscena, OpenSceneMode.Additive);
            try
            {
                GameObject[] raices = escena.GetRootGameObjects();
                GestorSimulacion gestor = raices.Select(r => r.GetComponentInChildren<GestorSimulacion>()).Single(g => g != null);
                Escenario[] escenarios = raices.SelectMany(r => r.GetComponentsInChildren<Escenario>(true)).ToArray();

                Assert.AreEqual(escenarios.Length, gestor.Escenarios.Count, "El gestor no conoce todos los escenarios.");
                TableroPractica[] tableros = raices.SelectMany(r => r.GetComponentsInChildren<TableroPractica>(true)).ToArray();
                foreach (Escenario escenario in escenarios)
                {
                    Assert.IsNotNull(escenario.Datos, escenario.name);

                    // Cada área necesita un volumen que detecte al usuario y su hoja de práctica.
                    ZonaEscenario zona = escenario.GetComponentInChildren<ZonaEscenario>(true);
                    Assert.IsNotNull(zona, $"{escenario.name} no tiene ZonaEscenario.");
                    Assert.IsTrue(zona.GetComponent<Collider>().isTrigger, $"La zona de {escenario.name} debe ser trigger.");
                    Assert.AreEqual(1, tableros.Count(t => new SerializedObject(t).FindProperty("escenario").objectReferenceValue == escenario),
                        $"{escenario.name} debe tener una hoja de práctica.");
                    Residuo[] residuos = escenario.GetComponentsInChildren<Residuo>(true);
                    Assert.IsNotEmpty(residuos, escenario.name);
                    foreach (Residuo residuo in residuos)
                        Assert.IsNotNull(residuo.Datos, $"{escenario.name}/{residuo.name}");
                }

                // Cada destino usado por un residuo necesita un recipiente en la escena.
                var destinos = raices.SelectMany(r => r.GetComponentsInChildren<Contenedor>(true)).Select(c => c.Destino).ToHashSet();
                foreach (Residuo residuo in escenarios.SelectMany(e => e.GetComponentsInChildren<Residuo>(true)))
                    Assert.IsTrue(destinos.Contains(residuo.Datos.destinoCorrecto),
                        $"No hay recipiente para {residuo.Datos.destinoCorrecto} ({residuo.Datos.nombre}).");
            }
            finally
            {
                EditorSceneManager.CloseScene(escena, true);
            }
        }
    }
}
