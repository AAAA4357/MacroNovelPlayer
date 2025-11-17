using MNP.Core.DOTS.Components.Transform2D;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs.Transform2D
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
