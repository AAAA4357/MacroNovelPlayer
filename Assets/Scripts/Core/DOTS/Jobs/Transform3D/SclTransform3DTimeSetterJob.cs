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
    [WithPresent(typeof(SclTransform3DInterruptComponent))]
    public partial struct SclTransform3DTimeSetterJob : IJobEntity
    {
        public float DeltaValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<SclTransform3DInterruptTimeComponent> sclInterruptTimeBuffer, ref SclTransform3DTimeComponent timeComponent, EnabledRefRW<SclTransform3DInterruptComponent> interruptComponent)
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
                    SclTransform3DInterruptTimeComponent component = sclInterruptTimeBuffer[i];
                    component.Interrupted = true;
                    sclInterruptTimeBuffer[i] = component;
                    timeComponent.InterrputedTime++;
                }
            }
        }
    }
}
