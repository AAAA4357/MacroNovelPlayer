using Unity.Burst;
using Unity.Mathematics;

namespace MNP.Core.DataStruct
{
    [BurstCompile]
    public struct LerpProperty2D
    {
        public float2 Value;

        public int AnimationIndex;
    }
}
