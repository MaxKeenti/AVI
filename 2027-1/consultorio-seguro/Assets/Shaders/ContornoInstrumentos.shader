Shader "Hidden/Consultorio/ContornoInstrumentos"
{
    Properties { _ColorContorno ("Color", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "Silueta"
            HLSLPROGRAM
            #pragma vertex Vertice
            #pragma fragment Fragmento
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _ColorContorno;
            CBUFFER_END
            struct Entrada { float3 positionOS : POSITION; };
            struct Salida { float4 positionCS : SV_POSITION; };
            Salida Vertice(Entrada entrada)
            {
                Salida salida;
                salida.positionCS = TransformObjectToHClip(entrada.positionOS);
                return salida;
            }
            half4 Fragmento(Salida entrada) : SV_Target { return _ColorContorno; }
            ENDHLSL
        }
        Pass
        {
            Name "Halo"
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragmento
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            float4 _PixelContorno;
            half4 Fragmento(Varyings entrada) : SV_Target
            {
                half4 centro = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, entrada.texcoord);
                half4 borde = centro;
                // Radio fijo de 2 píxeles, incluso en los instrumentos más delgados.
                [unroll] for (int y = -2; y <= 2; y++)
                [unroll] for (int x = -2; x <= 2; x++)
                {
                    half4 muestra = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp,
                        entrada.texcoord + float2(x, y) * _PixelContorno.xy);
                    if (muestra.a > borde.a) borde = muestra;
                }
                return half4(borde.rgb, saturate(borde.a - centro.a) + centro.a * .12h);
            }
            ENDHLSL
        }
    }
}
