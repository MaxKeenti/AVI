using System;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Rendering.Universal;

namespace ConsultorioSeguroNuevo.Editor
{
    public static partial class ClinicBuilder
    {
        const string Chair="polyhaven-modern-arm-chair-01/modern_arm_chair_01.obj";
        const string Plant="polyhaven-potted-plant-02/potted_plant_02.obj";
        static readonly Color Ink=new Color(.13f,.23f,.21f);
        static void MakeReception()
        {
            var root=Group("Recepción · registro y espera",furniture);
            Box("Fondo de roble",root,new Vector3(4.5f,1.5f,2.88f),new Vector3(5.6f,2.94f,.04f),wood,false);
            for(int i=0;i<20;i++)Box("Junta del revestimiento",root,new Vector3(1.83f+i*.28f,1.5f,2.849f),new Vector3(.008f,2.94f,.004f),dark,false);
            Text(root,"CONSULTORIO SEGURO",new Vector3(4.5f,2.05f,2.815f),4,.5f,.32f,Ink);
            Text(root,"RECEPCIÓN  /  REGISTRO",new Vector3(4.5f,1.67f,2.815f),4,.3f,.11f,Ink);
            Box("Mostrador · cuerpo",root,new Vector3(4.45f,.54f,-.15f),new Vector3(4.1f,1.08f,.91f),sage,true,.055f);
            Box("Mostrador · encimera",root,new Vector3(4.45f,1.10f,-.15f),new Vector3(4.20f,.07f,1.0f),white,true,.022f);
            Box("Mostrador · zócalo retranqueado",root,new Vector3(4.45f,.075f,-.11f),new Vector3(3.95f,.15f,.82f),dark,false);
            for(int i=0;i<31;i++)Box("Estría de roble",root,new Vector3(2.50f+i*.13f,.57f,-.618f),new Vector3(.036f,.88f,.025f),wood,false,.007f);
            Box("Registro accesible · cubierta",root,new Vector3(1.82f,.76f,-.15f),new Vector3(1.13f,.055f,1.02f),white,true,.017f);
            Box("Registro accesible · lateral",root,new Vector3(1.30f,.36f,-.15f),new Vector3(.06f,.72f,.90f),wood);
            Text(root,"REGISTRO",new Vector3(4.45f,.79f,-.65f),3,.3f,.12f,new Color(.96f,.93f,.83f));
            Monitor(root,new Vector3(4.0f,1.135f,.07f),180);
            Box("Teclado",root,new Vector3(4.0f,1.149f,.21f),new Vector3(.38f,.024f,.14f),dark,false,.01f);
            Model(Chair,root,new Vector3(4.0f,0,1.36f),1.02f,180);
            Model(Plant,root,new Vector3(6.70f,0,2.0f),1.35f);
            // Dos grupos de conversación dejan un recorrido frontal de más de dos metros.
            foreach(float z in new[]{-4.7f,-2.5f})
            {
                Model(Chair,root,new Vector3(-6.15f,0,z),1.02f,90);
                Model(Chair,root,new Vector3(-3.36f,0,z),1.02f,-90);
                RoundTable(root,new Vector3(-4.75f,0,z+.08f));
            }
            Model(Chair,root,new Vector3(-5.9f,0,.45f),1.02f,180);
            Model(Chair,root,new Vector3(-4.25f,0,.45f),1.02f,180);
            Model(Plant,root,new Vector3(-6.85f,0,2.25f),1.55f);
            Model(Plant,root,new Vector3(-6.75f,0,-6.1f),1.15f);
            Model(Plant,root,new Vector3(6.65f,0,-5.95f),1.15f);
            Sign(root,"BIENVENIDOS","Aprender a cuidar, en un entorno seguro.",new Vector3(-5.4f,1.9f,2.90f),new Vector2(2.8f,.75f));
            Sign(root,"SALAS DE PRÁCTICA","01 Clasificación    02 Esterilización\n03 Materiales       04 Procedimientos\n05 Radiografía",new Vector3(-2.65f,1.67f,2.90f),new Vector2(2.2f,1.13f));
            Sign(root,"HIGIENE DE MANOS","Antes y después de atender a cada paciente.",new Vector3(7.38f,1.65f,-3.3f),new Vector2(2.0f,.65f),90);
            Dispenser(root,new Vector3(7.29f,1.16f,-3.30f),90);
            Sign(root,"INFORMACIÓN AL PACIENTE","Atención con cita · Recepción al fondo\nTu seguridad forma parte de cada procedimiento.",new Vector3(-7.38f,1.7f,-3.5f),new Vector2(2.4f,.85f),-90);
            foreach(float x in new[]{-6.9f,6.9f})Outlet(root,new Vector3(x,.32f,-1.5f),x<0?-90:90);
            // Luminarias decorativas suspendidas sobre el mostrador, sin invadir la circulación.
            foreach(float x in new[]{3.3f,4.55f,5.8f})
            {Rod("Cable de suspensión",roof,new Vector3(x,2.98f,-.1f),new Vector3(x,2.40f,-.1f),.009f,dark);Cylinder("Pantalla de luz cálida",roof,new Vector3(x,2.35f,-.1f),new Vector3(.35f,.075f,.35f),white);Cylinder("Difusor cálido",roof,new Vector3(x,2.27f,-.1f),new Vector3(.29f,.008f,.29f),emissive);}
            Cabinet(root,new Vector3(6.85f,0,1.35f),1.0f,90,false);
        }
        static void RoundTable(Transform root,Vector3 p)
        {
            Cylinder("Mesa auxiliar · roble",root,p+Vector3.up*.48f,new Vector3(.76f,.025f,.76f),wood,true);
            Cylinder("Base de mesa",root,p+Vector3.up*.22f,new Vector3(.18f,.22f,.18f),dark);
            Cylinder("Pie de mesa",root,p+Vector3.up*.025f,new Vector3(.47f,.022f,.47f),dark);
            Box("Folleto de atención",root,p+new Vector3(.09f,.512f,-.1f),new Vector3(.20f,.012f,.27f),white,false,.003f);
            Box("Banda de folleto",root,p+new Vector3(.09f,.520f,-.13f),new Vector3(.18f,.001f,.05f),sage,false);
        }
        static void Monitor(Transform parent,Vector3 pos,float yaw)
        {
            var t=Group("Puesto informático",parent,pos);t.localRotation=Quaternion.Euler(0,yaw,0);
            Box("Base monitor",t,new Vector3(0,.012f,0),new Vector3(.26f,.025f,.18f),dark,false,.01f);
            Rod("Soporte monitor",t,new Vector3(0,.04f,0),new Vector3(0,.22f,.035f),.022f,metal);
            Box("Monitor",t,new Vector3(0,.34f,.035f),new Vector3(.51f,.31f,.028f),dark,false,.009f);
            var screen=Material("Pantalla clínica",new Color(.09f,.21f,.21f),.3f);screen.EnableKeyword("_EMISSION");screen.SetColor("_EmissionColor",new Color(.06f,.12f,.11f));
            Box("Pantalla",t,new Vector3(0,.34f,.018f),new Vector3(.477f,.274f,.003f),screen,false);
            Text(t,"CONSULTORIO\nSEGURO",new Vector3(0,.35f,.013f),.4f,.25f,.045f,Color.white);
        }
        static void Cabinet(Transform parent,Vector3 pos,float width,float yaw=0,bool sink=false)
        {
            var root=Group(sink?"Gabinete con lavabo":"Gabinete clínico",parent,pos);root.localRotation=Quaternion.Euler(0,yaw,0);
            Box("Cuerpo sanitario",root,new Vector3(0,.45f,0),new Vector3(width,.84f,.59f),white,true,.012f);
            Box("Encimera de superficie sólida",root,new Vector3(0,.9f,0),new Vector3(width+.055f,.055f,.64f),white,true,.012f);
            Box("Zócalo",root,new Vector3(0,.065f,.025f),new Vector3(width-.04f,.13f,.48f),dark,false);
            int count=Mathf.Max(1,Mathf.RoundToInt(width/.60f));
            for(int i=0;i<count;i++)
            {float x=-width*.5f+(i+.5f)*width/count;Box("Frente de puerta",root,new Vector3(x,.48f,-.306f),new Vector3(width/count-.012f,.73f,.018f),white,false,.005f);Rod("Tirador de acero",root,new Vector3(x-.12f,.72f,-.336f),new Vector3(x+.12f,.72f,-.336f),.009f,metal);}
            if(sink)
            {
                // Tarja modelada por separado, con reborde y cuenca oscura visible.
                Box("Reborde tarja",root,new Vector3(0,.935f,0),new Vector3(.55f,.025f,.40f),metal,false,.06f);
                Box("Cuenca",root,new Vector3(0,.949f,-.015f),new Vector3(.43f,.004f,.29f),Material("Interior de tarja",new Color(.36f,.41f,.40f),.55f,.7f),false,.05f);
                Rod("Grifo vertical",root,new Vector3(0,.94f,.245f),new Vector3(0,1.19f,.245f),.022f,metal);
                Rod("Grifo horizontal",root,new Vector3(0,1.19f,.245f),new Vector3(0,1.19f,.04f),.022f,metal);
                Cylinder("Desagüe",root,new Vector3(0,.953f,-.015f),new Vector3(.055f,.004f,.055f),dark);
                SoapBottle(root,new Vector3(width*.36f,.936f,.10f));
            }
        }
        static void Dispenser(Transform parent,Vector3 p,float yaw)
        {var g=Group("Dispensador de higiene",parent,p);g.localRotation=Quaternion.Euler(0,yaw,0);Box("Cuerpo",g,Vector3.zero,new Vector3(.15f,.25f,.12f),white,false,.025f);Box("Ventana de nivel",g,new Vector3(0,0,-.066f),new Vector3(.055f,.14f,.008f),sage,false,.012f);Box("Pulsador",g,new Vector3(0,-.085f,-.095f),new Vector3(.11f,.032f,.065f),metal,false,.008f);}
        static void Outlet(Transform parent,Vector3 p,float yaw)
        {var g=Group("Contacto eléctrico",parent,p);g.localRotation=Quaternion.Euler(0,yaw,0);Box("Placa de contacto",g,Vector3.zero,new Vector3(.085f,.13f,.008f),white,false,.007f);foreach(float x in new[]{-.018f,.018f})Box("Ranura",g,new Vector3(x,0,-.007f),new Vector3(.006f,.025f,.002f),dark,false);}
        static void MakeRooms()
        {
            string[] names={"Clasificación","Esterilización","Materiales","Procedimientos","Radiografía"};
            string[] descriptions={"Identifica cada residuo y elige su destino.\nLee el contexto antes de clasificar.","Distingue instrumental reutilizable y desechable.\nRecorre el flujo de reprocesamiento.","Reconoce los materiales y su condición de uso.\nEl riesgo depende de la contaminación.","Representación de un procedimiento dental.\nSepara los residuos al terminar.","Radiografía digital simulada.\nRetira barreras y clasifica el material."};
            for(int i=0;i<6;i++)
            {
                int side=i%2==0?-1:1;float z=6+(i/2)*6;float x=side*4.42f;
                var root=Group(i<5?$"{i+1:00} · {names[i]}":"06 · Descanso y almacén",furniture);
                Box("Paño salvia de fondo",root,new Vector3(x,1.5f,z+2.895f),new Vector3(5.95f,2.94f,.018f),sage,false);
                Sign(root,i<5?$"{i+1:00}  {names[i].ToUpperInvariant()}":"06  PERSONAL","",new Vector3(side*1.253f,2.13f,z+.65f),new Vector2(1.55f,.40f),side*90);
                // Banderolas de doble cara leíbles desde ambos extremos del pasillo.
                foreach(int direction in new[]{-1,1})Sign(root,i<5?$"{i+1:00}  {names[i]}":"06  Personal","",new Vector3(side*.91f,2.57f,z-1.97f+direction*.023f),new Vector2(.66f,.29f),direction==1?180:0,true);
                var manual=Sign(root,"MANUAL DE CONTROLES","E · Abrir   /   M · Consultar en cualquier momento",new Vector3(side*1.46f,1.5f,z+1.5f),new Vector2(1.7f,.50f),-side*90);manual.AddComponent<ClinicManual>().roomId=i<5?i+1:0;manual.AddComponent<BoxCollider>().size=new Vector3(1.7f,.5f,.08f);
                if(i<5)
                {
                    var room=root.gameObject.AddComponent<PracticeRoom>();room.roomId=i+1;room.title=names[i];room.instructions=descriptions[i];
                    var trigger=Group("Volumen de sala",root,new Vector3(x,1.5f,z)).gameObject.AddComponent<BoxCollider>();trigger.isTrigger=true;trigger.size=new Vector3(5.98f,3,5.85f);room.roomVolume=trigger;rooms.Add(room);
                    Sign(root,$"{i+1:00}  {names[i].ToUpperInvariant()}","",new Vector3(x,2.40f,z+2.855f),new Vector2(2.5f,.34f));
                    Sign(root,$"PRÁCTICA {i+1:00}",descriptions[i],new Vector3(x,1.64f,z-2.885f),new Vector2(2.50f,.75f),180);
                    OutfitRoom(root,i+1,x,z,side);
                }
                else StaffRoom(root,x,z);
                foreach(float dx in new[]{-1.4f,1.4f})Outlet(root,new Vector3(x+dx,.35f,z+2.866f),0);
                Box("Rejilla de extracción",roof,new Vector3(x,2.997f,z+1.6f),new Vector3(.6f,.035f,.36f),white,false);
                for(int k=0;k<8;k++)Box("Lama de ventilación",roof,new Vector3(x,2.975f,z+1.47f+k*.036f),new Vector3(.50f,.018f,.008f),dark,false);
            }
        }
        static void OutfitRoom(Transform root,int id,float x,float z,int side)
        {
            float wallx=side*6.95f;
            Cabinet(root,new Vector3(wallx,0,z+.25f),2.6f,side*90,true);
            Model("dental-practice/glove-and-mask-dispenser.glb",root,new Vector3(side*7.17f,1.25f,z-2.57f),.40f,-side*90,false);
            if(id==1||id==3)
            {
                Cabinet(root,new Vector3(x,.0f,z-.15f),1.9f,0,false);
                Box("Charola de materiales",root,new Vector3(x,.945f,z-.15f),new Vector3(1.58f,.025f,.48f),metal,false,.025f);
                if(id==3)
                {
                    Model("dental-practice/sterile-store-cabinet.glb",root,new Vector3(-1.97f,0,z+2.18f),1.7f,-90);
                    SupplyShelf(root,new Vector3(x-1.75f,1.15f,z+2.66f));
                    Step(root,id,0,"Examinar los materiales","Observa el uso indicado y si hay sangre visible; el color del objeto no determina el destino.",new Vector3(x-.65f,1.0f,z-1.12f),0);
                }
            }
            if(id==2)
            {
                Cabinet(root,new Vector3(x,0,z+2.27f),2.75f);
                Model("dental-practice/autoclave.obj",root,new Vector3(x+.63f,.94f,z+2.28f),.57f,180,false);
                Model("dental-practice/instrument-tray.glb",root,new Vector3(x-.72f,.94f,z+2.13f),.05f,0,false);
                Step(root,id,0,"Recibir instrumental en recipiente cerrado","Separa punzocortantes desechables de instrumental reutilizable. El traslado se representa con un recipiente cerrado.",new Vector3(x-1.2f,1.08f,z+.9f),0);
                Step(root,id,1,"Limpiar, secar e inspeccionar","La limpieza precede a la esterilización. Este simulador no sustituye el protocolo validado del equipo.",new Vector3(wallx-side*.45f,1.05f,z-.5f),side*90);
                Step(root,id,2,"Empacar y comprobar el ciclo","La liberación del instrumental requiere controles del proceso y empaques íntegros. Ciclo abreviado solo para la simulación.",new Vector3(x+.57f,1.3f,z+1.70f),0,2);
            }
            if(id==4||id==5)
            {
                Model("dental-practice/dental-chair-reclined.glb",root,new Vector3(x-.25f,0,z+.1f),1.15f,180);
                if(id==4)Model("dental-practice/operating-light.glb",root,new Vector3(x-.7f,0,z+.9f),2.13f,90,false);
                else
                {
                    Box("Columna de radiografía",root,new Vector3(x-1.55f,.8f,z+.68f),new Vector3(.16f,1.6f,.16f),white,true,.02f);
                    Model("dental-practice/intraoral-xray-arm.glb",root,new Vector3(x-1.55f,1.40f,z+.68f),.40f,90,false);
                }
                Model("tattoo-studio/carrito-vacio.obj",root,new Vector3(x+1.35f,0,z-.55f),.9f);
                Model("dental-practice/instrument-tray.glb",root,new Vector3(x+1.35f,.91f,z-.55f),.045f,0,false);
                Model("dental-practice/sterile-store-cabinet.glb",root,new Vector3(wallx,0,z+2.12f),1.75f,-side*90);
                Monitor(root,new Vector3(wallx,.93f,z+.60f),side*90);
                if(id==4)
                {
                    Step(root,id,0,"Preparar el área de atención","Se representa la preparación de la superficie y de las barreras antes del procedimiento.",new Vector3(x+1.35f,1.17f,z-1.2f),0);
                    Step(root,id,1,"Realizar exploración simulada","La práctica identifica los materiales utilizados; no enseña a ejecutar una intervención clínica real.",new Vector3(x-1.45f,1.18f,z+.40f),0);
                    Step(root,id,2,"Finalizar atención y separar materiales","Caso: gasa saturada de sangre, pieza extraída sin amalgama ni fijador y punzocortante desechable.",new Vector3(x+1.35f,1.2f,z+.6f),0);
                }
                else
                {
                    Step(root,id,0,"Preparar sensor digital y barrera","El sensor es reutilizable; su barrera desechable debe retirarse sin contaminar el equipo.",new Vector3(x+1.35f,1.17f,z-1.2f),0);
                    Step(root,id,1,"Representar la toma radiográfica","No se genera radiación. En la práctica real se aplican los controles y protocolos de protección radiológica.",new Vector3(x-1.45f,1.18f,z+.40f),0,1.5f);
                    Step(root,id,2,"Retirar barreras después del estudio","Caso digital: sin químicos de revelado y sin sangre visible. Evalúa los materiales del caso presentado.",new Vector3(x+1.35f,1.2f,z+.6f),0);
                }
            }
            float binz=id==2?z-2.3f:z+1.95f;
            Bin(root,id,WasteDestination.General,"COMUNES",new Vector3(x-1.5f,0,binz),new Color(.32f,.38f,.36f));
            Bin(root,id,WasteDestination.RedBag,"BOLSA ROJA",new Vector3(x-.63f,0,binz),new Color(.62f,.12f,.10f));
            Bin(root,id,WasteDestination.Sharps,"PUNZOCORTANTES",new Vector3(x+.22f,.0f,binz),new Color(.66f,.12f,.09f),true);
            if(id==4)Bin(root,id,WasteDestination.PathologicalYellow,"PATOLÓGICOS",new Vector3(x+1.1f,0,binz),new Color(.9f,.64f,.13f));
            if(id==2)Bin(root,id,WasteDestination.Sterilization,"REPROCESAMIENTO",new Vector3(x+1.42f,0,z-.1f),new Color(.3f,.48f,.50f));
            AddItems(root,id,x,z);
        }
        static void SupplyShelf(Transform parent,Vector3 pos)
        {var r=Group("Insumos organizados",parent,pos);for(int j=0;j<2;j++){Box("Repisa",r,new Vector3(0,j*.34f,0),new Vector3(.8f,.025f,.28f),wood,false);for(int i=0;i<3;i++){Box("Caja de consumibles",r,new Vector3(-.26f+i*.25f,.13f+j*.34f,-.015f),new Vector3(.21f,.23f,.22f),white,false,.008f);Box("Etiqueta de insumo",r,new Vector3(-.26f+i*.25f,.14f+j*.34f,-.129f),new Vector3(.14f,.042f,.002f),sage,false);}}}
        static void StaffRoom(Transform root,float x,float z)
        {
            Cabinet(root,new Vector3(x+1.1f,0,z+2.55f),2.7f,0,true);
            Model(Chair,root,new Vector3(6.3f,0,z-.35f),1.02f,-90);Model(Chair,root,new Vector3(6.3f,0,z+1.30f),1.02f,-90);RoundTable(root,new Vector3(6.1f,0,z+.48f));
            Model(Plant,root,new Vector3(6.85f,0,16.05f),1.2f);
            for(int i=0;i<3;i++){Box("Casillero de personal",root,new Vector3(2.05f+i*.55f,.95f,z+2.50f),new Vector3(.53f,1.9f,.5f),sage,true,.015f);Rod("Tirador de casillero",root,new Vector3(2.05f+i*.55f,1.1f,z+2.22f),new Vector3(2.05f+i*.55f,.95f,z+2.22f),.011f,metal);}
            Sign(root,"PERSONAL","Descanso · Almacenamiento limpio",new Vector3(x,1.9f,z+2.88f),new Vector2(2.5f,.7f));
        }
        static void Bin(Transform parent,int roomId,WasteDestination destination,string label,Vector3 pos,Color color,bool sharps=false)
        {
            var g=Group(label,parent,pos);var mat=Material("Recipiente "+label,color,.35f);
            Box("Cuerpo",g,new Vector3(0,.33f,0),new Vector3(.49f,.64f,.42f),mat,true,.04f);
            Box("Tapa",g,new Vector3(0,.67f,0),new Vector3(.515f,.065f,.445f),dark,true,.035f);
            if(sharps)Box("Abertura segura",g,new Vector3(0,.706f,0),new Vector3(.12f,.008f,.10f),dark,false,.01f);
            else Box("Pedal",g,new Vector3(0,.025f,-.255f),new Vector3(.19f,.035f,.11f),metal,false,.01f);
            var sign=Sign(g,label,"",new Vector3(0,.39f,-.228f),new Vector2(.45f,.15f));

            var d=g.gameObject.AddComponent<ClinicDestination>();d.roomId=roomId;d.destination=destination;
        }
        static PracticeStation Step(Transform parent,int roomId,int index,string action,string result,Vector3 pos,float yaw,float duration=0)
        {
            var panel=Group($"PASO {index+1} · "+action,parent,pos);panel.localRotation=Quaternion.Euler(0,yaw,0);
            Box("Panel compacto de práctica",panel,Vector3.zero,new Vector3(.54f,.24f,.025f),white,false,.012f);
            Text(panel,$"PASO {index+1}",new Vector3(0,.057f,-.019f),.51f,.08f,.067f,Ink);
            Text(panel,action,new Vector3(0,-.036f,-.019f),.51f,.09f,.038f,Ink);
            Rod("Pedestal del panel",parent,pos+Vector3.down*.13f,new Vector3(pos.x,.035f,pos.z),.009f,metal);
            Box("Pie del panel",parent,new Vector3(pos.x,.015f,pos.z),new Vector3(.22f,.025f,.19f),metal,false,.01f);
            var collider=panel.gameObject.AddComponent<BoxCollider>();collider.size=new Vector3(.56f,.26f,.08f);
            var step=panel.gameObject.AddComponent<PracticeStation>();step.roomId=roomId;step.stepIndex=index;step.actionText=action;step.completionText=result;step.duration=duration;step.showOutline=false;return step;
        }
        static void SoapBottle(Transform parent,Vector3 pos)
        {
            var bottle=Group("Jabón clínico sobre encimera",parent,pos);
            Box("Envase",bottle,new Vector3(0,.083f,0),new Vector3(.09f,.166f,.075f),white,false,.019f);
            Cylinder("Cuello dosificador",bottle,new Vector3(0,.18f,0),new Vector3(.027f,.015f,.027f),dark);
            Box("Bomba",bottle,new Vector3(0,.203f,-.018f),new Vector3(.026f,.016f,.065f),dark,false,.006f);
            Box("Etiqueta",bottle,new Vector3(0,.087f,-.039f),new Vector3(.057f,.061f,.002f),sage,false);
        }

        static void AddItems(Transform root,int id,float x,float z)
        {
            Vector3 center=(id==4||id==5)?new Vector3(x+1.35f,.99f,z-.55f):id==2?new Vector3(x-.68f,1.02f,z+2.12f):new Vector3(x,.99f,z-.15f);
            if(id==1)
            {
                Item(root,id,"Gasa saturada de sangre","Gasa empapada y goteando sangre del procedimiento.",WasteDestination.RedBag,"field-medicine/gauze-stack.glb",center+Vector3.left*.52f,.07f);
                Item(root,id,"Aguja usada","Aguja hipodérmica desechable usada, sin volver a tapar.",WasteDestination.Sharps,"syringe-jtoastie/aguja.obj",center,.025f);
                Item(root,id,"Vaso sin sangre visible","Vaso del paciente con saliva, sin sangre visible.",WasteDestination.General,"paper-cup/cup.glb",center+Vector3.right*.53f,.12f);
            }
            else if(id==2)
            {
                Item(root,id,"Espejo reutilizable","Espejo dental reutilizable; se separa de los desechables para su reprocesamiento.",WasteDestination.Sterilization,"dental-practice/espejo.obj",center+Vector3.left*.25f,.03f);
                Item(root,id,"Hoja desechable usada","Hoja de bisturí de un solo uso; no se reprocesa.",WasteDestination.Sharps,"scalpel-google/hoja.obj",center+Vector3.right*.25f,.022f);
            }
            else if(id==3)
            {
                Item(root,id,"Algodón limpio","Rollos de algodón descartados sin uso y sin contaminación.",WasteDestination.General,"dental-practice/algodon.obj",center+Vector3.left*.50f,.055f);
                Item(root,id,"Guantes sin sangre visible","Guantes de revisión sin sangre visible; el caso no incluye contaminación especial adicional.",WasteDestination.General,"gallery-gloves/guantes.obj",center,.045f);
                Item(root,id,"Aguja de un solo uso","Aguja desechable usada durante la atención.",WasteDestination.Sharps,"syringe-jtoastie/aguja.obj",center+Vector3.right*.50f,.025f);
            }
            else if(id==4)
            {
                Item(root,id,"Gasa de extracción","Gasa saturada de sangre después del procedimiento.",WasteDestination.RedBag,"field-medicine/gauze-stack.glb",center+Vector3.left*.25f,.055f);
                Item(root,id,"Pieza dental extraída","Pieza dental extraída sin amalgama ni conservador químico.",WasteDestination.PathologicalYellow,"tooth-sugamo/tooth.glb",center,.05f);
                Item(root,id,"Hoja de bisturí usada","Punzocortante desechable utilizado durante la atención.",WasteDestination.Sharps,"scalpel-google/hoja.obj",center+Vector3.right*.25f,.024f);
            }
            else
            {
                Item(root,id,"Barrera del sensor","Barrera desechable de radiografía digital: saliva, sin sangre visible.",WasteDestination.General,"tattoo-studio/hoja-barrera.obj",center+Vector3.left*.22f,.015f);
                Item(root,id,"Guantes de radiografía","Guantes del estudio digital sin sangre visible. No se utilizaron químicos de revelado.",WasteDestination.General,"gallery-gloves/guantes.obj",center+Vector3.right*.18f,.045f);
            }
        }
        static void Item(Transform root,int id,string label,string description,WasteDestination destination,string model,Vector3 pos,float height)
        {
            var t=Model(model,root,pos,height,0,false);t.name=label;
            var initial=t.GetComponentsInChildren<Renderer>();Bounds dimensions=initial[0].bounds;foreach(var renderer in initial.Skip(1))dimensions.Encapsulate(renderer.bounds);
            float longest=Mathf.Max(dimensions.size.x,Mathf.Max(dimensions.size.y,dimensions.size.z));
            float desired=model.Contains("aguja")?.09f:model.Contains("hoja.obj")?.045f:model.Contains("espejo")?.18f:model.Contains("guantes")?.22f:model.Contains("hoja-barrera")?.15f:model.Contains("algodon")?.08f:longest;
            if(longest>0){float factor=desired/longest;foreach(Transform child in t){child.localScale*=factor;child.localPosition*=factor;}}
            var collider=t.gameObject.AddComponent<BoxCollider>();var rs=t.GetComponentsInChildren<Renderer>();Bounds b=rs[0].bounds;foreach(var r in rs.Skip(1))b.Encapsulate(r.bounds);collider.center=t.InverseTransformPoint(b.center);collider.size=Vector3.Max(b.size,new Vector3(.14f,.10f,.14f));
            var item=t.gameObject.AddComponent<ClinicItem>();item.roomId=id;item.displayName=label;item.description=description;item.correctDestination=destination;
        }
        static void MakePlayer()
        {
            var root=Group("06 · Simulación y visitante");
            var game=root.gameObject.AddComponent<ClinicGame>();game.rooms=rooms.ToArray();
            var player=Group("Visitante",root,new Vector3(0,.05f,-9));var controller=player.gameObject.AddComponent<CharacterController>();controller.height=1.75f;controller.center=new Vector3(0,.90f,0);controller.radius=.27f;controller.stepOffset=.20f;controller.skinWidth=.025f;
            var p=player.gameObject.AddComponent<ClinicPlayer>();p.game=game;
            var camera=Group("Vista en primera persona",player,new Vector3(0,1.65f,0)).gameObject.AddComponent<Camera>();camera.tag="MainCamera";camera.nearClipPlane=.035f;camera.farClipPlane=180;camera.fieldOfView=65;camera.allowHDR=true;camera.gameObject.AddComponent<AudioListener>();p.eye=camera;
            var data=camera.GetUniversalAdditionalCameraData();data.renderPostProcessing=true;data.antialiasing=AntialiasingMode.SubpixelMorphologicalAntiAliasing;data.antialiasingQuality=AntialiasingQuality.High;
            game.player=p;game.outlineShader=Shader.Find("ConsultorioSeguro/Silueta");
            string credits="CREDITOS.md";game.credits=File.Exists(credits)?File.ReadAllText(credits):"Consultorio Seguro · Equipo 7 · UPIICSA\nModelos y texturas: Poly Haven (CC0), 3D Assets (CC0), Poly by Google, J-Toastie y sugamo (CC BY 3.0).\nConsulta CREDITOS.md para enlaces y modificaciones.";
        }
    }
}
