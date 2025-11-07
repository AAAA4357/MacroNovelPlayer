using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Animation1DArrayComponent : IComponentData
    {
        public NativeArray<float2> KeyFrameArray;
        public NativeArray<int> IndexArray;
    }
}
