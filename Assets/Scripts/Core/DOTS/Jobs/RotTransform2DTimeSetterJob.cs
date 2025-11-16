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
    [WithPresent(typeof(RotTransform2DInterruptComponent))]
    public partial struct RotTransform2DTimeSetterJob : IJobEntity
    {
        public float DeltaValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<RotTransformInterruptTimeComponent> rotInterruptTimeBuffer, ref RotTransform2DTimeComponent timeComponent, EnabledRefRW<RotTransform2DInterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO)
            {
                return;
            }
            timeComponent.Time += DeltaValue;
            for (int i = 0; i < rotInterruptTimeBuffer.Length; i++)
            {
                if (!rotInterruptTimeBuffer[i].Interrupted &&
                    Mathf.Abs(timeComponent.Time - rotInterruptTimeBuffer[i].InterruptTime) < UtilityHelper.InterruptTorloance)
                {
                    interruptComponent.ValueRW = true;
                    RotTransformInterruptTimeComponent component = rotInterruptTimeBuffer[i];
                    component.Interrupted = true;
                    rotInterruptTimeBuffer[i] = component;
                    timeComponent.InterrputedTime++;
                }
            }
        }
    }
}
