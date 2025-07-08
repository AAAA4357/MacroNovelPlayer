using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Jobs;
using MNP.Core.Misc;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateBefore(typeof(Animation2DSystem))]
    [UpdateBefore(typeof(Animation3DSystem))]
    partial struct TimeSystem : ISystem, ISystemStartStop
    {
        private bool Interrupted;

        private float currentLoopTime;

        private float currentTime;

        public UnmanagedTimer timer;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TimeEnabledComponent>();

            currentLoopTime = 0;
            currentTime = 0;

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

            if (!Interrupted)
            {
                currentTime += elapsedSeconds;
                TimeSetterJob job = new()
                {
                    TargetValue = currentTime
                };
                job.ScheduleParallel();
            }

            currentLoopTime += elapsedSeconds;
            LoopTimeSetterJob loopjob = new()
            {
                TargetValue = currentLoopTime
            };
            loopjob.ScheduleParallel();

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
            Interrupted = true;
        }

        public void Resume()
        {
            Interrupted = false;
        }
    }
}
