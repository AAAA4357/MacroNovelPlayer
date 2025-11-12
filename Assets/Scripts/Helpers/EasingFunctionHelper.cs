using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class EasingFunctionHelper
    {
        [BurstCompile]
        public static float GetEase(in NativeSlice<float4> keyframeArray, float t)
        {
            if (t <= 0)
                return 0;
            if (t >= 1)
                return 1;
            UtilityHelper.GetFloorIndexInSlice(keyframeArray, v => v.x, t, out int index, out float fixedT);
            return HermiteInterpolate(keyframeArray[index].x, keyframeArray[index + 1].y, keyframeArray[index].w, keyframeArray[index + 1].z, fixedT);
        }

        [BurstCompile]
        public static float GetEase(FixedList128Bytes<float4> keyFrameList, float t)
        {
            for (int i = 0; i < keyFrameList.Capacity; i++)
            {
                if (t > keyFrameList[i].x)
                {
                    continue;
                }
                else if (t == keyFrameList[i].x)
                {
                    return keyFrameList[i].y;
                }
                else
                {
                    float start = keyFrameList[i].x;
                    float duration = keyFrameList[i + 1].x - start;
                    float fixedT = (t - start) / duration;
                    return HermiteInterpolate(keyFrameList[i].y, keyFrameList[i + 1].y, keyFrameList[i].w, keyFrameList[i + 1].z, fixedT);
                }
            }
            return float.NaN;
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