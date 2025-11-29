using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct AnimationBezierBakeDataComponent : IBufferElementData
    {
        //float2(4*2=8)*16=128, 15keys, 14segments
        public FixedList128Bytes<float2> BezierLengthMap;
    }
}
