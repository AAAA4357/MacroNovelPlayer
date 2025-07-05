using Unity.Burst;
using Unity.Mathematics;

namespace MNP.Core.DataStruct
{
    [BurstCompile]
    public struct Transform2D
    {
        public float2 Position;

        public float Rotation;

        public float2 Scale;
    }
}
