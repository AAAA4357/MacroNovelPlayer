using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.Transform2D;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace MNP.Core.DOTS.Jobs.Transform2D
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    [WithPresent(typeof(SclTransform2DInterruptComponent))]
    public partial struct SclTransform2DTimeSetterJob : IJobEntity
    {
        public float DeltaValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<SclTransform2DInterruptTimeComponent> sclInterruptTimeBuffer, ref SclTransform2DTimeComponent timeComponent, EnabledRefRW<SclTransform2DInterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO)
            {
                return;
            }
            timeComponent.Time += DeltaValue;
            for (int i = 0; i < sclInterruptTimeBuffer.Length; i++)
            {
                if (!sclInterruptTimeBuffer[i].Interrupted &&
                    Mathf.Abs(timeComponent.Time - sclInterruptTimeBuffer[i].InterruptTime) < UtilityHelper.InterruptTorloance)
                {
                    interruptComponent.ValueRW = true;
                    SclTransform2DInterruptTimeComponent component = sclInterruptTimeBuffer[i];
                    component.Interrupted = true;
                    sclInterruptTimeBuffer[i] = component;
                    timeComponent.InterrputedTime++;
                }
            }
        }
    }
}
