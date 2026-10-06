using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using Object = UnityEngine.Object;

namespace ConsultorioSeguroNuevo.Editor
{
    public static partial class ClinicBuilder
    {
        static void SetupPipeline()
        {
            var renderer=ScriptableObject.CreateInstance<UniversalRendererData>();
            string rendererPath=Generated+"/ClinicaRenderer.asset";
            var old=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);
            if(old){Object.DestroyImmediate(renderer);renderer=old;}
            else AssetDatabase.CreateAsset(renderer,rendererPath);
            // Los paños grandes reciben todas las luces cercanas, sin el salto de ocho luces por objeto.
            renderer.renderingMode=RenderingMode.ForwardPlus;
            var rp=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(Generated+"/ClinicaURP.asset");
            if(!rp){rp=UniversalRenderPipelineAsset.Create(renderer);AssetDatabase.CreateAsset(rp,Generated+"/ClinicaURP.asset");}
            rp.supportsHDR=true;rp.msaaSampleCount=4;rp.renderScale=1;rp.shadowDistance=48;rp.shadowCascadeCount=4;
            var so=new SerializedObject(rp);
            Set(so,"m_RequireDepthTexture",true);Set(so,"m_RequireOpaqueTexture",true);
            Set(so,"m_MainLightShadowmapResolution",4096);Set(so,"m_AdditionalLightsRenderingMode",1);
            Set(so,"m_AdditionalLightsPerObjectLimit",8);
            Set(so,"m_AdditionalLightsShadowResolutionTierLow",256);Set(so,"m_AdditionalLightsShadowResolutionTierMedium",512);Set(so,"m_AdditionalLightsShadowResolutionTierHigh",1024);
            Set(so,"m_ReflectionProbeBlending",true);Set(so,"m_ReflectionProbeBoxProjection",true);Set(so,"m_SoftShadowQuality",3);Set(so,"m_AdditionalLightShadowsSupported",true);Set(so,"m_AdditionalLightsShadowmapResolution",4096);Set(so,"m_SoftShadowsSupported",true);
            so.ApplyModifiedPropertiesWithoutUndo();
            var aoType=typeof(UniversalRenderPipeline).Assembly.GetType("UnityEngine.Rendering.Universal.ScreenSpaceAmbientOcclusion");
            if(aoType!=null)
            {
                var ao=renderer.rendererFeatures.FirstOrDefault(feature=>feature&&feature.GetType()==aoType);
                if(!ao){ao=(ScriptableRendererFeature)ScriptableObject.CreateInstance(aoType);ao.name="Oclusión de contacto";AssetDatabase.AddObjectToAsset(ao,renderer);renderer.rendererFeatures.Add(ao);}
                var aos=new SerializedObject(ao);
                Set(aos,"m_Settings.Intensity",.48f);Set(aos,"m_Settings.Radius",.085f);
                Set(aos,"m_Settings.DirectLightingStrength",.12f);Set(aos,"m_Settings.Falloff",32f);
                Set(aos,"m_Settings.Downsample",false);Set(aos,"m_Settings.Samples",0);Set(aos,"m_Settings.BlurQuality",0);
                aos.ApplyModifiedPropertiesWithoutUndo();ao.SetActive(true);ao.Create();EditorUtility.SetDirty(ao);
            }
            GraphicsSettings.defaultRenderPipeline=rp;QualitySettings.renderPipeline=rp;
            QualitySettings.realtimeReflectionProbes=true;QualitySettings.shadows=UnityEngine.ShadowQuality.All;QualitySettings.shadowResolution=UnityEngine.ShadowResolution.VeryHigh;QualitySettings.antiAliasing=4;
            EditorUtility.SetDirty(renderer);EditorUtility.SetDirty(rp);
        }
        static void Set(SerializedObject o,string n,int value){var p=o.FindProperty(n);if(p!=null)p.intValue=value;}
        static void Set(SerializedObject o,string n,float value){var p=o.FindProperty(n);if(p!=null)p.floatValue=value;}
        static void Set(SerializedObject o,string n,bool value){var p=o.FindProperty(n);if(p!=null)p.boolValue=value;}
        static void SetupMaterials()
        {
            plaster=Material("Yeso cálido",new Color(.97f,.97f,.95f),.18f);Pbr(plaster,"polyhaven-beige-wall-001","beige_wall_001",.14f);plaster.SetTexture("_BaseMap",null);plaster.SetTexture("_OcclusionMap",null);plaster.DisableKeyword("_OCCLUSIONMAP");plaster.SetTexture("_MetallicGlossMap",null);plaster.DisableKeyword("_METALLICSPECGLOSSMAP");plaster.SetFloat("_Metallic",0);plaster.SetFloat("_Smoothness",.12f);plaster.SetFloat("_BumpScale",.055f);
            sage=Material("Salvia mate",new Color(.52f,.62f,.55f),.2f);
            wood=Material("Roble natural",Color.white,.3f);Pbr(wood,"polyhaven-oak-veneer-01","oak_veneer_01",.10f);wood.SetColor("_BaseColor",new Color(1,.98f,.91f));wood.SetFloat("_Smoothness",.7f);wood.SetFloat("_OcclusionStrength",.25f);
            white=Material("Cerámica y laca marfil",new Color(.91f,.925f,.9f),.53f);
            metal=Material("Acero cepillado",new Color(.67f,.70f,.7f),.69f,.9f);
            dark=Material("Aluminio oscuro",new Color(.1f,.145f,.14f),.45f,.55f);
            linen=Material("Tapizado lino",new Color(.66f,.64f,.58f),.14f);
            stone=Material("Piedra travertino",new Color(.69f,.67f,.60f),.25f);
            tile=Material("Porcelanato satinado",new Color(.90f,.89f,.865f),.33f);
            string path=Generated+"/Porcelanato.png";
            if(!File.Exists(path))
            {
                var t=new Texture2D(512,512);var pixels=new Color[512*512];
                for(int y=0;y<512;y++)for(int x=0;x<512;x++){float noise=Mathf.PerlinNoise(x*.035f,y*.035f)*.016f;float v=x<2||y<2?.82f:.96f+noise;pixels[y*512+x]=new Color(v,v,v);}
                t.SetPixels(pixels);t.Apply();File.WriteAllBytes(path,t.EncodeToPNG());Object.DestroyImmediate(t);AssetDatabase.ImportAsset(path);
            }
            tile.SetTexture("_BaseMap",Texture(path));
            glass=Material("Vidrio arquitectónico",new Color(.75f,.88f,.89f,.10f),.9f,.05f);
            glass.SetFloat("_SpecularHighlights",0);glass.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
            glass.SetFloat("_Surface",1);glass.SetFloat("_ZWrite",0);glass.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);glass.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);glass.SetOverrideTag("RenderType","Transparent");glass.renderQueue=3000;glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            emissive=Material("Difusor de luminaria",new Color(1,.97f,.90f),.3f);emissive.EnableKeyword("_EMISSION");emissive.SetColor("_EmissionColor",new Color(1,.965f,.90f)*1.05f);emissive.globalIlluminationFlags=MaterialGlobalIlluminationFlags.BakedEmissive;CoreUtils.SetKeyword(emissive,"_EMISSION",true);EditorUtility.SetDirty(emissive);
        }
        static void Floor(Transform parent,string name,Vector3 pos,Vector2 size)
        {
            var mat=Material("Piso "+name,Color.white,.4f);mat.CopyPropertiesFromMaterial(tile);mat.SetTextureScale("_BaseMap",size/.9f);
            Box(name,parent,pos,new Vector3(size.x,.18f,size.y),mat);
        }
        static void Wall(Transform parent,string name,Vector3 pos,Vector3 size,Material mat=null)
        {Box(name,parent,pos,size,mat??plaster);}
        static void MakeArchitecture()
        {
            var lobby=Group("Recepción · 150 m²",architecture);
            Floor(lobby,"Porcelanato recepción",new Vector3(0,-.09f,-2),new Vector2(15,10));
            Floor(architecture,"Porcelanato pasillo y salas",new Vector3(0,-.09f,12),new Vector2(15,18));
            var ceiling=Material("Pintura mate de techo",new Color(.93f,.935f,.915f),.015f);
            Box("Techo recepción",roof,new Vector3(0,3.13f,-2),new Vector3(15,.22f,10),ceiling);
            Box("Techo salas",roof,new Vector3(0,3.13f,12),new Vector3(15,.22f,18),ceiling);
            Wall(architecture,"Muro posterior",new Vector3(0,1.5f,21),new Vector3(15,3,.18f));
            foreach(int side in new[]{-1,1})
            {
                float x=side*7.5f;
                Wall(lobby,"Muro lateral de recepción",new Vector3(x,1.5f,-2),new Vector3(.18f,3,10));
                foreach(float z in new[]{3f,9f,15f})
                {
                    Wall(architecture,"Tabique entre salas",new Vector3(side*4.42f,1.5f,z),new Vector3(6.16f,3,.14f));
                    float doorway=z+1.85f;
                    Wall(architecture,"Pasillo · paño corto",new Vector3(side*1.35f,1.5f,z+.50f),new Vector3(.16f,3,1));
                    Wall(architecture,"Pasillo · paño largo",new Vector3(side*1.35f,1.5f,z+4.30f),new Vector3(.16f,3,3.40f));
                    Wall(architecture,"Dintel",new Vector3(side*1.35f,2.72f,doorway),new Vector3(.16f,.56f,1.70f));
                    foreach(float dz in new[]{-.85f,.85f})Box("Jamba de roble",architecture,new Vector3(side*1.35f,1.20f,doorway+dz),new Vector3(.23f,2.4f,.055f),wood,false);
                    Box("Marco superior",architecture,new Vector3(side*1.35f,2.40f,doorway),new Vector3(.23f,.055f,1.75f),wood,false);
                    // Las puertas quedan aparcadas junto al tabique, fuera del paso útil de 1.70 m.
                    var leaf=Box("Puerta abierta",architecture,new Vector3(side*2.13f,1.16f,doorway+.79f),new Vector3(1.42f,2.30f,.065f),white,true,.013f);
                    Box("Mirilla de puerta",leaf.transform,new Vector3(0,.50f,-.043f),new Vector3(.30f,.52f,.013f),glass,false).GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
                    Rod("Manija de puerta",architecture,new Vector3(side*2.55f,1.05f,doorway+.69f),new Vector3(side*2.30f,1.05f,doorway+.69f),.018f,metal);
                    float center=z+3;
                    Wall(architecture,"Antepecho ventana",new Vector3(x,.48f,center),new Vector3(.18f,.96f,6));
                    Wall(architecture,"Dintel ventana",new Vector3(x,2.84f,center),new Vector3(.18f,.32f,6));
                    foreach(float dz in new[]{-2.57f,2.57f})Wall(architecture,"Machón ventana",new Vector3(x,1.82f,center+dz),new Vector3(.18f,1.72f,.86f));
                    Window(architecture,new Vector3(x,1.82f,center),new Vector2(4.28f,1.72f),side*90);
                    Box("Zoclo exterior sala",architecture,new Vector3(side*7.39f,.06f,center),new Vector3(.025f,.12f,5.9f),stone,false);
                    Box("Zoclo pared de fondo",architecture,new Vector3(side*4.40f,.06f,z+5.90f),new Vector3(5.96f,.12f,.026f),stone,false);
                }
                for(int i=0;i<3;i++)
                {
                    float start=3+i*6;
                    Box("Zoclo pasillo",architecture,new Vector3(side*1.255f,.06f,start+4.27f),new Vector3(.028f,.12f,3.33f),stone,false);
                    Box("Zoclo pasillo corto",architecture,new Vector3(side*1.255f,.06f,start+.5f),new Vector3(.028f,.12f,1),stone,false);
                }
            }
            // Frente acristalado con acceso central a nivel de la banqueta.
            foreach(int side in new[]{-1,1})
            {
                Window(lobby,new Vector3(side*4.4f,1.47f,-7),new Vector2(6.2f,2.82f),0);
                Box("Puerta de vidrio abierta",lobby,new Vector3(side*1.30f,1.42f,-7.65f),new Vector3(.035f,2.8f,1.3f),glass,true).GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
                Rod("Tirador de acceso",lobby,new Vector3(side*1.25f,.88f,-7.25f),new Vector3(side*1.25f,1.5f,-7.25f),.018f,metal);
            }
            Box("Dintel de fachada",lobby,new Vector3(0,3.05f,-7.09f),new Vector3(15,.38f,.4f),sage);
            Sign(lobby,"CONSULTORIO SEGURO","CLÍNICA DENTAL · FORMACIÓN EN BIOSEGURIDAD",new Vector3(0,3.4f,-7.33f),new Vector2(5.8f,.75f),0,true);
            Box("Marquesina de acceso",lobby,new Vector3(0,2.95f,-7.95f),new Vector3(5.5f,.10f,1.75f),wood);
            for(int i=0;i<3;i++)Box("Luz bajo marquesina",lobby,new Vector3(-1.6f+i*1.6f,2.885f,-8),new Vector3(.6f,.01f,.045f),emissive,false);
        }
        static void Window(Transform parent,Vector3 p,Vector2 size,float yaw)
        {
            var g=Group("Ventana con vidrio y marco",parent,p);g.localRotation=Quaternion.Euler(0,yaw,0);
            var pane=Box("Vidrio",g,Vector3.zero,new Vector3(size.x,size.y,.018f),glass);
            pane.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
            for(int k=-1;k<=1;k++)Box("Montante",g,new Vector3(k*size.x*.5f,0,0),new Vector3(.045f,size.y,.075f),dark,false);
            foreach(float k in new[]{-1f,1f})Box("Travesaño",g,new Vector3(0,k*size.y*.5f,0),new Vector3(size.x,.045f,.075f),dark,false);
            if(yaw!=0)
            {
                Box("Alféizar de piedra",g,new Vector3(0,-size.y*.5f,-.075f),new Vector3(size.x+.12f,.045f,.24f),stone,false,.006f);
                // Estor parcialmente recogido: se conserva la entrada de luz y la vista exterior.
                Box("Estor sanitario",g,new Vector3(0,size.y*.5f-.14f,-.065f),new Vector3(size.x-.07f,.25f,.025f),linen,false);
            }
        }
        static void MakeLighting()
        {
            var sky=Material("Cielo físico",Color.white);sky.shader=Shader.Find("Skybox/Procedural");sky.SetFloat("_SunSize",.025f);sky.SetFloat("_AtmosphereThickness",.75f);sky.SetColor("_SkyTint",new Color(.53f,.60f,.68f));sky.SetColor("_GroundColor",new Color(.40f,.42f,.43f));sky.SetFloat("_Exposure",1.15f);RenderSettings.skybox=sky;
            RenderSettings.ambientMode=AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.64f,.68f,.72f);RenderSettings.ambientEquatorColor=new Color(.59f,.61f,.59f);RenderSettings.ambientGroundColor=new Color(.55f,.55f,.51f);RenderSettings.ambientIntensity=1;
            var sh=new SphericalHarmonicsL2();sh.AddAmbientLight(new Color(.58f,.60f,.57f));sh.AddDirectionalLight(Vector3.up,new Color(.80f,.90f,1),.10f);sh.AddDirectionalLight(Vector3.down,new Color(1,.95f,.85f),.12f);RenderSettings.ambientProbe=sh;
            var sun=new GameObject("Sol de la mañana").AddComponent<Light>();sun.transform.SetParent(lighting);sun.type=LightType.Directional;sun.color=new Color(1,.93f,.81f);sun.intensity=2.0f;sun.transform.rotation=Quaternion.Euler(36,-48,0);sun.shadows=LightShadows.Soft;sun.shadowBias=.035f;sun.shadowNormalBias=.075f;sun.GetUniversalAdditionalLightData().usePipelineSettings=false;sun.GetUniversalAdditionalLightData().softShadowQuality=SoftShadowQuality.High;RenderSettings.sun=sun;
            foreach(float x in new[]{-4.4f,0,4.4f})foreach(float z in new[]{-4.5f,0f})CeilingLight(new Vector3(x,2.96f,z),x==0?.65f:.95f);
            foreach(float z in new[]{5f,8f,11f,14f,17f,20f})CeilingLight(new Vector3(0,2.96f,z),.58f);
            foreach(float x in new[]{-4.4f,4.4f})foreach(float z in new[]{6f,12f,18f})
            {
                CeilingLight(new Vector3(x,2.96f,z),1.25f);
                var fill=new GameObject("Luz rebotada de ventana").AddComponent<Light>();fill.transform.SetParent(lighting);fill.transform.position=new Vector3(Mathf.Sign(x)*6.65f,1.55f,z);fill.type=LightType.Point;fill.range=6;fill.intensity=.65f;fill.color=new Color(.89f,.95f,1);fill.shadows=LightShadows.None;
                Probe(new Vector3(x,1.5f,z),new Vector3(6.3f,3.3f,6.3f));
            }
            // El relleno indirecto procede del ambiente; un punto junto al asiento produce brillos sin una luminaria real.
            Probe(new Vector3(0,1.5f,-2),new Vector3(15.4f,3.4f,10.4f));Probe(new Vector3(0,1.5f,12),new Vector3(2.95f,3.4f,18.3f));
            var volume=new GameObject("Exposición y tono uniforme").AddComponent<Volume>();volume.transform.SetParent(lighting);volume.isGlobal=true;
            var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Generated+"/Ambiente.asset");if(!profile){profile=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(profile,Generated+"/Ambiente.asset");}
            if(!profile.TryGet<Tonemapping>(out var tonemap)){tonemap=profile.Add<Tonemapping>();AssetDatabase.AddObjectToAsset(tonemap,profile);}tonemap.mode.Override(TonemappingMode.ACES);
            if(!profile.TryGet<ColorAdjustments>(out var adjust)){adjust=profile.Add<ColorAdjustments>();AssetDatabase.AddObjectToAsset(adjust,profile);}adjust.postExposure.Override(.65f);adjust.contrast.Override(4);adjust.saturation.Override(-3);
            if(!profile.TryGet<Bloom>(out var bloom)){bloom=profile.Add<Bloom>();AssetDatabase.AddObjectToAsset(bloom,profile);}bloom.intensity.Override(.012f);bloom.threshold.Override(2f);
            volume.sharedProfile=profile;EditorUtility.SetDirty(profile);EditorUtility.SetDirty(tonemap);EditorUtility.SetDirty(adjust);EditorUtility.SetDirty(bloom);
        }
        static void CeilingLight(Vector3 pos,float intensity)
        {
            Box("Marco de luminaria",roof,pos,new Vector3(1.28f,.06f,.45f),white,false,.018f);
            Box("Difusor LED",roof,pos+Vector3.down*.032f,new Vector3(1.18f,.012f,.36f),emissive,false,.01f);
            var light=new GameObject("Luz clínica 4000 K").AddComponent<Light>();light.transform.SetParent(lighting);light.transform.position=pos+Vector3.down*.11f;light.transform.rotation=Quaternion.Euler(90,0,0);light.type=LightType.Spot;light.spotAngle=140;light.innerSpotAngle=100;light.range=7;light.intensity=intensity*2;light.color=new Color(1,.965f,.92f);light.shadows=LightShadows.Soft;light.shadowBias=.02f;light.shadowNormalBias=.035f;
            var data=light.GetUniversalAdditionalLightData();data.usePipelineSettings=false;data.softShadowQuality=SoftShadowQuality.High;
            var serialized=new SerializedObject(data);Set(serialized,"m_AdditionalLightsShadowResolutionTier",1);serialized.ApplyModifiedPropertiesWithoutUndo();
        }
        static void Probe(Vector3 pos,Vector3 size)
        {var p=new GameObject("Reflejo interior").AddComponent<ReflectionProbe>();p.transform.SetParent(lighting);p.transform.position=pos;p.size=size;p.boxProjection=true;p.resolution=256;p.mode=ReflectionProbeMode.Realtime;p.refreshMode=ReflectionProbeRefreshMode.OnAwake;p.timeSlicingMode=ReflectionProbeTimeSlicingMode.NoTimeSlicing;p.blendDistance=.32f;p.intensity=.85f;p.clearFlags=ReflectionProbeClearFlags.Skybox;}
    }
}
