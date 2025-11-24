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

        public JobHandle PreprocessHandle;
        public bool BufferB;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            jobs = new(4, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!BufferB)
            {
                jobs[0] = new Animation1DBufferALerpJob().ScheduleParallel(PreprocessHandle);
                jobs[1] = new Animation2DBufferALerpJob().ScheduleParallel(PreprocessHandle);
                jobs[2] = new Animation3DBufferALerpJob().ScheduleParallel(PreprocessHandle);
                jobs[3] = new Animation4DBufferALerpJob().ScheduleParallel(PreprocessHandle);
            }
            else
            {
                jobs[0] = new Animation1DBufferBLerpJob().ScheduleParallel(PreprocessHandle);
                jobs[1] = new Animation2DBufferBLerpJob().ScheduleParallel(PreprocessHandle);
                jobs[2] = new Animation3DBufferBLerpJob().ScheduleParallel(PreprocessHandle);
                jobs[3] = new Animation4DBufferBLerpJob().ScheduleParallel(PreprocessHandle);
            }
            SystemHandle handle = state.WorldUnmanaged.GetExistingUnmanagedSystem<TimeSystem>();
            ref TimeSystem system = ref state.WorldUnmanaged.GetUnsafeSystemRef<TimeSystem>(handle);
            system.LerpHandle = JobHandle.CombineDependencies(jobs);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            jobs.Dispose();
        }
    }
}
