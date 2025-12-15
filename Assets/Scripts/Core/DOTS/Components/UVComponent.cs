using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct UVComponent : IComponentData
    {
        public float4 UV;
    }
}
