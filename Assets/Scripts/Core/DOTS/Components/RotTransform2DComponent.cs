using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct RotTransform2DPropertyComponent : IComponentData
    {
        public float Value;
    }
}
