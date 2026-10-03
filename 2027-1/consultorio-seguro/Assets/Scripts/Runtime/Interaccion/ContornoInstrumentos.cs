using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace ConsultorioSeguro
{
    // Máscara independiente y dilatación en píxeles: también funciona con agujas
    // subpíxel y modelos cuyas normales no permiten un contorno por extrusión.
    public sealed class ContornoInstrumentos : ScriptableRendererFeature
    {
        [SerializeField] Shader sombreado;
        Material blanco, dorado;
        PasoContorno pase;

        public override void Create()
        {
            pase = null;
            CoreUtils.Destroy(blanco);
            CoreUtils.Destroy(dorado);
            if (sombreado == null) return;
            blanco = CoreUtils.CreateEngineMaterial(sombreado);
            dorado = CoreUtils.CreateEngineMaterial(sombreado);
            blanco.SetColor("_ColorContorno", Color.white);
            dorado.SetColor("_ColorContorno", new Color(1, .68f, .12f));
            pase = new PasoContorno(blanco, dorado) { renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (pase == null || !Application.isPlaying || renderingData.cameraData.cameraType != CameraType.Game) return;
            if (GestorSimulacion.Instancia == null || GestorSimulacion.Instancia.Estado != EstadoSimulacion.Jugando) return;
            if (pase.Preparar(renderingData.cameraData.camera)) renderer.EnqueuePass(pase);
        }

        protected override void Dispose(bool disposing)
        {
            pase = null;
            CoreUtils.Destroy(blanco);
            CoreUtils.Destroy(dorado);
        }

        sealed class PasoContorno : ScriptableRenderPass
        {
            readonly Material blanco, dorado;
            readonly List<Dibujo> dibujos = new();
            readonly MaterialPropertyBlock propiedades = new();
            readonly struct Dibujo
            {
                public readonly Renderer malla;
                public readonly int submallas;
                public readonly bool enfocado;
                public Dibujo(Renderer malla, int submallas, bool enfocado)
                { this.malla = malla; this.submallas = submallas; this.enfocado = enfocado; }
            }
            sealed class DatosMascara
            {
                public List<Dibujo> dibujos;
                public Material blanco, dorado;
            }
            sealed class DatosContorno
            {
                public TextureHandle mascara;
                public Material material;
                public MaterialPropertyBlock propiedades;
                public Vector4 pixel;
            }

            public PasoContorno(Material blanco, Material dorado) { this.blanco = blanco; this.dorado = dorado; }

            // Decide antes de encolar: fuera de una sala con instrumental no se fuerza un pase adicional.
            public bool Preparar(Camera camara)
            {
                var interactor = camara.GetComponentInParent<Interactor>();
                dibujos.Clear();
                foreach (var instrumento in ResaltadoInstrumento.Activos)
                {
                    if (!instrumento.DebeMostrar(camara, interactor)) continue;
                    foreach (var malla in instrumento.Mallas)
                    {
                        if (!malla.enabled || !malla.gameObject.activeInHierarchy) continue;
                        var filtro = malla.GetComponent<MeshFilter>();
                        if (filtro == null || filtro.sharedMesh == null) continue;
                        dibujos.Add(new Dibujo(malla, filtro.sharedMesh.subMeshCount, instrumento.Enfocado(interactor)));
                    }
                }
                return dibujos.Count > 0;
            }

            public override void RecordRenderGraph(RenderGraph grafo, ContextContainer datos)
            {
                if (dibujos.Count == 0) return;
                var camara = datos.Get<UniversalCameraData>();
                var recursos = datos.Get<UniversalResourceData>();
                var descriptor = camara.cameraTargetDescriptor;
                descriptor.depthBufferBits = 0;
                descriptor.msaaSamples = 1;
                descriptor.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
                var mascara = UniversalRenderer.CreateRenderGraphTexture(grafo, descriptor, "Siluetas de instrumentos", true);
                using (var constructor = grafo.AddRasterRenderPass<DatosMascara>("Máscara de instrumentos", out var d))
                {
                    d.dibujos = dibujos;
                    d.blanco = blanco; d.dorado = dorado;
                    constructor.SetRenderAttachment(mascara, 0, AccessFlags.Write);
                    constructor.SetRenderFunc(static (DatosMascara p, RasterGraphContext contexto) =>
                    {
                        foreach (var dibujo in p.dibujos)
                            for (int sub = 0; sub < dibujo.submallas; sub++)
                                contexto.cmd.DrawRenderer(dibujo.malla, dibujo.enfocado ? p.dorado : p.blanco, sub, 0);
                    });
                }
                using (var constructor = grafo.AddRasterRenderPass<DatosContorno>("Contorno espectral de instrumentos", out var d))
                {
                    d.mascara = mascara; d.material = blanco; d.propiedades = propiedades;
                    d.pixel = new Vector4(1f / descriptor.width, 1f / descriptor.height, 0, 0);
                    constructor.UseTexture(mascara, AccessFlags.Read);
                    // Conserva el color de la cámara y mezcla únicamente el halo.
                    constructor.SetRenderAttachment(recursos.activeColorTexture, 0, AccessFlags.ReadWrite);
                    constructor.SetRenderFunc(static (DatosContorno p, RasterGraphContext contexto) =>
                    {
                        p.propiedades.Clear();
                        RTHandle textura = p.mascara;
                        p.propiedades.SetTexture("_BlitTexture", textura);
                        p.propiedades.SetVector("_BlitScaleBias", new Vector4(1, 1, 0, 0));
                        p.propiedades.SetVector("_PixelContorno", p.pixel);
                        contexto.cmd.DrawProcedural(Matrix4x4.identity, p.material, 1, MeshTopology.Triangles, 3, 1, p.propiedades);
                    });
                }
            }
        }
    }
}
