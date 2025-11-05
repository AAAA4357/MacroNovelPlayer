using Unity.Burst;

namespace MNP.Core.DataStruct
{
    [BurstCompile]
    public struct LerpProperty1D
    {
        public float Value;

        public int AnimationIndex;
    }
}
