using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace MNP.Core.DataStruct
{
    [BurstCompile]
    public struct EasingFunction
    {
        public NativeArray<float4> Segments;
    }
}
