using Unity.Burst;

namespace MNP.Core.DataStruct.Animations
{
    [BurstCompile]
    public struct AnimationProperty
    {
        public BezierCurve Path;

        public EasingFunction Ease;
    }
}
