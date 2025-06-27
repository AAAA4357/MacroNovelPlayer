using UnityEngine;
using MNP.Core.DataStruct;

namespace MNP.Helper
{
    public static class BezierCurveHelper
    {
        public static Vector2 GetPoint(this BezierCurve curve, float t)
        {
            t = Mathf.Clamp01(t);

            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = curve.P0 * uuu;
            p += curve.P1 * 3 * uu * t;
            p += curve.P2 * 3 * u * tt;
            p += curve.P3 * ttt;

            return p;
        }

        public static Vector2 GetPointAtDistanceRatio(this BezierCurve curve, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            if (ratio == 0) return curve.P0;
            if (ratio == 1) return curve.P3;

            float totalLength = CalculateBezierLength(curve);
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
                float currentLength = CalculatePartialBezierLength(curve, 0f, currentT);

                if (Mathf.Abs(currentLength - targetLength) < precision)
                {
                    break;
                }

                if (currentLength < targetLength)
                {
                    lowT = currentT;
                }
                else
                {
                    highT = currentT;
                }

                iterations++;
            }

            return GetPoint(curve, currentT);
        }

        private static float CalculatePartialBezierLength(BezierCurve curve, float fromT, float toT)
        {
            const int segments = 50;
            float length = 0f;
            Vector2 prevPoint = GetPoint(curve, fromT);

            for (int i = 1; i <= segments; i++)
            {
                float t = fromT + (toT - fromT) * i / segments;
                Vector2 currentPoint = GetPoint(curve, t);
                length += Vector2.Distance(prevPoint, currentPoint);
                prevPoint = currentPoint;
            }

            return length;
        }

        private static float CalculateBezierLength(BezierCurve curve)
        {
            return CalculatePartialBezierLength(curve, 0f, 1f);
        }
    }
}
