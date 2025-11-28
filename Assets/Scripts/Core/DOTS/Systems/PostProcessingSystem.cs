using MNP.Core.DOTS.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PropertyLerpSystem))]
    partial struct PostprocessingSystem : ISystem
    {
        public NativeArray<float4> PropertyArray;

        [BurstCompile]
        void OnUpdate(ref SystemState state)
        {
            state.Dependency = new Postprocess1DJob()
            {
                OutputArray = PropertyArray
            }.ScheduleParallel(state.Dependency);
            state.Dependency = new Postprocess2DJob()
            {
                OutputArray = PropertyArray
            }.ScheduleParallel(state.Dependency);
            state.Dependency = new Postprocess3DJob()
            {
                OutputArray = PropertyArray
            }.ScheduleParallel(state.Dependency);
            state.Dependency = new Postprocess4DJob()
            {
                OutputArray = PropertyArray
            }.ScheduleParallel(state.Dependency);
            state.CompleteDependency();
        }
    }
}
