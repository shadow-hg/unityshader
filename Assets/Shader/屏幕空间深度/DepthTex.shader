Shader "Unlit/DepthTex"
{
    Properties
    {
        _value("Value",float) = 0
        _value2("Value2",range(0,0.1)) = 0
        _value3("Value3",float) = 1
        _noiseTex("NoiseTex",2D) = "white"{}
        _color("Color",color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalRenderPipeline" "Queue" = "Transparent"}

        Pass
        {
            ZWrite off
            blend SrcAlpha OneMinusSrcAlpha
            
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
                float3 scrPos : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
            float _value;
            float _value2;
            float _value3;
            float4 _noiseTex_ST;
            half4 _color;
            CBUFFER_END

            TEXTURE2D(_noiseTex);
            SAMPLER(sampler_noiseTex);
            
            TEXTURE2D_X_FLOAT(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.scrPos = ComputeScreenPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv,_noiseTex);
                o.uv.zw = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                
                float depth = i.vertex.z;
                depth = Linear01Depth(depth,_ZBufferParams);
                float col = SAMPLE_TEXTURE2D_X(_CameraDepthTexture,sampler_CameraDepthTexture,i.vertex.xy/_ScreenParams.xy).r * _value2;
                col = Linear01Depth(col,_ZBufferParams);

                col = saturate(depth - col + _value3)* _value;

                float ccc = SAMPLE_TEXTURE2D(_noiseTex,sampler_noiseTex,col.rr).xx * col;
                //col*= _color ;
                
                return float4(1,1,1,ccc);
            }
            ENDHLSL
        }
    }
}
