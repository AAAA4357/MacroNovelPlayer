using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Animation3DArrayComponent : IComponentData
    {
        public NativeArray<float4> PathKeyframeArray;
        public NativeArray<float3> PathControlArray;
        public NativeArray<bool> PathLinearLerpArray;
        public NativeArray<int> PathIndexArray;
        public NativeArray<float4> EaseKeyframeArray;
        public NativeArray<int> EaseIndexArray;
        public NativeArray<float> TimeArray;
    }
}
