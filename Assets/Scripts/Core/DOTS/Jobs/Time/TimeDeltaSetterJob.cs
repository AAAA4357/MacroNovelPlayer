using MNP.Core.DOTS.Components;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct TimeDeltaSetterJob : IJobEntity
    {
        public float DeltaValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer, ref TimeComponent timeComponent, EnabledRefRW<InterruptComponent> interruptComponent, EnabledRefRO<TimeEnabledComponent> _)
        {
            if (interruptComponent.ValueRO)
            {
                return;
            }
            timeComponent.Time += DeltaValue;
            for (int i = 0; i < interruptTimeBuffer.Length; i++)
            {
                if (!interruptTimeBuffer[i].Interrupted &&
                    (timeComponent.Time - interruptTimeBuffer[i].InterruptTime) < UtilityHelper.InterruptTorloance)
                {
                    interruptComponent.ValueRW = true;
                    InterruptTimeComponent component = interruptTimeBuffer[i];
                    component.Interrupted = true;
                    interruptTimeBuffer[i] = component;
                }
            }
        }
    }
}
