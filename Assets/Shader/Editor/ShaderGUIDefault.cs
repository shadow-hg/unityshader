using System;
using UnityEditor;
using UnityEngine;

/* 通用CustomShaderGUI:
 * 
 * shader需要符合以下属性命名规范，以下GUI才会生效
 *
 * 颜色：_Color
 * 亮度：_ColorStrength
 * 主贴图: _MainTex
 * 遮罩：_MaskTex
 * 指数：_Pow
 * 深度偏移：_DepthOffset
 * 是否使用mask贴图：[Toggle]_UseMask
 * 主贴图是否开启UV动画：_MainAnimation
 * mask贴图是否开启UV动画：_MaskAnimation
 * 混合模式：_SrcAlpha
 *         _DstAlpha
 * 模板测试：
 *      _Stencil
 *      _StencilComp
 *      _StencilOp
 * 剔除：_Cull
 * 
 */

public class ShaderGUIDefault : ShaderGUI
{
    public MaterialEditor materialEditor;
    public MaterialProperty[] properties;
    public Material targetMat;

    private bool useMaskBool = false;
    private bool maskBool = false;
    private bool animBool = false;
    private bool steniBool = false;
    private bool blendBool = false;
    private bool maskAdditiveBool = false;
    public bool showHelpBool = false;
    private bool showAlphaTestBool = false;

    private GUIStyle TittleStyle;
    private GUIStyle LabStyle;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.materialEditor = materialEditor;
        this.properties = properties;
        targetMat = materialEditor.target as Material;
        GUIStyles();//控制全局各个模块的样式
        RenderType();
        ShowTex();
        ShowMask();
        ShowColorSetting();
        ShowFresnel();
        ShowBlendMode();
        ShowCull();
        ShowAnim();
        ShowStencil();
        ShowAlphaTest();
        materialEditor.RenderQueueField();
        ShowHelp();
        
    }
    
    void GUIStyles()
    {
        TittleStyle = new GUIStyle();
        TittleStyle.fontStyle = FontStyle.Bold;
        TittleStyle.fontSize = 12;
        TittleStyle.normal.textColor = Color.cyan;
        
        LabStyle = new GUIStyle();
        LabStyle.fontSize = 10;
        LabStyle.normal.textColor = Color.gray;
    }

    void RenderType()
    {
        if (targetMat.HasProperty("_RenderType"))
        {
            MaterialProperty renderType = FindProperty("_RenderType", properties);
            materialEditor.ShaderProperty(renderType, "渲染类型：");

            switch (renderType.floatValue)
            {
                case 0:
                    targetMat.renderQueue = 2000;
                    targetMat.SetFloat("_ZTest",4.0f);
                    targetMat.SetFloat("_ZWrite",4.0f);
                    break;
                case 1:
                    targetMat.renderQueue = 2450;
                    targetMat.SetFloat("_ZTest",4.0f);
                    targetMat.SetFloat("_ZWrite",4.0f);
                    break;
                case 2:
                    targetMat.renderQueue = 3000;
                    targetMat.SetFloat("_ZTest",4.0f);
                    targetMat.SetFloat("_ZWrite",0.0f);
                    break;
                case 3:
                    //自定义
                    break;
            }
        }
    }

    void ShowColorSetting()
    {
        if (targetMat.HasProperty("_Color"))
        {
            MaterialProperty color = FindProperty("_Color", properties);
            materialEditor.ShaderProperty(color, "Color：");
        }

        if (useMaskBool)
        {
            if (maskAdditiveBool)
            {
                if (targetMat.HasProperty("_MaskColor"))
                {
                    EditorGUILayout.LabelField("注：使用Mask颜色叠加时，两个贴图颜色只能通过R通道和Color参数控制。",LabStyle);
                    MaterialProperty maskColor = FindProperty("_MaskColor", properties);
                    materialEditor.ShaderProperty(maskColor, "Mask颜色：");
                }
            }
        }

        if (targetMat.HasProperty("_ColorStrength"))
        {
            MaterialProperty colorStr = FindProperty("_ColorStrength", properties);
            materialEditor.ShaderProperty(colorStr, "亮度：");
        }

        if (targetMat.HasProperty("_Pow"))
        {
            MaterialProperty pow = FindProperty("_Pow", properties);
            materialEditor.ShaderProperty(pow, "曝光：");
        }

        if (targetMat.HasProperty("_DepthOffset"))
        {
            MaterialProperty depthOffset = FindProperty("_DepthOffset", properties);
            materialEditor.ShaderProperty(depthOffset, "深度偏移：");
        }
    }

    void ShowTex()
    {
        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        materialEditor.ShaderProperty(mainTex, "MainTex");
    }

    void ShowMask()
    {
        if (targetMat.HasProperty("_UseMask")) //如果shader里有是否使用mask的toggle则显示这个toggle
        {
            MaterialProperty useMask = FindProperty("_UseMask", properties);
            materialEditor.ShaderProperty(useMask, "Mask贴图：");
            if (useMask.floatValue == 1)
            {
                useMaskBool = true;
                
                if (useMaskBool)
                {
                    if (targetMat.HasProperty("_MaskAdditive"))
                    {
                        MaterialProperty maskAdditive = FindProperty("_MaskAdditive", properties);
                        materialEditor.ShaderProperty(maskAdditive, "Mask颜色叠加：");
                        if (maskAdditive.floatValue == 1)
                        {
                            maskAdditiveBool = true;
                        }
                        else
                        {
                            maskAdditiveBool = false;
                        }
                    }
                }
                
                if (targetMat.HasProperty("_AlphaClip"))
                {
                    MaterialProperty alphaClip = FindProperty("_AlphaClip", properties);
                    materialEditor.ShaderProperty(alphaClip, "溶解效果");
                    if (alphaClip.floatValue == 1)
                    {
                        EditorGUILayout.LabelField("开启溶解效果后，请将控制溶解的纹理添加到Mask的G通道,并设置“溶解率”参数",LabStyle);
                        
                        if (targetMat.HasProperty("_Cutoff"))
                        {
                            MaterialProperty clip = FindProperty("_Cutoff", properties);
                            materialEditor.ShaderProperty(clip, "溶解率");
                        }
                    }
                }

                if (targetMat.HasProperty("_MaskTex"))
                {
                    MaterialProperty maskTex = FindProperty("_MaskTex", properties);
                    materialEditor.ShaderProperty(maskTex, "MaskTex");
                }
                else
                {
                    EditorGUILayout.LabelField("本Shader没有MaskTex功能，请使用UIDefault Shader",LabStyle);
                }
            }
            else
            {
                useMaskBool = false;
            }
        }
        else //如果shader里没有使用mask的toggle则不显示这个toggle
        {
            useMaskBool = false;

            if (targetMat.HasProperty("_MaskTex"))
            {
                MaterialProperty maskTex = FindProperty("_MaskTex", properties);
                materialEditor.ShaderProperty(maskTex, "MaskTex");
            }
        }
    }

    void ShowFresnel()
    {
        if (targetMat.HasProperty("_FresnelBool"))
        {
            MaterialProperty fresnel = FindProperty("_FresnelBool", properties);
            materialEditor.ShaderProperty(fresnel, "菲涅尔效果");

            if (fresnel.floatValue == 1)
            {
                if (targetMat.HasProperty("_FresnelCol"))
                {
                    MaterialProperty fresnelCol = FindProperty("_FresnelCol", properties);
                    materialEditor.ShaderProperty(fresnelCol, "菲涅尔颜色");
                }
                if (targetMat.HasProperty("_FresnelRange"))
                {
                    MaterialProperty fresnelRange = FindProperty("_FresnelRange", properties);
                    materialEditor.ShaderProperty(fresnelRange, "菲涅尔范围");
                }
                if (targetMat.HasProperty("_FresnelMin"))
                {
                    MaterialProperty fresnelMin = FindProperty("_FresnelMin", properties);
                    materialEditor.ShaderProperty(fresnelMin, "菲涅尔强度");
                }
                if (targetMat.HasProperty("_RimFallOff"))
                {
                    MaterialProperty rimFallOff = FindProperty("_RimFallOff", properties);
                    materialEditor.ShaderProperty(rimFallOff, "菲涅尔指数");
                }
            }
        }
        
    }

    void ShowAnim()
    {
        animBool = EditorGUILayout.BeginFoldoutHeaderGroup(animBool, "UV动画");
        if (animBool)
        {
            EditorGUILayout.LabelField("(值越大速度越快)");

            if (targetMat.HasProperty("_MainAnimation"))
            {
                MaterialProperty mainAnimation = FindProperty("_MainAnimation", properties);
                materialEditor.ShaderProperty(mainAnimation, "MainSpeed");
                if (mainAnimation.floatValue == 1)
                {
                    MaterialProperty MainSpeedU = FindProperty("_MainSpeedU", properties);

                    materialEditor.ShaderProperty(MainSpeedU, "主图X轴位移：");
                    MaterialProperty MainSpeedV = FindProperty("_MainSpeedV", properties);
                    materialEditor.ShaderProperty(MainSpeedV, "主图Y轴位移：");
                }
            }

            if (useMaskBool)
            {
                if (targetMat.HasProperty("_MaskAnimation"))
                {
                    MaterialProperty maskAnimation = FindProperty("_MaskAnimation", properties);
                    materialEditor.ShaderProperty(maskAnimation, "MaskSpeed");
                    if (maskAnimation.floatValue == 1)
                    {
                        MaterialProperty MaskSpeedU = FindProperty("_MaskSpeedU", properties);

                        materialEditor.ShaderProperty(MaskSpeedU, "Mask图X轴位移：");
                        MaterialProperty MaskSpeedV = FindProperty("_MaskSpeedV", properties);
                        materialEditor.ShaderProperty(MaskSpeedV, "Mask图Y轴位移：");
                    }
                }
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void ShowBlendMode()
    {
        if (targetMat.HasProperty("_BlendModeEnum"))
        {
            MaterialProperty blendmodeEnum = FindProperty("_BlendModeEnum", properties);
            materialEditor.ShaderProperty(blendmodeEnum, "混合模式");
            switch (blendmodeEnum.floatValue)
            {
                case 0:
                    targetMat.SetFloat("_SrcAlpha", 5);
                    targetMat.SetFloat("_DstAlpha", 10);
                    break;
                case 1:
                    targetMat.SetFloat("_SrcAlpha", 5);
                    targetMat.SetFloat("_DstAlpha", 1);
                    break;
                case 2:
                    targetMat.SetFloat("_SrcAlpha", 2);
                    targetMat.SetFloat("_DstAlpha", 0);
                    break;
                case 3:
                    targetMat.SetFloat("_SrcAlpha", 1);
                    targetMat.SetFloat("_DstAlpha", 4);
                    break;
                case 4:
                    targetMat.SetFloat("_SrcAlpha", 2);
                    targetMat.SetFloat("_DstAlpha", 3);
                    break;
            }
        }
    }

    void ShowStencil()
    {
        if (targetMat.HasProperty("_Stencil"))
        {
            steniBool = EditorGUILayout.BeginFoldoutHeaderGroup(steniBool, "模板测试");
            if (steniBool)
            {
            
                MaterialProperty stencilID = FindProperty("_Stencil", properties);
                materialEditor.ShaderProperty(stencilID, "Stencil ID");

                MaterialProperty stencilComp = FindProperty("_StencilComp", properties);
                materialEditor.ShaderProperty(stencilComp, "Stencil Comparison");
                MaterialProperty stencilOp = FindProperty("_StencilOp", properties);
                materialEditor.ShaderProperty(stencilOp, "Stencil Operation");
            
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    void ShowCull()
    {
        if (targetMat.HasProperty("_Cull"))
        {
            MaterialProperty cull = FindProperty("_Cull", properties);
            materialEditor.ShaderProperty(cull, "选择剔除");
        }
    }

    void ShowHelp()
    {
        EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------------------------------");
        showHelpBool = EditorGUILayout.BeginFoldoutHeaderGroup(showHelpBool, "查看帮助？");
        if (showHelpBool)
        {
            EditorGUILayout.LabelField("一 些 特 殊 参 数 的 作 用：",TittleStyle);
            EditorGUILayout.LabelField("曝光：默认值为1；0-1时暗部越亮；大于1时亮的地方越亮，暗的越暗.");
            EditorGUILayout.LabelField("深度偏移：默认值为0；小于0时显示在物体前面；大于1时会被物体遮挡；可用于特效与地面的接缝.");
            EditorGUILayout.LabelField("选择剔除：默认值为Back（剔除模型背面不渲染）；当特效或模型的内面永远不会被摄像机拍到时选择Back.");
            EditorGUILayout.LabelField("Mask贴图：开启Mask贴图后，除了可以使用Main贴图的a通道控制透明度，还可以使用mask贴图的 R 通道控制透明度");
            EditorGUILayout.LabelField("Mask颜色叠加：开启后Main贴图的R通道配合Color参数控制第一层颜色，Mask贴图的R通道配合Mask参数控制第二层颜色");
            EditorGUILayout.LabelField("如果需要使用溶解效果，请先开启Mask贴图，并将控制溶解的纹理放置在Mask的G通道。");
            if (targetMat.HasProperty("_Stencil"))
            {
                EditorGUILayout.LabelField("模板测试：默认值为0,Always,Keep可用于UI的裁剪.");
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void ShowAlphaTest()
    {
        if (targetMat.HasProperty("_ZWrite"))
        {
            showAlphaTestBool = EditorGUILayout.BeginFoldoutHeaderGroup(showAlphaTestBool, "AlphaTest");
            if (showAlphaTestBool)
            {
                EditorGUILayout.LabelField("注意：非特殊情况请勿修改以下两个设置！",LabStyle);
                EditorGUILayout.LabelField("出现透明物体渲染顺序错误时可以修改，具体请寻找TA。",LabStyle);
            
                if (targetMat.HasProperty("_ZTest"))
                {
                    MaterialProperty zTest = FindProperty("_ZTest", properties);
                    materialEditor.ShaderProperty(zTest, "深度测试");
                }
        
                if (targetMat.HasProperty("_ZWrite"))
                {
                    MaterialProperty zWrite = FindProperty("_ZWrite", properties);
                    materialEditor.ShaderProperty(zWrite, "深度写入");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}