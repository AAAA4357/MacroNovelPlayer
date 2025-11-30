using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct PropertyComponent : IBufferElementData
    {
        public float4 Value;
    }
}
