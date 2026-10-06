using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace ConsultorioSeguroNuevo.Editor
{
    public static partial class ClinicBuilder
    {
        // Complementos originales; los archivos CC0 importados se conservan sin cambios.
        static void RefineDentalEquipment()
        {
            var upholstery = Material("Vinilo clínico salvia", new Color(.35f,.48f,.42f), .31f);
            var seam = Material("Costura de vinilo", new Color(.235f,.345f,.29f), .26f);
            var shell = Material("Polímero sanitario satinado", new Color(.89f,.91f,.875f), .38f);
            var rubber = Material("Elastómero de instrumental", new Color(.20f,.235f,.22f), .22f);
            var chrome = Material("Cromo de equipamiento", new Color(.67f,.73f,.73f), .72f, .86f);
            ApplyVinylGrain(upholstery);
            foreach (var room in rooms.Where(r => r.roomId == 4 || r.roomId == 5))
            {
                var chair = room.transform.Find("dental-chair-reclined");
                if (!chair) continue;
                DressDentalChair(chair, upholstery, seam, shell, rubber, chrome);
                if (room.roomId == 4)
                {
                    DentalDelivery(room.transform, chair.position, shell, rubber, chrome);
                    DentalStool(room.transform, chair.position + new Vector3(-1.12f,0,.90f), upholstery, seam, shell, rubber, chrome);
                    DressOperatingLight(room.transform, chrome);
                }
                else
                {
                    DentalStool(room.transform, chair.position + new Vector3(-1.10f,0,.28f), upholstery, seam, shell, rubber, chrome);
                    DressXrayHead(room.transform, shell, rubber, chrome);
                }
            }
        }

        static void ApplyVinylGrain(Material material)
        {
            const string path = Generated + "/Grano fino de vinilo.png";
            if (!File.Exists(path))
            {
                const int n=256;
                var pixels=new Color[n*n];
                var random=new System.Random(740);
                for (int i=0;i<pixels.Length;i++)
                {
                    float x=(float)(random.NextDouble()-.5)*.095f;
                    float y=(float)(random.NextDouble()-.5)*.095f;
                    pixels[i]=new Color(.5f+x,.5f+y,1,1);
                }
                var texture=new Texture2D(n,n,TextureFormat.RGB24,false,true);
                texture.SetPixels(pixels);texture.Apply();File.WriteAllBytes(path,texture.EncodeToPNG());
                Object.DestroyImmediate(texture);AssetDatabase.ImportAsset(path);
            }
            material.SetTexture("_BumpMap",Texture(path,true));
            material.SetFloat("_BumpScale",.20f);
            material.SetTextureScale("_BaseMap",new Vector2(8,8));
            material.EnableKeyword("_NORMALMAP");EditorUtility.SetDirty(material);
        }

        static void DressDentalChair(Transform chair,Material upholstery,Material seam,Material shell,Material rubber,Material chrome)
        {
            // Se retienen bastidor, carcasa, articulaciones y volumen de colisión CC0.
            foreach (var renderer in chair.GetComponentsInChildren<MeshRenderer>())
            {
                string name=renderer.GetComponent<MeshFilter>()?.sharedMesh?.name ?? renderer.name;
                if (name=="dental-chair-reclined_1" || name=="chair-back-pad" || name=="headrest-pad")
                    renderer.enabled=false;
            }
            var tailored=Group("Tapicería anatómica y detalles",chair);
            // El GLB original se centra y escala a 1,15 m por Model; glTFast refleja X.
            const float scale=1.15f/1.100f;
            tailored.localScale=Vector3.one*scale;
            tailored.localPosition=new Vector3(0,0,.07550f*scale);
            DentalCushion("Asiento contorneado",tailored,new Vector3(0,.808f,.182f),new Vector3(.515f,.108f,.690f),0,1,upholstery,seam);
            DentalCushion("Respaldo lumbar",tailored,new Vector3(0,.887f,-.522f),new Vector3(.475f,.112f,.720f),20,.86f,upholstery,seam);
            DentalCushion("Apoyo de piernas",tailored,new Vector3(0,.786f,.764f),new Vector3(.487f,.097f,.420f),0,.93f,upholstery,seam);
            DentalCushion("Reposacabezas anatómico",tailored,new Vector3(0,1.049f,-1.026f),new Vector3(.297f,.094f,.225f),20,.87f,upholstery,seam);
            foreach (float side in new[]{-1f,1f})
            {
                var arm=Box("Apoyo blando de antebrazo",tailored,new Vector3(side*.284f,.899f,.235f),new Vector3(.068f,.034f,.30f),upholstery,false,.023f);
                DentalCylinderBetween("Tapa de articulación",tailored,new Vector3(side*.277f,.741f,-.145f),new Vector3(side*.30f,.741f,-.145f),.043f,shell);
                DentalCylinderBetween("Eje de articulación",tailored,new Vector3(side*.301f,.741f,-.145f),new Vector3(side*.306f,.741f,-.145f),.018f,chrome);
            }
            // Collarines y fuelle hacen legible la elevación hidráulica existente.
            for(int i=0;i<5;i++)Cylinder("Pliegue del fuelle hidráulico",tailored,new Vector3(0,.29f+i*.033f,.18f),new Vector3(.222f,.013f,.222f),rubber);
            Cylinder("Collarín del émbolo",tailored,new Vector3(0,.455f,.18f),new Vector3(.234f,.016f,.234f),chrome);
            var pedal=Group("Mando de pie",chair,new Vector3(-.36f,.022f,-.55f));
            Box("Carcasa del pedal",pedal,new Vector3(0,.025f,0),new Vector3(.18f,.05f,.145f),shell,false,.026f);
            Box("Superficie antideslizante",pedal,new Vector3(0,.054f,-.015f),new Vector3(.115f,.016f,.08f),rubber,false,.018f);
            DentalCable("Cable del mando",chair,new[]{new Vector3(-.34f,.035f,-.48f),new Vector3(-.40f,.025f,-.31f),new Vector3(-.26f,.023f,-.08f),new Vector3(-.15f,.08f,.08f)},.009f,rubber);
        }

        static void DentalCushion(string name,Transform parent,Vector3 p,Vector3 size,float pitch,float taper,Material mat,Material seam)
        {
            var g=Group(name,parent,p);g.localRotation=Quaternion.Euler(pitch,0,0);
            string key=$"Tapizado-v3-{size.x:F3}-{size.y:F3}-{size.z:F3}-{taper:F2}";
            var mesh=DentalMesh(key,()=>
            {
                const int rings=20,around=64;
                var verts=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
                for(int i=0;i<=rings;i++)
                {
                    float phi=-Mathf.PI*.5f+Mathf.PI*i/rings;
                    float ring=Mathf.Pow(Mathf.Max(0,Mathf.Cos(phi)),.43f);
                    for(int j=0;j<=around;j++)
                    {
                        float t=Mathf.PI*2*j/around;
                        float xn=SignedPower(Mathf.Cos(t),.37f)*ring;
                        float zn=SignedPower(Mathf.Sin(t),.37f)*ring;
                        float width=Mathf.Lerp(taper,1,(zn+1)*.5f);
                        float y=SignedPower(Mathf.Sin(phi),.52f)*size.y*.5f;
                        if(phi>0)y+=.010f*(1-xn*xn)*(1-zn*zn);
                        verts.Add(new Vector3(xn*size.x*.5f*width,y,zn*size.z*.5f));
                        uv.Add(new Vector2((xn+1)*.5f,(zn+1)*.5f));
                    }
                }
                for(int i=0;i<rings;i++)for(int j=0;j<around;j++)
                {int a=i*(around+1)+j,b=a+around+1;triangles.AddRange(new[]{a,b,a+1,a+1,b,b+1});}
                return DentalCreateMesh(key,verts,uv,triangles);
            });
            DentalMeshObject(name,g,mesh,mat);
            var piping=new List<Vector3>();
            for(int i=0;i<=96;i++)
            {
                float t=Mathf.PI*2*i/96;float xn=SignedPower(Mathf.Cos(t),.37f),zn=SignedPower(Mathf.Sin(t),.37f);
                piping.Add(new Vector3(xn*size.x*.493f*Mathf.Lerp(taper,1,(zn+1)*.5f),-.006f,zn*size.z*.493f));
            }
            DentalTube("Costura perimetral sellada",g,piping,.00135f,seam);
        }

        static void DentalDelivery(Transform parent,Vector3 chair,Material shell,Material rubber,Material chrome)
        {
            var g=Group("Unidad dental · agua e instrumental",parent,chair+new Vector3(.77f,0,.18f));
            Box("Columna de servicio",g,new Vector3(0,.49f,.19f),new Vector3(.26f,.76f,.31f),shell,true,.055f);
            Box("Zócalo de unidad",g,new Vector3(0,.094f,.19f),new Vector3(.32f,.085f,.39f),rubber,false,.038f);
            Box("Tapa de servicio",g,new Vector3(.137f,.52f,.19f),new Vector3(.009f,.47f,.24f),white,false,.005f);
            DentalCylinderBetween("Brazo de consola",g,new Vector3(0,.79f,.14f),new Vector3(0,.88f,-.25f),.04f,shell);
            Box("Consola de instrumentos",g,new Vector3(0,.99f,-.36f),new Vector3(.49f,.115f,.34f),shell,true,.045f);
            Box("Inserto de consola",g,new Vector3(0,1.05f,-.35f),new Vector3(.42f,.009f,.255f),rubber,false,.022f);
            Box("Pantalla de ajuste",g,new Vector3(.11f,1.058f,-.30f),new Vector3(.13f,.005f,.083f),Material("Pantalla de unidad dental",new Color(.13f,.29f,.29f),.55f),false,.008f);
            for(int i=0;i<3;i++)
            {
                float x=-.148f+i*.08f;
                Cylinder("Botón de consola",g,new Vector3(x,1.06f,-.28f),new Vector3(.025f,.004f,.025f),white);
                DentalCylinderBetween("Acople de manguera",g,new Vector3(x,.945f,-.51f),new Vector3(x,.91f,-.55f),.018f,chrome);
                var hose=new[]{new Vector3(x,.922f,-.54f),new Vector3(x-.012f,.66f,-.65f),new Vector3(x+.045f,.43f,-.54f),new Vector3(x+.10f,.53f,-.40f),new Vector3(x+.086f,.92f,-.44f)};
                DentalCable("Manguera de pieza de mano",g,hose,.009f,shell);
                DentalCylinderBetween("Pieza de mano",g,new Vector3(x+.085f,.91f,-.44f),new Vector3(x+.075f,1.078f,-.45f),.012f,chrome);
                DentalCylinderBetween("Cabezal de pieza",g,new Vector3(x+.075f,1.075f,-.45f),new Vector3(x+.064f,1.10f,-.476f),.010f,chrome);
            }
            var basin=Group("Escupidera cerámica",g,new Vector3(.015f,.972f,.285f));
            var ceramic=Material("Cerámica sanitaria esmaltada",new Color(.94f,.95f,.91f),.69f);
            DentalLathe("Cuenca de escupidera",basin,new[]{new Vector2(.026f,-.084f),new Vector2(.065f,-.078f),new Vector2(.128f,-.055f),new Vector2(.177f,-.011f),new Vector2(.190f,0),new Vector2(.20f,-.006f),new Vector2(.194f,-.025f),new Vector2(.147f,-.083f),new Vector2(.06f,-.112f),new Vector2(0,-.112f)}.Reverse().ToArray(),ceramic);
            Cylinder("Desagüe de cuenca",basin,new Vector3(0,-.081f,0),new Vector3(.046f,.002f,.046f),chrome);
            DentalCable("Grifo de enjuague",g,new[]{new Vector3(-.12f,.91f,.47f),new Vector3(-.12f,1.07f,.47f),new Vector3(-.10f,1.105f,.435f),new Vector3(-.06f,1.08f,.375f)},.014f,chrome);
            DentalLathe("Vaso de enjuague",g,new[]{new Vector2(.025f,0),new Vector2(.032f,.09f),new Vector2(.035f,.092f),new Vector2(.027f,.09f),new Vector2(.021f,.008f)},white,new Vector3(.09f,.967f,.52f));
            DentalCable("Manguera de aspiración",g,new[]{new Vector3(-.13f,.74f,.26f),new Vector3(-.25f,.50f,.32f),new Vector3(-.28f,.42f,.08f),new Vector3(-.23f,.66f,-.025f),new Vector3(-.20f,.94f,.04f)},.012f,rubber);
            DentalCylinderBetween("Cánula de aspiración",g,new Vector3(-.20f,.91f,.04f),new Vector3(-.20f,1.064f,.04f),.011f,white);
        }

        static void DentalStool(Transform parent,Vector3 p,Material upholstery,Material seam,Material shell,Material rubber,Material chrome)
        {
            var g=Group("Taburete clínico regulable",parent,p);
            var collision=g.gameObject.AddComponent<CapsuleCollider>();
            collision.center=new Vector3(0,.31f,0);collision.height=.62f;collision.radius=.225f;
            DentalCushion("Asiento de operador",g,new Vector3(0,.566f,0),new Vector3(.41f,.085f,.36f),0,.98f,upholstery,seam);
            Cylinder("Plato de asiento",g,new Vector3(0,.515f,0),new Vector3(.29f,.016f,.29f),shell);
            Cylinder("Pistón de taburete",g,new Vector3(0,.33f,0),new Vector3(.052f,.18f,.052f),chrome);
            Cylinder("Fuelle de taburete",g,new Vector3(0,.20f,0),new Vector3(.092f,.08f,.092f),rubber);
            for(int i=0;i<5;i++)
            {
                float a=i*Mathf.PI*2/5;var foot=new Vector3(Mathf.Cos(a)*.245f,.086f,Mathf.Sin(a)*.245f);
                DentalCylinderBetween("Radio de base",g,new Vector3(0,.16f,0),foot,.016f,chrome);
                var wheel=Cylinder("Rueda de taburete",g,foot+Vector3.down*.025f,new Vector3(.063f,.017f,.063f),rubber);
                wheel.transform.localRotation=Quaternion.Euler(90,0,a*Mathf.Rad2Deg);
            }
            DentalCylinderBetween("Palanca de altura",g,new Vector3(.06f,.495f,0),new Vector3(.23f,.495f,-.04f),.007f,chrome);
            Box("Empuñadura de palanca",g,new Vector3(.235f,.495f,-.04f),new Vector3(.075f,.016f,.03f),rubber,false,.01f);
        }

        static void DressOperatingLight(Transform room,Material chrome)
        {
            var source=room.Find("operating-light");if(!source)return;
            var lens=source.GetComponentsInChildren<Transform>().FirstOrDefault(t=>t.name=="light-head-lens");
            if(!lens)return;
            var b=DentalLocalBounds(lens);
            var optical=Material("Óptica de lámpara dental",new Color(.87f,.92f,.87f),.64f,.10f);
            optical.EnableKeyword("_EMISSION");optical.SetColor("_EmissionColor",new Color(.28f,.31f,.26f));
            for(int x=0;x<3;x++)for(int z=0;z<2;z++)
            {
                var p=new Vector3(b.center.x+(x-1)*b.size.x*.28f,b.min.y-.003f,b.center.z+(z-.5f)*b.size.z*.50f);
                Cylinder("Reflector óptico",lens,p,new Vector3(b.size.x*.23f,.003f,b.size.z*.43f),chrome);
                Cylinder("Lente LED",lens,p+Vector3.down*.004f,new Vector3(b.size.x*.19f,.002f,b.size.z*.35f),optical);
            }
        }

        static void DressXrayHead(Transform room,Material shell,Material rubber,Material chrome)
        {
            var source=room.Find("intraoral-xray-arm");if(!source)return;
            var head=source.GetComponentsInChildren<MeshFilter>().FirstOrDefault(f=>f.sharedMesh.name=="xray-arm/dark");
            if(!head)return;
            var b=DentalBoundsRelativeTo(head,source);
            var g=Group("Detalle de colimador",source,new Vector3(b.center.x,b.center.y,b.max.z));
            // Una abertura negra y un aro suavizado sustituyen la lectura de bloque sólido.
            g.localRotation=Quaternion.Euler(90,0,0);
            DentalLathe("Aro del colimador",g,new[]{new Vector2(.066f,-.006f),new Vector2(.070f,0),new Vector2(.070f,.012f),new Vector2(.054f,.014f),new Vector2(.052f,.01f),new Vector2(.052f,-.004f)},shell);
            Cylinder("Interior del colimador",g,new Vector3(0,-.006f,0),new Vector3(.104f,.002f,.104f),rubber);
        }

        static Bounds DentalBoundsRelativeTo(MeshFilter filter,Transform parent)
        {
            var meshBounds=filter.sharedMesh.bounds;var result=new Bounds();
            for(int c=0;c<8;c++)
            {
                var p=meshBounds.center+Vector3.Scale(meshBounds.extents,new Vector3((c&1)==0?-1:1,(c&2)==0?-1:1,(c&4)==0?-1:1));
                p=parent.InverseTransformPoint(filter.transform.TransformPoint(p));
                if(c==0)result=new Bounds(p,Vector3.zero);else result.Encapsulate(p);
            }
            return result;
        }

        static Bounds DentalLocalBounds(Transform parent)
        {
            var b=new Bounds();bool first=true;
            foreach(var f in parent.GetComponentsInChildren<MeshFilter>())
            {
                var bounds=f.sharedMesh.bounds;
                for(int c=0;c<8;c++)
                {
                    var p=bounds.center+Vector3.Scale(bounds.extents,new Vector3((c&1)==0?-1:1,(c&2)==0?-1:1,(c&4)==0?-1:1));
                    p=parent.InverseTransformPoint(f.transform.TransformPoint(p));
                    if(first){b=new Bounds(p,Vector3.zero);first=false;}else b.Encapsulate(p);
                }
            }
            return b;
        }

        static float SignedPower(float value,float power)=>Mathf.Sign(value)*Mathf.Pow(Mathf.Abs(value),power);
        static void DentalCylinderBetween(string name,Transform parent,Vector3 a,Vector3 b,float radius,Material mat)
        {
            var g=Cylinder(name,parent,(a+b)*.5f,new Vector3(radius*2,Vector3.Distance(a,b)*.5f,radius*2),mat);
            g.transform.localRotation=Quaternion.FromToRotation(Vector3.up,(b-a).normalized);
        }
        static void DentalCable(string name,Transform parent,IList<Vector3> control,float radius,Material mat)
        {
            var points=new List<Vector3>();
            for(int i=0;i<control.Count-1;i++)
            {
                Vector3 a=control[Mathf.Max(0,i-1)],b=control[i],c=control[i+1],d=control[Mathf.Min(control.Count-1,i+2)];
                for(int k=0;k<12;k++)
                {
                    float t=k/12f,t2=t*t,t3=t2*t;
                    points.Add(.5f*((2*b)+(-a+c)*t+(2*a-5*b+4*c-d)*t2+(-a+3*b-3*c+d)*t3));
                }
            }
            points.Add(control[control.Count-1]);DentalTube(name,parent,points,radius,mat);
        }
        static void DentalTube(string name,Transform parent,IList<Vector3> points,float radius,Material mat)
        {
            var verts=new List<Vector3>();var uv=new List<Vector2>();var tris=new List<int>();const int radial=8;
            for(int i=0;i<points.Count;i++)
            {
                var tangent=(points[Mathf.Min(i+1,points.Count-1)]-points[Mathf.Max(0,i-1)]).normalized;
                var axis=Vector3.Cross(tangent,Mathf.Abs(tangent.y)>.95f?Vector3.right:Vector3.up).normalized;
                var up=Vector3.Cross(tangent,axis).normalized;
                for(int j=0;j<=radial;j++)
                {float a=j*Mathf.PI*2/radial;verts.Add(points[i]+radius*(axis*Mathf.Cos(a)+up*Mathf.Sin(a)));uv.Add(new Vector2(j/(float)radial,i/(float)points.Count));}
                if(i>0)for(int j=0;j<radial;j++){int a=(i-1)*(radial+1)+j,b=i*(radial+1)+j;tris.AddRange(new[]{a,a+1,b,b,a+1,b+1});}
            }
            string key="Conducto-"+DentalStableHash(name+string.Join(";",points.Select(p=>$"{p.x:F4},{p.y:F4},{p.z:F4}"))+radius.ToString("F4"));
            DentalMeshObject(name,parent,DentalMesh(key,()=>DentalCreateMesh(name,verts,uv,tris)),mat);
        }
        static void DentalLathe(string name,Transform parent,IList<Vector2> profile,Material mat,Vector3 p=default)
        {
            string key="Torno-"+DentalStableHash(name+string.Join(";",profile.Select(v=>$"{v.x:F4},{v.y:F4}")));
            var mesh=DentalMesh(key,()=>
            {
                var verts=new List<Vector3>();var uv=new List<Vector2>();var tris=new List<int>();const int n=64;
                for(int i=0;i<profile.Count;i++)for(int j=0;j<=n;j++)
                {
                    float a=j*Mathf.PI*2/n;verts.Add(new Vector3(Mathf.Cos(a)*profile[i].x,profile[i].y,Mathf.Sin(a)*profile[i].x));uv.Add(new Vector2(j/(float)n,i/(float)profile.Count));
                    if(i>0&&j<n){int b=i*(n+1)+j,a0=b-n-1;tris.AddRange(new[]{a0,b,a0+1,a0+1,b,b+1});}
                }
                return DentalCreateMesh(name,verts,uv,tris);
            });
            var go=DentalMeshObject(name,parent,mesh,mat);go.transform.localPosition=p;
        }
        static string DentalStableHash(string value)
        {unchecked{uint hash=2166136261;foreach(char c in value){hash^=c;hash*=16777619;}return hash.ToString("x8");}}
        static Mesh DentalMesh(string key,Func<Mesh> create)
        {
            string path=Generated+"/"+key+".asset";
            var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(!mesh){mesh=create();AssetDatabase.CreateAsset(mesh,path);}return mesh;
        }
        static Mesh DentalCreateMesh(string name,List<Vector3> vertices,List<Vector2> uv,List<int> triangles)
        {
            var mesh=new Mesh{name=name};mesh.SetVertices(vertices);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateTangents();mesh.RecalculateBounds();return mesh;
        }
        static GameObject DentalMeshObject(string name,Transform parent,Mesh mesh,Material material)
        {
            var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);go.GetComponent<MeshFilter>().sharedMesh=mesh;
            var r=go.GetComponent<MeshRenderer>();r.sharedMaterial=material;r.reflectionProbeUsage=ReflectionProbeUsage.BlendProbes;r.lightProbeUsage=LightProbeUsage.BlendProbes;return go;
        }
    }
}
