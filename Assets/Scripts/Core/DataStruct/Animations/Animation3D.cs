using Unity.Burst;
using Unity.Collections;

namespace MNP.Core.DataStruct.Animations
{
    [BurstCompile]
    public struct Animation3D
    {
        public Animation3DFrame StartFrame;

        public Animation3DFrame EndFrame;

        public NativeHashMap<int, AnimationProperty> PropertyMap;
    }
}
