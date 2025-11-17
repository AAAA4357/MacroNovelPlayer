using MNP.Core.DOTS.Components.Transform3D;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs.Transform3D
{
    [BurstCompile]
    [WithPresent(typeof(RotTransform3DInterruptComponent))]
    public partial struct ResumeAllRotTransform3DInterruptJob : IJobEntity
    {
        public void Execute(EnabledRefRW<RotTransform3DInterruptComponent> interruptComponent)
        {
            interruptComponent.ValueRW = false;
        }
    }
}
