using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Jobs;
using MNP.Core.DOTS.Jobs.Transform2D;
using MNP.Core.DOTS.Jobs.Transform3D;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(BakeSystem))]
    partial struct PreprocessingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InitializedPropertyComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.TempJob);
            PreprocessTransform2DJob transform2DJob = new()
            {
                ecbWriter = ecb.AsParallelWriter()
            };
            JobHandle Job2DHandle = transform2DJob.ScheduleParallel(state.Dependency);
            PreprocessTransform3DJob transform3DJob = new()
            {
                ecbWriter = ecb.AsParallelWriter()
            };
            JobHandle Job3DHandle = transform3DJob.ScheduleParallel(state.Dependency);
            state.Dependency = JobHandle.CombineDependencies(Job2DHandle, Job3DHandle);
            state.CompleteDependency();
            PreprocessJob job = new()
            {
                ecbWriter = ecb.AsParallelWriter()
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
            state.CompleteDependency();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
