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
            EntityCommandBuffer ecb2D = new(Allocator.TempJob);
            EntityCommandBuffer ecb3D = new(Allocator.TempJob);
            PreprocessTransform2DJob transform2DJob = new()
            {
                ecbWriter = ecb2D.AsParallelWriter()
            };
            JobHandle job2DHandle = transform2DJob.ScheduleParallel(state.Dependency);
            PreprocessTransform3DJob transform3DJob = new()
            {
                ecbWriter = ecb3D.AsParallelWriter()
            };
            JobHandle job3DHandle = transform3DJob.ScheduleParallel(state.Dependency);
            PreprocessJob job = new()
            {
                ecbWriter = ecb.AsParallelWriter()
            };
            JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
            state.Dependency = JobHandle.CombineDependencies(jobHandle, job2DHandle, job3DHandle);
            state.CompleteDependency();
            ecb.Playback(state.EntityManager);
            ecb2D.Playback(state.EntityManager);
            ecb3D.Playback(state.EntityManager);
            ecb.Dispose();
            ecb2D.Dispose();
            ecb3D.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
