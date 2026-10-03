using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Editor
{
    public static partial class ConstructorConsultorio
    {
        static void AplicarDisenoVisual()
        {
            var raiz=Raiz("Diseño interior");
            var pintura=Acabado("Pintura lino",new Color(.93f,.925f,.89f),.12f);
            var techo=Acabado("Techo mate",new Color(.91f,.925f,.92f),.08f);
            var acento=Acabado("Salvia mineral",new Color(.51f,.64f,.59f),.18f);
            var junta=Acabado("Junta de cielo raso",new Color(.74f,.78f,.77f),.1f);
            var grafito=Acabado("Marcos de señalética",new Color(.14f,.25f,.25f),.28f);
            var papel=Acabado("Panel informativo marfil",new Color(.93f,.945f,.92f),.15f);
            foreach(var r in GameObject.Find("Salas de la clínica").GetComponentsInChildren<Renderer>())
            {
                if(r.name.StartsWith("Muro") || r.name.StartsWith("Tabique") || r.name=="Dintel sala" || r.name=="Lateral conexión") r.sharedMaterial=pintura;
                if(r.name.StartsWith("Techo")) r.sharedMaterial=techo;
            }
            foreach(string n in new[]{"Muro izquierdo","Muro derecho","Fachada/Izquierda","Fachada/Derecha","Fachada/Dintel"}) Revestir("Edificio/"+n,pintura);
            Revestir("Edificio/Techo",techo);
            var espejo=Acabado("Espejo de recepción",new Color(.75f,.82f,.86f),.95f);
            espejo.SetFloat("_Metallic",1);
            // Las luces de relleno no tienen una luminaria visible: no dibujar discos sobre el espejo.
            // El reflejo del interior y de las luminarias procede de la sonda de recepción.
            espejo.SetFloat("_SpecularHighlights",0);
            espejo.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
            EditorUtility.SetDirty(espejo);
            Revestir("Recepción/Lavabo/Espejo",espejo);
            foreach(int lado in new[]{-1,1})
            for(int fila=0;fila<3;fila++)
            {
                float z=4.5f+fila*6.5f;
                // Revestimiento al ras: no tapa placas, mobiliario ni puertas.
                float caraPosterior=fila==2?23.997f:z+6.425f;
                Detalle(raiz,"Paño de acento",new Vector3(lado*4.375f,1.4f,caraPosterior),new Vector3(6.05f,2.7f,.004f),acento);
                for(int i=1;i<=10;i++)
                    Detalle(raiz,"Techo · junta transversal",new Vector3(lado*4.375f,2.795f,z+i*.6f),new Vector3(6.1f,.006f,.012f),junta);
                foreach(float desplazamiento in new[]{-3f,-1.8f,-.6f,.6f,1.8f,3f})
                    Detalle(raiz,"Techo · junta longitudinal",new Vector3(lado*4.375f+desplazamiento,2.795f,z+3.25f),new Vector3(.012f,.006f,6.35f),junta);
            }
            // Remates del pasillo interrumpidos en cada vano de puerta.
            foreach(int lado in new[]{-1,1})
            for(int fila=0;fila<3;fila++)
            {
                float z=4.5f+fila*6.5f;
                foreach(var tramo in new[]{new Vector2(z,z+1.4f),new Vector2(z+3.2f,z+6.5f)})
                    Detalle(raiz,"Zoclo de pasillo",new Vector3(lado*1.164f,.065f,(tramo.x+tramo.y)*.5f),new Vector3(.025f,.13f,tramo.y-tramo.x),grafito);
                Detalle(raiz,"Remate de pasillo",new Vector3(lado*1.17f,2.73f,z+3.25f),new Vector3(.055f,.1f,6.5f),papel);
            }
            var orientacion=Placa(raiz,"Orientación al fondo",new Vector3(0,1.65f,23.98f),0,Color.white,new Vector2(1.9f,.85f));
            orientacion.texto.text="<b>05 RADIOGRAFÍA</b> · izquierda\n<b>DESCANSO</b> · derecha\n<size=80%>Recepción · al regresar</size>";
            // Placas al ras del muro: se leen desde ambos sentidos sin tapar los nombres.
            for(int fila=0;fila<3;fila++)
            foreach(int lado in new[]{-1,1})
            {
                var numero=Placa(raiz,"Número de sala",new Vector3(lado*1.16f,1.65f,5.35f+fila*6.5f),lado*90,Color.white,new Vector2(.42f,.42f));
                numero.texto.text=fila==2 && lado==1?"D":(fila*2+(lado<0?1:2)).ToString("00");
            }
            ComponerRecepcion(raiz);
            AmueblarEsperaDeEntrada(raiz);
            ConfigurarContornos();
            MaterialesConRelieve();
            RefinarSenaletica(grafito,papel);
            IluminacionInterior(raiz);
            PerfilInterior(raiz);
            foreach(var camara in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)) ConfigurarCamaraInterior(camara);
        }

        static void ComponerRecepcion(Transform raiz)
        {
            const float zMostrador=1.5f;
            Vector3 delta=Vector3.forward*(zMostrador+5.2f);
            GameObject.Find("Recepción/Mostrador de recepción").transform.position+=delta;
            foreach(Transform t in GameObject.Find("Modelos de terceros").transform)
            {
                if(t.childCount==0) continue;
                string ruta=AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(t.GetChild(0).gameObject));
                if(ruta.EndsWith("mostrador.obj")) t.position+=delta;
                if(t.name=="computerScreen" && t.position.z<-4)
                {
                    t.position+=delta;
                    t.position=new Vector3(t.position.x,1.1f,t.position.z);
                    t.rotation=Quaternion.Euler(0,180,0);
                }
            }
            foreach(Transform t in GameObject.Find("Acabados arquitectónicos").transform)
                if(t.name=="Listón de recepción")
                {
                    t.position=new Vector3(t.position.x,t.position.y,zMostrador-.335f);
                    t.GetComponent<Renderer>().enabled=true;
                }
            var frente=Acabado("Frente de recepción",new Color(.3f,.39f,.35f),.2f);
            Detalle(raiz,"Panel frontal de recepción",new Vector3(2.4f,.54f,zMostrador-.314f),new Vector3(1.88f,1.02f,.02f),frente);
            var silla=ColocarModelo(raiz,"chairModernFrameCushion",new Vector3(2.45f,0,zMostrador+.95f),.95f,0);
            silla.name="Silla de recepción";
            var col=silla.gameObject.AddComponent<BoxCollider>();
            col.center=Vector3.up*.45f; col.size=new Vector3(.65f,.9f,.65f);
            // El rótulo identifica el mostrador; se conserva la bienvenida con controles en la pared lateral.
            var cartel=Placa(raiz,"Registro de pacientes",new Vector3(2.375f,1.9f,4.39f),0,Color.white,new Vector2(1.95f,.72f));
            foreach(Transform t in GameObject.Find("Recepción ampliada y guías").transform)
                if(t.name=="pottedPlant" && t.position.x>0) t.position=new Vector3(3,0,3.8f);
            cartel.texto.text="<b>RECEPCIÓN</b>\n<size=70%>Registro de pacientes</size>";
        }

        static void RefinarSenaletica(Material marco,Material fondo)
        {
            foreach(var placa in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.name=="Hoja de práctica" || t.name=="Manual de sala" || t.name=="Bienvenida recepción" || t.name=="Registro de pacientes" || t.name=="Directorio de salas" || t.name=="Orientación al fondo" || t.name=="Número de sala").ToArray())
            {
                var cuerpo=placa.Find("Placa");
                cuerpo.GetComponent<Renderer>().sharedMaterial=fondo;
                var medida=cuerpo.localScale;
                foreach(var texto in placa.GetComponentsInChildren<TMP_Text>())
                {
                    texto.color=new Color(.025f,.06f,.055f);
                    if(placa.name=="Hoja de práctica" || placa.name=="Directorio de salas" || placa.name=="Número de sala") texto.fontStyle=FontStyles.Bold;
                    texto.lineSpacing=4;
                    texto.margin=new Vector4(.035f,.025f,.035f,.025f);
                }
                foreach(float signo in new[]{-1f,1f})
                {
                    Detalle(placa,"Marco vertical",new Vector3(signo*(medida.x*.5f+.012f),0,0),new Vector3(.024f,medida.y+.048f,.035f),marco);
                    Detalle(placa,"Marco horizontal",new Vector3(0,signo*(medida.y*.5f+.012f),0),new Vector3(medida.x,.024f,.035f),marco);
                }
            }
        }

        static void IluminacionInterior(Transform raiz)
        {
            foreach(var luz in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if(luz.name=="Luz de techo")
                {
                    luz.type=LightType.Spot;
                    luz.transform.position=new Vector3(luz.transform.position.x,2.6f,luz.transform.position.z);
                    luz.transform.rotation=Quaternion.Euler(90,0,0);
                    luz.spotAngle=140; luz.innerSpotAngle=100;
                    luz.intensity=2.7f; luz.range=7;
                    luz.color=new Color(1,.96f,.89f);
                }
                if(luz.type!=LightType.Directional && luz.shadows!=LightShadows.None)
                {
                    var datos=luz.GetUniversalAdditionalLightData();
                    datos.usePipelineSettings=false;
                    var sombra=new SerializedObject(datos);
                    sombra.FindProperty("m_AdditionalLightsShadowResolutionTier").intValue=UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierMedium;
                    sombra.ApplyModifiedPropertiesWithoutUndo();
                    luz.shadowBias=.02f; luz.shadowNormalBias=.12f;
                }
                if(luz.name=="Luz ambiente de sala") luz.intensity=.7f;
            }
            foreach(float z in new[]{-4f,1f})
            {
                var luz=new GameObject("Relleno recepción").AddComponent<Light>();
                luz.transform.SetParent(raiz,false);
                luz.transform.position=new Vector3(0,1.5f,z);
                luz.type=LightType.Point; luz.range=4.5f; luz.intensity=.55f;
                luz.color=new Color(1,.96f,.9f); luz.shadows=LightShadows.None;
            }
            foreach(float z in new[]{7.75f,14.25f,20.75f})
            {
                var luz=new GameObject("Relleno de pasillo").AddComponent<Light>();
                luz.transform.SetParent(raiz,false);
                luz.transform.position=new Vector3(0,1.8f,z);
                luz.type=LightType.Point; luz.range=3.4f; luz.intensity=.65f;
                luz.color=new Color(1,.98f,.94f); luz.shadows=LightShadows.None;
            }
            // Las sondas siguen la recepción abierta y cubren también el pasillo.
            Object.DestroyImmediate(GameObject.Find("Edificio/Sonda consultorio"));
            var recepcion=Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None).Single(s=>s.name=="Sonda recepción");
            recepcion.transform.position=new Vector3(0,1.4f,-1.25f);
            recepcion.size=new Vector3(7,2.8f,11.5f);
            SondaDeReflejos(raiz,"Sonda pasillo",new Vector3(0,1.4f,14.25f),new Vector3(2.5f,2.8f,19.5f));
            RenderSettings.ambientSkyColor=new Color(.69f,.73f,.74f);
            RenderSettings.ambientEquatorColor=new Color(.55f,.59f,.58f);
            RenderSettings.ambientGroundColor=new Color(.32f,.35f,.34f);
            foreach(var sonda in Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None))
            {
                sonda.boxProjection=true;
                sonda.intensity=.65f;
                sonda.resolution=128;
                sonda.blendDistance=.25f;
                // El piso y los muros quedan dentro del peso completo, sin mezcla con el cielo.
                sonda.size += Vector3.one * .6f;
                sonda.clearFlags=ReflectionProbeClearFlags.SolidColor;
                sonda.backgroundColor=new Color(.55f,.58f,.55f);
            }
            var rp=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/PC_RPAsset.asset");
            var ajustes=new SerializedObject(rp);
            ajustes.FindProperty("m_AdditionalLightsShadowmapResolution").intValue=4096;
            ajustes.FindProperty("m_SoftShadowQuality").intValue=2;
            ajustes.ApplyModifiedPropertiesWithoutUndo();
            var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Settings/PC_Renderer.asset");
            foreach(var efecto in renderer.rendererFeatures.Where(f=>f!=null && f.name=="ScreenSpaceAmbientOcclusion"))
            {
                var ao=new SerializedObject(efecto);
                ao.FindProperty("m_Settings.Intensity").floatValue=.25f;
                ao.FindProperty("m_Settings.Radius").floatValue=.16f;
                ao.FindProperty("m_Settings.DirectLightingStrength").floatValue=.15f;
                ao.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        static void PerfilInterior(Transform raiz)
        {
            const string ruta="Assets/Settings/InteriorClinica.asset";
            var perfil=AssetDatabase.LoadAssetAtPath<VolumeProfile>(ruta);
            if(perfil==null) { perfil=ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(perfil,ruta); }
            // El operador tonal es global: no cambia de golpe al atravesar una puerta.
            if(perfil.TryGet<Tonemapping>(out var anterior)) { perfil.Remove<Tonemapping>(); Object.DestroyImmediate(anterior,true); }
            const string rutaGlobal="Assets/Settings/TonoClinica.asset";
            var global=AssetDatabase.LoadAssetAtPath<VolumeProfile>(rutaGlobal);
            if(global==null) { global=ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(global,rutaGlobal); }
            if(!global.TryGet<Tonemapping>(out var tonos)) { tonos=global.Add<Tonemapping>(); AssetDatabase.AddObjectToAsset(tonos,global); }
            tonos.mode.Override(TonemappingMode.Neutral);
            var tono=new GameObject("Tono uniforme").AddComponent<Volume>();
            tono.transform.SetParent(raiz,false); tono.isGlobal=true; tono.sharedProfile=global;
            EditorUtility.SetDirty(global);
            if(!perfil.TryGet<ColorAdjustments>(out var color)) { color=perfil.Add<ColorAdjustments>(); AssetDatabase.AddObjectToAsset(color,perfil); }
            color.postExposure.Override(.2f); color.contrast.Override(5); color.saturation.Override(-3);
            EditorUtility.SetDirty(perfil); EditorUtility.SetDirty(tonos); EditorUtility.SetDirty(color);
            foreach(var espacio in new[]{new Bounds(new Vector3(0,1.4f,-1.25f),new Vector3(7,2.8f,11.5f)),new Bounds(new Vector3(0,1.4f,14.25f),new Vector3(15,2.8f,19.5f))})
            {
                var go=new GameObject("Color interior"); go.transform.SetParent(raiz,false); go.transform.position=espacio.center;
                var volumen=go.AddComponent<Volume>(); volumen.isGlobal=false; volumen.priority=5; volumen.blendDistance=1.2f; volumen.sharedProfile=perfil;
                var caja=go.AddComponent<BoxCollider>(); caja.isTrigger=true; caja.size=espacio.size;
            }
        }

        static void ConfigurarCamaraInterior(Camera camara)
        {
            camara.allowHDR=true;
            var datos=camara.GetUniversalAdditionalCameraData();
            datos.renderPostProcessing=true;
            datos.antialiasing=AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            datos.antialiasingQuality=AntialiasingQuality.High;
        }

        static void MaterialesConRelieve()
        {
            var color=TexturaSuperficie("Piedra difusa",0);
            var normal=TexturaSuperficie("Piedra normal",1);
            var brillo=TexturaSuperficie("Piedra pulido",2);
            foreach(string nombre in new[]{"Porcelanato marfil","Porcelanato ampliación"})
            {
                var mat=AssetDatabase.LoadAssetAtPath<Material>(CarpetaAcabados+"/"+nombre+".mat");
                mat.SetColor("_BaseColor",new Color(.88f,.87f,.82f));
                mat.SetTexture("_BaseMap",color); mat.SetTexture("_BumpMap",normal); mat.SetFloat("_BumpScale",.2f);
                mat.SetTexture("_MetallicGlossMap",brillo); mat.SetFloat("_Smoothness",.75f);
                mat.EnableKeyword("_NORMALMAP"); mat.EnableKeyword("_METALLICSPECGLOSSMAP");
                EditorUtility.SetDirty(mat);
            }
            var conexion=Acabado("Porcelanato conexión",Color.white,.5f);
            conexion.CopyPropertiesFromMaterial(AssetDatabase.LoadAssetAtPath<Material>(CarpetaAcabados+"/Porcelanato ampliación.mat"));
            conexion.SetTextureScale("_BaseMap",new Vector2(7,1.5f));
            EditorUtility.SetDirty(conexion);
            Revestir("Salas de la clínica/Piso conexión",conexion);
            var veta=TexturaSuperficie("Roble veta",3);
            foreach(string nombre in new[]{"Roble recepción","Roble miel"})
            {
                var mat=AssetDatabase.LoadAssetAtPath<Material>(CarpetaAcabados+"/"+nombre+".mat");
                mat.SetTexture("_BaseMap",veta); mat.SetColor("_BaseColor",new Color(.76f,.57f,.36f)); mat.SetFloat("_Smoothness",.28f);
                EditorUtility.SetDirty(mat);
            }
        }

        // Texturas procedurales reproducibles: piedra satinada y veta continua de roble.
        static Texture2D TexturaSuperficie(string nombre,int tipo)
        {
            string ruta=CarpetaAcabados+"/"+nombre+".png";
            const int n=512;
            var textura=new Texture2D(n,n,TextureFormat.RGBA32,false);
            var pixeles=new Color[n*n];
            for(int y=0;y<n;y++) for(int x=0;x<n;x++)
            {
                float u=(float)x/n,v=(float)y/n;
                float ruido=Mathf.Lerp(Mathf.Lerp(Mathf.PerlinNoise(u*7,v*7),Mathf.PerlinNoise((u-1)*7,v*7),u),Mathf.Lerp(Mathf.PerlinNoise(u*7,(v-1)*7),Mathf.PerlinNoise((u-1)*7,(v-1)*7),u),v);
                float onda=(ruido-.5f)*.025f;
                float junta=x<2 || y<2 ? .84f:1;
                if(tipo==0) pixeles[y*n+x]=new Color((.965f+onda)*junta,(.97f+onda)*junta,(.975f+onda)*junta);
                else if(tipo==1)
                {
                    float nx=x<4?(x<2?-.35f:.35f):onda*.7f;
                    float ny=y<4?(y<2?-.35f:.35f):onda*.7f;
                    pixeles[y*n+x]=new Color(.5f+nx,.5f+ny,1);
                }
                else if(tipo==2) pixeles[y*n+x]=new Color(0,0,0,junta<1?.16f:.66f+onda*4);
                else
                {
                    float veta=.85f+.09f*Mathf.Sin(u*Mathf.PI*6+Mathf.Sin(v*Mathf.PI*2)*.65f)+.035f*Mathf.Sin(u*Mathf.PI*18+Mathf.Cos(v*Mathf.PI*4)*.9f);
                    pixeles[y*n+x]=new Color(veta,veta*.97f,veta*.92f);
                }
            }
            textura.SetPixels(pixeles); textura.Apply();
            byte[] png=textura.EncodeToPNG(); Object.DestroyImmediate(textura);
            // Conserva el GUID y solo reimporta si cambió la generación.
            if(!File.Exists(ruta) || !File.ReadAllBytes(ruta).SequenceEqual(png))
            {
                File.WriteAllBytes(ruta,png);
                AssetDatabase.ImportAsset(ruta);
            }
            var importador=(TextureImporter)AssetImporter.GetAtPath(ruta);
            importador.wrapMode=TextureWrapMode.Repeat; importador.anisoLevel=8;
            if(tipo==1) importador.textureType=TextureImporterType.NormalMap;
            if(tipo==2) importador.sRGBTexture=false;
            importador.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(ruta);
        }
    }
}
