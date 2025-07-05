using MNP.Core.DataStruct.Animation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Animation2DArrayComponent : IComponentData
    {
        public NativeArray<Animation2DFrame> Frames;

        public NativeArray<Animaion2D> Animations;
    }
}
