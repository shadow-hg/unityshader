Shader "HRP/GGame/UI/UIDefault"
{
    Properties
    {

        _MainTex("MainTex", 2D) = "white" {}

        [Toggle]_UseMask("UseMask",float) = 0
        _MaskTex("MaskTex", 2D) = "white" {}

        _Color("Color",Color) = (0.5,0.5,0.5,0.5)
        _ColorStrength("ColorStrength",float) = 1.0
        _Pow("Pow",float) = 1.0
        _DepthOffset("DepthOffset",float) = 0.0

        [Toggle]_MainAnimation("MainAnimation",float) = 0
        _MainSpeedU("MainSpeedU", Float) = 0
        _MainSpeedV("MainSpeedV", Float) = 0
        [Toggle]_MaskAnimation("MaskAnimation",float) = 0
        _MaskSpeedU("MaskSpeedU", Float) = 0
        _MaskSpeedV("MaskSpeedV", Float) = 0

        [Enum(Default,0,Additive,1,Multiply,2,SoftAdditive,3,2xMultiply,4)]
        _BlendModeEnum("_BlendModeEnum",float) = 0
        _SrcAlpha("SrcAlpha",float) = 5 //5,4,2,1 /5-10,4-1,2-0,1-1,6-1
        _DstAlpha("DstAlpha",float) = 10 //10,1,0,6
        
        _Stencil("Stencil ID", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("Stencil Comparison", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOp("Stencil Operation", Float) = 0
        //_StencilWriteMask("Stencil Write Mask", Float) = 255
        //_StencilReadMask("Stencil Read Mask", Float) = 255

        [Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull",float) = 2

        //[Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest", Float) = 4

    }
    SubShader
    {
        Tags
        {
            "LightMode" = "HRPForward"
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            //ReadMask[_StencilReadMask]
            //WriteMask[_StencilWriteMask]
        }
        Pass
        {

            Blend [_SrcAlpha] [_DstAlpha]
            Cull [_Cull]
            //ZTest[_ZTest]
            ZWrite off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _PACKUV
            #pragma shader_feature _USEMASK_ON
            #pragma shader_feature _MAINANIMATION_ON
            #pragma shader_feature _MASKANIMATION_ON

            #include "Packages/com.yoozoo.owl.rendering.hrp/RenderPipeline/ShaderLibrary/HPipelineData.hlsl"
            //#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal
            #include "GTA_H_FX_Common.hlsl"

            CBUFFER_START(UnityPerMaterial)
            half _ColorStrength;
            half _Pow;
            half _TimeModY;
            half _TimeCycle;
            half _TimeOffset;
            CBUFFER_END

            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;

                #if defined(_USEMASK_ON)
                float4 texcoord1 : TEXCOORD1;
                #endif

                half4 vertexColor : COLOR;
            };

            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                
                VERTEX_INPUT_PACK_UV(0)

                #if defined(_USEMASK_ON)
                VERTEX_INPUT_PACK_UV(1)
                float4 movingUV :TEXCOORD2;
                #else
                float2 movingUV :TEXCOORD2;
                #endif

                half4 color:TEXCOORD3;
            };

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.pos = TransformObjectToHClip(v.vertex);

                SETUP_PACK_UV(0, 0)

                #if defined(_USEMASK_ON)
                SETUP_PACK_UV(1, 1)
                #endif

                o.pos = vertDepthOffset(o.pos, _DepthOffset);
                
                o.movingUV.xy = v.texcoord0.xy;
                #if defined (_MAINANIMATION_ON)
                o.movingUV.xy = half2(_MainSpeedU, _MainSpeedV) * _Time.g + v.texcoord0.xy;
                #endif
                o.movingUV.xy = TRANSFORM_TEX(o.movingUV.xy, _MainTex);

                #if defined (_USEMASK_ON)
                o.movingUV.zw = v.texcoord1.xy;
                #if defined (_MASKANIMATION_ON)
                o.movingUV.zw = half2(_MaskSpeedU, _MaskSpeedV) * _Time.g + v.texcoord1.xy;
                #endif
                o.movingUV.zw = TRANSFORM_TEX(o.movingUV.zw, _MaskTex);
                #endif

                o.color = v.vertexColor * _Color;
                return o;
            }

            half4 frag(VertexOutput i) : SV_Target
            {
                float2 uvMain = i.movingUV.xy;
                APPLY_PACK_UV(uvMain, 0)

                #if defined (_USEMASK_ON)
                float2 uvMask = i.movingUV.zw;
                APPLY_PACK_UV(uvMask, 1)
                half4 _MaskTex_var = tex2D(_MaskTex, uvMask);
                #endif
                
                half4 _MainTexVar = tex2D(_MainTex, uvMain);

                half3 finalColor = _MainTexVar.rgb * i.color.rgb;
                finalColor = saturate(pow(finalColor * _ColorStrength, _Pow));
                
                #if defined(_USEMASK_ON)
                half finalAlpha = _MainTexVar.a * i.color.a * _MaskTex_var.r;
                #else
                half finalAlpha = _MainTexVar.a * i.color.a;
                #endif

                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
    CustomEditor"ShaderGUIDefault"
}