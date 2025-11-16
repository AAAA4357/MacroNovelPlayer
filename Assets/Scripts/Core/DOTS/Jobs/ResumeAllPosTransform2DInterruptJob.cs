using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithPresent(typeof(PosTransform2DInterruptComponent))]
    public partial struct ResumeAllPosTransform2DInterruptJob : IJobEntity
    {
        public void Execute(EnabledRefRW<PosTransform2DInterruptComponent> interruptComponent)
        {
            interruptComponent.ValueRW = false;
        }
    }
}
