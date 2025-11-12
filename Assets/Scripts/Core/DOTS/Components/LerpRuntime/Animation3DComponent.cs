using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Animation3DComponent : IBufferElementData
    {
        public float3 StartValue;
        public float3 EndValue;
        public float3 Control0;
        public float3 Control1;
        //float4(4*4=16)*8=128, 7keys, 6segmentsx
        public FixedList128Bytes<float4> EaseKeyframeList;
        public bool Linear;
        public float StartTime;
        public float DurationTime;
    }
}
