using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeComponent), typeof(LoopTimeComponent))]
    public partial struct LoopTimeSetterJob : IJobEntity
    {
        public float TargetValue;

        [BurstCompile]
        public void Execute(ref TimeComponent time)
        {
            time.Time = TargetValue;
        }
    }
}
