using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Animation1DComponent : IBufferElementData
    {
        public float StartValue;
        public float EndValue;
        //float4(4*4=16)*8=128, 7keys, 6segments
        public FixedList128Bytes<float4> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
    }
}
