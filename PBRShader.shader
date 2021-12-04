Shader "Custom/S01"
{
//测试行
    Properties{
        _MainTex("Maintex",2D) = "White"{}
        _Roughness("Roughness",2D) = "White"{}
        _CubeMap("CubeMap",cube) = "_Skybox"{}
        _NormalTex("NormalTex",2D) = "blue"{}
        _NormalScale("NormalScale",float) = 1.0
        _Gloss("Gloss",float) = 1.0
        _refScale("refScale",float) = 0.5
        }
    SubShader{
        
        Pass{
            Tags{"RenderType" = "Opaque" "LightMode" = "ForwardBase" "Queue" = "Geometry"}
            ZWrite on
            cull back
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct a2v
            {
                float2 UV0 : TEXCOORD0;
                float4 vertex : POSITION ;
                float3 normal : NORMAL ;
                float4 tangent : TANGENT ;
                
            };

            struct v2f
            {
                float4 UV0 : TEXCOORD0 ;
                float4 pos : SV_POSITION ;
                float4 T2W0 : TEXCOORD1;
                float4 T2W1 : TEXCOORD2;
                float4 T2W2 : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormalTex;
            float4 _NormalTex_ST;
            sampler2D _Roughness;
            float4 _Roughness_ST;
            samplerCUBE _CubeMap;
            float4 _CubeMap_ST;

            half _NormalScale;
            half _Gloss;
            half _refScale;

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.UV0.xy = TRANSFORM_TEX(v.UV0,_MainTex);
                o.UV0.zw = TRANSFORM_TEX(v.UV0,_NormalTex);
                o.UV0.zw = TRANSFORM_TEX(v.UV0,_Roughness);
                o.UV0.zw = TRANSFORM_TEX(v.UV0,_CubeMap);

                float3 WldNormal = UnityObjectToWorldNormal(v.normal);
                float3 WldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 WldPos = mul(unity_ObjectToWorld,v.vertex);
                

                float3 binTangent = cross(WldTangent,WldNormal) * v.tangent.w;

                o.T2W0 = float4(WldTangent.x,WldTangent.x,WldNormal.x,WldPos.x);
                o.T2W1 = float4(WldTangent.y,WldTangent.y,WldNormal.y,WldPos.y);
                o.T2W2 = float4(WldTangent.z,WldTangent.z,WldNormal.z,WldPos.z);

                TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 mainTex = tex2D(_MainTex, i.UV0.xy);
                fixed3 roughness = tex2D(_Roughness, i.UV0.zw);
                fixed3 normalTex = UnpackNormal(tex2D(_NormalTex,i.UV0.zw));

                fixed3 viewDir = normalize(_WorldSpaceCameraPos - float3(i.T2W0.w,i.T2W1.w,i.T2W2.w));
                fixed3 halfDir = normalize(_WorldSpaceLightPos0.xyz + viewDir);

                fixed3 ambent = UNITY_LIGHTMODEL_AMBIENT * mainTex ;

                normalTex.xy *= _NormalScale;
                normalTex.z = sqrt(1 - saturate(dot(normalTex.xy,normalTex.xy)));
                normalTex = normalize( half3(dot(i.T2W0,normalTex),dot(i.T2W1,normalTex),dot(i.T2W2,normalTex)));

                //fixed3 refDir = reflect(normalize(_WorldSpaceLightPos0.xyz),normalTex);
                fixed specular = pow(saturate(dot(normalTex,halfDir)),_Gloss) * roughness;
                half3 WldReflect = reflect(-viewDir,normalTex).xyz;
                fixed3 reflectColor = texCUBE(_CubeMap,WldReflect);

                fixed atten = SHADOW_ATTENUATION(i);
                fixed3 diffuse = max(0,dot(_WorldSpaceLightPos0,normalTex)) *atten ;
                

                fixed3 aboent = diffuse * mainTex;
                aboent = lerp(aboent,reflectColor,_refScale);

                return fixed4(specular + ambent +aboent,1);
            }
            ENDCG
            }
        }
    fallback "Diffuse"
    }
