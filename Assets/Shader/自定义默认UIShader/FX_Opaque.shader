Shader "HRP/GGame/FX/FX_Opaque"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
        _ColorStrength("ColorStrength",float) = 1.0
        _Pow("Pow",float) = 1.0
        _DepthOffset("DepthOffset",float) = 0.0

        [Toggle]_MainAnimation("MainAnimation",int) = 0
        _MainSpeedU("MainSpeedU", Float) = 0
        _MainSpeedV("MainSpeedV", Float) = 0
        
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull",int) = 2
    }
    SubShader
    {
        Tags
        {
            "LightMode" = "HRPForward"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        Pass
        {

            Cull [_Cull]
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _PACKUV
            #pragma shader_feature _MAINANIMATION_ON

            #include "Packages/com.yoozoo.owl.rendering.hrp/RenderPipeline/ShaderLibrary/HPipelineData.hlsl"
            //#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal
            #include "GTA_H_FX_Common.hlsl"

            CBUFFER_START(UnityPerMaterial)
            half _TimeModY;
            half _TimeCycle;
            half _TimeOffset;
            CBUFFER_END

            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                half4 vertexColor : COLOR;
            };

            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                VERTEX_INPUT_PACK_UV(0)
                float2 movingUV :TEXCOORD2;
                half4 color:TEXCOORD3;
            };

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.pos = TransformObjectToHClip(v.vertex);

                SETUP_PACK_UV(0, 0)
                
                o.pos = vertDepthOffset(o.pos, _DepthOffset);
                
                o.movingUV.xy = v.texcoord0.xy;
                #if defined (_MAINANIMATION_ON)
                o.movingUV.xy = half2(_MainSpeedU, _MainSpeedV) * _Time.g + v.texcoord0.xy;
                #endif
                o.movingUV.xy = TRANSFORM_TEX(o.movingUV.xy, _MainTex);
                
                o.color = v.vertexColor * _Color;
                return o;
            }

            half4 frag(VertexOutput i) : SV_Target
            {
                float2 uvMain = i.movingUV.xy;
                APPLY_PACK_UV(uvMain, 0)
                
                half4 _MainTexVar = tex2D(_MainTex, uvMain);

                half3 finalColor = _MainTexVar.rgb * i.color.rgb;
                finalColor = saturate(pow(finalColor * _ColorStrength, _Pow));

                return half4(finalColor,1);
            }
            ENDHLSL
        }
    }
    CustomEditor"ShaderGUIDefault"
}