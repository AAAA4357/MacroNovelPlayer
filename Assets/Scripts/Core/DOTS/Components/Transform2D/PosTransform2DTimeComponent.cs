using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Transform2D
{
    [BurstCompile]
    public struct PosTransform2DTimeComponent : IComponentData
    {
        public float Time;

        public int InterrputedTime;
    }
}
