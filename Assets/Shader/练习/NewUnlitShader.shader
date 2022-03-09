Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",color) = (1,0,0,1)
        [Enum(UnityEngine.Rendering.CompareFunction)]_Pass01ZWrite("Pass01ZWrite",float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_Pass01ZTest("Pass01ZTest",float) = 0
        [Space(12)]
        [Enum(UnityEngine.Rendering.CompareFunction)]_Pass02ZWrite("Pass02ZWrite",float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_Pass02ZTest("Pass02ZTest",float) = 0
    }
    SubShader
    {
        Tags{
            "RenderPipeline" = "UniverialRenderPipeline"
            }
        Pass
        {
            Tags{
                "RenderType"="Opaque"
                
                }
            ZWrite on
            ZTest [_Pass01ZTest]
            cull back
            
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                
                float4 vertex : SV_POSITION;
            };
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Color;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv);
                return col;
            }
            ENDHLSL
        }
        
        Pass
        {
            Tags{
                "RenderType"="Opaque"
                "Queue" = "Geometry"
                }
            ZWrite on
            ZTest [_Pass01ZTest]
            //blend SrcAlpha OneMinusSrcAlpha
            cull off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                
                float4 vertex : SV_POSITION;
            };
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Color;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = _Color;
                return col;
            }
            ENDHLSL
        }
        
        
    }
}
