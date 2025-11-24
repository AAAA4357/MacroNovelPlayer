using MNP.Core.DOTS.Components;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent), typeof(PipelineBufferBComponent))]
    [WithNone(typeof(PipelineBufferAComponent))]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct TimeSetterBufferBJob : IJobEntity
    {
        public float TimeValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer, ref TimeComponent timeComponent, EnabledRefRW<InterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO)
            {
                return;
            }
            timeComponent.Time = TimeValue;
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
