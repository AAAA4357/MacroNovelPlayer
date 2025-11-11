using MNP.Helpers.Unsafe;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class EasingFunctionHelper
    {
        [BurstCompile]
        public static float GetEase(in NativeArray<float4> keyframeArray, float t)
        {
            if (t <= 0)
                return 0;
            if (t >= 1)
                return 1;
            UtilityHelper.GetFloorIndexInArray(keyframeArray, v => v.x, t, out int index, out float fixedT);
            return HermiteInterpolate(keyframeArray[index].x, keyframeArray[index + 1].y, keyframeArray[index].w, keyframeArray[index + 1].z, fixedT);
        }

        public static unsafe float GetEaseUnsafe(float4* keyframeArrayPtr, int index, int length, float t)
        {
            if (t <= 0)
                return 0;
            if (t >= 1)
                return 1;
            UtilityHelperUnsafe.GetFloorIndexInArrayUnsafe(keyframeArrayPtr, index, length, v => v.x, t, out int easeIndex, out float fixedT);
            return HermiteInterpolate(keyframeArrayPtr[index].x, keyframeArrayPtr[index + 1].y, keyframeArrayPtr[index].w, keyframeArrayPtr[index + 1].z, fixedT);
        }

        [BurstCompile]
        public static float HermiteInterpolate(float p0, float p1, float m0, float m1, float t)
        {
            if (t <= 0)
                return p0;
            if (t >= 1)
                return p1;
            float a = 2 * p0 - 2 * p1 + m0 + m1;
            float b = -3 * p0 + 3 * p1 - 2 * m0 - m1;
            float c = m0;
            float d = p0;
            return ((a * t + b) * t + c) * t + d;
        }
    }
}