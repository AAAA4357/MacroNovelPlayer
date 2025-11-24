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
        JobHandle prevHandle;
        bool hasPrevHandle;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            jobs = new(4, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (hasPrevHandle)
            {
                state.Dependency = prevHandle;
                state.CompleteDependency();
            }
            jobs[0] = new Animation1DLerpJob().ScheduleParallel(state.Dependency);
            jobs[1] = new Animation2DLerpJob().ScheduleParallel(state.Dependency);
            jobs[2] = new Animation3DLerpJob().ScheduleParallel(state.Dependency);
            jobs[3] = new Animation4DLerpJob().ScheduleParallel(state.Dependency);
            prevHandle = JobHandle.CombineDependencies(jobs);
            hasPrevHandle = true;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            jobs.Dispose();
        }
    }
}
