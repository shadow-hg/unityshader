using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomPass : ScriptableRenderPass
{
    public ComputeShader computeShader;
    private float intensity;
    private float threshold;
    private string profilerTag = "BloomPass";
    private RenderTargetIdentifier cameraTex;
    private RenderTextureDescriptor camSetting;
    private RenderTextureDescriptor temp1Descriptor;
    private RenderTargetHandle temp1Handle;
    private RenderTargetHandle temp2Handle;

    public BloomPass(RenderPassEvent renderPassEvent,ComputeShader computeShader,float intensity,float threshold)
    {
        this.renderPassEvent = renderPassEvent;
        this.computeShader = computeShader;
        this.intensity = intensity;
        this.threshold = threshold;
        temp1Handle.Init("Temp Handle 1");
        temp1Handle.Init("Temp Handle 2");
    }

    public void SetUp(RenderTargetIdentifier cameraTarget)
    {
        this.cameraTex = cameraTarget;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor camSetting)
    {
        this.camSetting = camSetting;
        camSetting.enableRandomWrite = true;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
        camSetting.enableRandomWrite = true;
        float skip = 0.5f;
        cmd.GetTemporaryRT(temp1Handle.id,camSetting);
        cmd.SetComputeTextureParam(computeShader,0,"Result",temp1Handle.Identifier());
        cmd.SetComputeTextureParam(computeShader,0,"Source",cameraTex);
        cmd.SetComputeIntParam(computeShader,"width",camSetting.width);
        cmd.SetComputeIntParam(computeShader,"height",camSetting.height);
        cmd.SetComputeFloatParam(computeShader,"skip",skip);
        
        cmd.DispatchCompute(computeShader,0,camSetting.height/8,camSetting.width/8,1);
        cmd.Blit(temp1Handle.Identifier(),cameraTex);
        cmd.ReleaseTemporaryRT(temp1Handle.id);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
