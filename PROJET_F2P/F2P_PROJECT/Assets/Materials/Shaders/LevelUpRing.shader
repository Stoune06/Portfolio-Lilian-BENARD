Shader "Custom/LevelUpRing"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1, 0.8, 0.2, 1)
        _Alpha ("Alpha", Range(0, 1)) = 1
        _InnerRadius ("Inner Radius", Range(0, 0.5)) = 0.3
        _OuterRadius ("Outer Radius", Range(0, 0.5)) = 0.45
        _Smoothness ("Edge Smoothness", Range(0.001, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Alpha;
                float _InnerRadius;
                float _OuterRadius;
                float _Smoothness;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 center = input.uv - 0.5;
                float dist = length(center);

                float ring = smoothstep(_InnerRadius - _Smoothness, _InnerRadius, dist)
                           - smoothstep(_OuterRadius, _OuterRadius + _Smoothness, dist);

                half4 col = _Color;
                col.a = ring * _Alpha;
                return col;
            }
            ENDHLSL
        }
    }
}
