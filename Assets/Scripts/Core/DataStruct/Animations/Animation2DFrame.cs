using Unity.Burst;

namespace MNP.Core.DataStruct.Animations
{
    [BurstCompile]
    public struct Animation2DFrame
    {
        public float Time;

        public Transform2D Transform;
    }
}
