using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    public partial struct TimeSetterJob : IJobEntity
    {
        public float TargetValue;

        [BurstCompile]
        public void Execute(ref TimeComponent timeComponent, EnabledRefRO<TimeEnabledComponent> _)
        {
            timeComponent.Time = TargetValue;
        }
    }
}
