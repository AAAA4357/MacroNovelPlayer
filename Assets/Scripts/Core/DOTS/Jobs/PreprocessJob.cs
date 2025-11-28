using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    public partial struct PreprocessJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecbWriter;

        [BurstCompile]
        public void Execute(ref PropertyInfoComponent propertyInfoComponent, in TimeComponent timeComponent, in Entity entity, [EntityIndexInQuery] int entityIndexInQuery)
        {
            if (timeComponent.Time < propertyInfoComponent.StartTime ||
                timeComponent.Time > propertyInfoComponent.EndTime)
            {
                if (!propertyInfoComponent.LerpEnabled)
                {
                    return;
                }
                ecbWriter.RemoveComponent<LerpEnabledComponent>(entityIndexInQuery, entity);
                propertyInfoComponent.LerpEnabled = false;
            }
            else
            {
                if (propertyInfoComponent.LerpEnabled)
                {
                    return;
                }
                ecbWriter.AddComponent<LerpEnabledComponent>(entityIndexInQuery, entity);
                propertyInfoComponent.LerpEnabled = true;
            }
        }
    }
}
