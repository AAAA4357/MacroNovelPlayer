using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Animation1DArrayComponent : IComponentData
    {
        public NativeArray<float> Anchors;
        public NativeArray<int> Indices;
    }
}
