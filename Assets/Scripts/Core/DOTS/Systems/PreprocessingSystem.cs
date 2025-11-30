using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(TimeSystem))]
    public partial struct PreprocessingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InitializedPropertyComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new PreprocessJob().ScheduleParallel(state.Dependency);
            state.CompleteDependency();
        }
    }
}
