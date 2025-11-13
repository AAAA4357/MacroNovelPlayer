using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Transform2DPropertyComponent : IComponentData
    {
        public float2 Position;
        public float Rotation;
        public float2 Scale;
    }
}
