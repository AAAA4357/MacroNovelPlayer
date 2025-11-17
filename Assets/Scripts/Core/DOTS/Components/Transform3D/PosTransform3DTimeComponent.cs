using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Transform3D
{
    [BurstCompile]
    public struct PosTransform3DTimeComponent : IComponentData
    {
        public float Time;

        public int InterrputedTime;
    }
}
