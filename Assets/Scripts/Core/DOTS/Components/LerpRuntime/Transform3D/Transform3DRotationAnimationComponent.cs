using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime.Transform3D
{
    [BurstCompile]
    public struct Transform3DRotationAnimationComponent : IBufferElementData
    {
        public float4 StartValue;
        public float4 EndValue;
        public float4 Control0;
        public float4 Control1;
        //float4(4*4=16)*8=128, 7keys, 6segments
        public FixedList128Bytes<float4> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
    }
}
