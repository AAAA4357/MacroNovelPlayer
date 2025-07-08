using UnityEngine;
using MNP.Core.DataStruct;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using NUnit.Framework.Interfaces;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class BezierCurveHelper
    {
        [BurstCompile]
        public static void GetPoint(ref float2 result, in float2 P0, in float2 P1, in float2 P2, in float2 P3, float t)
        {
            t = Mathf.Clamp01(t);
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            result = P0 * uuu;
            result += 3 * t * uu * P1;
            result += 3 * tt * u * P2;
            result += P3 * ttt;
        }

        [BurstCompile]
        public static void GetPoint(this ref BezierCurve curve, ref NativeArray<float> container, float t)
        {
            t = Mathf.Clamp01(t);
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            for (int i = 0; i < curve.Dimension; i++)
            {
                container[i] = curve.ControlPointP0[i] * uuu;
                container[i] += curve.ControlPointP1[i] * 3 * uu * t;
                container[i] += curve.ControlPointP2[i] * 3 * u * tt;
                container[i] += curve.ControlPointP3[i] * ttt;
            }
        }

        [BurstCompile]
        public static void GetPointAtDistanceRatio(this ref BezierCurve curve, ref NativeArray<float> container, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            if (ratio == 0)
            {
                for (int i = 0; i < curve.Dimension; i++)
                {
                    container[i] = curve.ControlPointP0[i];
                }
            }
            if (ratio == 1)
            {
                for (int i = 0; i < curve.Dimension; i++)
                {
                    container[i] = curve.ControlPointP3[i];
                }
            }
            float totalLength = CalculateBezierLength(ref curve);
            float targetLength = totalLength * ratio;
            float lowT = 0f;
            float highT = 1f;
            float currentT = 0.5f;
            const float precision = 0.0001f;
            const int maxIterations = 100;
            int iterations = 0;
            while (iterations < maxIterations)
            {
                currentT = (lowT + highT) / 2f;
                float currentLength = CalculateBezierLength(ref curve, 0f, currentT);
                if (Mathf.Abs(currentLength - targetLength) < precision)
                    break;
                if (currentLength < targetLength)
                    lowT = currentT;
                else
                    highT = currentT;
                iterations++;
            }

            GetPoint(ref curve, ref container, currentT);
        }

        [BurstCompile]
        private static float CalculateBezierLength(ref BezierCurve curve, float fromT = 0f, float toT = 1f)
        {
            const int segments = 20;
            float length = 0f;
            NativeArray<float> prevPoint = new(curve.Dimension, Allocator.Temp);
            NativeArray<float> currentPoint = new(curve.Dimension, Allocator.Temp);
            GetPoint(ref curve, ref prevPoint, fromT);
            for (int i = 1; i <= segments; i++)
            {
                float t = fromT + (toT - fromT) * i / segments;
                GetPoint(ref curve, ref currentPoint, t);
                length += DistanceBetween(ref prevPoint, ref currentPoint, curve.Dimension);
                prevPoint = currentPoint;
            }
            prevPoint.Dispose();
            currentPoint.Dispose();
            return length;
        }

        [BurstCompile]
        private static float DistanceBetween(ref NativeArray<float> start, ref NativeArray<float> end, int Dimension)
        {
            float result = 0;
            for (int i = 0; i < Dimension; i++)
            {
                result += end[i] * end[i] - start[i] * start[i];
            }
            result = Mathf.Sqrt(result);
            start.Dispose();
            end.Dispose();
            return result;
        }
    }
}