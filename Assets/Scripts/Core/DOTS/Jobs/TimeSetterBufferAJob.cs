using MNP.Core.DOTS.Components;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent), typeof(PipelineBufferAComponent))]
    [WithNone(typeof(PipelineBufferBComponent))]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct TimeSetterBufferAJob : IJobEntity
    {
        public float TargetValue;

        [BurstCompile]
        public void Execute(DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer, ref TimeComponent timeComponent, EnabledRefRW<InterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO)
            {
                return;
            }
            timeComponent.Time = TargetValue;
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
