using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Tests
{
    public class ContornoInstrumentosTests
    {
        [UnityTest]
        public IEnumerator ElHaloSeRenderizaYRespetaElEstadoDelInstrumento()
        {
            yield return SceneManager.LoadSceneAsync("Consultorio");
            yield return null;
            var gestor = GestorSimulacion.Instancia;
            var jugador = Object.FindFirstObjectByType<ControladorPrimeraPersona>();
            var interactor = jugador.GetComponent<Interactor>();
            var camara = jugador.GetComponentInChildren<Camera>();
            var residuo = Object.FindObjectsByType<Residuo>(FindObjectsSortMode.None)
                .First(r => r.Datos.nombre.Contains("Espejo") && r.Escenario.Datos.accion == AccionSimulacion.Esterilizacion);
            var halo = residuo.GetComponent<ResaltadoInstrumento>();
            Assert.NotNull(halo);
            var origen = residuo.transform.position + new Vector3(-.9f, .7f, -.12f);
            camara.transform.SetPositionAndRotation(origen, Quaternion.LookRotation(residuo.transform.position - origen));
            Assert.IsFalse(halo.DebeMostrar(camara, interactor), "No se dibuja en el menú.");
            gestor.Entrar(); jugador.Habilitado = false;
            var interfaz = GameObject.Find("Interfaz"); interfaz.SetActive(false);
            for (int i = 0; i < 8; i++) yield return null;
            Assert.AreSame(residuo, interactor.Objetivo, "Se debe poder apuntar al instrumento real.");
            Assert.IsTrue(halo.Enfocado(interactor));
            Assert.IsTrue(halo.DebeMostrar(camara, interactor));
            var rt = new RenderTexture(1600, 1000, 24); rt.Create();
            camara.targetTexture = rt;
            Texture2D conHalo = null, sinHalo = null;
            Keyboard teclado = null;
            var focoAnterior = InputSystem.settings.editorInputBehaviorInPlayMode;
            var fondoAnterior = InputSystem.settings.backgroundBehavior;
            try
            {
                conHalo = Capturar(camara, rt);
                var activos = ResaltadoInstrumento.Activos.ToArray();
                foreach (var h in activos) h.enabled = false;
                sinHalo = Capturar(camara, rt);
                foreach (var h in activos) h.enabled = true;
                var a = conHalo.GetPixels32(); var b = sinHalo.GetPixels32();
                int cambiados = 0, dorados = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    if (Math.Abs(a[i].r - b[i].r) + Math.Abs(a[i].g - b[i].g) + Math.Abs(a[i].b - b[i].b) < 90) continue;
                    cambiados++;
                    if (a[i].r > 210 && a[i].g > 100 && a[i].g < 210 && a[i].b < 100) dorados++;
                }
                Assert.Greater(cambiados, 100, "El shader debe dibujar un halo visible, no solo registrar el componente.");
                Assert.Greater(dorados, 20, "El objetivo debe tener un borde dorado visible.");
                string carpeta = Environment.GetEnvironmentVariable("AVI_CAPTURAS_REVISION");
                if (!string.IsNullOrEmpty(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                    File.WriteAllBytes(Path.Combine(carpeta, "instrumentos-contorno.png"), conHalo.EncodeToPNG());
                    File.WriteAllBytes(Path.Combine(carpeta, "instrumentos-sin-contorno.png"), sinHalo.EncodeToPNG());
                }
                gestor.Pausar(); Assert.IsFalse(halo.DebeMostrar(camara, interactor));
                gestor.Reanudar(); jugador.Habilitado = false;
                yield return null;
                StringAssert.StartsWith("[E]", interactor.Indicacion, "El aviso debe describir una pulsación, no mantener la tecla.");
                // El ejecutor sin ventana enfocada debe recibir la entrada sintética como un jugador.
                InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
                InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
                teclado = InputSystem.AddDevice<Keyboard>();
                yield return null;
                Assert.IsTrue(InputSystem.actions.FindAction("Player/Interact").enabled);
                InputSystem.QueueStateEvent(teclado, new KeyboardState(Key.E));
                yield return null;
                InputSystem.QueueStateEvent(teclado, new KeyboardState());
                yield return null;
                Assert.AreSame(residuo, interactor.Sostenido, "Una pulsación breve de E toma el instrumento.");
                Assert.IsFalse(halo.DebeMostrar(camara, interactor), "No se resalta el objeto en la mano.");
                interactor.Liberar(); Assert.IsTrue(halo.DebeMostrar(camara, interactor));
                residuo.MarcarClasificado(); Assert.IsFalse(ResaltadoInstrumento.Activos.Contains(halo));
                residuo.Restablecer(true); Assert.IsTrue(halo.DebeMostrar(camara, interactor));
                camara.transform.position = new Vector3(6.5f, 1.6f, 11.6f);
                Assert.Less(Vector3.Distance(camara.transform.position, residuo.transform.position), 4.5f,
                    "La comprobación debe aislar el límite de la sala, no la distancia.");
                Assert.IsFalse(halo.DebeMostrar(camara, interactor), "No atraviesa paredes de otras salas.");

                var jeringa = Object.FindObjectsByType<Paso>(FindObjectsSortMode.None).Single(p => p.name == "Jeringa con anestesia");
                var gasas = Object.FindObjectsByType<Paso>(FindObjectsSortMode.None).Single(p => p.name == "Paquete de gasas");
                camara.transform.position = jeringa.transform.position + Vector3.up * .7f;
                Assert.IsTrue(jeringa.GetComponent<ResaltadoInstrumento>().DebeMostrar(camara, interactor));
                Assert.IsFalse(gasas.GetComponent<ResaltadoInstrumento>().DebeMostrar(camara, interactor));
                var secuencia = jeringa.Secuencia;
                secuencia.Completar(jeringa);
                secuencia.Completar(secuencia.PasoActual);
                Assert.AreSame(gasas, secuencia.PasoActual);
                Assert.IsFalse(jeringa.GetComponent<ResaltadoInstrumento>().DebeMostrar(camara, interactor));
                Assert.IsTrue(gasas.GetComponent<ResaltadoInstrumento>().DebeMostrar(camara, interactor));
            }
            finally
            {
                if (teclado != null) InputSystem.RemoveDevice(teclado);
                InputSystem.settings.editorInputBehaviorInPlayMode = focoAnterior;
                InputSystem.settings.backgroundBehavior = fondoAnterior;
                camara.targetTexture = null; rt.Release(); Object.Destroy(rt);
                if (conHalo != null) Object.Destroy(conHalo);
                if (sinHalo != null) Object.Destroy(sinHalo);
                interfaz.SetActive(true);
            }
        }

        static Texture2D Capturar(Camera camara, RenderTexture rt)
        {
            camara.Render();
            var anterior = RenderTexture.active; RenderTexture.active = rt;
            var imagen = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            imagen.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0); imagen.Apply();
            RenderTexture.active = anterior;
            return imagen;
        }
    }
}
