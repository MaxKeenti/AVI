using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace ConsultorioSeguroNuevo.Editor
{
    public static partial class ClinicBuilder
    {
        static void MakeCity()
        {
            var paving=Material("Banqueta · concreto lavado",new Color(.58f,.58f,.55f),.20f);
            var asphalt=Material("Calle · asfalto grafito",new Color(.15f,.17f,.18f),.13f);
            Pbr(paving,"polyhaven-beige-wall-001","beige_wall_001",.25f);paving.SetTextureScale("_BaseMap",new Vector2(30,5));
            Pbr(asphalt,"polyhaven-beige-wall-001","beige_wall_001",.40f);asphalt.SetTextureScale("_BaseMap",new Vector2(28,2));
            var marking=Material("Señalización vial blanca",new Color(.83f,.82f,.76f),.18f);
            var earth=Material("Tierra de jardineras",new Color(.12f,.105f,.075f),.1f);
            var facadeIvory=Material("Fachada · piedra marfil",new Color(.72f,.70f,.64f),.24f);
            var facadeSand=Material("Fachada · concreto arena",new Color(.57f,.54f,.47f),.19f);
            var facadeGrey=Material("Fachada · concreto cálido",new Color(.48f,.49f,.47f),.23f);
            var facadeClay=Material("Fachada · cerámica terracota",new Color(.45f,.29f,.22f),.25f);
            var urban=Group("Avenida y espacio público",city);
            Box("Terreno urbano",urban,new Vector3(0,-.34f,-2),new Vector3(112,.30f,82),paving);
            Box("Calzada de dos carriles",urban,new Vector3(0,-.20f,-15),new Vector3(112,.16f,8),asphalt);
            Box("Banqueta del consultorio",urban,new Vector3(0,-.09f,-9),new Vector3(112,.18f,4),paving);
            Box("Banqueta opuesta",urban,new Vector3(0,-.09f,-20.5f),new Vector3(112,.18f,3),paving);
            foreach(int side in new[]{-1,1})
                Box("Paso lateral de mantenimiento",urban,new Vector3(side*9,-.09f,9),new Vector3(3,.18f,32),paving);
            // El cruce elevado mantiene el acceso y ambas aceras a la misma cota.
            Box("Cruce peatonal elevado a nivel de acera",urban,new Vector3(0,-.07f,-15),new Vector3(4,.14f,8.05f),paving,true,.055f);
            for(int i=0;i<10;i++)
                Box("Franja de paso peatonal",urban,new Vector3(0,.008f,-11.4f-i*.79f),new Vector3(3.15f,.008f,.39f),marking,false);
            foreach(float z in new[]{-11f,-19f})
            foreach(int side in new[]{-1,1})
                Box("Guarnición de concreto",urban,new Vector3(side*28.9f,-.025f,z),new Vector3(53.6f,.19f,.18f),stone);
            for(int i=-17;i<=17;i++)
            {
                float x=i*3.1f;
                if(Mathf.Abs(x)>4)Box("Línea discontinua central",urban,new Vector3(x,-.113f,-15),new Vector3(1.5f,.009f,.09f),marking,false);
                Box("Junta de banqueta",urban,new Vector3(x,.003f,-9),new Vector3(.012f,.004f,3.85f),stone,false);
                Box("Junta de banqueta opuesta",urban,new Vector3(x,.003f,-20.5f),new Vector3(.012f,.004f,2.85f),stone,false);
            }
            foreach(float x in new[]{-6.8f,-3.4f,3.4f,6.8f})
            {
                Cylinder("Bolardo de protección peatonal",urban,new Vector3(x,.40f,-10.6f),new Vector3(.12f,.40f,.12f),dark,true);
                Cylinder("Banda reflectante",urban,new Vector3(x,.63f,-10.6f),new Vector3(.124f,.025f,.124f),marking);
            }
            CityBench(urban,new Vector3(-5.45f,0,-9.65f));
            CityBench(urban,new Vector3(5.45f,0,-9.65f));
            foreach(float x in new[]{-8.4f,8.4f})
            {
                var planter=Group("Jardinera urbana",urban,new Vector3(x,0,-8.6f));
                Box("Macetero de concreto",planter,new Vector3(0,.31f,0),new Vector3(.92f,.62f,.92f),facadeGrey,true,.035f);
                Box("Sustrato",planter,new Vector3(0,.627f,0),new Vector3(.78f,.035f,.78f),earth,false);
                Model("polyhaven-potted-plant-02/potted_plant_02.obj",planter,new Vector3(0,.10f,0),1.58f,x<0?20:160,false);
            }
            foreach(float x in new[]{-13f,13f,33f,-33f})CityLamp(urban,new Vector3(x,0,-10.4f));
            foreach(float x in new[]{-8f,8f})CityLamp(urban,new Vector3(x,0,-19.5f),180);
            var way=Group("Señal de zona peatonal",urban,new Vector3(3.2f,0,-20.4f));
            Cylinder("Poste",way,new Vector3(0,1.28f,0),new Vector3(.07f,1.28f,.07f),dark);
            Sign(way,"ZONA PEATONAL","VELOCIDAD MÁXIMA 20 km/h",new Vector3(0,2.21f,0),new Vector2(1.65f,.51f),180);
            var avenue=Group("Nomenclatura de avenida",urban,new Vector3(-10.8f,0,-7.2f));
            Cylinder("Poste de nomenclatura",avenue,new Vector3(0,1.49f,.06f),new Vector3(.08f,1.49f,.08f),dark,true);
            Sign(avenue,"AVENIDA DE LA SALUD","COLONIA JARDÍN",new Vector3(0,2.8f,0),new Vector2(2.45f,.5f),0,true);
            // La torre comparte la raíz desmontable de techos para inspección cenital.
            CityBuilding("Torre del consultorio · nueve plantas",roof,new Vector3(0,3.25f,-2),14.4f,9.4f,9,facadeIvory,0,true);
            // Se conservan al menos tres metros libres frente a las ventanas laterales.
            CityBuilding("Edificio residencial de esquina",city,new Vector3(-16.2f,0,1),10.8f,16,7,facadeSand,0,false);
            CityBuilding("Edificio de oficinas vecino",city,new Vector3(17.5f,0,3),13.8f,20,8,facadeGrey,0,false);
            CityBuilding("Viviendas del patio oeste",city,new Vector3(-18.5f,0,24),13,15,4,facadeIvory,0,false);
            CityBuilding("Oficinas del patio este",city,new Vector3(18.5f,0,26),14,16,5,facadeSand,0,false);
            CityBuilding("Edificio residencial occidental",city,new Vector3(-31.5f,0,2),14,18,5,facadeIvory,0,false);
            CityBuilding("Edificio comercial oriental",city,new Vector3(34,0,3),14,20,6,facadeSand,0,false);
            CityBuilding("Farmacia y viviendas",city,new Vector3(-15,0,-30),13.6f,15,5,facadeClay,180,false);
            CityBuilding("Cafetería y oficinas",city,new Vector3(0,0,-30.8f),12.8f,16,7,facadeIvory,180,false);
            CityBuilding("Librería y viviendas",city,new Vector3(15.6f,0,-30),14.2f,15,6,facadeGrey,180,false);
            CityBuilding("Bloque al fondo oeste",city,new Vector3(-34,0,-32),18,19,8,facadeSand,180,false);
            CityBuilding("Bloque al fondo este",city,new Vector3(35,0,-32),17,19,5,facadeClay,180,false);
            Sign(city,"FARMACIA","SALUD Y BIENESTAR",new Vector3(-15,2.58f,-22.47f),new Vector2(4.2f,.55f),180,true);
            Sign(city,"CAFETERÍA JARDÍN","CAFÉ · PAN · DESAYUNOS",new Vector3(0,2.58f,-22.76f),new Vector2(4.1f,.55f),180,true);
            Sign(city,"LIBRERÍA","LIBROS Y PAPELERÍA",new Vector3(15.6f,2.58f,-22.47f),new Vector2(4.2f,.55f),180,true);
            foreach(float x in new[]{-15f,15.6f})CityBench(urban,new Vector3(x,0,-20.4f),180);
            CityBicycleRack(urban,new Vector3(10.2f,0,-8.5f));
            foreach(var renderer in city.GetComponentsInChildren<Renderer>())
                GameObjectUtility.SetStaticEditorFlags(renderer.gameObject,StaticEditorFlags.BatchingStatic|StaticEditorFlags.OccluderStatic|StaticEditorFlags.OccludeeStatic);
        }

        static void CityBuilding(string name,Transform parent,Vector3 position,float width,float depth,int floors,Material facade,float yaw,bool tower)
        {
            var b=Group(name,parent,position);b.localRotation=Quaternion.Euler(0,yaw,0);
            float h=floors*3.1f;
            var urbanGlass=Material("Vidrio urbano con reflejo gris azulado",new Color(.18f,.25f,.28f),.88f,.38f);
            var blind=Material("Estores de oficinas",new Color(.52f,.53f,.50f),.22f);
            Box("Volumen de mampostería",b,new Vector3(0,h*.5f,0),new Vector3(width,h,depth),facade);
            Box("Zócalo de piedra natural",b,new Vector3(0,.24f,0),new Vector3(width+.08f,.48f,depth+.08f),stone);
            int bays=Mathf.Max(3,Mathf.FloorToInt(width/2.4f));float pitch=(width-.9f)/bays;
            for(int floor=0;floor<floors;floor++)
            {
                float level=floor*3.1f;
                Box("Banda horizontal de forjado",b,new Vector3(0,level+.13f,-depth*.5f-.045f),new Vector3(width+.10f,.18f,.16f),stone,false);
                Box("Banda horizontal lateral izquierda",b,new Vector3(-width*.5f-.035f,level+.13f,0),new Vector3(.12f,.18f,depth),stone,false);
                Box("Banda horizontal lateral derecha",b,new Vector3(width*.5f+.035f,level+.13f,0),new Vector3(.12f,.18f,depth),stone,false);
                for(int col=0;col<bays;col++)
                {
                    float x=(col-(bays-1)*.5f)*pitch;float windowWidth=pitch-.50f;
                    if(floor==0 && col==bays/2 && !tower)
                        CityEntry(b,new Vector3(x,0,-depth*.5f-.015f),Mathf.Min(windowWidth,1.65f),urbanGlass);
                    else CityWindow(b,new Vector3(x,level+1.65f,-depth*.5f-.015f),new Vector2(windowWidth,1.91f),urbanGlass,0,(col+floor)%4==0?blind:null);
                }
                int sideBays=Mathf.Max(2,Mathf.FloorToInt(depth/3.2f));float sidePitch=(depth-.9f)/sideBays;
                foreach(int side in new[]{-1,1})for(int col=0;col<sideBays;col++)
                {
                    float z=(col-(sideBays-1)*.5f)*sidePitch;
                    CityWindow(b,new Vector3(side*(width*.5f+.015f),level+1.65f,z),new Vector2(sidePitch-.62f,1.91f),urbanGlass,-side*90,(floor+col)%5==0?blind:null);
                }
                if(floor>0 && floor%3==0 && !tower)
                {
                    for(int col=0;col<bays;col+=2)
                    {
                        float x=(col-(bays-1)*.5f)*pitch;
                        Box("Balcón · losa",b,new Vector3(x,level+.41f,-depth*.5f-.45f),new Vector3(pitch-.22f,.13f,.80f),stone,false);
                        Rod("Balcón · pasamanos",b,new Vector3(x-pitch*.42f,level+1.4f,-depth*.5f-.82f),new Vector3(x+pitch*.42f,level+1.4f,-depth*.5f-.82f),.019f,dark);
                        for(int k=0;k<7;k++)Rod("Balcón · barrote",b,new Vector3(x-pitch*.42f+k*pitch*.14f,level+.46f,-depth*.5f-.82f),new Vector3(x-pitch*.42f+k*pitch*.14f,level+1.4f,-depth*.5f-.82f),.011f,dark);
                    }
                }
            }
            for(int col=0;col<=bays;col++)
            {
                float x=-width*.5f+.24f+col*(width-.48f)/bays;
                Box("Aleta vertical de fachada",b,new Vector3(x,h*.5f,-depth*.5f-.12f),new Vector3(.085f,h-.10f,.28f),tower?white:stone,false);
            }
            Box("Cornisa superior",b,new Vector3(0,h+.10f,0),new Vector3(width+.32f,.20f,depth+.32f),stone,false);
            Box("Pretil frontal",b,new Vector3(0,h+.48f,-depth*.5f+.1f),new Vector3(width,.68f,.2f),facade,false);
            foreach(int side in new[]{-1,1})Box("Pretil lateral",b,new Vector3(side*(width*.5f-.1f),h+.48f,0),new Vector3(.2f,.68f,depth),facade,false);
            Box("Cuarto de instalaciones",b,new Vector3(width*.15f,h+.95f,depth*.20f),new Vector3(width*.30f,1.70f,depth*.25f),facadeGreyForCity(),false);
            for(int i=0;i<3;i++)
            {
                Box("Equipo de ventilación de azotea",b,new Vector3(-width*.25f+i*.9f,h+.32f,depth*.23f),new Vector3(.70f,.45f,.90f),metal,false,.015f);
                for(int k=0;k<5;k++)Box("Rejilla de equipo",b,new Vector3(-width*.25f+i*.9f,h+.18f+k*.055f,depth*.23f-.458f),new Vector3(.54f,.018f,.008f),dark,false);
            }
        }

        static void CityEntry(Transform parent,Vector3 position,float width,Material pane)
        {
            var entry=Group("Acceso de edificio vecino",parent,position);
            Box("Receso oscuro de acceso",entry,new Vector3(0,1.27f,-.028f),new Vector3(width+.15f,2.54f,.045f),dark,false);
            Box("Puerta acristalada",entry,new Vector3(0,1.24f,-.065f),new Vector3(width,2.45f,.022f),pane,false);
            foreach(int side in new[]{-1,1})
                Box("Marco vertical de acceso",entry,new Vector3(side*(width*.5f-.022f),1.26f,-.098f),new Vector3(.045f,2.52f,.06f),dark,false);
            Box("Travesaño de acceso",entry,new Vector3(0,2.50f,-.098f),new Vector3(width,.045f,.06f),dark,false);
            Box("Umbral a nivel de banqueta",entry,new Vector3(0,.022f,-.16f),new Vector3(width+.15f,.044f,.28f),stone,false);
            Rod("Tirador vertical de acceso",entry,new Vector3(width*.28f,.89f,-.15f),new Vector3(width*.28f,1.36f,-.15f),.016f,metal);
        }

        static Material facadeGreyForCity()=>Material("Fachada · instalaciones",new Color(.48f,.49f,.47f),.2f);

        static void CityWindow(Transform parent,Vector3 position,Vector2 size,Material pane,float yaw,Material shutter)
        {
            var w=Group("Hueco de ventana",parent,position);w.localRotation=Quaternion.Euler(0,yaw,0);
            Box("Sombra del marco",w,new Vector3(0,0,-.028f),new Vector3(size.x+.15f,size.y+.15f,.038f),dark,false);
            Box("Paño reflectante",w,new Vector3(0,0,-.060f),new Vector3(size.x,size.y,.018f),pane,false);
            Box("Montante de ventana",w,new Vector3(0,0,-.095f),new Vector3(.035f,size.y,.065f),dark,false);
            Box("Vierteaguas",w,new Vector3(0,-size.y*.5f-.07f,-.10f),new Vector3(size.x+.20f,.06f,.24f),stone,false);
            if(shutter)Box("Estor parcialmente recogido",w,new Vector3(-size.x*.255f,size.y*.33f,-.076f),new Vector3(size.x*.47f,size.y*.29f,.01f),shutter,false);
        }

        static void CityBench(Transform parent,Vector3 position,float yaw=0)
        {
            var b=Group("Banco de descanso",parent,position);b.localRotation=Quaternion.Euler(0,yaw,0);
            foreach(float x in new[]{-.77f,.77f})
            {
                Box("Soporte de banco",b,new Vector3(x,.235f,0),new Vector3(.065f,.47f,.43f),dark,true,.01f);
                Rod("Soporte del respaldo",b,new Vector3(x,.40f,.13f),new Vector3(x,.94f,.21f),.024f,dark);
                Rod("Apoyabrazos",b,new Vector3(x,.68f,-.22f),new Vector3(x,.68f,.17f),.022f,dark);
            }
            for(int i=0;i<5;i++)Box("Listón del asiento",b,new Vector3(0,.46f,-.20f+i*.105f),new Vector3(1.92f,.045f,.090f),wood,true,.008f);
            for(int i=0;i<3;i++)Box("Listón del respaldo",b,new Vector3(0,.69f+i*.106f,.19f+i*.018f),new Vector3(1.92f,.090f,.045f),wood,true,.008f);
        }

        static void CityLamp(Transform parent,Vector3 position,float yaw=0)
        {
            var lamp=Group("Luminaria urbana",parent,position);lamp.localRotation=Quaternion.Euler(0,yaw,0);
            Cylinder("Base de luminaria",lamp,new Vector3(0,.11f,0),new Vector3(.31f,.11f,.31f),dark,true);
            Cylinder("Columna de luminaria",lamp,new Vector3(0,2.20f,0),new Vector3(.09f,2.2f,.09f),dark,true);
            Rod("Brazo de luminaria",lamp,new Vector3(0,4.25f,0),new Vector3(0,4.40f,-.69f),.041f,dark);
            Box("Cabezal LED",lamp,new Vector3(0,4.38f,-.77f),new Vector3(.25f,.065f,.63f),dark,false,.015f);
            Box("Difusor de luminaria urbana",lamp,new Vector3(0,4.343f,-.77f),new Vector3(.20f,.015f,.50f),emissive,false);
        }

        static void CityBicycleRack(Transform parent,Vector3 position)
        {
            var rack=Group("Aparcabicicletas",parent,position);
            for(int i=0;i<3;i++)
            {
                float x=i*.6f;
                Rod("Soporte vertical",rack,new Vector3(x,0,-.3f),new Vector3(x,.76f,-.3f),.025f,metal);
                Rod("Soporte vertical",rack,new Vector3(x,0,.3f),new Vector3(x,.76f,.3f),.025f,metal);
                Rod("Arco de apoyo",rack,new Vector3(x,.76f,-.3f),new Vector3(x,.76f,.3f),.025f,metal);
            }
        }
    }
}
