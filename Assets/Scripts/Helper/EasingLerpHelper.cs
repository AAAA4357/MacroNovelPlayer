using Unity.Mathematics;
using MNP.Core.DataStruct;

namespace MNP.Helper
{
    public static class EasingLerpHelper
    {
        private const float ThreeHalfPI = math.PI * 2 / 3;
        private const float FourOutOfNinePI = math.PI * 4 / 9;

        private const float BackConstFloat1 = 1.70158f;
        private const float BackConstFloat2 = 2.70158f;
        private const float BackConstFloat3 = 2.5949095f;
        private const float BackConstFloat4 = 3.5949095f;

        private const float BounceConstFloat1 = 7.5625f;
        private const float BounceConstFloat2 = 2.75f;

        public static float SuddenEase(float t) => t == 0 ? 0 : 1;

        public static float MiddleSuddenEase(float t) => t < 0.5f ? 0 : 1;

        public static float LateSuddenEase(float t) => t == 1 ? 1 : 0;

        public static float LinearEase(float t) => t;

        public static float InQuadraticEase(float t) => math.pow(t, 2);

        public static float OutQuadraticEase(float t) => -math.pow(1 - t, 2) + 1;

        public static float InOutQuadraticEase(float t) => t < 0.5f ? 2 * math.pow(t, 2) : -0.5f * math.pow(-2 * t + 2, 2) + 1;

        public static float InCubicEase(float t) => math.pow(t, 3);

        public static float OutCubicEase(float t) => -math.pow(1 - t, 3) + 1;

        public static float InOutCubicEase(float t) => t < 0.5f ? 4 * math.pow(t, 3) : -0.5f * math.pow(-2 * t + 2, 3) + 1;

        public static float InQuarticEase(float t) => math.pow(t, 4);

        public static float OutQuarticEase(float t) => -math.pow(1 - t, 4) + 1;

        public static float InOutQuarticEase(float t) => t < 0.5f ? 8 * math.pow(t, 4) : -0.5f * math.pow(-2 * t + 2, 4) + 1;

        public static float InQuinticEase(float t) => math.pow(t, 5);

        public static float OutQuinticEase(float t) => -math.pow(1 - t, 5) + 1;

        public static float InOutQuinticEase(float t) => t < 0.5f ? 16 * math.pow(t, 5) : -0.5f * math.pow(-2 * t + 2, 5) + 1;

        public static float InSineEase(float t) => -math.cos(0.5f * math.PI * t) + 1;

        public static float OutSineEase(float t) => math.sin(0.5f * math.PI * t);

        public static float InOutSineEase(float t) => -0.5f * (math.cos(math.PI * t) - 1);

        public static float InExponentialEase(float t) => t == 0 ? 0 : math.pow(2, 10 * t - 10);

        public static float OutExponentialEase(float t) => t == 1 ? 1 : -math.pow(2, -10 * t) + 1;

        public static float InOutExponentialEase(float t) => t == 0 ? 0 : t == 1 ? 1 : t < 0.5f ? 0.5f * math.pow(2, 20 * t - 10) : 0.5f * (2 - math.pow(2, -20 * t + 10));

        public static float InNaturalExponentialEase(float t) => t == 0 ? 0 : math.pow(math.E, 10 * t - 10);

        public static float OutNaturalExponentialEase(float t) => t == 1 ? 1 : -math.pow(math.E, -10 * t) + 1;

        public static float InOutNaturalExponentialEase(float t) => t == 0 ? 0 : t == 1 ? 1 : t < 0.5f ? 0.5f * math.pow(math.E, 20 * t - 10) : 0.5f * (2 - math.pow(math.E, -20 * t + 10));

        public static float InCircularEase(float t) => -math.sqrt(1 - math.pow(t, 2)) + 1;

        public static float OutCircularEase(float t) => math.sqrt(1 - math.pow(t - 1, 2));

        public static float InOutCircularEase(float t) => t < 0.5f ? 0.5f * (1 - math.sqrt(1 - math.pow(2 * t, 2))) : 0.5f * (math.sqrt(1 - math.pow(-2 * t + 2, 2)) + 1);

        public static float InElasticEase(float t) => t == 0 ? 0 : t == 1 ? 1 : -math.pow(2, 10 * t - 10) * math.sin((10 * t - 10.75f) * ThreeHalfPI);

        public static float OutElasticEase(float t) => t == 0 ? 0 : t == 1 ? 1 : math.pow(2, -10 * t) * math.sin((10 * t - 0.75f) * ThreeHalfPI) + 1;

        public static float InOutElasticEase(float t) => t == 0 ? 0 : t == 1 ? 1 : t < 0.5f ? -0.5f * (math.pow(2, 20 * t - 10) * math.sin((20 * t - 11.125f) * FourOutOfNinePI)) : 0.5f * (math.pow(2, -20 * t + 10) * math.sin((20 * t - 11.125f) * FourOutOfNinePI)) + 1;

        public static float InBackEase(float t) => BackConstFloat2 * math.pow(t, 3) - BackConstFloat1 * math.pow(t, 2);

        public static float OutBackEase(float t) => BackConstFloat2 * math.pow(t - 1, 3) - BackConstFloat1 * math.pow(t - 1, 2) + 1;

        public static float InOutBackEase(float t) => t < 0.5f ? 0.5f * (math.pow(2 * t, 2) * (BackConstFloat4 * 2 * t - BackConstFloat3)) : 0.5f * (math.pow(2 * t - 2, 2) * (BackConstFloat4 * (2 * t - 2) + BackConstFloat3) + 2);

        public static float InBounceEase(float t) => -OutBounceEase(1 - t) + 1;

        public static float OutBounceEase(float t) => t < 1 / BounceConstFloat2 ? BounceConstFloat1 * math.pow(t, 2) : t < 2 / BounceConstFloat2 ? BounceConstFloat1 * math.pow(t - 1.5f / BounceConstFloat2, 2) + 0.75f : t < 2.5f / BounceConstFloat2 ? BounceConstFloat1 * math.pow(t - 2.25f / BounceConstFloat2, 2) + 0.9375f : BounceConstFloat1 * math.pow(t - 2.625f / BounceConstFloat2, 2) + 0.984375f;

        public static float InOutBounceEase(float t) => t < 0.5f ? 0.5f * (1 - OutBounceEase(-2 * t + 1)) : 0.5f * OutBounceEase(2 * t - 1);

        public static EasingLerp MergeEasing(this EasingLerp easing, EasingLerp other)
        {
            return null;
        }
    }
}