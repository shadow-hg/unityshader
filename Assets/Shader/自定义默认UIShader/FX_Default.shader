Shader "HRP/GGame/FX/FX_Default"
{
    Properties
    {
        [Enum(Geometry,0,AlphaTest,1,Transparent,2,Custom,3)] _RenderType("RenderType",float) = 2
        
        _MainTex("MainTex", 2D) = "white" {}

        [Toggle]_UseMask("UseMask",int) = 0
        _MaskTex("MaskTex", 2D) = "white" {}

        _Color("Color",Color) = (1,1,1,1)
        _ColorStrength("ColorStrength",float) = 1.0
        _Pow("Pow",float) = 1.0
        [Toggle]_MaskAdditive("MaskAdditive",int) = 0
        _MaskColor("MaskColor",color) = (1,1,1,1)
        _DepthOffset("DepthOffset",float) = 0.0

        [Toggle]_MainAnimation("MainAnimation",int) = 0
        _MainSpeedU("MainSpeedU", Float) = 0
        _MainSpeedV("MainSpeedV", Float) = 0
        [Toggle]_MaskAnimation("MaskAnimation",int) = 0
        _MaskSpeedU("MaskSpeedU", Float) = 0
        _MaskSpeedV("MaskSpeedV", Float) = 0

        [Toggle]_FresnelBool("FresnelBool",int) = 0
        _FresnelCol("FresnelColor",color) = (1,1,1,1)
        _FresnelRange("FresnelRange",float) = 1.0
        _FresnelMin("FresnelMin",float) = 0.0
        _RimFallOff("RimFallOff",float) = 1.0

        [Toggle]_AlphaClip("AlphaClip",int) = 0
        _Cutoff("Clip",range(0,1.0)) = 0.0

        [Enum(Default,0,Additive,1,Multiply,2,SoftAdditive,3,2xMultiply,4)]
        _BlendModeEnum("_BlendModeEnum",float) = 0
        _SrcAlpha("SrcAlpha",float) = 5 //5,4,2,1 /5-10,4-1,2-0,1-1,6-1
        _DstAlpha("DstAlpha",float) = 10 //10,1,0,6
        
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull",int) = 2

        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZWrite("ZWrite", Float) = 0

    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        Pass
        {
            Tags
            {
                "LightMode" = "HRPForward"
                "IgnoreProjector" = "True"
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
            }

            Blend [_SrcAlpha] [_DstAlpha]
            Cull [_Cull]
            ZTest[_ZTest]
            ZWrite [_ZWrite]

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                //ReadMask[_StencilReadMask]
                //WriteMask[_StencilWriteMask]
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _PACKUV
            #pragma shader_feature _USEMASK_ON
            #pragma shader_feature _MAINANIMATION_ON
            #pragma shader_feature _MASKANIMATION_ON
            #pragma shader_feature _MASKADDITIVE_ON
            #pragma shader_feature _FRESNELBOOL_ON
            #pragma shader_feature _ALPHACLIP_ON

            #include "Packages/com.yoozoo.owl.rendering.hrp/RenderPipeline/ShaderLibrary/HPipelineData.hlsl"
            //#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal
            #include "GTA_H_FX_Common.hlsl"

            CBUFFER_START(UnityPerMaterial)
            half _TimeOffset;
            half _TimeModY;
            half _TimeCycle;

            CBUFFER_END

            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;

                #if defined(_USEMASK_ON)
                float4 texcoord1 : TEXCOORD1;
                #endif

                #if defined(_FRESNELBOOL_ON)
                float3 normal : NORMAL;
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

                #if defined(_FRESNELBOOL_ON)
                float3 ViewDir : TEXCOORD4;
                float3 WldNormal : TEXCOORD5;
                #endif
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

                #if defined(_FRESNELBOOL_ON)
                    //视角方向
                    half3 WldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.ViewDir = normalize(_WorldSpaceCameraPos - WldPos);
                    o.WldNormal = TransformObjectToWorldNormal(v.normal);
                #endif

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

                o.color = v.vertexColor;
                return o;
            }

            half4 frag(VertexOutput i) : SV_Target
            {
                i.color *= _Color;

                float2 uvMain = i.movingUV.xy;
                APPLY_PACK_UV(uvMain, 0)

                #if defined (_USEMASK_ON)
                    float2 uvMask = i.movingUV.zw;
                    APPLY_PACK_UV(uvMask, 1)
                    half4 _MaskTexVar = tex2D(_MaskTex, uvMask);
                #endif

                half4 _MainTexVar = tex2D(_MainTex, uvMain);

                float NdotV = 0;
                #if defined (_FRESNELBOOL_ON)
                    NdotV = saturate(dot(normalize(i.WldNormal), normalize(i.ViewDir)));
                    NdotV = (1 - NdotV) * _FresnelRange + _FresnelMin;
                    NdotV = pow(NdotV, _RimFallOff);
                #endif

                #if defined (_USEMASK_ON)
                    #if defined(_MASKADDITIVE_ON)//如果使用了Additive模式，mainTex的颜色需要通过材质面板去调，r通道控制颜色纹理
                        half3 finalColor = _MainTexVar.r * i.color.rgb + _MainTexVar.a * _MaskTexVar.r * _MaskColor.rgb;
                        finalColor = pow(finalColor * _ColorStrength, _Pow);

                    #else//如果没有使用使用Additive模式，颜色既可以先在ps软件中调好，也可以在材质面板调整
                        half3 finalColor = _MainTexVar.rgb * i.color.rgb;
                        finalColor = pow(finalColor * _ColorStrength, _Pow);
                    #endif
                #else
                    half3 finalColor = _MainTexVar.rgb * i.color.rgb;
                    finalColor = pow(finalColor * _ColorStrength, _Pow);
                #endif

                #if defined (_FRESNELBOOL_ON)
                    finalColor = lerp(finalColor, _FresnelCol, NdotV);
                #endif

                #if defined(_USEMASK_ON)
                    #if defined(_MASKADDITIVE_ON)
                        //当使用mask贴图作为叠加的第二层颜色纹理的话，BaseColor的r通道控制颜色范围，具体颜色需要在材质面板调整，不能通过ps先调整；
                        //BaseColor的g通道需要作为baseColor的Alpha通道来使用，baseColor的alpha作为mask的alpha来使用。
                        //为什么不把控制叠加在第一层上面的mask的alpha写在mask贴图的a通道里面，是因为要提高mask的复用率，如下
                        //哪种情况下需要同一个mask贴图在不同的范围内显示，就用那种情况的MainTex的a通道去控制mask的显示范围
                        half finalAlpha = _MainTexVar.r * i.color.a + _MainTexVar.a * _MaskTexVar.r * _MaskColor.a;
                    #else//如果mask只是普通的控制透明通道的作用，就是正常使用mainTex的a通道
                        half finalAlpha = _MainTexVar.a * i.color.a * _MaskTexVar.r;
                    #endif
                #else//如果完全不使用Mask贴图，那就用MainTex和Color的a通道去控制Alpha
                    half finalAlpha = _MainTexVar.a * i.color.a;
                #endif

                #if defined(_USEMASK_ON)//需要开启了mask功能才可以使用溶解效果
                    #if defined(_ALPHACLIP_ON)
                        clip(_MaskTexVar.g - _Cutoff);
                        finalAlpha = 1;
                    #endif
                #endif

                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
    CustomEditor"ShaderGUIDefault"
}