using System;
using System.Linq;
using MNP.Core.DataStruct;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class EasingFunctionHelper
    {
        [BurstCompile]
        public static float GetEase(in NativeArray<float4> segments, float t)
        {
            if (t < 0)
                return 0;
            int count = segments.Length;
            float temp = t * count;
            int index = Mathf.FloorToInt(temp);
            float lerp = temp - index;
            if (index >= segments.Length)
                return segments[segments.Length - 1].w;
            float4 segment = segments[index];
            return HermiteInterpolate(segment.x, segment.y, segment.z, segment.w, lerp);
        }

        [BurstCompile]
        public static float GetEase(this ref EasingFunction function, float t)
        {
            if (t == 0)
                return 0;
            if (t == 1)
                return 1;
            int count = function.Segments.Length;
            float temp = t * count;
            int index = Mathf.FloorToInt(temp);
            float lerp = temp - index;
            float4 segment = function.Segments[index];
            return HermiteInterpolate(segment.x, segment.y, segment.z, segment.w, lerp);
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
