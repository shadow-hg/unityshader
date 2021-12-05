Shader "Unlit/DistanceTex"
{
    Properties
    {
        _Border ("Border", 2D) = "white" {} //边款
        _AlphaBG ("AlphaBG", 2D) = "white" {} //背景
        _AlphaText ("AlphaText", 2D) = "white" {} //内容文本
        
        [HDR] _Border_Color("Border_Color",color) = (1,1,1,1) //边款
        [HDR] _AlphaBG_Color("AlphaBG_Color",color) = (1,1,1,1) //背景
        [HDR] _AlphaText_Color("AlphaText_Color",color) = (1,1,1,1) ////内容文本
        
        _BorderAlpha("BorderAlpha",float) = 1 //边款透明度
        _BGAlpha("BGAlpha",float) = 1 //背景透明度
        _TextAlpha("TextAlpha",float) = 1 //内容文本透明度
        
        _AllAlpha("AllAlpha",float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" "Queue" = "Transparent"}
        ZWrite off
        blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
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
                float2 uv01 : TEXCOORD1;
                float2 uv02 : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Border;
            float4 _Border_ST;
            sampler2D _AlphaBG;
            float4 _AlphaBG_ST;
            sampler2D _AlphaText;
            float4 _AlphaText_ST;

            fixed3 _Border_Color;
            fixed3 _AlphaBG_Color;
            fixed3 _AlphaText_Color;
            fixed _BorderAlpha;
            fixed _BGAlpha;
            fixed _TextAlpha;
            fixed _AllAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Border);
                o.uv01 = TRANSFORM_TEX(v.uv, _AlphaBG);
                o.uv02 = TRANSFORM_TEX(v.uv, _AlphaText);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 border = tex2D(_Border, i.uv);
                fixed3 bg = tex2D(_AlphaBG, i.uv01);
                fixed3 text = tex2D(_AlphaText, i.uv02);
                
                fixed AllAlpha = border.r*_BorderAlpha + _BGAlpha * (saturate(bg.r - text.r)) + text.r * _TextAlpha;
                
                fixed3 Color = border.r * _Border_Color + (saturate(bg.r - text.r)) * _AlphaBG_Color + text.r * _AlphaText_Color;

                return fixed4(Color,AllAlpha* _AllAlpha);
            }
            ENDCG
        }
        
    }
}
