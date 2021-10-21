Shader "Unlit/01"
{
    Properties{
        _CubeMap("CubeMap",cube) = "white"{}
        _MainTex("MainTex",2D) = "white"{}
        _NormalTex("NormalTex",2D) = "white"{}
        _gloss("Gloss",float) = 1.0
        _Roughness("_Roughness",range(0,1)) = 1.0
        _smoothness("_smoothness",range(0,1)) = 0.5
        _NormalScale("_NormalScale",range(0,1)) = 0.5
        }
    
    SubShader{
        Pass{
            Tags{"RenderType" = "Opaque" "LightMode" = "ForwardBase" "Queue" = "Geometry"}
            ZWrite on
            Cull back
            
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
                float4 pos : SV_POSITION ;
                float3 WldNormal : TEXCOORD0;
                float3 WldPos : TEXCOORD1;
                SHADOW_COORDS(2)
                float4 UV0:TEXCOORD3;

                float3 T2W0 : TEXCOORD4;
                float3 T2W1 : TEXCOORD5;
                float3 T2W2 : TEXCOORD6;
            };

            half _gloss ;
            fixed _Roughness;
            fixed _smoothness;
            fixed _NormalScale;
            samplerCUBE _CubeMap;
            float4 _CubeMap_ST;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormalTex;
            float4 _NormalTex_ST;

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.WldNormal  = UnityObjectToWorldNormal(v.normal);
                o.WldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                o.UV0.xy = TRANSFORM_TEX(v.UV0,_MainTex);
                o.UV0.zw = TRANSFORM_TEX(v.UV0,_NormalTex);
                float3 WldTangent = UnityObjectToWorldDir(v.tangent);

                float3 binTangent = cross(WldTangent,o.WldNormal) * v.tangent.w ;
                o.T2W0 = float3(WldTangent.x,binTangent.x,o.WldNormal.x);
                o.T2W1 = float3(WldTangent.y,binTangent.y,o.WldNormal.y);
                o.T2W2 = float3(WldTangent.z,binTangent.z,o.WldNormal.z);

                TRANSFER_SHADOW(o)//shadow

                return o;
            }

            fixed4 frag(v2f i):SV_Target{

                fixed3 Color = tex2D(_MainTex,i.UV0.xy);
                half3 normalMap = UnpackNormal(tex2D(_NormalTex,i.UV0.zw));
                normalMap.xy *= _NormalScale;
                normalMap.z = sqrt(1 - saturate(dot(normalMap.xy,normalMap.xy)));
                normalMap = fixed3 (dot(i.T2W0,normalMap),dot(i.T2W1,normalMap),dot(i.T2W2,normalMap));
                
                fixed shadow = SHADOW_ATTENUATION(i);
                float3 WldLightDir = _WorldSpaceLightPos0.xyz ;
                fixed diffuse = saturate(dot(normalize(WldLightDir),normalize(normalMap))) * shadow;
                fixed3 ViewDir =normalize( _WorldSpaceCameraPos.xyz - i.WldPos) ;

                fixed3 halfDir = normalize(normalize(WldLightDir) + ViewDir);

                fixed specular = lerp(0,pow(max(0,dot(normalize(normalMap),halfDir)),_gloss),lerp(0,1,lerp(0,4.88,_Roughness))) * shadow;

                fixed3 reflectDir = normalize(reflect(-ViewDir,normalMap));
                fixed3 cubeColor = texCUBE(_CubeMap,reflectDir);
                

                return fixed4(lerp(lerp(diffuse.xxx * Color ,cubeColor * diffuse,_Roughness),cubeColor,_smoothness)  + specular,1);

            }
            
            ENDCG
            
            }//endPass
        
    }//endSubshader
    fallback "Diffuse"
}//endShader
