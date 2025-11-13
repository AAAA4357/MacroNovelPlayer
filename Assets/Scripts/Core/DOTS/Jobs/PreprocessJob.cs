using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
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
        public void Execute(ref PropertyInfoComponent propertyInfoComponent, in TimeComponent timeComponent, in Entity entity, [ChunkIndexInQuery] int chunkIndexInQuery)
        {
            if (timeComponent.Time < propertyInfoComponent.StartTime ||
                timeComponent.Time > propertyInfoComponent.EndTime)
            {
                if (!propertyInfoComponent.LerpEnabled)
                {
                    return;
                }
                ecbWriter.RemoveComponent<LerpEnabledComponent>(chunkIndexInQuery, entity);
                propertyInfoComponent.LerpEnabled = false;
            }
            else
            {
                if (propertyInfoComponent.LerpEnabled)
                {
                    return;
                }
                ecbWriter.AddComponent<LerpEnabledComponent>(chunkIndexInQuery, entity);
                propertyInfoComponent.LerpEnabled = true;
            }
        }
    }
}
