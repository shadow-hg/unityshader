Shader "Unlit/OpaqueColor"
{
    Properties
    {
        _value("Value",range(0,1)) = 0
        _noiseTex("NoiseTex",2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalRenderPipeline" "Queue" = "Transparent"}

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Library/PackageCache/com.unity.render-pipelines.universal@7.7.1/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
            float _value;
            float4 _noiseTex_ST;
            CBUFFER_END

            TEXTURE2D(_noiseTex);
            SAMPLER(sampler_noiseTex);
            
            float4 _CameraColorTexture_TexelSize;
            SAMPLER(_CameraColorTexture);

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv,_noiseTex);
                o.uv.zw = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 noiseCol = SAMPLE_TEXTURE2D(_noiseTex,sampler_noiseTex,i.uv.zw).xx;
                half3 col = tex2D(_CameraColorTexture,lerp(i.vertex.xy/_ScreenParams.xy,noiseCol,_value));
                
                return half4(col,1);
            }
            ENDHLSL
        }
    }
}
