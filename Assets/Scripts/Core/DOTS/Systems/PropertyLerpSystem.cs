using MNP.Core.DOTS.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PreprocessingSystem))]
    partial struct PropertyLerpSystem : ISystem
    {
        NativeArray<JobHandle> jobs;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            jobs = new(4, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            jobs[0] = new Animation1DLerpJob().ScheduleParallel(state.Dependency);
            jobs[1] = new Animation2DLerpJob().ScheduleParallel(state.Dependency);
            jobs[2] = new Animation3DLerpJob().ScheduleParallel(state.Dependency);
            jobs[3] = new Animation4DLerpJob().ScheduleParallel(state.Dependency);
            state.Dependency = JobHandle.CombineDependencies(jobs);
            state.CompleteDependency();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            jobs.Dispose();
        }
    }
}
