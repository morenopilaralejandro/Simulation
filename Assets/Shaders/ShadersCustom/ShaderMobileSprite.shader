Shader "Custom/Unlit/Mobile/MobileSprite"
{
    Properties
    {
        _BaseMap("Sprite Texture", 2D) = "white" {}
        _BaseColor("Tint Color", Color) = (1, 1, 1, 1)
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
            Name "Sprite"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma prefer_hlsl3
            #pragma target 3.0

            #pragma vertex SpriteVertex
            #pragma fragment SpriteFragment

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
                half4 color : COLOR;              // sprite tint
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;              // pass tint through
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings SpriteVertex(Attributes input)
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

            half4 SpriteFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);

                if (texColor.a < 0.001h)
                    discard;

                half4 finalColor = texColor * _BaseColor * input.color;
                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
