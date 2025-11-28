using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Jobs;
using MNP.Core.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(BakeSystem))]
    partial struct TimeSystem : ISystem, ISystemStartStop
    {
        public UnmanagedTimer timer;

        bool resumeAllInterrupt;
        NativeArray<JobHandle> resumeJobs;
        NativeArray<JobHandle> setterJobs;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TimeEnabledComponent>();

            resumeJobs = new(7, Allocator.Persistent);
            setterJobs = new(7, Allocator.Persistent);

            timer.Initialize();
            timer.Reset();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            timer.Start();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            timer.Stop();

            float elapsedSeconds = timer.GetElapsedSeconds();

            if (resumeAllInterrupt)
            {
                state.Dependency = new ResumeAllInterruptJob().ScheduleParallel(state.Dependency);
                state.CompleteDependency();
                resumeAllInterrupt = false;
            }

            TimeSetterJob setterJob = new()
            {
                DeltaValue = elapsedSeconds
            };
            state.Dependency = setterJob.ScheduleParallel(state.Dependency);
            state.CompleteDependency();

            timer.Start();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            timer.Stop();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            timer.Dispose();
        }

        public void Interrupt()
        {
            
        }

        public void Resume()
        {
            
        }

        public void ResumeAll()
        {
            resumeAllInterrupt = true;
        }
    }
}
