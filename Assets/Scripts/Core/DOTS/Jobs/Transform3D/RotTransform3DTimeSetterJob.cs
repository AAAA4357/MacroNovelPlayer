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
    [WithPresent(typeof(RotTransform3DInterruptComponent))]
    public partial struct RotTransform3DTimeSetterJob : IJobEntity
    {
        public float DeltaValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<RotTransform3DInterruptTimeComponent> rotInterruptTimeBuffer, ref RotTransform3DTimeComponent timeComponent, EnabledRefRW<RotTransform3DInterruptComponent> interruptComponent)
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
                    RotTransform3DInterruptTimeComponent component = rotInterruptTimeBuffer[i];
                    component.Interrupted = true;
                    rotInterruptTimeBuffer[i] = component;
                    timeComponent.InterrputedTime++;
                }
            }
        }
    }
}
