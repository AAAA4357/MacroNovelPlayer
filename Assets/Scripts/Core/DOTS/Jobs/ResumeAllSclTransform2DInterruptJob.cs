using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithPresent(typeof(SclTransform2DInterruptComponent))]
    public partial struct ResumeAllSclTransform2DInterruptJob : IJobEntity
    {
        public void Execute(EnabledRefRW<SclTransform2DInterruptComponent> interruptComponent)
        {
            interruptComponent.ValueRW = false;
        }
    }
}
