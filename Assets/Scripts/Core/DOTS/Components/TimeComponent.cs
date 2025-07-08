using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct TimeComponent : IComponentData
    {
        public float Time;

        public int InterrputedTime;
    }
}
