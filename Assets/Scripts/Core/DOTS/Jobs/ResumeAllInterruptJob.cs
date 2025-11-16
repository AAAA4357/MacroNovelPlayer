using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct ResumeAllInterruptJob : IJobEntity
    {
        public void Execute(EnabledRefRW<InterruptComponent> interruptComponent)
        {
            interruptComponent.ValueRW = false;
        }
    }
}
