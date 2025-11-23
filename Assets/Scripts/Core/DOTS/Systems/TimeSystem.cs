using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Jobs;
using MNP.Core.DOTS.Jobs.Transform2D;
using MNP.Core.DOTS.Jobs.Transform3D;
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
                resumeJobs[0] = new ResumeAllPosTransform2DInterruptJob().ScheduleParallel(state.Dependency);
                resumeJobs[1] = new ResumeAllRotTransform2DInterruptJob().ScheduleParallel(state.Dependency);
                resumeJobs[2] = new ResumeAllSclTransform2DInterruptJob().ScheduleParallel(state.Dependency);
                resumeJobs[3] = new ResumeAllPosTransform3DInterruptJob().ScheduleParallel(state.Dependency);
                resumeJobs[4] = new ResumeAllRotTransform3DInterruptJob().ScheduleParallel(state.Dependency);
                resumeJobs[5] = new ResumeAllSclTransform3DInterruptJob().ScheduleParallel(state.Dependency);
                resumeJobs[6] = new ResumeAllInterruptJob().ScheduleParallel(state.Dependency);
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
            PosTransform3DTimeSetterJob posTransform3DJob = new()
            {
                DeltaValue = elapsedSeconds
            };
            RotTransform3DTimeSetterJob rotTransform3DJob = new()
            {
                DeltaValue = elapsedSeconds
            };
            SclTransform3DTimeSetterJob sclTransform3DJob = new()
            {
                DeltaValue = elapsedSeconds
            };
            TimeSetterJob setterJob = new()
            {
                DeltaValue = elapsedSeconds
            };

            setterJobs[0] = posTransform2DJob.ScheduleParallel(state.Dependency);
            setterJobs[1] = rotTransform2DJob.ScheduleParallel(state.Dependency);
            setterJobs[2] = sclTransform2DJob.ScheduleParallel(state.Dependency);
            setterJobs[3] = posTransform3DJob.ScheduleParallel(state.Dependency);
            setterJobs[4] = rotTransform3DJob.ScheduleParallel(state.Dependency);
            setterJobs[5] = sclTransform3DJob.ScheduleParallel(state.Dependency);
            setterJobs[6] = setterJob.ScheduleParallel(state.Dependency);
            state.Dependency = JobHandle.CombineDependencies(setterJobs);
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
