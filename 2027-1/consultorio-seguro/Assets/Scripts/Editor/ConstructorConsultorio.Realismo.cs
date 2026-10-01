using UnityEngine;
using UnityEditor;

namespace ConsultorioSeguro.Editor
{
    public static partial class ConstructorConsultorio
    {
        static void AfinarInterior()
        {
            var raiz=Raiz("Detalles interiores");
            var zoclo=Acabado("Zoclo sanitario",new Color(.68f,.72f,.71f),.28f);
            var marco=Acabado("Carpintería blanca",new Color(.83f,.85f,.83f),.32f);
            var metal=Acabado("Aluminio cepillado",new Color(.55f,.59f,.61f),.48f);
            metal.SetFloat("_Metallic",.75f);
            EditorUtility.SetDirty(metal);
            foreach(var luz in GameObject.Find("Salas de la clínica").GetComponentsInChildren<Light>())
                Object.DestroyImmediate(luz.gameObject);
            foreach(Transform t in GameObject.Find("Salas de la clínica").transform)
                if(t.name=="Lámpara de techo") t.GetComponent<Renderer>().enabled=false;

            for(int fila=0;fila<3;fila++)
            {
                float inicio=4.5f+fila*6.5f;
                foreach(int lado in new[]{-1,1})
                {
                    float x=lado*4.375f;
                    // Juntas de encuentro, zoclo y marcos a escala de construcción.
                    foreach(float z in new[]{inicio+.09f,inicio+6.41f})
                        SinColision(Caja("Zoclo transversal",raiz,new Vector3(x,.065f,z),new Vector3(6.1f,.13f,.035f),zoclo));
                    SinColision(Caja("Zoclo exterior",raiz,new Vector3(lado*7.48f,.065f,inicio+3.25f),new Vector3(.035f,.13f,6.3f),zoclo));
                    foreach(float z in new[]{inicio+1.44f,inicio+3.16f})
                        SinColision(Caja("Jamba de puerta",raiz,new Vector3(lado*1.25f,1.11f,z),new Vector3(.23f,2.22f,.07f),marco));
                    SinColision(Caja("Marco superior",raiz,new Vector3(lado*1.25f,2.2f,inicio+2.3f),new Vector3(.23f,.07f,1.79f),marco));
                    foreach(float z in new[]{inicio+1.65f,inicio+4.85f})
                        PanelInterior(raiz,new Vector3(x,2.73f,z));
                    var relleno=new GameObject("Luz ambiente de sala").AddComponent<Light>();
                    relleno.transform.SetParent(raiz,false);
                    relleno.transform.position=new Vector3(x,1.9f,inicio+3.25f);
                    relleno.type=LightType.Point; relleno.range=4.5f; relleno.intensity=.85f;
                    relleno.color=new Color(1,.98f,.94f);
                    relleno.shadows=LightShadows.None;
                    relleno.shadowBias=.015f; relleno.shadowNormalBias=.1f;
                    // Rejilla de retorno de aire, sin invadir los accesos.
                    SinColision(Caja("Marco ventilación",raiz,new Vector3(x,2.76f,inicio+3.25f),new Vector3(.62f,.035f,.35f),marco));
                    for(int i=0;i<8;i++)
                        SinColision(Caja("Lama ventilación",raiz,new Vector3(x,2.738f,inicio+3.12f+i*.035f),new Vector3(.55f,.012f,.014f),metal));
                }
                PanelInterior(raiz,new Vector3(0,2.73f,inicio+3.25f));
            }
            // Mobiliario adicional de la misma colección acreditada; se conservan las áreas de trabajo.
            MuebleInterior(raiz,"cabinet-run-sink-module",new Vector3(6.9f,0,16.1f),new Vector3(2.1f,.9f,.6f),-90);
            MuebleInterior(raiz,"sterile-store-cabinet",new Vector3(6.9f,0,9.8f),new Vector3(.85f,1.95f,.5f),-90);
            MuebleInterior(raiz,"cabinet-run-module",new Vector3(-4.5f,0,23.6f),new Vector3(1.8f,.9f,.55f),0);
            MuebleInterior(raiz,"cabinet-run-sink-module",new Vector3(-6.9f,0,9.5f),new Vector3(1.8f,.9f,.6f),90);
            foreach(var puesto in new[]{new Vector3(7.43f,1.25f,15.8f),new Vector3(7.43f,1.25f,5.25f),new Vector3(-7.43f,1.25f,5.3f)})
                ModeloExterno(raiz,"dental-practice/glove-and-mask-dispenser.glb",puesto,new Vector3(.55f,.42f,.16f),puesto.x>0?-90:90);
            ColocarModelo(raiz,"sideTable",new Vector3(5.6f,0,22.9f),.55f,0);
        }

        static void MuebleInterior(Transform raiz,string modelo,Vector3 posicion,Vector3 medidas,float giro)
        {
            var mueble=ModeloExterno(raiz,"dental-practice/"+modelo+".glb",posicion,medidas,giro);
            var col=mueble.gameObject.AddComponent<BoxCollider>();
            col.center=Vector3.up*medidas.y*.5f;
            col.size=medidas;
        }

        static void PanelInterior(Transform raiz,Vector3 posicion)
        {
            ModeloExterno(raiz,"dental-practice/ceiling-light-panel.glb",posicion,new Vector3(1.2f,.055f,.6f));
            var difusor=Acabado("Difusor LED",new Color(.92f,.97f,1),.2f);
            difusor.globalIlluminationFlags=MaterialGlobalIlluminationFlags.RealtimeEmissive;
            difusor.EnableKeyword("_EMISSION");
            difusor.SetColor("_EmissionColor",new Color(.92f,.97f,1)*1.5f);
            EditorUtility.SetDirty(difusor);
            SinColision(Caja("Difusor de panel",raiz,posicion+Vector3.down*.006f,new Vector3(1.1f,.008f,.5f),difusor));
            var luz=new GameObject("Iluminación de panel").AddComponent<Light>();
            luz.transform.SetParent(raiz,false);
            luz.transform.position=posicion+Vector3.down*.12f;
            luz.transform.rotation=Quaternion.Euler(90,0,0);
            luz.type=LightType.Spot;
            luz.spotAngle=145; luz.innerSpotAngle=105;
            luz.range=6; luz.intensity=2.4f;
            luz.color=new Color(.96f,.98f,1);
            luz.shadows=LightShadows.Soft;
            luz.shadowBias=.015f; luz.shadowNormalBias=.12f;
        }
    }
}
