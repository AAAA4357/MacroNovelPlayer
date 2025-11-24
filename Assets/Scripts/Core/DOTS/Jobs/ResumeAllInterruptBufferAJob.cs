using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(PipelineBufferAComponent))]
    [WithNone(typeof(PipelineBufferBComponent))]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct ResumeAllInterruptBufferAJob : IJobEntity
    {
        public void Execute(EnabledRefRW<InterruptComponent> interruptComponent)
        {
            interruptComponent.ValueRW = false;
        }
    }
}
