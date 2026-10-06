using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;

namespace ConsultorioSeguroNuevo.Editor
{
    public static partial class ClinicBuilder
    {
        public const string ScenePath = "Assets/ConsultorioSeguro/Escenas/ConsultorioSeguro.unity";
        const string Generated = "Assets/ConsultorioSeguro/Generados";
        static Material plaster, sage, wood, white, metal, dark, glass, linen, tile, emissive, stone;
        static Transform architecture, furniture, roof, city, lighting;
        static readonly Dictionary<string, Material> mats = new();
        static readonly Dictionary<string, Mesh> meshes = new();
        static readonly List<PracticeRoom> rooms = new();
        static readonly List<string> usedAssets = new();
        static TMP_FontAsset font;

        [MenuItem("Consultorio Seguro/Crear escena nueva")]
        public static void Build()
        {
            Directory.CreateDirectory(Generated); Directory.CreateDirectory("Assets/ConsultorioSeguro/Escenas");
            AssetDatabase.Refresh(); mats.Clear(); meshes.Clear(); rooms.Clear(); usedAssets.Clear();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupPipeline(); SetupMaterials();
            SetupFont();
            architecture = Group("01 · Arquitectura"); roof = Group("02 · Techos y torre");
            furniture = Group("03 · Mobiliario y equipamiento"); city = Group("04 · Entorno urbano"); lighting = Group("05 · Iluminación");
            MakeArchitecture(); MakeReception(); MakeRooms(); MakeCity(); MakeLighting(); MakePlayer(); MakeViews();
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), ScenePath);
            EditorBuildSettings.scenes = new[]{new EditorBuildSettingsScene(ScenePath,true)};
            PlayerSettings.companyName="Equipo 7 · UPIICSA"; PlayerSettings.productName="Consultorio Seguro";
            PlayerSettings.colorSpace=ColorSpace.Linear;
            AssetDatabase.SaveAssets();
            Directory.CreateDirectory("Verificacion");
            File.WriteAllLines("Verificacion/modelos-utilizados.txt",usedAssets.Distinct().OrderBy(x=>x));
            Debug.Log("CONSULTORIO_LISTO · Escena nueva guardada con cinco salas.");
        }

        static Transform Group(string name, Transform parent=null, Vector3 pos=default)
        { var t=new GameObject(name).transform; t.SetParent(parent,false); t.localPosition=pos; return t; }
        static GameObject Box(string name,Transform parent,Vector3 pos,Vector3 size,Material mat,bool solid=true,float bevel=0)
        {
            GameObject go;
            if(bevel>0)
            {
                go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));
                go.GetComponent<MeshFilter>().sharedMesh=Beveled(size,bevel);
                if(solid){var c=go.AddComponent<BoxCollider>();c.size=size;}
            }
            else {go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.localScale=size;if(!solid) Object.DestroyImmediate(go.GetComponent<Collider>());}
            go.transform.SetParent(parent,false);go.transform.localPosition=pos;
            var r=go.GetComponent<Renderer>();r.sharedMaterial=mat;r.receiveShadows=true;
            return go;
        }
        static GameObject Cylinder(string name,Transform parent,Vector3 pos,Vector3 size,Material mat,bool solid=false)
        {var g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;if(!solid)Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
        static void Rod(string name,Transform parent,Vector3 a,Vector3 b,float radius,Material mat)
        {var g=Cylinder(name,parent,(a+b)*.5f,new Vector3(radius*2,Vector3.Distance(a,b)*.5f,radius*2),mat);g.transform.up=(b-a).normalized;}
        static Material Material(string name,Color color,float smooth=.35f,float metallic=0)
        {
            if(mats.TryGetValue(name,out var m))return m;
            string path=Generated+"/"+name+".mat";m=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,path);}
            m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",smooth);m.SetFloat("_Metallic",metallic);
            EditorUtility.SetDirty(m);mats[name]=m;return m;
        }
        static Texture2D Texture(string path,bool normal=false,bool linear=false)
        {
            if(!File.Exists(path))return null;
            var imp=(TextureImporter)AssetImporter.GetAtPath(path);
            if(imp!=null)
            {bool change=(normal&&imp.textureType!=TextureImporterType.NormalMap)||(linear&&imp.sRGBTexture)||imp.anisoLevel!=8;
             if(normal)imp.textureType=TextureImporterType.NormalMap;if(linear)imp.sRGBTexture=false;imp.anisoLevel=8;imp.maxTextureSize=2048;if(change)imp.SaveAndReimport();}
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        static void Pbr(Material m,string directory,string stem,float bump=.3f)
        {
            string root="Assets/Terceros/"+directory+"/"; string p=root+(Directory.Exists(root+"textures")?"textures/":"")+stem;
            var color=Texture(p+"_diff_2k.jpg");if(color)m.SetTexture("_BaseMap",color);
            var normal=Texture(p+"_nor_gl_2k.jpg",true);if(normal){m.SetTexture("_BumpMap",normal);m.SetFloat("_BumpScale",bump);m.EnableKeyword("_NORMALMAP");}
            string packed=Directory.GetFiles("Assets/Terceros/"+directory,"*metallic*",SearchOption.AllDirectories).FirstOrDefault(x=>!x.EndsWith(".meta") && Path.GetFileName(x).StartsWith(stem));
            if(packed!=null){m.SetTexture("_MetallicGlossMap",Texture(packed,false,true));m.EnableKeyword("_METALLICSPECGLOSSMAP");m.SetFloat("_Smoothness",1);m.SetFloat("_Metallic",1);}
            var ao=Texture(p+"_occlusion_2k.png",false,true);if(ao){m.SetTexture("_OcclusionMap",ao);m.SetFloat("_OcclusionStrength",.75f);m.EnableKeyword("_OCCLUSIONMAP");}
            EditorUtility.SetDirty(m);
        }
        static Transform Model(string path,Transform parent,Vector3 position,float height,float yaw=0,bool collision=true)
        {
            string full="Assets/Terceros/"+path;
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(full);
            if(!prefab)throw new Exception("No se importó el modelo "+full);
            var pivot=Group(Path.GetFileNameWithoutExtension(path),parent);
            var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab,pivot);
            go.transform.localPosition=Vector3.zero;go.transform.localRotation=Quaternion.identity;
            var rs=go.GetComponentsInChildren<Renderer>();
            if(rs.Length==0)throw new Exception("Modelo sin mallas "+path);
            Bounds b=new Bounds();bool first=true;foreach(var filter in go.GetComponentsInChildren<MeshFilter>()){var mb=filter.sharedMesh.bounds;for(int corner=0;corner<8;corner++){Vector3 local=mb.center+Vector3.Scale(mb.extents,new Vector3((corner&1)==0?-1:1,(corner&2)==0?-1:1,(corner&4)==0?-1:1));Vector3 point=pivot.InverseTransformPoint(filter.transform.TransformPoint(local));if(first){b=new Bounds(point,Vector3.zero);first=false;}else b.Encapsulate(point);}}
            float scale=height/b.size.y;go.transform.localScale*=scale;
            go.transform.localPosition=new Vector3(-b.center.x,-b.min.y,-b.center.z)*scale;
            foreach(var r in rs)
            {
                r.sharedMaterials=r.sharedMaterials.Select(original=>ConvertMaterial(original,path)).ToArray();
                r.reflectionProbeUsage=ReflectionProbeUsage.BlendProbes;r.lightProbeUsage=LightProbeUsage.BlendProbes;
            }
            if(collision){var box=pivot.gameObject.AddComponent<BoxCollider>();box.center=Vector3.up*height*.5f;box.size=new Vector3(b.size.x*scale,height,b.size.z*scale);}
            pivot.localPosition=position;pivot.localRotation=Quaternion.Euler(0,yaw,0);
            usedAssets.Add(path);return pivot;
        }
        static Material ConvertMaterial(Material source,string path)
        {
            string n=source?source.name.Replace(" (Instance)",""):"Plástico";
            string safe=Path.GetDirectoryName(path).Replace('/','-')+"-"+n;
            if(mats.TryGetValue(safe,out var existing))return existing;
            Color c=source && source.HasProperty("_BaseColor")?source.GetColor("_BaseColor"):source&&source.HasProperty("_Color")?source.color:Color.white;
            var m=Material(safe,c,.4f);
            if(path.StartsWith("polyhaven-"))
            {
                string dir=path.Split('/')[0];m.SetColor("_BaseColor",Color.white);Pbr(m,dir,n,.65f);
                m.SetFloat("_Cull",0);
                if(n.Contains("leaves")){m.SetTexture("_BaseMap",Texture("Assets/Terceros/"+dir+"/textures/"+n+"_base_alpha_2k.png"));m.SetFloat("_AlphaClip",1);m.SetFloat("_Cutoff",.5f);m.EnableKeyword("_ALPHATEST_ON");m.renderQueue=2450;}
                if(n.Contains("pillow"))m.SetFloat("_Smoothness",.25f);
            }
            else
            {
                if(source && source.HasProperty("_BaseMap"))m.SetTexture("_BaseMap",source.GetTexture("_BaseMap"));
                string lower=n.ToLowerInvariant();
                if(lower.Contains("steel")||lower.Contains("metal")||lower.Contains("chrome")){m.SetFloat("_Metallic",.82f);m.SetFloat("_Smoothness",.65f);}
                if(lower.Contains("teal")||lower.Contains("cushion")||lower.Contains("upholstery"))m.SetColor("_BaseColor",new Color(.38f,.52f,.47f));
            }
            return m;
        }
        static void SetupFont()
        {
            string path=Generated+"/Tipografía clínica.asset";
            font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if(!font)
            {
                var source=AssetDatabase.LoadAssetAtPath<Font>("Assets/Terceros/noto-sans/NotoSans-Regular.ttf");
                font=TMP_FontAsset.CreateFontAsset(source,90,9,GlyphRenderMode.SDFAA,2048,2048,AtlasPopulationMode.Dynamic,true);
                font.name="Noto Sans · señalética";AssetDatabase.CreateAsset(font,path);
                font.TryAddCharacters(string.Concat(Enumerable.Range(32,224).Select(c=>(char)c))+"→←·²");
                AssetDatabase.AddObjectToAsset(font.material,font);
                foreach(var atlas in font.atlasTextures)AssetDatabase.AddObjectToAsset(atlas,font);
            }
            font.material.SetFloat("_ZTestMode",4);font.material.SetFloat("_CullMode",2);
            EditorUtility.SetDirty(font);EditorUtility.SetDirty(font.material);
        }
        static void Text(Transform parent,string content,Vector3 position,float width,float height,float fontHeight,Color color,float yaw=0)
        {
            var go=new GameObject("Texto · "+content.Replace('\n',' '),typeof(RectTransform));go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localRotation=Quaternion.Euler(0,yaw,0);
            var t=go.AddComponent<TextMeshPro>();t.font=font;t.text=content;t.rectTransform.sizeDelta=new Vector2(width*.92f,height*.90f);t.alignment=TextAlignmentOptions.Center;t.color=color;t.enableAutoSizing=true;t.fontSizeMin=.15f;t.fontSizeMax=fontHeight*10;t.textWrappingMode=TextWrappingModes.NoWrap;t.overflowMode=TextOverflowModes.Overflow;t.ForceMeshUpdate();
            t.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
        }
        static GameObject Sign(Transform parent,string title,string sub,Vector3 position,Vector2 size,float yaw=0,bool darkFace=false)
        {
            var p=Group(title,parent,position);p.localRotation=Quaternion.Euler(0,yaw,0);
            Box("Placa",p,Vector3.zero,new Vector3(size.x,size.y,.025f),darkFace?sage:white,false,.008f);
            var color=darkFace?new Color(.95f,.95f,.90f):new Color(.12f,.22f,.2f);
            Text(p,title,new Vector3(0,string.IsNullOrEmpty(sub)?0:size.y*.12f,-.019f),size.x,size.y,.18f,color);
            if(!string.IsNullOrEmpty(sub))Text(p,sub,new Vector3(0,-size.y*.22f,-.019f),size.x,size.y,.075f,color);
            return p.gameObject;
        }
        static Mesh Beveled(Vector3 size,float radius)
        {
            string key=$"Bisel-{size.x:F3}-{size.y:F3}-{size.z:F3}-{radius:F3}";
            if(meshes.TryGetValue(key,out var found))return found;
            string path=Generated+"/"+key+".asset";found=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(found){meshes[key]=found;return found;}
            var verts=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var tri=new List<int>();
            var half=size*.5f;var inner=half-Vector3.one*Mathf.Min(radius,Mathf.Min(half.x,Mathf.Min(half.y,half.z))*.8f);float r=half.x-inner.x;
            // La malla conserva las caras planas y suaviza los bordes del mobiliario.
            float[] f={-1,-.995f,-.98f,-.94f,-.82f,0,.82f,.94f,.98f,.995f,1};
            for(int face=0;face<6;face++)
            {
                int start=verts.Count;int axis=face/2;float sign=face%2==0?1:-1;
                Vector3 n=Vector3.zero;n[axis]=sign;Vector3 u=axis==0?Vector3.forward:Vector3.right;Vector3 v=Vector3.Cross(n,u);
                for(int y=0;y<f.Length;y++)for(int x=0;x<f.Length;x++)
                {
                    Vector3 raw=Vector3.Scale(n+u*f[x]+v*f[y],half);
                    Vector3 core=new Vector3(Mathf.Clamp(raw.x,-inner.x,inner.x),Mathf.Clamp(raw.y,-inner.y,inner.y),Mathf.Clamp(raw.z,-inner.z,inner.z));
                    Vector3 normal=(raw-core).normalized;verts.Add(core+normal*r);normals.Add(normal);uv.Add(new Vector2((f[x]+1)*.5f,(f[y]+1)*.5f));
                }
                for(int y=0;y<f.Length-1;y++)for(int x=0;x<f.Length-1;x++)
                {int a=start+y*f.Length+x;tri.Add(a);tri.Add(a+1);tri.Add(a+f.Length+1);tri.Add(a);tri.Add(a+f.Length+1);tri.Add(a+f.Length);}
            }
            var mesh=new Mesh{name=key};mesh.SetVertices(verts);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(tri,0);mesh.RecalculateBounds();mesh.RecalculateTangents();AssetDatabase.CreateAsset(mesh,path);meshes[key]=mesh;return mesh;
        }
    }
}
