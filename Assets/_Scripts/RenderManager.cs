using System.Collections.Generic;
using UnityEngine.Rendering;

public class RenderManager
{
    private static RenderManager _instance;

    private List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();
    public static RenderManager GetInstance()
    {
        if(_instance == null)
            _instance = new RenderManager();
        
        return _instance;
    }

    public List<ShaderTagId> SetPassTags()
    {
        _shaderTagIdList.Clear();
        _shaderTagIdList.Add(new ShaderTagId( "Always" ));
        _shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
        _shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
        _shaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
        _shaderTagIdList.Add(new ShaderTagId("LightweightForward"));
        return _shaderTagIdList;
    }
}
