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
    [UpdateAfter(typeof(InputSystem))]
    partial struct TimeSystem : ISystem
    {
        public UnmanagedTimer timer;

        bool startTimer;

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

            TimeDeltaSetterJob setterJob = new()
            {
                DeltaValue = elapsedSeconds
            };
            state.Dependency = setterJob.ScheduleParallel(state.Dependency);
            state.CompleteDependency();

            if (startTimer)
            {
                timer.Start();
            }
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

        public void StartTime()
        {
            startTimer = true;
        }

        public void StopTime()
        {
            startTimer = false;
        }
    }
}
