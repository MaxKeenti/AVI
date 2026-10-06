using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace ConsultorioSeguroNuevo.Editor
{
    public static partial class ClinicBuilder
    {
        static Material OakFinish(string name, Vector2 scale)
        {
            var finish=Material(name,Color.white,.3f);finish.CopyPropertiesFromMaterial(wood);
            finish.SetTextureScale("_BaseMap",scale);finish.SetFloat("_BumpScale",.08f);
            finish.SetFloat("_OcclusionStrength",.2f);EditorUtility.SetDirty(finish);return finish;
        }

        static void ReceptionJoinery(Transform root)
        {
            var ivory=Material("Piedra clara del registro",new Color(.88f,.85f,.77f),.23f);
            var oakPanel=OakFinish("Roble · revestimiento proporcionado",new Vector2(2.4f,1.4f));
            var oakSlat=OakFinish("Roble · listón fino",new Vector2(.10f,1.1f));
            var brass=Material("Latón satinado de detalles",new Color(.53f,.42f,.26f),.4f,.72f);
            Box("Revestimiento claro de recepción",root,new Vector3(4.5f,1.5f,2.867f),new Vector3(5.60f,2.94f,.048f),ivory,false);
            Box("Panel lateral de roble",root,new Vector3(6.75f,1.5f,2.823f),new Vector3(1.05f,2.94f,.045f),oakPanel,false);
            for(int i=0;i<14;i++)Box("Listón de pared",root,new Vector3(6.255f+i*.075f,1.5f,2.788f),new Vector3(.048f,2.94f,.035f),oakSlat,false,.006f);
            Box("Junta de latón del revestimiento",root,new Vector3(6.20f,1.5f,2.782f),new Vector3(.012f,2.94f,.012f),brass,false);
            Text(root,"CONSULTORIO",new Vector3(4.35f,2.15f,2.803f),3.4f,.40f,.29f,Ink);
            Text(root,"SEGURO",new Vector3(4.35f,1.82f,2.803f),3.4f,.40f,.32f,Ink);
            Text(root,"CUIDADO DENTAL · BIOSEGURIDAD",new Vector3(4.35f,1.48f,2.803f),3.4f,.20f,.063f,Ink);
            ToothEmblem(root,new Vector3(4.35f,2.62f,2.793f),.28f);
            Box("Mueble de archivo posterior",root,new Vector3(4.05f,.41f,2.58f),new Vector3(3.6f,.82f,.42f),ivory,true,.016f);
            Box("Tapa del archivo",root,new Vector3(4.05f,.839f,2.58f),new Vector3(3.65f,.038f,.45f),white,false,.008f);
            for(int i=0;i<5;i++)
            {
                float x=2.62f+i*.715f;
                Box("Puerta del archivo",root,new Vector3(x,.42f,2.352f),new Vector3(.703f,.77f,.016f),ivory,false,.005f);
                Box("Uñero del archivo",root,new Vector3(x,.748f,2.34f),new Vector3(.21f,.01f,.01f),dark,false);
            }
            RoundedPlan("Mostrador · cuerpo curvo",root,new Vector3(4.45f,.57f,-.15f),new Vector3(4.1f,1.02f,.91f),.21f,sage,true);
            RoundedPlan("Mostrador · superficie sólida",root,new Vector3(4.45f,1.10f,-.15f),new Vector3(4.22f,.07f,1.03f),.245f,ivory,true);
            RoundedPlan("Mostrador · zócalo retranqueado",root,new Vector3(4.45f,.075f,-.11f),new Vector3(3.86f,.15f,.74f),.16f,dark,false);
            for(int i=0;i<34;i++)Box("Acanaladura de roble",root,new Vector3(2.64f+i*.11f,.57f,-.619f),new Vector3(.057f,.87f,.032f),oakSlat,false,.008f);
            Box("Remate inferior de latón",root,new Vector3(4.45f,.135f,-.639f),new Vector3(3.72f,.014f,.014f),brass,false,.004f);
            RoundedPlan("Registro accesible · cubierta",root,new Vector3(1.82f,.76f,-.15f),new Vector3(1.13f,.055f,1.02f),.08f,ivory,true);
            Box("Registro accesible · lateral",root,new Vector3(1.30f,.36f,-.15f),new Vector3(.06f,.72f,.90f),oakPanel,true,.012f);
            var badge=Sign(root,"RECEPCIÓN","",new Vector3(4.45f,.75f,-.66f),new Vector2(1.14f,.19f),0,true);
            badge.name="Identificación del mostrador";
            // Luz dirigida al plano de marca; el resto del recinto conserva sus luminarias clínicas.
            var wash=new GameObject("Bañador de pared de recepción").AddComponent<Light>();wash.transform.SetParent(root);
            wash.transform.position=new Vector3(4.35f,2.83f,1.65f);wash.transform.LookAt(new Vector3(4.35f,1.9f,2.86f));
            wash.type=LightType.Spot;wash.spotAngle=105;wash.innerSpotAngle=70;wash.range=3.8f;wash.intensity=.75f;wash.color=new Color(1,.94f,.83f);wash.shadows=LightShadows.None;
            Box("Bandeja de registro",root,new Vector3(5.64f,1.15f,-.06f),new Vector3(.35f,.035f,.25f),brass,false,.014f);
            Box("Formulario de recepción",root,new Vector3(5.64f,1.17f,-.06f),new Vector3(.29f,.006f,.21f),white,false,.004f);
            Rod("Bolígrafo de recepción",root,new Vector3(5.78f,1.179f,-.13f),new Vector3(5.78f,1.179f,.01f),.004f,dark);
            Sign(root,"REGISTRO","Te damos la bienvenida",new Vector3(3.10f,1.225f,-.32f),new Vector2(.47f,.20f),0,true);
        }

        static void ReceptionInformation(Transform root)
        {
            var directory=Group("Directorio junto al pasillo",root,new Vector3(-2.62f,1.61f,2.875f));
            Box("Marco del directorio",directory,Vector3.zero,new Vector3(1.46f,1.40f,.042f),dark,false,.015f);
            Box("Panel del directorio",directory,new Vector3(0,0,-.025f),new Vector3(1.40f,1.34f,.009f),white,false,.008f);
            Text(directory,"SALAS DE PRÁCTICA",new Vector3(0,.49f,-.037f),1.23f,.18f,.093f,Ink);
            string[] labels={"01   Clasificación","02   Esterilización","03   Materiales","04   Procedimientos","05   Radiografía"};
            for(int i=0;i<labels.Length;i++)
            {
                float y=.26f-i*.17f;
                Text(directory,labels[i],new Vector3(-.08f,y,-.037f),1.07f,.13f,.073f,Ink);
                Text(directory,"→",new Vector3(.57f,y,-.037f),.10f,.12f,.08f,Ink);
                if(i<4)Box("Separador del directorio",directory,new Vector3(0,y-.08f,-.032f),new Vector3(1.18f,.003f,.003f),stone,false);
            }
            Sign(root,"BIENVENIDOS","Un espacio para aprender a cuidar",new Vector3(-5.18f,2.14f,2.872f),new Vector2(2.35f,.53f));
            BotanicalPanel(root,new Vector3(-5.70f,1.15f,2.858f),0,.72f);
            BotanicalPanel(root,new Vector3(-4.65f,1.15f,2.858f),0,.72f,true);
            var patient=Group("Información al paciente enmarcada",root,new Vector3(-7.375f,1.73f,-3.4f));patient.localRotation=Quaternion.Euler(0,-90,0);
            Box("Marco de información",patient,Vector3.zero,new Vector3(1.16f,1.22f,.043f),wood,false,.012f);
            Box("Panel marfil",patient,new Vector3(0,0,-.026f),new Vector3(1.10f,1.16f,.01f),white,false);
            ToothEmblem(patient,new Vector3(0,.33f,-.04f),.15f);
            Text(patient,"TU SALUD,\nNUESTRA PRIORIDAD",new Vector3(0,.06f,-.041f),1.03f,.3f,.095f,Ink);
            Text(patient,"Atención con cita\nHigiene en cada procedimiento\nConsulta tus dudas en recepción",new Vector3(0,-.31f,-.041f),1.0f,.34f,.050f,Ink);
            Sign(root,"HIGIENE DE MANOS","Antes y después de cada atención",new Vector3(7.38f,1.72f,-3.3f),new Vector2(1.25f,.47f),90);
            Dispenser(root,new Vector3(7.29f,1.16f,-3.30f),90);
        }

        static void BotanicalPanel(Transform parent,Vector3 position,float yaw,float width,bool reverse=false)
        {
            var panel=Group("Lámina botánica · relieve original",parent,position);panel.localRotation=Quaternion.Euler(0,yaw,0);
            Box("Marco de roble",panel,Vector3.zero,new Vector3(width,.84f,.035f),wood,false,.008f);
            Box("Fondo de papel",panel,new Vector3(0,0,-.022f),new Vector3(width-.042f,.798f,.008f),white,false);
            var art=Material("Verde de lámina botánica",new Color(.45f,.53f,.41f),.04f);
            float flip=reverse?-1:1;
            Rod("Tallo ilustrado",panel,new Vector3(0,-.30f,-.029f),new Vector3(flip*.045f,.26f,-.029f),.004f,art);
            for(int i=0;i<5;i++)
            {
                float direction=(i%2==0?1:-1)*flip;
                var leaf=GameObject.CreatePrimitive(PrimitiveType.Sphere);leaf.name="Hoja de relieve";leaf.transform.SetParent(panel,false);
                leaf.transform.localPosition=new Vector3(direction*.087f,-.20f+i*.105f,-.029f);leaf.transform.localScale=new Vector3(.085f,.20f,.004f);
                leaf.transform.localRotation=Quaternion.Euler(0,0,-direction*48);leaf.GetComponent<Renderer>().sharedMaterial=art;Object.DestroyImmediate(leaf.GetComponent<Collider>());
            }
        }

        static void ToothEmblem(Transform parent,Vector3 position,float scale)
        {
            Vector2[] points={new Vector2(0,.63f),new Vector2(-.36f,.83f),new Vector2(-.68f,.58f),new Vector2(-.69f,.18f),new Vector2(-.49f,-.23f),new Vector2(-.33f,-.86f),new Vector2(-.19f,-.79f),new Vector2(0,-.22f),new Vector2(.19f,-.79f),new Vector2(.33f,-.86f),new Vector2(.49f,-.23f),new Vector2(.69f,.18f),new Vector2(.68f,.58f),new Vector2(.36f,.83f)};
            var line=new GameObject("Emblema dental original").AddComponent<LineRenderer>();line.transform.SetParent(parent,false);line.transform.localPosition=position;
            line.useWorldSpace=false;line.loop=true;line.widthMultiplier=scale*.064f;line.numCornerVertices=4;line.numCapVertices=4;line.alignment=LineAlignment.TransformZ;
            var ink=Material("Tinta de emblema",Ink,.1f);ink.shader=Shader.Find("Universal Render Pipeline/Unlit");line.sharedMaterial=ink;
            var samples=new List<Vector3>();
            for(int i=0;i<points.Length;i++)for(int k=0;k<8;k++)
            {
                float t=k/8f;Vector2 a=points[(i+points.Length-1)%points.Length],b=points[i],c=points[(i+1)%points.Length],d=points[(i+2)%points.Length];
                Vector2 v=.5f*((2*b)+(-a+c)*t+(2*a-5*b+4*c-d)*t*t+(-a+3*b-3*c+d)*t*t*t);
                samples.Add(new Vector3(v.x*scale,v.y*scale,0));
            }
            line.positionCount=samples.Count;line.SetPositions(samples.ToArray());line.shadowCastingMode=ShadowCastingMode.Off;
        }

        static void MakeInteriorDetails()
        {
            var joinery=Group("Acabados y señalización de circulación",architecture);
            var dado=Material("Protección mural · greige",new Color(.76f,.78f,.72f),.19f);
            foreach(int side in new[]{-1,1})foreach(float z in new[]{3f,9f,15f})
            {
                foreach(var segment in new[]{new Vector2(z+.5f,1f),new Vector2(z+4.3f,3.4f)})
                {
                    Box("Protección inferior del pasillo",joinery,new Vector3(side*1.261f,.47f,segment.x),new Vector3(.016f,.81f,segment.y-.025f),dado,false);
                    Box("Remate de protección",joinery,new Vector3(side*1.245f,.897f,segment.x),new Vector3(.03f,.029f,segment.y-.025f),white,false,.005f);
                }
            }
            BotanicalPanel(joinery,new Vector3(0,1.77f,20.89f),0,.85f);
            Text(joinery,"APRENDER A CUIDAR",new Vector3(0,1.17f,20.85f),1.50f,.18f,.070f,Ink);
            var canopy=architecture.GetComponentsInChildren<Transform>().FirstOrDefault(t=>t.name=="Marquesina de acceso");
            if(canopy)canopy.GetComponent<Renderer>().sharedMaterial=OakFinish("Roble · marquesina a escala",new Vector2(3.6f,1.4f));
        }

        static GameObject RoundedPlan(string name,Transform parent,Vector3 position,Vector3 size,float radius,Material material,bool solid)
        {
            string key=$"Planta-redondeada-{size.x:F3}-{size.y:F3}-{size.z:F3}-{radius:F3}";
            string path=Generated+"/"+key+".asset";var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(!mesh)
            {
                var vertices=new List<Vector3>();var normals=new List<Vector3>();var uvs=new List<Vector2>();var triangles=new List<int>();
                const int perimeter=48;float edge=Mathf.Min(.012f,size.y*.25f);
                for(int ring=0;ring<4;ring++)
                {
                    float inset=ring==0||ring==3?edge:0;float y=ring==0?-size.y*.5f:ring==1?-size.y*.5f+edge:ring==2?size.y*.5f-edge:size.y*.5f;
                    for(int k=0;k<perimeter;k++)
                    {
                        int corner=k/12;float angle=(corner*90+(k%12)*90f/11)*Mathf.Deg2Rad;
                        float cx=corner==0||corner==3?size.x*.5f-radius:-size.x*.5f+radius;
                        float cz=corner<2?size.z*.5f-radius:-size.z*.5f+radius;
                        var p=new Vector3(cx+Mathf.Cos(angle)*(radius-inset),y,cz+Mathf.Sin(angle)*(radius-inset));vertices.Add(p);
                        Vector3 n=new Vector3(Mathf.Cos(angle),ring==0?-1:ring==3?1:0,Mathf.Sin(angle));normals.Add(n.normalized);uvs.Add(new Vector2(k/(float)perimeter,ring/3f));
                    }
                }
                for(int ring=0;ring<3;ring++)for(int k=0;k<perimeter;k++)
                {int a=ring*perimeter+k,b=ring*perimeter+(k+1)%perimeter,c=b+perimeter,d=a+perimeter;triangles.AddRange(new[]{a,d,b,b,d,c});}
                for(int side=0;side<2;side++)
                {
                    int start=vertices.Count;float y=(side==0?-1:1)*size.y*.5f;vertices.Add(new Vector3(0,y,0));normals.Add(side==0?Vector3.down:Vector3.up);uvs.Add(Vector2.one*.5f);
                    int source=(side==0?0:3)*perimeter;
                    for(int k=0;k<perimeter;k++){var p=vertices[source+k];vertices.Add(p);normals.Add(side==0?Vector3.down:Vector3.up);uvs.Add(new Vector2(p.x/size.x+.5f,p.z/size.z+.5f));}
                    for(int k=0;k<perimeter;k++){int a=start+1+k,b=start+1+(k+1)%perimeter;triangles.AddRange(side==0?new[]{start,a,b}:new[]{start,b,a});}
                }
                mesh=new Mesh{name=key};mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uvs);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();mesh.RecalculateTangents();AssetDatabase.CreateAsset(mesh,path);
            }
            var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);go.transform.localPosition=position;
            go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<Renderer>().sharedMaterial=material;if(solid)go.AddComponent<BoxCollider>().size=size;return go;
        }
    }
}
