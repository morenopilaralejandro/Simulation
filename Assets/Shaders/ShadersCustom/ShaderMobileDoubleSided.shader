Shader "Custom/Unlit/Mobile/MobileDoubleSided"
{
    Properties
    {
        _BaseMap("Base Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Unlit"
            "IgnoreProjector" = "True"
            "Queue" = "Geometry"
        }

        LOD 100

        Pass
        {
            Name "DoubleSided"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            Cull Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma prefer_hlsl3
            #pragma target 3.0

            #pragma vertex DoubleSidedVertex
            #pragma fragment DoubleSidedFragment

            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings DoubleSidedVertex(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output = (Varyings)0;
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

                return output;
            }

            half4 DoubleSidedFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half4 finalColor = texColor * _BaseColor;

                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

// Use TWO single-sided planes instead of one double-sided:
// - Front plane faces camera
// - Back plane faces opposite direction
// Each uses Mobile Unlit with Cull Back

// Advantages:
// - Same visual result
// - Each plane culls efficiently (only 1 side rendered)
// - Better for spatial organization
// - Easier to add effects per-side

// Disadvantage: Must manage two objects
