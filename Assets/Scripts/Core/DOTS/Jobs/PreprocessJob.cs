using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    [WithPresent(typeof(LerpEnabledComponent))]
    public partial struct PreprocessJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(ref PropertyInfoComponent propertyInfoComponent, in TimeComponent timeComponent, EnabledRefRW<LerpEnabledComponent> lerpEnabledComponent)
        {
            if (timeComponent.Time < propertyInfoComponent.StartTime ||
                timeComponent.Time > propertyInfoComponent.EndTime)
            {
                lerpEnabledComponent.ValueRW = false;
            }
            else
            {
                lerpEnabledComponent.ValueRW = true;
            }
        }
    }
}
