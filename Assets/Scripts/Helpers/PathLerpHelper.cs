using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class PathLerpHelper
    {
        [BurstCompile]
        public static float Lerp1DLinear(float start, float end, float t)
        {
            return start * (1 - t) + end * t;
        }

        [BurstCompile]
        public static float2 Lerp2DLinear(float2 start, float2 end, float t)
        {
            return start * (1 - t) + end * t;
        }

        [BurstCompile]
        public static float3 Lerp3DLinear(float3 start, float3 end, float t)
        {
            return start * (1 - t) + end * t;
        }
        
        [BurstCompile]
        public static float2 GetBezierPoint2D(in float2 P0, in float2 P1, in float2 P2, in float2 P3, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            float2 result = P0 * uuu;
            result += 3 * t * uu * P1;
            result += 3 * tt * u * P2;
            result += P3 * ttt;
            return result;
        }
        
        [BurstCompile]
        public static float3 GetBezierPoint3D(in float3 P0, in float3 P1, in float3 P2, in float3 P3, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            float3 result = P0 * uuu;
            result += 3 * t * uu * P1;
            result += 3 * tt * u * P2;
            result += P3 * ttt;
            return result;
        }
        
        [BurstCompile]
        public static float4 FastSquad(float4 q0, float4 q1, float4 q2, float4 q3, float t)
        {
            float4 s1 = CalculateIntermediateControlPoint(q0, q1, q2);
            float4 s2 = CalculateIntermediateControlPoint(q1, q2, q3);
            return SquadInterpolate(q1, q2, s1, s2, t);
        }
        
        [BurstCompile]
        private static float4 CalculateIntermediateControlPoint(float4 q0, float4 q1, float4 q2)
        {
            float4 q1Conj = new(-q1.x, -q1.y, -q1.z, q1.w);
            float4 log1 = QuaternionLog(QuaternionMultiply(q1Conj, q0));
            float4 log2 = QuaternionLog(QuaternionMultiply(q1Conj, q2));
            float4 sum = new float4(
                -0.25f * (log1.x + log2.x),
                -0.25f * (log1.y + log2.y),
                -0.25f * (log1.z + log2.z),
                -0.25f * (log1.w + log2.w)
            );
            
            return QuaternionMultiply(q1, QuaternionExp(sum));
        }
        
        [BurstCompile]
        private static float4 SquadInterpolate(float4 q1, float4 q2, float4 s1, float4 s2, float t)
        {
            float4 a = SlerpNoInvert(q1, q2, t);
            float4 b = SlerpNoInvert(s1, s2, t);
            return SlerpNoInvert(a, b, 2.0f * t * (1.0f - t));
        }
        
        [BurstCompile]
        private static float4 SlerpNoInvert(float4 a, float4 b, float t)
        {
            float dotProduct = Dot(a, b);
            if (dotProduct < 0.0f)
            {
                b = new float4(-b.x, -b.y, -b.z, -b.w);
                dotProduct = -dotProduct;
            }
            const float DOT_THRESHOLD = 0.9995f;
            if (dotProduct > DOT_THRESHOLD)
            {
                float4 result = new float4(
                    a.x + t * (b.x - a.x),
                    a.y + t * (b.y - a.y),
                    a.z + t * (b.z - a.z),
                    a.w + t * (b.w - a.w)
                );
                return NormalizeQuaternion(result);
            }
            float theta_0 = Mathf.Acos(dotProduct);
            float theta = theta_0 * t;
            float sin_theta = Mathf.Sin(theta);
            float sin_theta_0 = Mathf.Sin(theta_0);
            float s0 = Mathf.Cos(theta) - dotProduct * sin_theta / sin_theta_0;
            float s1 = sin_theta / sin_theta_0;
            return new float4(
                s0 * a.x + s1 * b.x,
                s0 * a.y + s1 * b.y,
                s0 * a.z + s1 * b.z,
                s0 * a.w + s1 * b.w
            );
        }
        
        [BurstCompile]
        private static float4 QuaternionMultiply(float4 a, float4 b)
        {
            return new float4(
                a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
                a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
                a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
                a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
            );
        }
        
        [BurstCompile]
        private static float4 QuaternionLog(float4 q)
        {
            float len = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
            
            if (len < 1e-6f)
            {
                if (q.w > 0)
                    return new float4(q.x, q.y, q.z, 0);
                else
                    return new float4(Mathf.PI * q.x, Mathf.PI * q.y, Mathf.PI * q.z, 0);
            }
            float w = q.w;
            float angle = Mathf.Acos(Mathf.Clamp(w, -1f, 1f));
            float sinAngle = Mathf.Sin(angle);
            if (Mathf.Abs(sinAngle) < 1e-6f)
                return new float4(q.x, q.y, q.z, 0);
            float coeff = angle / sinAngle;
            return new float4(coeff * q.x, coeff * q.y, coeff * q.z, 0);
        }
        
        [BurstCompile]
        private static float4 QuaternionExp(float4 v)
        {
            float angle = Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
            if (angle < 1e-6f)
                return new float4(v.x, v.y, v.z, Mathf.Cos(angle));
            float sinAngle = Mathf.Sin(angle);
            float cosAngle = Mathf.Cos(angle);
            float coeff = sinAngle / angle;
            return new float4(
                coeff * v.x,
                coeff * v.y,
                coeff * v.z,
                cosAngle
            );
        }
        
        [BurstCompile]
        private static float Dot(float4 a, float4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }
        
        [BurstCompile]
        private static float4 NormalizeQuaternion(float4 q)
        {
            float magnitude = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            if (magnitude < 1e-6f)
                return new float4(0, 0, 0, 1);
            
            return new float4(q.x / magnitude, q.y / magnitude, q.z / magnitude, q.w / magnitude);
        }
    }
}