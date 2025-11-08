using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Animation1DArrayComponent : IComponentData
    {
        public NativeArray<float2> PathKeyFrameArray;
        public NativeArray<int> PathIndexArray;
        public NativeArray<float4> EaseKeyFrameArray;
        public NativeArray<int> EaseIndexArray;
        public NativeArray<float> TimeArray;
    }
}
