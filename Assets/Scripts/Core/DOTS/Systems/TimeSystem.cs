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
    partial struct TimeSystem : ISystem, ISystemStartStop
    {
        public UnmanagedTimer timer;

        bool resumeAllInterrupt;
        NativeArray<JobHandle> resumeJobs;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TimeEnabledComponent>();

            resumeJobs = new(4, Allocator.Persistent);

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
                resumeJobs[0] = new ResumeAllPosTransform2DInterruptJob().ScheduleParallel(state.Dependency);
                resumeJobs[1] = new ResumeAllRotTransform2DInterruptJob().ScheduleParallel(state.Dependency);
                resumeJobs[2] = new ResumeAllSclTransform2DInterruptJob().ScheduleParallel(state.Dependency);
                resumeJobs[3] = new ResumeAllInterruptJob().ScheduleParallel(state.Dependency);
                state.Dependency = JobHandle.CombineDependencies(resumeJobs);
                state.CompleteDependency();
                resumeAllInterrupt = false;
            }

            PosTransform2DTimeSetterJob posTransform2DJob = new()
            {
                DeltaValue = elapsedSeconds
            };
            RotTransform2DTimeSetterJob rotTransform2DJob = new()
            {
                DeltaValue = elapsedSeconds
            };
            SclTransform2DTimeSetterJob sclTransform2DJob = new()
            {
                DeltaValue = elapsedSeconds
            };
            JobHandle pos2DJob = posTransform2DJob.ScheduleParallel(state.Dependency);
            JobHandle rot2DJob = rotTransform2DJob.ScheduleParallel(state.Dependency);
            JobHandle scl2DJob = sclTransform2DJob.ScheduleParallel(state.Dependency);
            state.Dependency = JobHandle.CombineDependencies(pos2DJob, rot2DJob, scl2DJob);
            TimeSetterJob job = new()
            {
                DeltaValue = elapsedSeconds
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);

            LoopTimeSetterJob loopjob = new()
            {
                DeltaValue = elapsedSeconds
            };
            state.Dependency = loopjob.ScheduleParallel(state.Dependency);
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
