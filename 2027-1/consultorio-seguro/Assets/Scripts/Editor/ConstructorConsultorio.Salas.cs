using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Editor
{
    public static partial class ConstructorConsultorio
    {
        static Vector3 DesplazamientoSala(AccionSimulacion accion) => accion switch
        {
            AccionSimulacion.Clasificacion => new Vector3(-3,0,8),
            AccionSimulacion.Esterilizacion => new Vector3(4,0,8),
            AccionSimulacion.MuestraMateriales => new Vector3(-3,0,14.5f),
            AccionSimulacion.Procedimiento => new Vector3(4,0,14.5f),
            _ => new Vector3(-7,0,21),
        };

        static void MoverMueble(string nombre, AccionSimulacion accion)
        {
            var objeto = GameObject.Find("Mobiliario/" + nombre);
            if (objeto != null) objeto.transform.position += DesplazamientoSala(accion);
        }

        // Se aplica una sola vez, después de construir el mobiliario y sustituir sus mallas.
        static void DistribuirSalas()
        {
            var raiz = Raiz("Salas de la clínica");
            var pared = Acabado("Yeso cálido",new Color(.88f,.87f,.82f),.12f);
            var piso = Acabado("Porcelanato ampliación",new Color(.79f,.81f,.79f),.32f,true,true);
            piso.SetTextureScale("_BaseMap",new Vector2(15,19.5f));
            EditorUtility.SetDirty(piso);
            CajaEntre("Piso ampliación",raiz,new Vector3(-7.5f,-.25f,3),new Vector3(7.5f,0,22.5f),piso);
            CajaEntre("Techo ampliación",raiz,new Vector3(-7.7f,Alto,3),new Vector3(7.7f,Alto+.15f,22.7f),pared);
            foreach (float x in new[]{-7.6f,7.6f})
                Caja("Muro exterior",raiz,new Vector3(x,Alto/2,12.75f),new Vector3(.2f,Alto,19.7f),pared);
            Caja("Muro posterior",raiz,new Vector3(0,Alto/2,22.6f),new Vector3(15.4f,Alto,.2f),pared);
            GameObject.Find("Edificio/Muro fondo").SetActive(false);
            foreach (Transform t in GameObject.Find("Acabados arquitectónicos").transform)
                if (t.name=="Zoclo posterior" || t.name=="Friso posterior") t.gameObject.SetActive(false);
            // El límite anterior estaba detrás del consultorio original.
            var limite = GameObject.Find("Entorno urbano/Límite norte");
            if (limite != null) limite.SetActive(false);
            Limite("Límite posterior ampliado",raiz,new Vector3(-LimiteX,-1,23),new Vector3(LimiteX,8,23.5f));
            Limite("Límite lateral oeste",raiz,new Vector3(-LimiteX-.5f,-1,4),new Vector3(-LimiteX,8,23));
            Limite("Límite lateral este",raiz,new Vector3(LimiteX,-1,4),new Vector3(LimiteX+.5f,8,23));

            var nombres = new[]{"01 · CLASIFICACIÓN","02 · ESTERILIZACIÓN","03 · MATERIALES","04 · PROCEDIMIENTOS","05 · RADIOGRAFÍA","DESCANSO"};
            for(int fila=0;fila<3;fila++)
            {
                float z0=3+fila*6.5f;
                foreach(int lado in new[]{-1,1})
                {
                    int indice=fila*2+(lado>0 ? 1:0);
                    float x=lado*1.25f;
                    float puerta=z0+2.3f;
                    Caja("Tabique frontal",raiz,new Vector3(lado*4.375f,Alto/2,z0),new Vector3(6.25f,Alto,.14f),pared);
                    CajaEntre("Tabique pasillo",raiz,new Vector3(x-.07f,0,z0),new Vector3(x+.07f,Alto,puerta-.85f),pared);
                    CajaEntre("Tabique pasillo",raiz,new Vector3(x-.07f,0,puerta+.85f),new Vector3(x+.07f,Alto,z0+6.5f),pared);
                    Caja("Dintel sala",raiz,new Vector3(x,2.5f,puerta),new Vector3(.14f,.6f,1.7f),pared);
                    var placa=Placa(raiz,nombres[indice],new Vector3(x-lado*.09f,2.47f,puerta),lado*90,new Color(.18f,.39f,.4f),new Vector2(1.65f,.36f));
                    placa.texto.text=nombres[indice];
                    ModeloExterno(raiz,"dental-practice/puerta.obj",new Vector3(lado*1.85f,0,puerta-.86f),new Vector3(.14f,2.17f,1.1f),90);
                    var hoja=Caja("Colisión puerta abierta",raiz,new Vector3(lado*1.85f,1.085f,puerta-.86f),new Vector3(1.1f,2.17f,.14f),pared);
                    hoja.GetComponent<Renderer>().enabled=false;
                    LamparaDeTecho(raiz,new Vector3(lado*4.4f,Alto-.02f,z0+3.3f));
                    SondaDeReflejos(raiz,nombres[indice],new Vector3(lado*4.4f,1.4f,z0+3.25f),new Vector3(6,2.8f,6.3f));
                    var banda=Acabado("Banda salas",new Color(.29f,.46f,.44f),.24f);
                    SinColision(Caja("Protector sanitario",raiz,new Vector3(lado*7.48f,.96f,z0+3.25f),new Vector3(.025f,.15f,6.3f),banda));
                }
                LamparaDeTecho(raiz,new Vector3(0,Alto-.02f,z0+3.25f));
            }

            foreach(var escenario in Object.FindObjectsByType<Escenario>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                escenario.transform.position += DesplazamientoSala(escenario.Datos.accion);
                int fila=escenario.Datos.accion==AccionSimulacion.Clasificacion || escenario.Datos.accion==AccionSimulacion.Esterilizacion ? 0 : escenario.Datos.accion==AccionSimulacion.Radiografia ? 2:1;
                int lado=escenario.Datos.accion==AccionSimulacion.Esterilizacion || escenario.Datos.accion==AccionSimulacion.Procedimiento ? 1:-1;
                var zona=escenario.GetComponentInChildren<ZonaEscenario>();
                var volumen=zona.GetComponent<BoxCollider>();
                volumen.center=new Vector3(lado*4.4f,1.4f,7.75f+fila*6.5f)-zona.transform.position;
                volumen.size=new Vector3(6,2.8f,6.2f);
            }
            MoverMueble("Carrito de curación",AccionSimulacion.Clasificacion);
            MoverMueble("Estante de materiales",AccionSimulacion.MuestraMateriales);
            foreach(string nombre in new[]{"Unidad dental","Paciente (maniquí)","Mesa de instrumental"}) MoverMueble(nombre,AccionSimulacion.Procedimiento);
            foreach(string nombre in new[]{"Mostrador de esterilización","Autoclave","Letrero área de esterilización","Recipiente de punzocortantes"}) MoverMueble(nombre,AccionSimulacion.Esterilizacion);
            MoverMueble("Equipo de rayos X",AccionSimulacion.Radiografia);
            GameObject.Find("Recipientes").transform.position += new Vector3(-4,0,8);
            foreach(Transform t in GameObject.Find("Modelos de terceros").transform)
            {
                if(t.name=="Paciente adaptado") { t.position+=DesplazamientoSala(AccionSimulacion.Procedimiento); continue; }
                if(t.childCount==0) continue;
                var fuente=PrefabUtility.GetCorrespondingObjectFromSource(t.GetChild(0).gameObject);
                string ruta=AssetDatabase.GetAssetPath(fuente);
                Vector3 delta=Vector3.zero;
                if(ruta.Contains("dental-chair") || ruta.Contains("operating-light")) delta=DesplazamientoSala(AccionSimulacion.Procedimiento);
                else if(ruta.Contains("carrito-vacio")) delta=DesplazamientoSala(t.position.x<0 ? AccionSimulacion.Clasificacion : t.position.x<2 ? AccionSimulacion.Procedimiento : AccionSimulacion.Radiografia);
                else if(ruta.Contains("cabinet-run-module")) delta=DesplazamientoSala(AccionSimulacion.MuestraMateriales);
                else if(ruta.Contains("cabinet-run-sink") || ruta.EndsWith("autoclave.obj") || ruta.EndsWith("charola.obj")) delta=DesplazamientoSala(AccionSimulacion.Esterilizacion);
                else if(ruta.Contains("soporte-rayos")) delta=DesplazamientoSala(AccionSimulacion.Radiografia);
                else if(ruta.Contains("sharps-bin") || ruta.Contains("pedal-waste-bin")) delta=t.position.x>0 ? DesplazamientoSala(AccionSimulacion.Esterilizacion):new Vector3(-4,0,8);
                t.position+=delta;
            }
            // Recipientes locales para no llevar residuos desechables por el pasillo.
            foreach(var puesto in new[]{new Vector3(-7.1f,0,10.3f),new Vector3(7.1f,0,10.3f),new Vector3(-7.1f,0,17f)})
                EstacionResiduosSala(raiz,puesto);
            EstacionResiduosSala(raiz,new Vector3(2.1f,0,7.1f));

            var directorio=Placa(raiz,"Directorio de salas",new Vector3(-3.46f,1.6f,0),-90,new Color(.13f,.23f,.26f),new Vector2(2.25f,1.4f));
            directorio.texto.text="SALAS DE PRÁCTICA\n\n01 Clasificación · izquierda\n02 Esterilización · derecha\n03 Materiales · izquierda\n04 Procedimientos · derecha\n05 Radiografía · al fondo";
            foreach(var zona in Object.FindObjectsByType<ZonaMensaje>())
                if(zona.Titulo=="Consultorio") Asignar(zona,"texto","Sigue el pasillo para llegar a las cinco salas de práctica. Cada puerta tiene un número y cada sala su hoja de actividad. Los recipientes están dentro de las salas.");
            // Se deja libre la profundidad de los edificios vecinos y se conecta la ampliación.
            CajaEntre("Piso conexión",raiz,new Vector3(-3.5f,-.25f,1.5f),new Vector3(3.5f,0,3),piso);
            CajaEntre("Techo conexión",raiz,new Vector3(-3.5f,Alto,1.5f),new Vector3(3.5f,Alto+.15f,3),pared);
            foreach(float x in new[]{-3.6f,3.6f}) Caja("Lateral conexión",raiz,new Vector3(x,Alto/2,2.25f),new Vector3(.2f,Alto,1.5f),pared);
            var sillaRadio=ModeloExterno(raiz,"dental-practice/dental-chair-reclined.glb",new Vector3(-6.5f,0,20.1f),new Vector3(0,1.15f,0),180);
            var colSilla=sillaRadio.gameObject.AddComponent<BoxCollider>();
            colSilla.center=new Vector3(0,.575f,0); colSilla.size=new Vector3(.75f,1.15f,2.3f);
            var pacienteRadio=Object.Instantiate(GameObject.Find("Modelos de terceros/Paciente adaptado"),raiz);
            pacienteRadio.name="Paciente radiografía";
            pacienteRadio.transform.localPosition=new Vector3(-6.5f,.1f,20.7f);
            // Área de descanso: reutiliza mobiliario ya incluido en los créditos.
            ColocarModelo(raiz,"chairModernFrameCushion",new Vector3(5,0,21.6f),.95f,180);
            ColocarModelo(raiz,"chairModernFrameCushion",new Vector3(6.2f,0,21.6f),.95f,180);
            ColocarModelo(raiz,"pottedPlant",new Vector3(6.8f,0,17f),1.05f,0);
            foreach(var luz in raiz.GetComponentsInChildren<Light>())
            {
                luz.type=LightType.Spot;
                luz.transform.localRotation=Quaternion.Euler(90,0,0);
                luz.spotAngle=115; luz.innerSpotAngle=85; luz.range=7;
                luz.intensity=3; luz.color=new Color(1,.96f,.89f);
                luz.shadows=LightShadows.Soft; luz.shadowBias=.025f; luz.shadowNormalBias=.15f;
            }
            raiz.position=Vector3.forward*1.5f;
        }

        static void EstacionResiduosSala(Transform raiz,Vector3 inicio)
        {
            int i=0;
            foreach(var destino in new[]{Destino.BolsaRoja,Destino.BolsaAmarilla,Destino.Punzocortantes,Destino.BasuraComun})
            {
                Vector3 p=inicio+Vector3.forward*(i++*.65f);
                bool aguja=destino==Destino.Punzocortantes;
                if(aguja)
                {
                    Caja("Soporte punzocortantes",raiz,p+Vector3.up*.4f,new Vector3(.32f,.8f,.3f),Mat(new Color(.5f,.52f,.55f)));
                    p.y=.8f;
                }
                var grupo=Grupo("Recipiente local "+destino,raiz,p);
                var col=grupo.gameObject.AddComponent<BoxCollider>();
                col.center=Vector3.up*(aguja?.155f:.325f); col.size=aguja?new Vector3(.24f,.31f,.2f):new Vector3(.42f,.65f,.42f);
                Asignar(grupo.gameObject.AddComponent<Contenedor>(),"destino",destino);
                var modelo=ModeloExterno(raiz,"dental-practice/"+(aguja?"sharps-bin.glb":"pedal-waste-bin.glb"),p,aguja?new Vector3(.24f,.31f,.2f):new Vector3(.42f,.65f,.42f));
                foreach(var r in modelo.GetComponentsInChildren<Renderer>()) r.sharedMaterials=r.sharedMaterials.Select(m=>m.name.Contains("yellow") || m.name.Contains("steel")?Acabado("Recipiente_"+destino,destino.Color(),.35f):m).ToArray();
                var etiqueta=Placa(raiz,"Etiqueta "+destino,new Vector3(p.x,p.y+(aguja?.16f:.38f),p.z-.23f),0,destino.Color(),new Vector2(.38f,.18f));
                etiqueta.texto.text=destino.Nombre();
            }
        }
    }
}
