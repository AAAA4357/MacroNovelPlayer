using Unity.Burst;

namespace MNP.Core.DataStruct.Animation
{
    [BurstCompile]
    public struct AnimationProperty
    {
        public AnimationPath Path;

        public AnimationEase Ease;
    }
}
