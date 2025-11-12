using MNP.Core.DOTS.Components.LerpRuntime;
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
            Animation1DLerpJob Job1D = new();
            state.Dependency = Job1D.ScheduleParallel(state.Dependency);
            Animation2DLerpJob Job2D = new();
            state.Dependency = Job2D.ScheduleParallel(state.Dependency);
            Animation3DLerpJob Job3D = new();
            state.Dependency = Job3D.ScheduleParallel(state.Dependency);
            state.CompleteDependency();
        }
    }
}
