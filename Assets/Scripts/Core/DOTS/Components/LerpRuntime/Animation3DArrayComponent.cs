using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Animation3DArrayComponent : IComponentData
    {
        public NativeArray<float4> KeyFrameP0Array;
        public NativeArray<float4> KeyFrameP1Array;
        public NativeArray<float3> FrameC0Array;
        public NativeArray<float3> FrameC1Array;
        public NativeArray<int> IndexArray;
    }
}
