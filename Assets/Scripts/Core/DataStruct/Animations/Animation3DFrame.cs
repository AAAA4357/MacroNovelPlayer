using Unity.Burst;
using Unity.Collections;

namespace MNP.Core.DataStruct.Animations
{
    [BurstCompile]
    public struct Animation3DFrame
    {
        public float Time;

        public Transform3D Transform;

        public NativeHashMap<int, NativeArray<float>> Properties;
    }
}
