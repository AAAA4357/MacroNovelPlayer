using MNP.Core.DOTS.Components;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    [WithNone(typeof(LoopTimeComponent))]
    [WithPresent(typeof(SclTransform2DInterruptComponent))]
    public partial struct SclTransform2DTimeSetterJob : IJobEntity
    {
        public float DeltaValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<SclTransformInterruptTimeComponent> sclInterruptTimeBuffer, ref SclTransform2DTimeComponent timeComponent, EnabledRefRW<SclTransform2DInterruptComponent> interruptComponent)
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
                    SclTransformInterruptTimeComponent component = sclInterruptTimeBuffer[i];
                    component.Interrupted = true;
                    sclInterruptTimeBuffer[i] = component;
                    timeComponent.InterrputedTime++;
                }
            }
        }
    }
}
