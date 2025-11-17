using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.Transform2D
{
    [BurstCompile]
    public struct PosTransform2DPropertyComponent : IComponentData
    {
        public float2 Value;
    }
}
