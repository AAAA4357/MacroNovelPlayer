using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Animation2DArrayComponent : IComponentData
    {
        public NativeArray<float3> KeyFrameP0Array;
        public NativeArray<float3> KeyFrameP1Array;
        public NativeArray<float2> FrameC0Array;
        public NativeArray<float2> FrameC1Array;
        public NativeArray<int> IndexArray;
    }
}
