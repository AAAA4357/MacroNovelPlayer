using MNP.Core.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[WithAll(typeof(BakeReadyComponent))]
public partial struct Output2DJob : IJobEntity
{
    public NativeList<float4x4>.ParallelWriter ListWriter;

    public void Execute(in ElementComponent elementComponent, EnabledRefRO<TimeEnabledComponent> timeEnabledComponent)
    {
        if (!timeEnabledComponent.ValueRO)
        {
            return;
        }
        ListWriter.AddNoResize(elementComponent.TransformMatrix);
    }
}
