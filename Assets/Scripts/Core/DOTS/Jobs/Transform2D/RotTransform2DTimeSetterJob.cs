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
    [WithNone(typeof(LoopTimeComponent))]
    [WithPresent(typeof(RotTransform2DInterruptComponent))]
    public partial struct RotTransform2DTimeSetterJob : IJobEntity
    {
        public float DeltaValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<RotTransform2DInterruptTimeComponent> rotInterruptTimeBuffer, ref RotTransform2DTimeComponent timeComponent, EnabledRefRW<RotTransform2DInterruptComponent> interruptComponent)
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
                    RotTransform2DInterruptTimeComponent component = rotInterruptTimeBuffer[i];
                    component.Interrupted = true;
                    rotInterruptTimeBuffer[i] = component;
                    timeComponent.InterrputedTime++;
                }
            }
        }
    }
}
