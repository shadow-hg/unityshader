using Gameplay.PVE.Skill;
using RuntimeInspectorNamespace;
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
    bool maskBool = false;
    bool animBool = false;
    bool steniBool = false;
    bool blendBool = false;

    public bool showHelp = false;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.materialEditor = materialEditor;
        this.properties = properties;
        targetMat = materialEditor.target as Material;
        ShowTex();
        ShowMask();
        ShowColorSetting();
        ShowBlendMode();
        ShowCull();
        ShowAnim();
        ShowStencil();
        materialEditor.RenderQueueField();
        ShowHelp();
    }

    void ShowColorSetting()
    {
        if (targetMat.HasProperty("_Color"))
        {
            MaterialProperty color = FindProperty("_Color", properties);
            materialEditor.ShaderProperty(color, "Color：");
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

                if (targetMat.HasProperty("_MaskTex"))
                {
                    MaterialProperty maskTex = FindProperty("_MaskTex", properties);
                    materialEditor.ShaderProperty(maskTex, "MaskTex");
                }
                else
                {
                    EditorGUILayout.LabelField("本Shader没有MaskTex功能，请使用UIDefault Shader");
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
                    materialEditor.ShaderProperty(MainSpeedV, "主图X轴位移：");
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
                        materialEditor.ShaderProperty(MaskSpeedV, "Mask图X轴位移：");
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
                    targetMat.SetFloat("_SrcAlpha", 1);
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
        showHelp = EditorGUILayout.BeginFoldoutHeaderGroup(showHelp, "查看帮助");
        if (showHelp)
        {
            EditorGUILayout.LabelField("一些特殊参数的作用：");
            EditorGUILayout.LabelField("曝光：默认值为1；0-1时暗部越亮；大于1时亮的地方越亮，暗的越暗.");
            EditorGUILayout.LabelField("深度偏移：默认值为0；小于0时显示在物体前面；大于1时会被物体遮挡；可用于特效与地面的接缝.");
            EditorGUILayout.LabelField("选择剔除：默认值为Back（剔除模型背面不渲染）；当特效或模型的内面永远不会被摄像机拍到时选择Back.");
            if (targetMat.HasProperty("_Stencil"))
            {
                EditorGUILayout.LabelField("模板测试：默认值为0,Always,Keep可用于UI的裁剪.");
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}