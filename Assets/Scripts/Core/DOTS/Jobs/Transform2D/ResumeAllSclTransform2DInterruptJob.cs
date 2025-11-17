using MNP.Core.DOTS.Components.Transform2D;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs.Transform2D
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
