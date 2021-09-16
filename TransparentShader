Shader "Unlit/alpha"
{
    Properties
    {
        _Color("Color",color) = (1,1,1,1)
        _Pow("Pow",float) = 1.0
        _Alpha("Alpha",float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "LightMode" = "ForwardBase" "Queue"="Transparent"}
        ZWrite off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            
            #include "UnityCG.cginc"

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

            fixed4 _Color;
            half _Pow;
            fixed _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                fixed Mask = pow(i.uv.y,_Pow) * _Alpha;
                
                return half4(_Color.rgb,Mask);
            }
            ENDHLSL
        }
    }
}
