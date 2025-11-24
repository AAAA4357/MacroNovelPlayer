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

        public JobHandle LerpHandle;
        public bool BufferB;

        bool resumeAllInterrupt;
        NativeArray<JobHandle> resumeJobs;
        NativeArray<JobHandle> setterJobs;

        bool first;
        float TotalTime;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TimeEnabledComponent>();

            resumeJobs = new(7, Allocator.Persistent);
            setterJobs = new(7, Allocator.Persistent);

            timer.Initialize();
            timer.Reset();

            first = false;
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
            TotalTime += elapsedSeconds;

            LerpHandle.Complete();
            if (!BufferB && first)
            {
                state.Dependency = new SetBufferAReadyJob().ScheduleParallel(state.Dependency);
            }
            else if (first)
            {
                state.Dependency = new SetBufferBReadyJob().ScheduleParallel(state.Dependency);
            }
            state.CompleteDependency();

            BufferB = !BufferB;

            if (!BufferB)
            {
                if (resumeAllInterrupt)
                {
                    state.Dependency = new ResumeAllInterruptBufferAJob().ScheduleParallel(state.Dependency);
                    resumeAllInterrupt = false;
                }
                state.Dependency = new SetBufferADirtyJob().ScheduleParallel(state.Dependency);
                TimeSetterBufferAJob setterJob = new()
                {
                    TargetValue = TotalTime
                };
                state.Dependency = setterJob.ScheduleParallel(state.Dependency);
            }
            else
            {
                if (resumeAllInterrupt)
                {
                    state.Dependency = new ResumeAllInterruptBufferBJob().ScheduleParallel(state.Dependency);
                    resumeAllInterrupt = false;
                }
                state.Dependency = new SetBufferBDirtyJob().ScheduleParallel(state.Dependency);
                TimeSetterBufferBJob setterJob = new()
                {
                    TimeValue = TotalTime
                };
                state.Dependency = setterJob.ScheduleParallel(state.Dependency);
            }

            SystemHandle handle = state.WorldUnmanaged.GetExistingUnmanagedSystem<PreprocessingSystem>();
            ref PreprocessingSystem system = ref state.WorldUnmanaged.GetUnsafeSystemRef<PreprocessingSystem>(handle);
            system.TimeHandle = state.Dependency;
            system.BufferB = BufferB;

            first = true;

            timer.Start();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            TotalTime = 0;
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
