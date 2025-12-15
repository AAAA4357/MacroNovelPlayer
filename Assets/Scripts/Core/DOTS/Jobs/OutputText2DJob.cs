using MNP.Core.DataStruct;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[WithAll(typeof(BakeReadyComponent), typeof(Text2DComponent))]
public partial struct OutputText2DJob : IJobEntity
{
    [ReadOnly]
    public NativeArray<float4> PropertyArray;

    public void Execute(in ElementComponent elementComponent, PropertyStringComponent stringComponent, EnabledRefRO<TimeEnabledComponent> timeEnabledComponent)
    {
        if (!timeEnabledComponent.ValueRO)
        {
            return;
        }
        stringComponent.OutputText.transform.localPosition = PropertyArray[elementComponent.TransformPositionIndex].xyz;
        stringComponent.OutputText.transform.localRotation = Quaternion.Euler(0, 0, PropertyArray[elementComponent.TransformRotationIndex].x);
        stringComponent.OutputText.transform.localScale = new(PropertyArray[elementComponent.TransformScaleIndex].x,
                                                              PropertyArray[elementComponent.TransformScaleIndex].y, 
                                                              1);
        stringComponent.OutputText.text = stringComponent.Value;
    }
}
