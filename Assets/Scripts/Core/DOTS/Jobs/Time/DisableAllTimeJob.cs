using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithPresent(typeof(TimeEnabledComponent))]
    public partial struct DisableAllTimeJob : IJobEntity
    {
        public void Execute(EnabledRefRW<TimeEnabledComponent> timeEnabledComponent)
        {
            timeEnabledComponent.ValueRW = false;
        }
    }
}
