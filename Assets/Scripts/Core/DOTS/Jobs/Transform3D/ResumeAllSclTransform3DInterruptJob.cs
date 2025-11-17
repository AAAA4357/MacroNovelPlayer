using MNP.Core.DOTS.Components.Transform3D;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs.Transform3D
{
    [BurstCompile]
    [WithPresent(typeof(SclTransform3DInterruptComponent))]
    public partial struct ResumeAllSclTransform3DInterruptJob : IJobEntity
    {
        public void Execute(EnabledRefRW<SclTransform3DInterruptComponent> interruptComponent)
        {
            interruptComponent.ValueRW = false;
        }
    }
}
