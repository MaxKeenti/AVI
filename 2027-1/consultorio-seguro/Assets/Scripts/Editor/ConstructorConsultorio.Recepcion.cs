using UnityEngine;
using UnityEditor;

namespace ConsultorioSeguro.Editor
{
    public static partial class ConstructorConsultorio
    {
        static void MejorarRecepcionYGuias()
        {
            foreach(string ruta in new[]{"Edificio/Muro del consultorio","Edificio/Puerta del consultorio","Recepción/Póster de RPBI","Recepción/Póster de colores","Recepción/Letrero del consultorio"})
                GameObject.Find(ruta)?.SetActive(false);
            foreach(Transform t in GameObject.Find("Modelos de terceros").transform)
            {
                if(t.childCount==0) continue;
                string ruta=AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(t.GetChild(0).gameObject));
                if(ruta.EndsWith("puerta.obj") && t.position.z<0) t.gameObject.SetActive(false);
            }
            var raiz=Raiz("Recepción ampliada y guías");
            var madera=Acabado("Roble recepción",new Color(.55f,.39f,.25f),.25f,true);
            var verde=Acabado("Salvia recepción",new Color(.38f,.49f,.43f),.15f);
            SinColision(Caja("Panel cálido de espera",raiz,new Vector3(-3.47f,1.25f,-.25f),new Vector3(.035f,2.4f,4.8f),verde));
            for(int i=0;i<22;i++)
                SinColision(Caja("Listón de madera",raiz,new Vector3(-3.43f,1.2f,-2.4f+i*.2f),new Vector3(.05f,2.3f,.04f),madera));
            foreach(float z in new[]{-1.8f,-.6f,.6f,1.8f})
            {
                var silla=ColocarModelo(raiz,"chairModernFrameCushion",new Vector3(-2.95f,0,z),.95f,-90);
                var col=silla.gameObject.AddComponent<BoxCollider>();
                col.center=new Vector3(0,.45f,0); col.size=new Vector3(.65f,.9f,.65f);
            }
            ColocarModelo(raiz,"sideTable",new Vector3(-1.95f,0,0),.5f,0);
            ColocarModelo(raiz,"pottedPlant",new Vector3(-2.9f,0,2.65f),1.3f,0);
            ColocarModelo(raiz,"pottedPlant",new Vector3(3,0,2.65f),1.3f,0);
            var bienvenida=Placa(raiz,"Bienvenida recepción",new Vector3(3.46f,1.7f,-.4f),90,new Color(.15f,.27f,.25f),new Vector2(3,1.1f));
            bienvenida.texto.text="<b>BIENVENIDO A LA CLÍNICA</b>\nRegístrate y toma asiento.\nLas salas de práctica están al fondo.\nEsc · Manual de controles";
            var directorio=GameObject.Find("Salas de la clínica/Directorio de salas");
            directorio.transform.position=new Vector3(-2.4f,1.6f,4.39f);
            directorio.transform.rotation=Quaternion.identity;
            directorio.transform.localScale=Vector3.one*.9f;
            // Guías en las paredes frontales: visibles desde el interior, fuera del mobiliario.
            for(int fila=0;fila<3;fila++)
            foreach(int lado in new[]{-1,1})
            {
                float z=4.5f+fila*6.5f;
                var guia=Placa(raiz,"Manual de sala",new Vector3(lado*4.4f,1.55f,z+.1f),180,new Color(.1f,.22f,.25f),new Vector2(2.2f,1.8f));
                guia.texto.text=ManualControles.Pared;
            }
            foreach(var tablero in Object.FindObjectsByType<TableroPractica>(FindObjectsSortMode.None))
            {
                var escenario=tablero.GetComponentInParent<Escenario>();
                var accion=escenario.Datos.accion;
                bool atril=accion==AccionSimulacion.Clasificacion || accion==AccionSimulacion.Procedimiento;
                if (!atril)
                {
                    if (accion==AccionSimulacion.MuestraMateriales)
                        tablero.transform.position=new Vector3(-4.6f,1.75f,17.39f);
                    // Las placas murales permanecen paralelas a su pared.
                    tablero.transform.rotation=Quaternion.Euler(0,accion==AccionSimulacion.Esterilizacion?90:0,0);
                    continue;
                }
                var posicion=tablero.transform.position;
                var entrada=accion==AccionSimulacion.Clasificacion?new Vector3(-1.25f,posicion.y,6.8f):new Vector3(1.25f,posicion.y,13.3f);
                tablero.transform.rotation=Quaternion.LookRotation(posicion-entrada,Vector3.up);
                var frente=tablero.GetComponentInChildren<TMPro.TextMeshPro>();
                var reverso=Object.Instantiate(frente,tablero.transform);
                reverso.name="Texto reverso";
                reverso.transform.localPosition=new Vector3(0,0,.012f);
                reverso.transform.localRotation=Quaternion.Euler(0,180,0);
                Asignar(tablero,"textoReverso",reverso);
            }
        }
    }
}
