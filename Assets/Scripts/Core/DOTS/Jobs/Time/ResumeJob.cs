using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct ResumeJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<uint> IDArray;

        public void Execute(in ElementComponent element, EnabledRefRW<InterruptComponent> interruptComponent)
        {
            if (IDArray.Contains(element.ID))
            {
                interruptComponent.ValueRW = false;
            }
        }
    }
}
