using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.Transform3D;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace MNP.Core.DOTS.Jobs.Transform3D
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    [WithPresent(typeof(PosTransform3DInterruptComponent))]
    public partial struct PosTransform3DTimeSetterJob : IJobEntity
    {
        public float DeltaValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<PosTransform3DInterruptTimeComponent> posInterruptTimeBuffer, ref PosTransform3DTimeComponent timeComponent, EnabledRefRW<PosTransform3DInterruptComponent> interruptComponent)
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
                    PosTransform3DInterruptTimeComponent component = posInterruptTimeBuffer[i];
                    component.Interrupted = true;
                    posInterruptTimeBuffer[i] = component;
                    timeComponent.InterrputedTime++;
                }
            }
        }
    }
}
