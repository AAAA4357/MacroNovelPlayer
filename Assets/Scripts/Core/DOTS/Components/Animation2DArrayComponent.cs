using MNP.Core.DataStruct.Animations;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Animation2DArrayComponent : IComponentData
    {
        public NativeArray<Animation2D> Animations;

        public int AnimationCount;
    }
}
