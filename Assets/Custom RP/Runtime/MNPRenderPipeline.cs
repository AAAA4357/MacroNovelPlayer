using UnityEngine;
using UnityEngine.Rendering;

public class MNPRenderPipeline : RenderPipeline
{
    MNPCameraRenderer renderer = new();

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            renderer.Render(context, cameras[i]);
        }
    }
}
