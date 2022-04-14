using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class HLSettings
{
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    public Material mMat;
    public GraphicsBuffer.Target destination = GraphicsBuffer.Target.Index;
    public int blitMaterialPassIndex = -1;
    //这是一个shader的propertyId
    public string textureId = "_ScreenTexture";
    public float contrast = 0.5f;
}
//public HLSettings settings = new HLSettings();

