using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class CommonPassSetting
{
    public RenderObjects.FilterSettings filterSettings = new RenderObjects.FilterSettings();
    public RenderPassEvent passEvent;
    public Material overrideMaterial;
    public bool clearRenderTarget;
    public string profilingTag = string.Empty;
}

[Serializable]
public class CameraFovSettings
{
    public bool overrideCamera;
    // public bool restoreCamera = true;
    public float cameraFieldOfView = 60.0f;
}

[Serializable]
public class CameraFovPassSetting: CommonPassSetting
{
    public CameraFovSettings cameraSetting;
}

public class CameraFovRenderPass : ScriptableRenderPass
{
    private readonly CameraFovPassSetting _setting;
    private FilteringSettings _filteringSettings;
    private CommandBuffer _cmd;
    private RenderTargetIdentifier destination { get; set; }

    public CameraFovRenderPass(CameraFovPassSetting setting)
    {
        _setting = setting;
        profilingSampler = new ProfilingSampler(_setting.profilingTag);
        _filteringSettings =new FilteringSettings( setting.filterSettings.RenderQueueType == RenderQueueType.Opaque? RenderQueueRange.opaque: RenderQueueRange.transparent, setting.filterSettings.LayerMask);
        renderPassEvent = setting.passEvent;
    }
    
    public void Setup(RenderTargetIdentifier destId) 
    {
        destination = destId;
    }
    
    /// <summary>
    /// This method is called by the renderer before executing the render pass.
    /// Override this method if you need to to configure render targets and their clear state, and to create temporary render target textures.
    /// If a render pass doesn't override this method, this render pass renders to the active Camera's render target.
    /// You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    /// </summary>
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        _cmd = cmd;
        _cmd.name = _setting.profilingTag;
        ConfigureTarget(destination);
        
        if (_setting.clearRenderTarget)
            ConfigureClear(ClearFlag.All, new Color(0, 0, 0, 0));
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var sortingCriteria = SortingCriteria.CommonOpaque;
        var drawingSettings = CreateDrawingSettings(RenderManager.GetInstance().SetPassTags(), ref renderingData, sortingCriteria);
        var cameraAspect = renderingData.cameraData.camera.aspect;
            
        ref CameraData cameraData = ref renderingData.cameraData;
        if (_setting.cameraSetting.overrideCamera)
        {
            _cmd.Clear();
            var camera = cameraData.camera;
            var projectionMatrix = Matrix4x4.Perspective(_setting.cameraSetting.cameraFieldOfView,
                cameraAspect,
                camera.nearClipPlane, camera.farClipPlane);
            projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix,
                cameraData.IsCameraProjectionMatrixFlipped());
                
            var viewMatrix = cameraData.GetViewMatrix();
            RenderingUtils.SetViewAndProjectionMatrices(_cmd, viewMatrix, projectionMatrix, false);
                
            context.ExecuteCommandBuffer(_cmd);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
        }
        else
        {
            _cmd.Clear();
            RenderingUtils.SetViewAndProjectionMatrices(_cmd, cameraData.GetViewMatrix(),
                cameraData.GetGPUProjectionMatrix(), false);
                
            context.ExecuteCommandBuffer(_cmd);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
        }
    }
}
