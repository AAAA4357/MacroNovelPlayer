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
    [WithPresent(typeof(PosTransform2DInterruptComponent))]
    public partial struct PosTransform2DTimeSetterJob : IJobEntity
    {
        public float DeltaValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<PosTransformInterruptTimeComponent> posInterruptTimeBuffer, ref PosTransform2DTimeComponent timeComponent, EnabledRefRW<PosTransform2DInterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO)
            {
                return;
            }
            timeComponent.Time += DeltaValue;
            for (int i = 0; i < posInterruptTimeBuffer.Length; i++)
            {
                if (!posInterruptTimeBuffer[i].Interrupted &&
                    Mathf.Abs(timeComponent.Time - posInterruptTimeBuffer[i].InterruptTime) < UtilityHelper.InterruptTorloance)
                {
                    interruptComponent.ValueRW = true;
                    PosTransformInterruptTimeComponent component = posInterruptTimeBuffer[i];
                    component.Interrupted = true;
                    posInterruptTimeBuffer[i] = component;
                    timeComponent.InterrputedTime++;
                }
            }
        }
    }
}
