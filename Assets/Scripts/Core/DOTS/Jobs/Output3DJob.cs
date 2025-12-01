using MNP.Core.DataStruct;
using MNP.Core.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[WithAll(typeof(BakeReadyComponent))]
public partial struct Output3DJob : IJobEntity
{
    public NativeList<float4x4>.ParallelWriter ListWriter;

    public void Execute(in ElementComponent elementComponent, EnabledRefRO<TimeEnabledComponent> timeEnabledComponent)
    {
        if (!timeEnabledComponent.ValueRO || elementComponent.ObjectType != ObjectType.Object3D)
        {
            return;
        }
        ListWriter.AddNoResize(elementComponent.TransformMatrix);
    }
}
