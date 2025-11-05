using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Animation2DArrayComponent : IComponentData
    {
        public NativeArray<float2> Anchors;
        public NativeArray<int> Indices;
    }
}
