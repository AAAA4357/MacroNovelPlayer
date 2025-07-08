using MNP.Core.DataStruct.Animations;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Animation3DArrayComponent : IComponentData
    {
        [ReadOnly]
        public NativeArray<Animation3DFrame> Frames;

        [ReadOnly]
        public NativeArray<Animation3D> Animations;
    }
}
