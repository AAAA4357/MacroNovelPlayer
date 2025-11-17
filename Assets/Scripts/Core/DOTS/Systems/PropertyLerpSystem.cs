using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Jobs;
using MNP.Core.DOTS.Jobs.Transform2D;
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
            jobs = new(6, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            jobs[0] = new AnimationPosTransform2DLerpJob().ScheduleParallel(state.Dependency);
            jobs[1] = new AnimationRotTransform2DLerpJob().ScheduleParallel(state.Dependency);
            jobs[2] = new AnimationSclTransform2DLerpJob().ScheduleParallel(state.Dependency);
            jobs[3] = new Animation1DLerpJob().ScheduleParallel(state.Dependency);
            jobs[4] = new Animation2DLerpJob().ScheduleParallel(state.Dependency);
            jobs[5] = new Animation3DLerpJob().ScheduleParallel(state.Dependency);
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
