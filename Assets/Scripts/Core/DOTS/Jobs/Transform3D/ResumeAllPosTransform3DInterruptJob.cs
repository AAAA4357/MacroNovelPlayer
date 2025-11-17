using MNP.Core.DOTS.Components.Transform3D;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs.Transform3D
{
    [BurstCompile]
    [WithPresent(typeof(PosTransform3DInterruptComponent))]
    public partial struct ResumeAllPosTransform3DInterruptJob : IJobEntity
    {
        public void Execute(EnabledRefRW<PosTransform3DInterruptComponent> interruptComponent)
        {
            interruptComponent.ValueRW = false;
        }
    }
}
