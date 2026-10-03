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
            foreach(var tablero in Object.FindObjectsByType<TableroPractica>(FindObjectsSortMode.None))
            {
                var reverso=tablero.transform.Find("Texto reverso");
                if(reverso==null) continue;
                var frente=tablero.transform.Find("Texto").GetComponent<TMPro.TMP_Text>();
                Assert.IsNotEmpty(frente.text);
                Assert.AreEqual(frente.text,reverso.GetComponent<TMPro.TMP_Text>().text);
            }
        }

        [UnityTest]
        public IEnumerator ManualYCreditosPermitenVolver()
        {
            var interfaz=Object.FindFirstObjectByType<InterfazSimulador>();
            var gestor=GestorSimulacion.Instancia;
            var raiz=interfaz.transform;
            foreach(var resolucion in new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080)})
            {
                Screen.SetResolution(resolucion.x,resolucion.y,false);
                interfaz.AbrirCreditos();
                yield return null;
                Canvas.ForceUpdateCanvases();
                var boton=raiz.Find("PanelCreditos/BotonVolver").GetComponent<UnityEngine.UI.Button>();
                var esquinas=new Vector3[4];
                boton.GetComponent<RectTransform>().GetWorldCorners(esquinas);
                Assert.GreaterOrEqual(esquinas[0].y,0);
                Assert.LessOrEqual(esquinas[2].y,Screen.height);
                Assert.IsFalse(raiz.Find("PanelCreditos/Desplazamiento/Ventana/TextoCreditos").GetComponent<TMPro.TMP_Text>().text.Contains("Todavía no"));
                boton.onClick.Invoke();
                Assert.IsTrue(raiz.Find("PanelMenu").gameObject.activeSelf);
            }
            interfaz.AbrirManual();
            Assert.IsTrue(interfaz.PanelAuxiliarAbierto);
            interfaz.CerrarPanelAuxiliar();
            Assert.AreEqual(EstadoSimulacion.Menu,gestor.Estado);
            gestor.Entrar(); gestor.Pausar();
            interfaz.AbrirManual(); interfaz.CerrarPanelAuxiliar();
            Assert.IsTrue(raiz.Find("PanelPausa").gameObject.activeSelf);
            Assert.AreEqual(EstadoSimulacion.Pausa,gestor.Estado);
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

        [UnityTest]
        public IEnumerator LaAvenidaAmpliadaEsTransitable()
        {
            Object.FindAnyObjectByType<GestorSimulacion>().Entrar();
            var controlador = Object.FindAnyObjectByType<ControladorPrimeraPersona>().GetComponent<CharacterController>();
            controlador.enabled = false;
            controlador.transform.position = new Vector3(-40, .05f, -8.3f);
            controlador.enabled = true;
            foreach (var punto in new[] { new Vector3(40,0,-8.3f), new Vector3(0,0,-8.3f), new Vector3(0,0,-6.2f) })
            {
                float limite = Time.time + 12;
                while (DistanciaPlana(controlador.transform.position,punto) > .15f && Time.time < limite)
                {
                    Vector3 direccion = punto - controlador.transform.position;
                    direccion.y = 0;
                    controlador.Move(Vector3.ClampMagnitude(direccion,10 * Time.deltaTime));
                    yield return null;
                }
                Assert.Less(DistanciaPlana(controlador.transform.position,punto),.15f,"La banqueta o la entrada están bloqueadas.");
                Assert.Greater(controlador.transform.position.y,-.5f,"Falta suelo transitable.");
            }
            Assert.Greater(GameObject.Find("Modelos de terceros/Torre sobre la clínica").GetComponentsInChildren<Renderer>().Max(r=>r.bounds.max.y),20);
        }

        [UnityTest]
        public IEnumerator SeAccedeACadaSalaDesdeElPasillo()
        {
            var gestor=Object.FindAnyObjectByType<GestorSimulacion>();
            gestor.Entrar();
            var jugador=Object.FindAnyObjectByType<ControladorPrimeraPersona>();
            var controlador=jugador.GetComponent<CharacterController>();
            controlador.enabled=false;
            jugador.transform.position=new Vector3(0,.05f,-2);
            controlador.enabled=true;
            var acciones=new[]{AccionSimulacion.Clasificacion,AccionSimulacion.Esterilizacion,AccionSimulacion.MuestraMateriales,AccionSimulacion.Procedimiento,AccionSimulacion.Radiografia};
            for(int i=0;i<acciones.Length;i++)
            {
                float z=6.8f+(i/2)*6.5f;
                float lado=i%2==0 ? -1:1;
                foreach(var destino in new[]{new Vector3(0,0,jugador.transform.position.z),new Vector3(0,0,z),new Vector3(lado*3,0,z)})
                {
                    float limite=Time.time+10;
                    while(DistanciaPlana(jugador.transform.position,destino)>.15f && Time.time<limite)
                    {
                        Vector3 direccion=destino-jugador.transform.position; direccion.y=0;
                        controlador.Move(Vector3.ClampMagnitude(direccion,5*Time.deltaTime));
                        yield return null;
                    }
                    Assert.Less(DistanciaPlana(jugador.transform.position,destino),.15f,"Acceso bloqueado a "+acciones[i]);
                }
                yield return null;
                Assert.AreEqual(acciones[i],gestor.EscenarioEnFoco.Datos.accion,"El pasillo activa una sala equivocada.");
            }
        }

        [UnityTest]
        public IEnumerator LaRecepcionConservaLaGuiaYSePuedeConsultar()
        {
            var bienvenida=GameObject.Find("Recepción ampliada y guías/Bienvenida recepción");
            StringAssert.Contains("Manual de controles",bienvenida.GetComponentInChildren<TMPro.TMP_Text>().text);
            var registro=GameObject.Find("Diseño interior/Registro de pacientes");
            Assert.AreNotSame(bienvenida,registro);
            var limites=registro.transform.Find("Placa").GetComponent<Renderer>().bounds;
            Assert.Greater(limites.min.x,1.25f+.04f,"El rótulo invade el pasillo.");
            Assert.Less(limites.max.x,3.5f-.04f,"El rótulo se incrusta en la pared lateral.");
            var gestor=GestorSimulacion.Instancia;
            gestor.Entrar();
            var jugador=Object.FindAnyObjectByType<ControladorPrimeraPersona>();
            var controlador=jugador.GetComponent<CharacterController>();
            controlador.enabled=false;
            jugador.transform.position=new Vector3(0,.05f,-6.2f);
            controlador.enabled=true;
            foreach(var punto in new[]{new Vector3(0,0,.55f),new Vector3(2.4f,0,.55f),new Vector3(0,0,.55f),new Vector3(0,0,5)})
            {
                float limite=Time.time+10;
                while(DistanciaPlana(jugador.transform.position,punto)>.15f && Time.time<limite)
                {
                    Vector3 direccion=punto-jugador.transform.position; direccion.y=0;
                    controlador.Move(Vector3.ClampMagnitude(direccion,4*Time.deltaTime));
                    yield return null;
                }
                Assert.Less(DistanciaPlana(jugador.transform.position,punto),.15f,"Acceso a recepción bloqueado.");
                if(punto.x>2)
                {
                    var ojo=jugador.GetComponentInChildren<Camera>().transform.position;
                    var direccion=new Vector3(2.4f,.65f,1.5f)-ojo;
                    Assert.IsTrue(Physics.Raycast(ojo,direccion,out RaycastHit impacto,2.5f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore));
                    var info=impacto.collider.GetComponentInParent<ObjetoInformativo>();
                    Assert.NotNull(info,$"El rayo desde {ojo} golpeó {impacto.collider.name} ({impacto.collider.transform.parent?.name}) en {impacto.point}, sin interacción de recepción.");
                    jugador.Habilitado=false;
                    jugador.GetComponentInChildren<Camera>().transform.rotation=Quaternion.LookRotation(direccion);
                    yield return null;
                    StringAssert.Contains("Recepción",Object.FindAnyObjectByType<Interactor>().Indicacion);
                    string titulo=null;
                    gestor.InformacionSolicitada+=(t,_)=>titulo=t;
                    info.Interactuar(Object.FindAnyObjectByType<Interactor>());
                    Assert.AreEqual("Recepción",titulo);
                }
            }
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
