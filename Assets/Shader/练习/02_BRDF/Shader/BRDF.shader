Shader "Unlit/Study/BRDF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint("Tint",color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Library/PackageCache/com.unity.render-pipelines.universal@7.7.1/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);//采样贴图
            SAMPLER(sampler_MainTex);//贴图采样器
            
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Tint;
            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normal : NORMAL ;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 WldPos : TEXCOORD1;
                float3 NormalOS : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.NormalOS = v.normal;
                o.WldPos = mul(unity_ObjectToWorld,v.vertex);

                o.uv = TRANSFORM_TEX(v.uv0,_MainTex);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half3 DiffuseColor = half3(0,0,0);
                half3 DiffuseSpecular = half3(0,0,0);
                half3 finallDiffuse = DiffuseColor + DiffuseSpecular;

                half3 IBLdiffuse = half3(0,0,0);
                half3 IBLspecular = half3(0,0,0);
                half3 finallIBL = IBLdiffuse + IBLspecular;

                half3 Adoble = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv);

                half3 finallColor = finallDiffuse + finallIBL;
                half finallAlpha = _Tint.a;
                
                return half4(finallColor,finallAlpha);
            }
            ENDHLSL
        }
    }
}
