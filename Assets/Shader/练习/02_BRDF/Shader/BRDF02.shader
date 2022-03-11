Shader "Unlit/Study/BRDF02"
{
    properties
    {
        _Tint("Tint",color) = (1,1,1,1)
        _Smoothness("Smoothness",float) = 1.0
        [Gammer] _Metallic("Metallic",float) = 1.0
        _SpecularColor("SpecularColor",color) = (1,1,1,1)
        _ReflectColor("ReflectColor",color) = (1,1,1,1)
    }
    SubShader
    {

        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass
        {
            name "Stander"
            Tags
            {
                "RenderType" = "Opaque" "Queue" = "Geometry"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            #include "Library/PackageCache/com.unity.render-pipelines.universal@7.7.1/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@7.7.1/Shaders/LitForwardPass.hlsl"

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
                float2 lightmapUV   : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 WldNormal : TEXCOORD1;
                float3 WldPos : TEXCOORD2;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 3);
                float3 viewDirWS                : TEXCOORD5;
                float4  shadowCoord : TEXCOORD8;
                half4 fogFactorAndVertexLight   : TEXCOORD6;
            };

            struct UnityLight
            {
                half3 color;
                float3 dir;
                float3 ndotl;
            };

            struct DiffuseIndirect
            {
                float3 specular;
                float3 diffuse;
            };
            
            half4 _Tint;
            half3 _SpecularColor;
            half3 _ReflectColor;

            v2f vert(a2v v)
            {
                v2f o = (v2f)0;
                o.pos = TransformObjectToHClip(v.vertex.xyz);

                o.WldPos = TransformObjectToWorld(v.vertex);
                o.WldNormal = normalize(TransformObjectToWorldNormal(v.normal));

                OUTPUT_LIGHTMAP_UV(v.lightmapUV, unity_LightmapST, o.lightmapUV);
                OUTPUT_SH(o.WldNormal.xyz, o.vertexSH);

                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                UnityLight light;
                light.color = _MainLightColor.rgb;
                light.dir = _MainLightPosition.xyz ;
                light.ndotl = dot(i.WldNormal,light.dir);

                DiffuseIndirect indirect ;
                indirect.diffuse = 0;
                indirect.specular = 0;

                InputData inputData = (InputData)0;
                
                inputData.positionWS = i.WldPos;
                inputData.normalWS = i.WldNormal;
                inputData.viewDirectionWS = normalize(_WorldSpaceCameraPos - i.WldPos);
                //inputData.viewDirectionWS = SafeNormalize(i.viewDirWS);

                inputData.fogCoord = i.fogFactorAndVertexLight.x;
                inputData.vertexLighting = i.fogFactorAndVertexLight.yzw;
                inputData.bakedGI = SAMPLE_GI(i.lightmapUV, i.vertexSH, inputData.normalWS);
                
                float4 color;
                float finallAlpha = 1;
                
                color = UniversalFragmentPBR(inputData,_Tint,_Metallic,_SpecularColor,_Smoothness,1,0,finallAlpha);
                finallAlpha = color.a;

                return half4(color.rgb,finallAlpha);
                
            }
            ENDHLSL
        }
    }
}