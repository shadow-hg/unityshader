Shader "Unlit/OutMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor("BaseColor",color) = (1,1,1)
        [HDR] _EmissColor("EmissColor",color) = (1,1,1)
        _AllAlpha("Alpha",float) = 1.0
        _SetAlpha("SetAlpha",float) = 1.0
        _MaskBGAlpha("MaskBGAlpha",float) = 0.5
        _MaskLineAlpha("MaskLineAlpha",float) = 1.0
        _Value("Value",range(0,1)) = 1.0
        _Width("LineWidth",range(0,0.14)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "LightMode" = "ForwardBase" "Queue"="Transparent"}

        pass
        {
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull back
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 mmask : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Value ;
            fixed _AllAlpha;
            fixed _Width;
            fixed3 _BaseColor;
            fixed3 _EmissColor;
            fixed _MaskBGAlpha;
            fixed _MaskLineAlpha;
            fixed _SetAlpha ;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.mmask = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                half3 col = tex2D(_MainTex, i.uv);

                float mask = 1 - i.mmask.x;
                float mask2 = mask;
                
                float3 emissColor;
                float alpha = 0;

                if (_Value > 0)
                {
                    if (mask > _Value)
                    {
                        mask = 1.0;
                    }
                    else
                    {
                        mask = 0.0;
                    }

                    if (mask2 < (_Value + _Width))
                    {
                        mask2 = 1.0;
                    }
                    else
                    {
                        mask2 = 0.0;
                    }
                    
                    emissColor = mask.x * mask2.x * _EmissColor + (1 -mask.x) * col.x * _BaseColor + (1 -mask.x) * (1-col.x) * _BaseColor;
                    float allmask = mask * mask2 ;
                    alpha = (1 -mask.x) * col.x * _MaskBGAlpha + (1 - mask.x) * (1 - col.x) * _MaskLineAlpha;
                    alpha = allmask + alpha;
                }
                else
                {
                    alpha = 0.0;
                }
                
                return float4(emissColor,alpha * _AllAlpha * _SetAlpha);
            }
            ENDCG
        }
    }
}
