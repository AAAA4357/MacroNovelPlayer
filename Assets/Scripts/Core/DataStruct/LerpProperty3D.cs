using Unity.Burst;
using Unity.Mathematics;

namespace MNP.Core.DataStruct
{
    [BurstCompile]
    public struct LerpProperty3D
    {
        public float3 Value;

        public int AnimationIndex;
    }
}
