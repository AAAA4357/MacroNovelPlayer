using UnityEngine;
using UnityEngine.Rendering;

public class MNPRenderPipeline : RenderPipeline
{
    const string CommandBufferName = "MNP Renderer";

    static ShaderTagId unlitShaderTagId = new ShaderTagId("Opaque");

    CommandBuffer commandBuffer = new CommandBuffer()
    {
        name = CommandBufferName
    };

    CullingResults cullingResults;

    private ScriptableRenderContext context;

    private Camera camera;

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        this.context = context;
        camera = cameras[0];

        if (!Cull())
        {
            return;
        }

        Setup();

        DrawVisibleGeometry();

        Submit();
    }

    private void Setup()
    {
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.BeginSample(CommandBufferName);
        ExecuteBuffer();
        context.SetupCameraProperties(camera);
    }

    private void Submit()
    {
        commandBuffer.EndSample(CommandBufferName);
        ExecuteBuffer();
        context.Submit();
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }

    bool Cull()
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            cullingResults = context.Cull(ref p);
            return true;
        }
        return false;
    }

    void DrawVisibleGeometry()
    {
        var sortingSettings = new SortingSettings(camera);
        var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.all);
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }
}
