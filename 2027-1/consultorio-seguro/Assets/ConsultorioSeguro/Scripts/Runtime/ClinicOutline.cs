using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConsultorioSeguroNuevo
{
    // La carcasa sigue la malla y respeta la profundidad. No ilumina recipientes ni codifica respuestas.
    public sealed class ClinicOutline : MonoBehaviour
    {
        ClinicGame game;
        ClinicInteractable target;
        Shader shader;
        Material material;
        readonly List<MeshRenderer> shells = new List<MeshRenderer>();
        public bool Visible { get; private set; }
        public bool Focused { get; private set; }
        static readonly Color WarmGold = new Color(1f, .72f, .26f);

        public void Initialize(ClinicGame owner, ClinicInteractable item, Shader outlineShader)
        {
            game = owner;
            target = item;
            shader = outlineShader;
        }

        public void Refresh()
        {
            Visible = game && shader && game.ShouldOutline(target);
            Focused = Visible && game.Target == target;
            if (Visible && !material) CreateShells();
            if (material)
            {
                material.SetColor("_OutlineColor", Focused ? WarmGold : Color.white);
                material.SetFloat("_OutlinePixels", Focused ? 2f : 1.35f);
            }
            foreach (var shell in shells) if (shell) shell.enabled = Visible;
        }

        void CreateShells()
        {
            material = new Material(shader) { name = "Contorno de " + target.name, hideFlags = HideFlags.DontSave };
            foreach (var source in target.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (!source.enabled) continue;
                var filter = source.GetComponent<MeshFilter>();
                if (!filter || !filter.sharedMesh) continue;
                var shell = new GameObject("Silueta de interacción", typeof(MeshFilter), typeof(MeshRenderer));
                shell.hideFlags = HideFlags.DontSave;
                shell.layer = 2;
                shell.transform.SetParent(source.transform, false);
                shell.GetComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
                var renderer = shell.GetComponent<MeshRenderer>();
                var materials = new Material[filter.sharedMesh.subMeshCount];
                for (int i = 0; i < materials.Length; i++) materials[i] = material;
                renderer.sharedMaterials = materials;
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.lightProbeUsage = LightProbeUsage.Off;
                renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                shells.Add(renderer);
            }
        }
        void OnDisable()
        {
            Visible = Focused = false;
            foreach (var shell in shells) if (shell) shell.enabled = false;
        }
        void OnDestroy()
        {
            foreach (var shell in shells) if (shell) Destroy(shell.gameObject);
            if (material) Destroy(material);
        }
    }
}
