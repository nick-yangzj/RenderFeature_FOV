using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraFovRenderFeature : ScriptableRendererFeature
{
    public CameraFovPassSetting setting = new CameraFovPassSetting();
    private CameraFovRenderPass _scriptablePass;

    public override void Create()
    {
        _scriptablePass = new CameraFovRenderPass(setting);
    }
    
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // _scriptablePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(_scriptablePass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        _scriptablePass.Setup(renderer.cameraColorTarget);
    }
}
