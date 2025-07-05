using Unity.Burst;
using Unity.Collections;

namespace MNP.Core.DataStruct.Animation
{
    [BurstCompile]
    public struct Animaion2D
    {
        public Animation2DFrame StartFrame;

        public Animation2DFrame EndFrame;

        public NativeHashMap<int, AnimationProperty> PropertyMap;
    }
}
