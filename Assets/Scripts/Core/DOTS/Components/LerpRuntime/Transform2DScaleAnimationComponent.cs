using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Transform2DScaleAnimationComponent : IBufferElementData
    {
        public float2 StartValue;
        public float2 EndValue;
        public float2 Control0;
        public float2 Control1;
        //float4(4*4=16)*8=128, 7keys, 6segments
        public FixedList128Bytes<float4> EaseKeyframeList;
        public bool Linear;
        public float StartTime;
        public float DurationTime;
    }
}
