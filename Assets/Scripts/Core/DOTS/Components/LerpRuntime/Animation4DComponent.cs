using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Animation4DComponent : IBufferElementData
    {
        public float4 StartValue;
        public float4 EndValue;
        public float4 Control0;
        public float4 Control1;
        //float4(4*4=16)*8=128, 7keys, 6segmentsx
        public FixedList128Bytes<float4> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
        public int LerpType;
        public int BezierDataIndex;
        public int SquadDataIndex;
    }
}
