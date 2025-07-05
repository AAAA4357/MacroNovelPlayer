using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace MNP.Core.DataStruct.Animation
{
    [BurstCompile]
    public struct AnimationEase
    {
        public NativeArray<float4> ControlPoints;
    }
}
