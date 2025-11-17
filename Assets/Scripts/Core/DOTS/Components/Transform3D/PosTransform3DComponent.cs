using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.Transform3D
{
    [BurstCompile]
    public struct PosTransform3DPropertyComponent : IComponentData
    {
        public float3 Value;
    }
}
