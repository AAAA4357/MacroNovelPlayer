using MNP.Core.DataStruct;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[WithAll(typeof(BakeReadyComponent))]
public partial struct OutputText3DJob : IJobEntity
{
    [ReadOnly]
    public NativeArray<float4> PropertyArray;

    public void Execute(in ElementComponent elementComponent, PropertyStringComponent stringComponent, EnabledRefRO<TimeEnabledComponent> timeEnabledComponent)
    {
        if (!timeEnabledComponent.ValueRO|| elementComponent.ObjectType != ObjectType.Text3D)
        {
            return;
        }
        stringComponent.OutputText.transform.localPosition = PropertyArray[elementComponent.TransformPositionIndex].xyz;
        stringComponent.OutputText.transform.localRotation = (quaternion)PropertyArray[elementComponent.TransformRotationIndex];
        stringComponent.OutputText.transform.localScale = PropertyArray[elementComponent.TransformScaleIndex].xyz;
        stringComponent.OutputText.text = stringComponent.Value;
    }
}
