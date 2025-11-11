using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime.Unsafe
{
    [BurstCompile]
    public unsafe struct AnimationArrayUnsafeComponent : IComponentData
    {
        public float2** Path1DKeyframeArrayPtr;
        public int Path1DArrayLength;
        public float3** Path2DKeyframeArrayPtr;
        public int Path2DArrayLength;
        public float2** Path2DControlArrayPtr;
        public int Path2DControlArrayLength;
        public float4** Path3DKeyframeArrayPtr;
        public int Path3DArrayLength;
        public float3** Path3DControlArrayPtr;
        public int Path3DControlArrayLength;
        public float4** EaseKeyframeArrayPtr;
        public int EaseKeyframeArrayLength;
    }
}
