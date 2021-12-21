Shader "Unlit/BTShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Space(20)]_Space20("Space(20)",float) = 0
        [Header(DefaultSetting)]_Title("Title",float) = 0
        [Enum(off,0,on,1)]_ZWrite("ZWrite",float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcAlpha("SrcAlpha",float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DstAlpha("DstAlpha",float) = 1
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull",float) = 2
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest",float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWrite [_ZWrite]
            Blend [_SrcAlpha] [_DstAlpha]
            Cull [_Cull]
            ZTest [_ZTest]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
