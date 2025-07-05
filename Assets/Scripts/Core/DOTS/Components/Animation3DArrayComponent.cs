using MNP.Core.DataStruct.Animation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Animation3DArrayComponent : IComponentData
    {
        public NativeArray<Animation3DFrame> Frames;

        public NativeArray<Animaion3D> Animations;
    }
}
