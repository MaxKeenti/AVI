using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Tests
{
    public class RevisionVisualTests
    {
        [UnityTest]
        public IEnumerator ElInteriorMantieneTonoReflejosYTextosLegibles()
        {
            yield return SceneManager.LoadSceneAsync("Consultorio");
            yield return null;
            GestorSimulacion.Instancia.Entrar();
            var jugador=Object.FindFirstObjectByType<ControladorPrimeraPersona>();
            jugador.Habilitado=false;
            var camara=jugador.GetComponentInChildren<Camera>();
            var interfaz=GameObject.Find("Interfaz");
            interfaz.SetActive(false);
            var destino=new GameObject("Punto de revisión").transform;
            string carpeta=Environment.GetEnvironmentVariable("AVI_CAPTURAS_REVISION");
            var tiempos=new List<string>();
            var objetivo=new RenderTexture(1600,1000,24);
            objetivo.Create();
            camara.targetTexture=objetivo;
            if(!string.IsNullOrEmpty(carpeta)) Directory.CreateDirectory(carpeta);
            try
            {
                Assert.IsTrue(QualitySettings.realtimeReflectionProbes,"El perfil de calidad debe permitir los reflejos interiores.");
                // Se renderizan las sondas reales antes de evaluar las superficies.
                foreach(var sonda in Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None))
                {
                    int solicitud=sonda.RenderProbe();
                    float limite=Time.realtimeSinceStartup+15;
                    while(!sonda.IsFinishedRendering(solicitud) && Time.realtimeSinceStartup<limite) { camara.Render(); yield return null; }
                    Assert.IsTrue(sonda.IsFinishedRendering(solicitud),sonda.name);
                    Assert.NotNull(sonda.realtimeTexture,sonda.name);
                }
                foreach(float z in new[]{-8.3f,-7.8f,-7.3f,-6.8f,-6.3f,3.5f,5f,14f})
                {
                    destino.position=new Vector3(0,.05f,z);
                    jugador.Teletransportar(destino);
                    yield return null;
                    camara.Render();
                    Assert.AreEqual(TonemappingMode.Neutral,VolumeManager.instance.stack.GetComponent<Tonemapping>().mode.value,"Salto tonal en z="+z);
                    if(z>=-6.8f)
                        Assert.IsTrue(Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None).Any(s=>s.bounds.Contains(camara.transform.position)),"Sin reflejos interiores en z="+z);
                }
                // Cada número queda coplanar a la pared, debajo y junto al vano.
                // Un cartel saliente invadiría este límite aunque una toma oblicua parezca correcta.
                var numeros=Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.name=="Número de sala").ToArray();
                Assert.AreEqual(6,numeros.Length);
                foreach(var numero in numeros)
                {
                    var limites=numero.Find("Placa").GetComponent<Renderer>().bounds;
                    Assert.Greater(Mathf.Abs(limites.center.x)-limites.extents.x,1.1f,"El número no debe sobresalir en el pasillo.");
                    Assert.Less(limites.max.y,1.9f,"El número debe quedar por debajo del nombre.");
                    Assert.Greater(Vector3.Dot(-numero.forward,Vector3.left*Mathf.Sign(numero.position.x)),.99f,"Cara legible hacia el pasillo.");
                }
                var sondaRecepcion=Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None).Single(s=>s.name=="Sonda recepción");
                var volumenCompleto=sondaRecepcion.bounds;
                volumenCompleto.Expand(-2*sondaRecepcion.blendDistance);
                Assert.IsTrue(volumenCompleto.Contains(new Vector3(-3.47f,1.45f,-3.7f)),"El espejo no debe mezclar el cielo en el borde de la sonda.");
                Assert.IsTrue(volumenCompleto.Contains(new Vector3(0,0,-3)),"El piso debe recibir reflejos interiores completos.");
                var vistas=new[]
                {
                    ("recepcion",new Vector3(0,1.65f,-6.8f),new Vector3(0,1.4f,1.5f)),
                    ("registro",new Vector3(.2f,1.65f,-1),new Vector3(2.4f,1.2f,1.8f)),
                    ("puesto-recepcionista",new Vector3(1.05f,1.65f,3.55f),new Vector3(2.6f,.95f,1.5f)),
                    ("bienvenida",new Vector3(.7f,1.65f,-3.5f),new Vector3(3.46f,1.85f,-4.2f)),
                    ("lavabo",new Vector3(-1.7f,1.65f,-3.7f),new Vector3(-3.22f,.85f,-3.7f)),
                    ("espera-entrada",new Vector3(0,1.65f,-.3f),new Vector3(2.1f,1,-4.4f)),
                    ("rotulos-salas",new Vector3(-.6f,1.65f,4.5f),new Vector3(1.25f,2.18f,6.45f)),
                    ("pasillo",new Vector3(0,1.65f,3.2f),new Vector3(0,1.6f,20)),
                    ("pasillo-regreso",new Vector3(0,1.65f,23.4f),new Vector3(0,1.6f,4)),
                    ("clasificacion",new Vector3(-2.6f,1.65f,6.8f),new Vector3(-4.8f,1.45f,7.55f)),
                    ("esterilizacion",new Vector3(3,1.65f,6),new Vector3(7,1,7.2f)),
                    ("materiales",new Vector3(-4.4f,1.65f,15.1f),new Vector3(-4.6f,1.75f,17.39f)),
                    ("consultorio",new Vector3(3.1f,1.7f,11.9f),new Vector3(4.2f,1.1f,15.3f)),
                    ("radiografia",new Vector3(-2.8f,1.65f,19.2f),new Vector3(-6.2f,1.2f,22.7f)),
                };
                foreach(var (nombre,posicion,mirada) in vistas)
                {
                    camara.transform.SetPositionAndRotation(posicion,Quaternion.LookRotation(mirada-posicion));
                    for(int i=0;i<8;i++) yield return null;
                    camara.Render();
                    foreach(var texto in Object.FindObjectsByType<TMPro.TextMeshPro>(FindObjectsSortMode.None).Where(t=>t.transform.parent.name=="Hoja de práctica"))
                    {
                        texto.ForceMeshUpdate();
                        Assert.IsFalse(texto.isTextOverflowing,texto.transform.parent.parent.name);
                    }
                    if(string.IsNullOrEmpty(carpeta)) continue;
                    var anterior=RenderTexture.active;
                    RenderTexture.active=objetivo;
                    var imagen=new Texture2D(objetivo.width,objetivo.height,TextureFormat.RGB24,false);
                    imagen.ReadPixels(new Rect(0,0,objetivo.width,objetivo.height),0,0); imagen.Apply();
                    File.WriteAllBytes(Path.Combine(carpeta,nombre+".png"),imagen.EncodeToPNG());
                    Object.Destroy(imagen); RenderTexture.active=anterior;
                    // Tiempo real del Editor, orientativo: no es una medición de la compilación final.
                    var muestras=new List<float>();
                    for(int i=0;i<60;i++) { yield return null; muestras.Add(Time.unscaledDeltaTime*1000); }
                    muestras.Sort();
                    tiempos.Add($"{nombre}: mediana {muestras[30]:F1} ms; p95 {muestras[56]:F1} ms (Editor, cámara 1600×1000)");
                }
                if(!string.IsNullOrEmpty(carpeta)) File.WriteAllLines(Path.Combine(carpeta,"tiempos.txt"),tiempos);
            }
            finally
            {
                camara.targetTexture=null; objetivo.Release(); Object.Destroy(objetivo);
                Object.Destroy(destino.gameObject); interfaz.SetActive(true);
            }
        }
    }
}
