Shader "Custom/Unlit/Mobile/MobileAdditiveParticle"
{
    Properties
    {
        _BaseMap("Particle Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Intensity("Intensity", Range(0, 2)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Unlit"
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
        }

        LOD 100

        Pass
        {
            Name "AdditiveParticle"
            Tags { "LightMode" = "UniversalForward" }

            Blend One One
            ZWrite Off
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma prefer_hlsl3
            #pragma target 3.0

            #pragma vertex ParticleVertex
            #pragma fragment ParticleFragment

            #pragma multi_compile_particles
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _Color;
                half _Intensity;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings ParticleVertex(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output = (Varyings)0;
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.color = input.color;

                return output;
            }

            half4 ParticleFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);

                if (texColor.a < 0.01h)
                    discard;

                half3 finalColor = texColor.rgb * _Color.rgb * input.color.rgb * _Intensity;
                half finalAlpha = texColor.a * input.color.a;

                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
