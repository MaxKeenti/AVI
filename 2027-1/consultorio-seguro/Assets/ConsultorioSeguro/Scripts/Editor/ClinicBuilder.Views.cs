using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object=UnityEngine.Object;

namespace ConsultorioSeguroNuevo.Editor
{
    public static partial class ClinicBuilder
    {
        static readonly (string name,Vector3 position,Vector3 target)[] Views={
            ("01-exterior",new Vector3(-12.5f,1.65f,-21),new Vector3(0,13,-2)),
            ("02-acceso",new Vector3(0,1.65f,-10.5f),new Vector3(1,1.6f,-1.5f)),
            ("03-recepcion",new Vector3(-.3f,1.65f,-5.9f),new Vector3(3.1f,1.4f,.25f)),
            ("04-espera",new Vector3(-.1f,1.65f,-5.35f),new Vector3(-5.1f,1.05f,-1.65f)),
            ("05-pasillo",new Vector3(0,1.65f,2.1f),new Vector3(0,1.6f,20)),
            ("06-clasificacion",new Vector3(-3.15f,1.65f,3.9f),new Vector3(-4.9f,1.25f,6.5f)),
            ("07-esterilizacion",new Vector3(3.1f,1.65f,3.8f),new Vector3(5.15f,1.18f,7.35f)),
            ("08-materiales",new Vector3(-3.15f,1.65f,9.85f),new Vector3(-4.85f,1.2f,12.6f)),
            ("09-procedimientos",new Vector3(3.15f,1.65f,9.85f),new Vector3(4.8f,1.18f,12.85f)),
            ("10-radiografia",new Vector3(-3.45f,1.65f,15.9f),new Vector3(-4.6f,1.15f,18.5f)),
            ("11-personal",new Vector3(3.1f,1.65f,15.85f),new Vector3(4.95f,1.18f,18.9f)),
            ("12-corte-superior",new Vector3(-18,28,-16),new Vector3(0,0,7))
        };
        static void MakeViews()
        {
            var root=Group("07 · Vistas de inspección");
            foreach(var v in Views)
            {var t=Group(v.name,root,v.position);t.LookAt(v.target);var c=t.gameObject.AddComponent<Camera>();c.enabled=false;c.fieldOfView=62;c.nearClipPlane=.05f;c.farClipPlane=200;}
        }
        [MenuItem("Consultorio Seguro/Vistas/Exterior")]static void Exterior()=>EditorView(0);
        [MenuItem("Consultorio Seguro/Vistas/Recepción")]static void Reception()=>EditorView(2);
        [MenuItem("Consultorio Seguro/Vistas/Sala de espera")]static void Waiting()=>EditorView(3);
        [MenuItem("Consultorio Seguro/Vistas/Pasillo")]static void Corridor()=>EditorView(4);
        [MenuItem("Consultorio Seguro/Vistas/01 Clasificación")]static void R1()=>EditorView(5);
        [MenuItem("Consultorio Seguro/Vistas/02 Esterilización")]static void R2()=>EditorView(6);
        [MenuItem("Consultorio Seguro/Vistas/03 Materiales")]static void R3()=>EditorView(7);
        [MenuItem("Consultorio Seguro/Vistas/04 Procedimientos")]static void R4()=>EditorView(8);
        [MenuItem("Consultorio Seguro/Vistas/05 Radiografía")]static void R5()=>EditorView(9);
        [MenuItem("Consultorio Seguro/Vistas/Personal")]static void Staff()=>EditorView(10);
        [MenuItem("Consultorio Seguro/Vistas/Corte superior")]static void Cutaway()=>EditorView(11);
        static void EditorView(int index)
        {
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path!=ScenePath)EditorSceneManager.OpenScene(ScenePath);
            var roofObject=GameObject.Find("02 · Techos y torre");var cityObject=GameObject.Find("04 · Entorno urbano");
            if(index==11){SceneVisibilityManager.instance.Hide(roofObject,true);SceneVisibilityManager.instance.Hide(cityObject,true);}else{SceneVisibilityManager.instance.Show(roofObject,true);SceneVisibilityManager.instance.Show(cityObject,true);}
            var v=Views[index];var view=SceneView.lastActiveSceneView??EditorWindow.GetWindow<SceneView>();view.sceneLighting=true;
            view.LookAt(v.target,Quaternion.LookRotation(v.target-v.position),Vector3.Distance(v.position,v.target)*Mathf.Sin(view.cameraSettings.fieldOfView*.5f*Mathf.Deg2Rad),false,true);view.Repaint();
        }
        [MenuItem("Consultorio Seguro/Guardar capturas de Unity")]
        public static void Capture()
        {
            EditorSceneManager.OpenScene(ScenePath);
            Directory.CreateDirectory("Capturas");
            var obj=new GameObject("Cámara temporal de documentación");var camera=obj.AddComponent<Camera>();camera.nearClipPlane=.05f;camera.farClipPlane=200;camera.fieldOfView=65;camera.allowHDR=true;
            var data=camera.GetUniversalAdditionalCameraData();data.renderPostProcessing=true;data.antialiasing=AntialiasingMode.SubpixelMorphologicalAntiAliasing;data.antialiasingQuality=AntialiasingQuality.High;
            bool previous=ShaderUtil.allowAsyncCompilation;ShaderUtil.allowAsyncCompilation=false;
            try
            {
                // Sondas renderizadas y guardadas: los reflejos también se ven en el editor sin Play.
                int n=0;
                foreach(var probe in Object.FindObjectsByType<ReflectionProbe>())
                {
                    string path=Generated+"/Reflejo-"+(n++)+".exr";
                    bool baked=Lightmapping.BakeReflectionProbe(probe,path);
                    if(baked){AssetDatabase.ImportAsset(path);probe.mode=ReflectionProbeMode.Custom;probe.customBakedTexture=AssetDatabase.LoadAssetAtPath<Cubemap>(path);}
                    else Debug.LogWarning("No se pudo guardar la sonda "+probe.transform.position);
                }
                var main=Object.FindAnyObjectByType<ClinicPlayer>().eye;main.enabled=false;
                var ceiling=GameObject.Find("02 · Techos y torre");var cityObject=GameObject.Find("04 · Entorno urbano");
                foreach(var v in Views)
                {
                    bool cut=v.name.Contains("corte");ceiling.SetActive(!cut);cityObject.SetActive(!cut);
                    camera.transform.SetPositionAndRotation(v.position,Quaternion.LookRotation(v.target-v.position));camera.orthographic=cut;camera.orthographicSize=17;
                    SaveFrame(camera,"Capturas/"+v.name+".png",1800,1125);
                }
                ceiling.SetActive(true);cityObject.SetActive(true);main.enabled=true;
            }
            finally{ShaderUtil.allowAsyncCompilation=previous;Object.DestroyImmediate(obj);}
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);
            AssetDatabase.SaveAssets();
            File.WriteAllText("Capturas/ORIGEN.txt","Capturas renderizadas por Unity 6000.4.6f1 desde la escena guardada.\nFecha UTC: "+DateTime.UtcNow.ToString("O")+"\nResolución: 1800 × 1125. Altura de cámara interior: 1.65 m.\nEl exterior utiliza altura de ojos; el corte superior utiliza una cámara elevada. Posiciones en ClinicBuilder.Views.cs.\nEstas imágenes no sustituyen las pruebas de interacción.\n");
            Debug.Log("CAPTURAS_LISTAS");
        }
        public static void BuildAndCapture(){Build();Capture();}
        static void SaveFrame(Camera c,string path,int width,int height)
        {
            var rt=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB){antiAliasing=4};rt.Create();c.targetTexture=rt;
            c.Render();c.Render();
            var previous=RenderTexture.active;RenderTexture.active=rt;var t=new Texture2D(width,height,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,width,height),0,0);t.Apply();File.WriteAllBytes(path,t.EncodeToPNG());RenderTexture.active=previous;c.targetTexture=null;rt.Release();Object.DestroyImmediate(rt);Object.DestroyImmediate(t);
        }
    }
}
