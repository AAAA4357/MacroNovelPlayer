using MNP.Core.DOTS.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PreprocessingSystem))]
    partial struct PropertyLerpSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new Animation1DLerpJob().ScheduleParallel(state.Dependency);
            state.Dependency = new Animation2DLerpJob().ScheduleParallel(state.Dependency);
            state.Dependency = new Animation3DLerpJob().ScheduleParallel(state.Dependency);
            state.Dependency = new Animation4DLerpJob().ScheduleParallel(state.Dependency);
            state.CompleteDependency();
        }
    }
}
