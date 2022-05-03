using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RenderFeature_Bloom : ScriptableRendererFeature
{
    [System.Serializable]
    public struct BloomSetting
    {
        public RenderPassEvent Event;
        public ComputeShader computerShader;
        public float threshold;
        public float intensity;
    }

    public BloomSetting setting = new BloomSetting();
    private BloomPass bloomPass;

    public override void Create()
    {
        bloomPass = new BloomPass(setting.Event,setting.computerShader,setting.threshold,setting.intensity);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (setting.computerShader == null)
        {
            Debug.LogWarning("ComputerShader丢掉了！！！");
            return;
        }
        bloomPass.SetUp(renderer.cameraColorTarget);
        renderer.EnqueuePass(bloomPass);
    }
}
