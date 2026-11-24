using UnityEngine;
using UnityEngine.Rendering;

namespace ConsultorioSeguroNuevo
{
    /// <summary>
    /// Mano derecha con guante de nitrilo, de geometría propia, que sostiene el objeto en primera persona.
    /// Sus ejes: +Z hacia el frente de la vista, +Y hacia arriba; la palma queda bajo el objeto.
    /// </summary>
    public sealed class GlovedHand : MonoBehaviour
    {
        public const string HandName = "Mano enguantada";

        public static GlovedHand Build(Transform parent)
        {
            var root = new GameObject(HandName).transform;
            root.SetParent(parent, false);
            var glove = Load("Guante", new Color(.36f, .56f, .86f), .55f);
            var sleeve = Load("Manga de bata", new Color(.93f, .94f, .93f), .2f);
            Box("Palma", root, new Vector3(0, 0, .005f), new Vector3(.082f, .022f, .092f), glove);
            for (int i = 0; i < 4; i++)
            {
                // Dedos que rodean el objeto: un tramo hacia el frente y otro hacia arriba.
                float x = -.03f + i * .02f;
                float length = i == 0 || i == 3 ? .034f : .042f;
                Vector3 knuckle = new Vector3(x, .004f, .05f);
                Vector3 middle = knuckle + new Vector3(0, .006f, length);
                Segment("Dedo", root, knuckle, middle, .0085f, glove);
                Segment("Punta del dedo", root, middle, middle + new Vector3(0, .03f, .004f), .008f, glove);
            }
            Segment("Pulgar", root, new Vector3(-.04f, .002f, -.012f), new Vector3(-.058f, .022f, .035f), .0095f, glove);
            Segment("Punta del pulgar", root, new Vector3(-.058f, .022f, .035f), new Vector3(-.052f, .045f, .058f), .0088f, glove);
            Segment("Puño del guante", root, new Vector3(.005f, -.004f, -.045f), new Vector3(.03f, -.03f, -.13f), .03f, glove);
            Segment("Manga", root, new Vector3(.028f, -.028f, -.125f), new Vector3(.09f, -.11f, -.36f), .037f, sleeve);
            root.gameObject.SetActive(false);
            return root.gameObject.AddComponent<GlovedHand>();
        }

        static Material Load(string name, Color color, float smoothness)
        {
            var material = Resources.Load<Material>(SimulationFx.ResourceFolder + name);
            if (material) return material;
            material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"))
                { name = name, hideFlags = HideFlags.DontSave };
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", smoothness);
            return material;
        }

        static void Configure(GameObject part, Material material)
        {
            Destroy(part.GetComponent<Collider>());
            var renderer = part.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            part.layer = 2;
        }

        static void Box(string name, Transform parent, Vector3 position, Vector3 size, Material material)
        {
            var part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = size;
            Configure(part, material);
        }

        static void Segment(string name, Transform parent, Vector3 from, Vector3 to, float radius, Material material)
        {
            var part = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = (from + to) * .5f;
            part.transform.localRotation = Quaternion.FromToRotation(Vector3.up, to - from);
            float length = Vector3.Distance(from, to) + radius * 2;
            part.transform.localScale = new Vector3(radius * 2, length * .5f, radius * 2);
            Configure(part, material);
        }

        /// <summary>Coloca la palma bajo el objeto (o detrás, si sostiene pinzas) y la muestra.</summary>
        public void Grip(Bounds held, bool forceps)
        {
            // Sujeta el borde inferior cercano del objeto, con la palma inclinada hacia la vista.
            transform.localPosition = forceps
                ? new Vector3(.01f, -.035f, -.2f)
                : new Vector3(held.center.x + .01f, held.min.y - .004f, held.min.z + .03f);
            transform.localRotation = forceps ? Quaternion.identity : Quaternion.Euler(-28, 0, 0);
            gameObject.SetActive(true);
        }

        public void Release() => gameObject.SetActive(false);
    }
}
