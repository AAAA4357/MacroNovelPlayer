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
    public partial struct TimeSystem : ISystem
    {
        public float _systemtime;
        public float SystemTime
        {
            get => _systemtime;
            set
            {
                resetTime = value;
                _systemtime = value;
            }
        }

        UnmanagedTimer timer;
        bool startTimer;
        float? resetTime;
        bool pauseTime;

        NativeList<uint> interruptIDList;
        NativeList<uint> resumeIDList;

        bool resumeAllInterrupt;
        bool interruptAll;
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

            interruptIDList = new(Allocator.Persistent);
            resumeIDList = new(Allocator.Persistent);

            resetTime = null;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!startTimer)
            {
                return;
            }

            timer.Stop();
            float elapsedSeconds = pauseTime ? 0 : timer.GetElapsedSeconds();
            if (resetTime is not null)
            {
                state.Dependency = new TimeSetterJob()
                {
                    TargetValue = resetTime.Value
                }.ScheduleParallel(state.Dependency);
                state.CompleteDependency();
                resetTime = null;
                pauseTime = true;
                elapsedSeconds = 0;
            }
            else
            {
                _systemtime += elapsedSeconds;
            }

            if (startTimer)
            {
                state.Dependency = new EnableAllTimeJob().ScheduleParallel(state.Dependency);
                state.CompleteDependency();
            }
            else
            {
                state.Dependency = new TimeSetterJob()
                {
                    TargetValue = 0
                }.ScheduleParallel(state.Dependency);
                state.Dependency = new DisableAllTimeJob().ScheduleParallel(state.Dependency);
                state.CompleteDependency();
                timer.Reset();
                _systemtime = 0;
                return;
            }

            if (interruptAll)
            {
                state.Dependency = new InterruptAllJob().ScheduleParallel(state.Dependency);
                state.CompleteDependency();
                resumeAllInterrupt = false;
            }
            if (resumeAllInterrupt)
            {
                state.Dependency = new ResumeAllJob().ScheduleParallel(state.Dependency);
                state.CompleteDependency();
                resumeAllInterrupt = false;
            }

            if (interruptIDList.Length != 0)
            {
                state.Dependency = new InterruptJob()
                {
                    IDArray = interruptIDList.AsArray()
                }.ScheduleParallel(state.Dependency);
                state.CompleteDependency();
                interruptIDList.Clear();
            }
            if (resumeIDList.Length != 0)
            {
                state.Dependency = new ResumeJob()
                {
                    IDArray = resumeIDList.AsArray()
                }.ScheduleParallel(state.Dependency);
                state.CompleteDependency();
                interruptIDList.Clear();
            }

            TimeDeltaSetterJob setterJob = new()
            {
                DeltaValue = elapsedSeconds
            };
            state.Dependency = setterJob.ScheduleParallel(state.Dependency);
            state.CompleteDependency();

            timer.Start();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            timer.Dispose();
        }

        public void Interrupt(uint id)
        {
            interruptIDList.Add(id);
        }

        public void Resume(uint id)
        {
            resumeIDList.Add(id);
        }

        public void InterruptAll()
        {
            interruptAll = true;
        }

        public void ResumeAll()
        {
            resumeAllInterrupt = true;
        }

        public void StartTime()
        {
            startTimer = true;
            timer.Start();
        }

        public void StopTime()
        {
            startTimer = false;
            timer.Reset();
        }

        public void PauseTime()
        {
            pauseTime = true;
        }

        public void ResumeTime()
        {
            pauseTime = false;
        }
    }
}
