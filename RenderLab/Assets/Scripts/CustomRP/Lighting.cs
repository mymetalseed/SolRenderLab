using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    private const string bufferName = "Lighting";

    private static int
        dirLightCountId = Shader.PropertyToID("_DirectionalLightCount"),
        dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors"),
        dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

    private static Vector4[]
        dirLightColors = new Vector4[maxDirLightCount],
        dirLightDirections = new Vector4[maxDirLightCount];
    
    private CullingResults cullingResults;
    private const int maxDirLightCount = 4;

    private CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    public void Setup(ScriptableRenderContext context,CullingResults cullingResults)
    {
        this.cullingResults = cullingResults; 
        buffer.BeginSample(bufferName);
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    
    void SetupLights()
    {
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
        
        Light light = RenderSettings.sun;
        //direct set global value
        int dirLightCount = 0;
        for (int i = 0; i < visibleLights.Length; ++i)
        {
            VisibleLight visibleLight = visibleLights[i];
            if (visibleLight.lightType == LightType.Directional)
            {
                SetupDirectionalLight(dirLightCount++,ref visibleLight);
                if(dirLightCount > maxDirLightCount)
                    break;
            }
        }
        
        buffer.SetGlobalInt(dirLightCountId,visibleLights.Length);
        buffer.SetGlobalVectorArray(dirLightColorsId,dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId,dirLightDirections);
    }
    
    /// <summary>
    /// the VisibleLight struct is rather big.Ideally we retrieve it once
    /// from native array and don't also pass it as regular argument.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="visibleLight"></param>
    void SetupDirectionalLight(int index,ref VisibleLight visibleLight)
    {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }
}