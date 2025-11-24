using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent), typeof(PipelineBufferAComponent))]
    [WithNone(typeof(PipelineBufferBComponent))]
    public partial struct PreprocessBufferAJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(ref PropertyInfoComponent propertyInfoComponent, in TimeComponent timeComponent, EnabledRefRW<LerpEnabledComponent> lerpEnabledComponent)
        {
            if (timeComponent.Time < propertyInfoComponent.StartTime ||
                timeComponent.Time > propertyInfoComponent.EndTime)
            {
                if (!propertyInfoComponent.LerpEnabled)
                {
                    return;
                }
                lerpEnabledComponent.ValueRW = false;
                propertyInfoComponent.LerpEnabled = false;
            }
            else
            {
                if (propertyInfoComponent.LerpEnabled)
                {
                    return;
                }
                lerpEnabledComponent.ValueRW = true;
                propertyInfoComponent.LerpEnabled = true;
            }
        }
    }
}
