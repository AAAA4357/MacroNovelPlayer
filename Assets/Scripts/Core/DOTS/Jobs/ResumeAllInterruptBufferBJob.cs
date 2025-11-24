using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(PipelineBufferBComponent))]
    [WithNone(typeof(PipelineBufferAComponent))]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct ResumeAllInterruptBufferBJob : IJobEntity
    {
        public void Execute(EnabledRefRW<InterruptComponent> interruptComponent)
        {
            interruptComponent.ValueRW = false;
        }
    }
}
