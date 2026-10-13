Shader "ConsultorioSeguro/Silueta"
{
    Properties
    {
        _OutlineColor ("Color de contorno", Color) = (1,1,1,1)
        _OutlinePixels ("Anchura en píxeles", Range(0,4)) = 1.35
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent+10" }
        Pass
        {
            Name "Silueta"
            Tags { "LightMode"="SRPDefaultUnlit" }
            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlinePixels;
            CBUFFER_END
            struct Attributes { float3 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; };
            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float3 normalVS = TransformWorldToViewDir(normalWS, true);
                float2 projected = mul((float3x3)UNITY_MATRIX_P, normalVS).xy;
                float lengthSquared = dot(projected, projected);
                float2 direction = lengthSquared > .000001 ? projected * rsqrt(lengthSquared) : float2(0, 0);
                output.positionCS.xy += direction * (2 * _OutlinePixels / _ScaledScreenParams.xy) * output.positionCS.w;
                return output;
            }
            half4 Fragment(Varyings input) : SV_Target { return _OutlineColor; }
            ENDHLSL
        }
    }
}
