using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Tests
{
    // Recorre las cinco áreas como lo haría un usuario: entra caminando a la zona,
    // completa los pasos, se equivoca una vez, clasifica bien el resto y repite la
    // práctica desde su hoja.
    public class RecorridoTests
    {
        [UnitySetUp]
        public IEnumerator CargarEscena()
        {
            yield return SceneManager.LoadSceneAsync("Consultorio");
            yield return null;
        }

        [UnityTest]
        public IEnumerator LosModelosSiguenLaInteraccionYSeRestablecen()
        {
            var mano = new GameObject("Mano de prueba").transform;
            mano.position = new Vector3(10, 3, 10);
            try
            {
                var residuos = Object.FindObjectsByType<Residuo>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                int comprobados = 0;
                foreach (var residuo in residuos)
                {
                    var modelo = residuo.transform.Find("Modelo importado");
                    if (modelo == null) continue;
                    var renderer = modelo.GetComponentInChildren<Renderer>(true);
                    Vector3 centro = renderer.bounds.center;
                    Vector3 tamano = renderer.bounds.size;
                    residuo.Sujetar(mano);
                    Assert.Less(Vector3.Distance(renderer.bounds.center, mano.position), .5f, residuo.name);
                    residuo.Devolver();
                    Assert.Less(Vector3.Distance(renderer.bounds.center, centro), .001f, residuo.name);
                    Assert.Less(Vector3.Distance(renderer.bounds.size, tamano), .001f, residuo.name);
                    comprobados++;
                }
                Assert.GreaterOrEqual(comprobados, 15);
                var brazo = GameObject.Find("Mobiliario/Equipo de rayos X/Brazo").transform;
                var malla = brazo.Find("Modelo importado móvil").GetComponentInChildren<Renderer>();
                Vector3 antes = malla.bounds.center;
                brazo.Rotate(0, -60, 0);
                Assert.Greater(Vector3.Distance(antes, malla.bounds.center), .1f);
            }
            finally { Object.Destroy(mano.gameObject); }
            yield return null;
        }

        [UnityTest]
        public IEnumerator SeCompletanTodasLasAreas()
        {
            GestorSimulacion gestor = Object.FindAnyObjectByType<GestorSimulacion>();
            ControladorPrimeraPersona jugador = Object.FindAnyObjectByType<ControladorPrimeraPersona>();
            Interactor interactor = Object.FindAnyObjectByType<Interactor>();
            List<string> informados = new();
            gestor.InformacionSolicitada += (titulo, _) => informados.Add(titulo);

            Assert.AreEqual(EstadoSimulacion.Menu, gestor.Estado);
            Assert.AreEqual(5, gestor.Escenarios.Count);
            gestor.Entrar();
            Assert.AreEqual(EstadoSimulacion.Jugando, gestor.Estado);

            foreach (Escenario escenario in gestor.Escenarios)
            {
                Residuo[] residuos = escenario.GetComponentsInChildren<Residuo>(true);
                bool conPasos = escenario.Secuencia != null;
                Assert.AreEqual(!conPasos, residuos.All(r => r.gameObject.activeSelf),
                    "Sin pasos, los residuos están en su lugar desde el inicio; con pasos, aparecen al terminarlos.");

                jugador.Teletransportar(escenario.GetComponentInChildren<ZonaEscenario>().transform);
                yield return Esperar(() => gestor.EscenarioEnFoco == escenario, 2, $"Entrar a la zona de {escenario.name} debe enfocarla.");
                CollectionAssert.Contains(informados, escenario.Datos.nombre, "Al entrar por primera vez se muestran las instrucciones.");

                if (conPasos)
                    yield return CompletarPasos(escenario.Secuencia);
                Assert.IsTrue(residuos.All(r => r.gameObject.activeSelf), escenario.name);

                Residuo primero = residuos[0];
                Destino equivocado = primero.Datos.destinoCorrecto == Destino.BasuraComun ? Destino.BolsaRoja : Destino.BasuraComun;
                escenario.Clasificar(primero, equivocado);
                Assert.IsTrue(primero.gameObject.activeSelf, "Un residuo mal clasificado vuelve a su lugar.");

                foreach (Residuo residuo in residuos)
                    escenario.Clasificar(residuo, residuo.Datos.destinoCorrecto);

                Assert.IsTrue(escenario.Completado, escenario.name);
                Assert.AreEqual(residuos.Length, escenario.Puntuacion.Aciertos);
                Assert.AreEqual(1, escenario.Puntuacion.Errores);
                Assert.GreaterOrEqual(GestorSimulacion.MejorPuntuacion(escenario.Datos.accion), escenario.Puntuacion.Puntos);

                TableroPractica tablero = escenario.GetComponentInChildren<TableroPractica>();
                StringAssert.StartsWith("Repetir", tablero.Indicacion(interactor));
                tablero.Interactuar(interactor);
                Assert.IsFalse(escenario.Completado, "La hoja de práctica reinicia el área.");
                Assert.AreEqual(0, escenario.Puntuacion.Aciertos + escenario.Puntuacion.Errores);
                Assert.AreEqual(!conPasos, residuos.All(r => r.gameObject.activeSelf));
            }
        }

        // Camina desde el punto de inicio, cruzando la calle por el paso peatonal, hasta el
        // consultorio con el CharacterController real: falla si una guarnición, un muro, una
        // puerta o un mueble bloquea el camino.
        [UnityTest]
        public IEnumerator SeLlegaCaminandoDeLaCalleAlConsultorio()
        {
            GestorSimulacion gestor = Object.FindAnyObjectByType<GestorSimulacion>();
            CharacterController controlador = Object.FindAnyObjectByType<ControladorPrimeraPersona>().GetComponent<CharacterController>();
            List<string> informados = new();
            gestor.InformacionSolicitada += (titulo, _) => informados.Add(titulo);
            gestor.Entrar();

            Vector3[] ruta =
            {
                new(0, 0, -8.6f), // banqueta de la clínica, frente a la entrada
                new(0, 0, -6.2f), // recepción
                new(0, 0, -3.8f), // frente a la puerta del consultorio
                new(0, 0, -2f), // dentro del consultorio
            };

            foreach (Vector3 punto in ruta)
            {
                float limite = Time.time + 10;
                while (DistanciaPlana(controlador.transform.position, punto) > 0.15f && Time.time < limite)
                {
                    Vector3 direccion = punto - controlador.transform.position;
                    direccion.y = 0;
                    controlador.Move(Vector3.ClampMagnitude(direccion, 3f * Time.deltaTime));
                    yield return null;
                }

                Assert.Less(DistanciaPlana(controlador.transform.position, punto), 0.15f, $"El jugador se atoró antes de llegar a {punto}.");
            }

            foreach (ZonaMensaje zona in Object.FindObjectsByType<ZonaMensaje>())
                CollectionAssert.Contains(informados, zona.Titulo, $"Al pasar por {zona.name} se muestra su mensaje.");
        }

        static float DistanciaPlana(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));

        static IEnumerator CompletarPasos(SecuenciaPasos secuencia)
        {
            float limite = Time.time + 20;
            while (!secuencia.Terminada && Time.time < limite)
            {
                if (!secuencia.Esperando)
                    secuencia.Completar(secuencia.PasoActual);
                yield return null;
            }

            Assert.IsTrue(secuencia.Terminada, "La secuencia de pasos no terminó.");
        }

        static IEnumerator Esperar(Func<bool> condicion, float segundos, string mensaje)
        {
            float limite = Time.time + segundos;
            while (!condicion() && Time.time < limite)
                yield return null;
            Assert.IsTrue(condicion(), mensaje);
        }
    }
}
