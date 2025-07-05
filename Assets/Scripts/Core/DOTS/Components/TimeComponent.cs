using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct TimeComponent : ISharedComponentData
    {
        public float Time;
    }
}
