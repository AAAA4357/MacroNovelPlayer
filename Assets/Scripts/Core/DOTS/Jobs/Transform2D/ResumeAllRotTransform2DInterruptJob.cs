using MNP.Core.DOTS.Components.Transform2D;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs.Transform2D
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
