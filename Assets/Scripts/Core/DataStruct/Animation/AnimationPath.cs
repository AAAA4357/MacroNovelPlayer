using Unity.Burst;

namespace MNP.Core.DataStruct.Animation
{
    [BurstCompile]
    public struct AnimationPath
    {
        BezierCurve BezierCurve;
    }
}
