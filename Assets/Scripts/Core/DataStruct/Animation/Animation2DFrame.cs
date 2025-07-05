using Unity.Burst;
using Unity.Collections;

namespace MNP.Core.DataStruct.Animation
{
    [BurstCompile]
    public struct Animation2DFrame
    {
        public float Time;

        public Transform2D Transform;

        public NativeHashMap<int, NativeArray<float>> CustomValues;
    }
}
