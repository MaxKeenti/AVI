using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ConsultorioSeguro.Editor
{
    public static partial class ConstructorConsultorio
    {
        static Transform ModeloExterno(Transform padre, string ruta, Vector3 posicion, Vector3 dimensiones, float giro = 0)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Terceros/" + ruta);
            if (asset == null) throw new InvalidOperationException("Falta " + ruta);
            var pivote = new GameObject("Modelo importado").transform;
            pivote.SetParent(padre, false);
            var instancia = (GameObject)PrefabUtility.InstantiatePrefab(asset, pivote);
            var renderers = instancia.GetComponentsInChildren<Renderer>();
            Bounds limites = renderers[0].bounds;
            foreach (var r in renderers.Skip(1)) limites.Encapsulate(r.bounds);
            Vector3 escala = dimensiones.x > 0 ? new Vector3(dimensiones.x / limites.size.x, dimensiones.y / limites.size.y, dimensiones.z / limites.size.z) : Vector3.one * dimensiones.y / limites.size.y;
            instancia.transform.localScale = escala;
            instancia.transform.localPosition = Vector3.Scale(new Vector3(-limites.center.x, -limites.min.y, -limites.center.z), escala);
            if (ruta.EndsWith(".obj")) foreach (var r in renderers) r.sharedMaterials = r.sharedMaterials.Select(MaterialKenney).ToArray();
            pivote.localPosition = posicion;
            pivote.localRotation = Quaternion.Euler(0, giro, 0);
            return pivote;
        }

        static void Sustituir(Transform raiz, string original, string modelo, Vector3 posicion, Vector3 dimensiones, float giro = 0)
        {
            var objeto = GameObject.Find(original);
            if (objeto == null) { Debug.LogWarning("No se encontró " + original); return; }
            OcultarMallas(objeto);
            ModeloExterno(raiz, modelo, posicion, dimensiones, giro);
        }

        static void AplicarModelosClinicos(Transform raiz)
        {
            Sustituir(raiz, "Mobiliario/Unidad dental", "dental-practice/dental-chair-reclined.glb", new Vector3(0,0,.6f), new Vector3(0,1.15f,0), 180);
            Sustituir(raiz, "Mobiliario/Carrito de curación", "tattoo-studio/carrito-vacio.obj", new Vector3(-1.8f,0,-1), new Vector3(.8f,.85f,.5f));
            Sustituir(raiz, "Mobiliario/Mesa de instrumental", "tattoo-studio/carrito-vacio.obj", new Vector3(1,0,.1f), new Vector3(.8f,.85f,.5f));
            Sustituir(raiz, "Mobiliario/Estante de materiales", "dental-practice/cabinet-run-module.glb", new Vector3(-1.6f,0,2.75f), new Vector3(1.3f,1.06f,.4f),180);
            // Solo se sustituyen las piezas del mostrador: los equipos conservan sus componentes.
            Sustituir(raiz, "Mobiliario/Mostrador de esterilización/Gabinete", "dental-practice/cabinet-run-sink-module.glb", new Vector3(3.15f,0,-1), new Vector3(2.4f,1,.6f),-90);
            var cubierta = GameObject.Find("Mobiliario/Mostrador de esterilización/Cubierta");
            if (cubierta != null) OcultarMallas(cubierta);
            Sustituir(raiz, "Mobiliario/Autoclave", "dental-practice/autoclave.obj", new Vector3(3.15f,.94f,-1.85f), new Vector3(.5f,.4f,.5f),-90);
            ModeloExterno(raiz, "dental-practice/operating-light.glb", new Vector3(-.7f,0,1.8f), new Vector3(0,2.3f,0),180);
            Transform urbano = GameObject.Find("Entorno urbano/Mobiliario urbano")?.transform;
            if (urbano != null) foreach (Transform objeto in urbano)
            {
                string modelo = objeto.name.StartsWith("Árbol") ? "kenney-nature-kit/tree_oak.glb" : objeto.name.StartsWith("Auto") ? "kenney-car-kit/sedan.glb" : objeto.name.StartsWith("Banca") ? "kenney-furniture-kit/bench.obj" : objeto.name.StartsWith("Bote") ? "kenney-furniture-kit/trashcan.obj" : null;
                if (modelo == null) continue;
                OcultarMallas(objeto.gameObject);
                bool auto = objeto.name.StartsWith("Auto");
                ModeloExterno(raiz, modelo, objeto.position, auto ? new Vector3(1.8f,1.4f,4.2f) : objeto.name.StartsWith("Banca") ? new Vector3(1.6f,.85f,.5f) : new Vector3(0,objeto.name.StartsWith("Árbol") ? 5 : .95f,0), auto ? 90 : 0);
            }
            Transform vecinos = GameObject.Find("Entorno urbano/Edificios vecinos")?.transform;
            if (vecinos != null) foreach (Transform objeto in vecinos)
            {
                var volumen = objeto.Find("Volumen")?.GetComponent<Renderer>();
                if (volumen == null) continue;
                Bounds b = volumen.bounds;
                OcultarMallas(objeto.gameObject);
                ModeloExterno(raiz, "kenney-city-kit/building-a.glb", new Vector3(b.center.x,b.min.y,b.center.z),b.size);
            }
            Sustituir(raiz, "Mobiliario/Equipo de rayos X/Mesita del equipo de rayos X", "tattoo-studio/carrito-vacio.obj", new Vector3(2.75f,0,2.5f),new Vector3(.5f,.8f,.4f));
            Sustituir(raiz, "Mobiliario/Mostrador de esterilización/Charola de instrumental sucio", "dental-practice/charola.obj", new Vector3(3.15f,.94f,-.1f),new Vector3(.4f,.025f,.6f));
            var tarja = GameObject.Find("Mobiliario/Mostrador de esterilización/Tarja");
            if (tarja != null) OcultarMallas(tarja);
            var acabados = GameObject.Find("Acabados arquitectónicos");
            if (acabados != null) foreach (Transform t in acabados.transform)
                if (new[]{"Listón de recepción","Frente de cajón","Tirador de cajón","Zócalo de gabinete","Puerta esterilización","Tirador esterilización","Puerta de materiales","Tirador de materiales"}.Contains(t.name)) OcultarMallas(t.gameObject);
            Sustituir(raiz,"Recepción/Mostrador de recepción/Cuerpo","dental-practice/mostrador.obj",new Vector3(2.4f,0,-5.2f),new Vector3(1.9f,1.09f,.6f),180);
            var tapaRecepcion=GameObject.Find("Recepción/Mostrador de recepción/Cubierta");
            if(tapaRecepcion!=null) OcultarMallas(tapaRecepcion);
            Sustituir(raiz,"Edificio/Puerta del consultorio","dental-practice/puerta.obj",new Vector3(.58f,.02f,-3.575f),new Vector3(.14f,2.06f,.95f));
            AplicarPaciente(raiz);
            AplicarInstrumentos(raiz);
            AplicarRayosX(raiz);
            foreach (var contenedor in Object.FindObjectsByType<Contenedor>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                if (contenedor.Destino == Destino.Esterilizacion) continue;
                OcultarMallas(contenedor.gameObject);
                bool aguja = contenedor.Destino == Destino.Punzocortantes;
                var modelo = ModeloExterno(raiz,"dental-practice/"+(aguja ? "sharps-bin.glb" : "pedal-waste-bin.glb"),contenedor.transform.position,new Vector3(aguja ? .24f:.42f,aguja ? .31f:.65f,aguja ? .2f:.42f));
                foreach (var r in modelo.GetComponentsInChildren<Renderer>())
                    r.sharedMaterials = r.sharedMaterials.Select(m => m.name.Contains("yellow") || m.name.Contains("steel") ? Acabado("Recipiente_"+contenedor.Destino,contenedor.Destino.Color(),.35f) : m).ToArray();
            }
            RegistrarFuentesExternas();
        }

        static void AplicarPaciente(Transform raiz)
        {
            OcultarMallas(GameObject.Find("Mobiliario/Paciente (maniquí)"));
            var paciente = new GameObject("Paciente adaptado").transform;
            paciente.SetParent(raiz,false);
            paciente.localPosition = new Vector3(0,.1f,1.2f);
            paciente.localRotation = Quaternion.Euler(0,180,0);
            for(int i=0;i<9;i++)
            {
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Terceros/display-mannequin/paciente-{i}.obj");
                var pieza = (GameObject)PrefabUtility.InstantiatePrefab(asset,paciente);
                if (i == 2) GameObject.Find("Mobiliario/Paciente (maniquí)/Cabeza").transform.position = pieza.GetComponentInChildren<Renderer>().bounds.center;
                foreach (var r in pieza.GetComponentsInChildren<Renderer>())
                    r.sharedMaterial = Acabado(i==0 || i>=5 ? "Ropa paciente" : "Piel paciente",i==0 || i>=5 ? new Color(.19f,.3f,.39f) : new Color(.67f,.46f,.33f),.2f);
            }
        }

        static void AplicarRayosX(Transform raiz)
        {
            var brazo = GameObject.Find("Mobiliario/Equipo de rayos X/Brazo")?.transform;
            if (brazo == null) return;
            var previo = brazo.Find("Modelo importado móvil");
            if (previo != null) Object.DestroyImmediate(previo.gameObject);
            foreach (var r in brazo.GetComponentsInChildren<MeshRenderer>())
                if (r.name != "Barrera colocada") r.enabled=false;
            Sustituir(raiz,"Mobiliario/Equipo de rayos X/Soporte de pared","dental-practice/soporte-rayos.obj",new Vector3(.9f,1.48f,2.95f),new Vector3(.24f,.36f,.1f),180);
            var asset=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Terceros/dental-practice/brazo-rayos.obj");
            var modelo=(GameObject)PrefabUtility.InstantiatePrefab(asset,brazo);
            modelo.name="Modelo importado móvil";
            modelo.transform.localRotation=Quaternion.Euler(0,180,0);
            foreach(var r in modelo.GetComponentsInChildren<Renderer>()) r.sharedMaterials=r.sharedMaterials.Select(MaterialKenney).ToArray();
        }

        static void AplicarInstrumentos(Transform raiz)
        {
            foreach(var residuo in Object.FindObjectsByType<Residuo>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                string id=Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(residuo.Datos));
                string modelo = id=="vaso-desechable" ? "paper-cup/cup.glb" : id=="hoja-bisturi" ? "scalpel-google/hoja.obj" : id.Contains("gasa") ? "field-medicine/gauze-stack.glb" : id.StartsWith("aguja") && id != "aguja-sutura" ? "syringe-jtoastie/aguja.obj" : id.Contains("guantes") ? "gallery-gloves/guantes.obj" : id=="espejo-dental" ? "dental-practice/espejo.obj" : id=="explorador" ? "dental-practice/explorador.obj" : id=="rollos-algodon" ? "dental-practice/algodon.obj" : id=="jeringa-carpule" ? "syringe-jtoastie/jeringa.obj" : id=="pieza-dental" ? "tooth-sugamo/tooth.glb" : id.Contains("barrera") || id=="funda-sensor" || id=="envoltura-papel" || id=="babero" ? "tattoo-studio/hoja-barrera.obj" : null;
                if(modelo==null) continue;
                var previo=residuo.transform.Find("Modelo importado");
                if(previo!=null) Object.DestroyImmediate(previo.gameObject);
                var original=residuo.GetComponent<MeshRenderer>();
                if(original!=null) original.enabled=false;
                Vector3 d = residuo.transform.lossyScale;
                Vector3 dimensiones = new Vector3(Mathf.Max(d.x,.003f),Mathf.Max(d.y,.002f),Mathf.Max(d.z,.003f));
                var nuevo=ModeloExterno(raiz,modelo,residuo.transform.position,dimensiones);
                // Mantener la malla dentro del residuo permite tomarla, ocultarla y restablecerla.
                nuevo.SetParent(residuo.transform,true);
                nuevo.rotation=residuo.transform.rotation;
                if(id.Contains("guantes") || id.Contains("gasa") || id=="envoltura-papel" || id=="babero")
                    foreach(var r in nuevo.GetComponentsInChildren<Renderer>()) r.sharedMaterial=Acabado("Consumible_"+id,original != null ? original.sharedMaterial.color : Color.white,.2f);
            }
        }

        static void RegistrarFuentesExternas()
        {
            string[][] fuentes = {
                new[]{"paper-cup", "Drinks cup, small", "3D Assets", "https://3dassets.dev/assets/fast-food-and-drive-thru-drinks-cup-small-185a05b6", "CC0 1.0 Universal"},
                new[]{"scalpel-google", "Scalpel", "Poly by Google", "https://poly.pizza/m/9yKgpOpblnf", "CC BY 3.0"},
                new[]{"gallery-gloves", "Gloves and tools tray", "3D Assets", "https://3dassets.dev/assets/art-gallery-and-exhibition-rooms-gloves-and-tools-tray-029ffe6f", "CC0 1.0 Universal"},
                new[]{"dental-practice", "Dental Practice and Surgery", "3D Assets", "https://3dassets.dev/packs/dental-practice-and-surgery", "CC0 1.0 Universal"},
                new[]{"field-medicine", "Field Medicine and Recovery", "3D Assets", "https://3dassets.dev/packs/field-medicine-and-recovery", "CC0 1.0 Universal"},
                new[]{"tattoo-studio", "Tattoo and Piercing Studio", "3D Assets", "https://3dassets.dev/packs/tattoo-and-piercing-studio", "CC0 1.0 Universal"},
                new[]{"display-mannequin", "Seated mannequin", "3D Assets", "https://3dassets.dev/assets/retail-store-fixtures-and-mall-mannequin-seated-05ab1b9a", "CC0 1.0 Universal"},
                new[]{"syringe-jtoastie", "Syringe", "J-Toastie", "https://poly.pizza/m/MURJ8NK4N9", "CC BY 3.0"},
                new[]{"tooth-sugamo", "Tooth", "sugamo", "https://poly.pizza/m/66NBoNdhb03", "CC BY 3.0"},
                new[]{"kenney-car-kit", "Car Kit", "Kenney", "https://kenney.nl/assets/car-kit", "CC0 1.0 Universal"},
                new[]{"kenney-nature-kit", "Nature Kit", "Kenney", "https://kenney.nl/assets/nature-kit", "CC0 1.0 Universal"},
                new[]{"kenney-city-kit", "City Kit Commercial", "Kenney", "https://kenney.nl/assets/city-kit-commercial", "CC0 1.0 Universal"}
            };
            foreach (var f in fuentes)
            {
                string ruta = "Assets/Terceros/" + f[0] + "/Atribucion.asset";
                var credito = AssetDatabase.LoadAssetAtPath<Atribucion>(ruta);
                if (credito == null) { credito = ScriptableObject.CreateInstance<Atribucion>(); AssetDatabase.CreateAsset(credito,ruta); }
                credito.titulo=f[1]; credito.autor=f[2]; credito.url=f[3]; credito.licencia=f[4]; credito.fechaConsulta="2026-09-28";
                credito.modificaciones="Escala, orientación y adaptación de materiales.";
                if (Directory.GetFiles("Assets/Terceros/"+f[0],"*.obj").Length > 0)
                    credito.modificaciones += " Separación de piezas de las mallas originales mediante Tools/preparar_modelos.py.";
                if (f[0] == "display-mannequin") credito.modificaciones += " Retiro de la base y adaptación de la postura al sillón.";
                if (f[0] == "gallery-gloves") credito.modificaciones += " Guantes de manipulación de arte recoloreados como representación clínica esquemática.";
                if (f[0] == "syringe-jtoastie") credito.modificaciones += " Aproximación visual de la jeringa carpule a partir de una jeringa genérica.";
                if (f[2] == "3D Assets") credito.modificaciones += " El proveedor declara generación mediante IA. Se conservan sus metadatos en procedencia.json.";
                EditorUtility.SetDirty(credito);
            }
        }

    }
}
