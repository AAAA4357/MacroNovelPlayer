using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithPresent(typeof(TimeEnabledComponent))]
    public partial struct EnableAllTimeJob : IJobEntity
    {
        public void Execute(EnabledRefRW<TimeEnabledComponent> timeEnabledComponent)
        {
            timeEnabledComponent.ValueRW = true;
        }
    }
}
