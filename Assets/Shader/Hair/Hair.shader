Shader "Unlit/Hair"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("MaskTex", 2D) = "white" {}
        _Color("Color",color) = (1,1,1,1)
        _shadow("Shadow",float) = 0.1
        _SpecularColor("SpecularColor",color) = (1,1,1,1)
        _SpecularColor2("SpecularColor2",color) = (1,1,1,1)
        //_LightDir("LightDir",vector) = (0.5,0.5,0.5)
        _PrimaryShift("PrimaryShift",float) = 1.0
        _SecondaryShift("SecondaryShift",float) = 1.0
        _SpecularMultiplier("SpecularMultiplier",float) = 1.0
        _SpecularMultiplier2("SpecularMultiplier2",float) = 1.0
        _Specular("Specular",float) = 1.0
        _Cut("CutOff",float) = 0
    }
    SubShader
    {

        Pass
        {
            //Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        ZWrite on
        Blend SrcAlpha OneMinusSrcAlpha
        Cull off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 OTW1 : TEXCOORD1;
                float4 OTW2 : TEXCOORD2;
                float4 OTW3 : TEXCOORD3;
                float3 WldLightDir : TEXCOORD4;
                float3 WldViewDir : TEXCOORD5;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            CBUFFER_START(UnityPreMaterial)
            half3 _Color;
            half _shadow;
            half3 _SpecularColor;
            half3 _SpecularColor2;
            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            half _PrimaryShift;
            half _SecondaryShift;
            half _Specular;
            half _SpecularMultiplier;
            half _SpecularMultiplier2;
            half _LightDir;
            half _Cut;
            CBUFFER_END

            //获取头发高光
            fixed StrandSpecular(fixed3 T, fixed3 V, fixed3 L, fixed exponent)
            {
                fixed3 H = normalize(L + V);
                fixed dotTH = dot(T, H);
                fixed sinTH = sqrt(1 - dotTH * dotTH);
                fixed dirAtten = smoothstep(-1, 0, dotTH);
                return dirAtten * pow(sinTH, exponent);
            }

            //沿着法线方向调整Tangent方向
            fixed3 ShiftTangent(fixed3 T, fixed3 N, fixed shift)
            {
                return normalize(T + shift * N);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.uv, _MaskTex);

                half3 WldNornam = UnityObjectToWorldNormal(v.normal);
                half3 WldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                half3 BinTangent = cross(WldTangent, WldNornam) * v.tangent.w;
                half3 WldPos = mul(unity_ObjectToWorld, v.vertex);

                o.OTW1 = float4(WldTangent.x, BinTangent.x, WldNornam.x, WldPos.x);
                o.OTW2 = float4(WldTangent.y, BinTangent.y, WldNornam.y, WldPos.y);
                o.OTW3 = float4(WldTangent.z, BinTangent.z, WldNornam.z, WldPos.z);

                o.WldLightDir = _WorldSpaceLightPos0;
                o.WldViewDir = _WorldSpaceCameraPos - WldPos;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 spec = tex2D(_MaskTex, i.uv.zw).rgb;
                half3 Diffuse = tex2D(_MainTex, i.uv.xy).rgb;
                
                half3 worldNormal = normalize(float3(i.OTW1.z, i.OTW2.z, i.OTW3.z));
                half3 worldBinormal = normalize(float3(i.OTW1.y, i.OTW2.y, i.OTW3.y));
                half3 worldLightDir = normalize(i.WldLightDir);
                half3 worldViewDir = normalize(i.WldViewDir);

                half LdotN = max(dot(worldLightDir,worldNormal),_shadow);
                Diffuse *= LdotN ;
                Diffuse *= _Color;

                //计算切线方向的偏移度
                half shiftTex = spec.g;
                half3 t1 = ShiftTangent(worldBinormal, worldNormal, _PrimaryShift + shiftTex);
                half3 t2 = ShiftTangent(worldBinormal, worldNormal, _SecondaryShift + shiftTex);
                //计算高光强度        
                half3 spec1 = StrandSpecular(t1, worldViewDir, worldLightDir, _SpecularMultiplier) * _SpecularColor;
                half3 spec2 = StrandSpecular(t2, worldViewDir, worldLightDir, _SpecularMultiplier2) * _SpecularColor2;

                half4 finalColor = 0;
                finalColor.rgb = spec1 * _SpecularColor; //第一层高光
                finalColor.rgb += saturate(spec2 * _SpecularColor2 * spec.b * _Specular); //第二层高光，spec.b用于添加噪点
                finalColor.rgb += Diffuse;
                //finalColor.rgb *= unity_LightColor0; //受灯光影响
                
                if (spec.r > _Cut)
                {
                    spec.r = 1;
                }
                else
                {
                    spec.r = 0;
                }
                finalColor.a = spec.r;

                return half4(finalColor);
            }
            ENDCG
        }

    }
}