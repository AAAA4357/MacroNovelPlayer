using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithPresent(typeof(RotTransform2DInterruptComponent))]
    public partial struct ResumeAllRotTransform2DInterruptJob : IJobEntity
    {
        public void Execute(EnabledRefRW<RotTransform2DInterruptComponent> interruptComponent)
        {
            interruptComponent.ValueRW = false;
        }
    }
}
