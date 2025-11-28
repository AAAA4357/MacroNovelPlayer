using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct AnimationBezierBakeDataComponent : IBufferElementData
    {
        public float BezierLength;
    }
}
