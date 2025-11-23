using Unity.Burst;
using Unity.Entities.UniversalDelegates;
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
        public static float4 Lerp4DLinear(float4 start, float4 end, float t)
        {
            return start * (1 - t) + end * t;
        }
        
        [BurstCompile]
        public static float4 SLerp4DLinear(float4 start, float4 end, float t)
        {
            float dot = math.dot(start, end);
            if (dot < 0.0f)
            {
                end = -end;
                dot = -dot;
            }
            const float DOT_THRESHOLD = 0.9995f;
            float4 result;
            if (dot > DOT_THRESHOLD)
            {
                //NLerp
                result = Lerp4DLinear(start, end, t);
                return math.normalize(result);
            }
            dot = math.clamp(dot, -1.0f, 1.0f);
            float theta = math.acos(dot) * t;
            float4 q = end - start * dot;
            q = math.normalize(q);
            return start * math.cos(theta) + q * math.sin(theta);
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
        public static float4 GetBezierPoint4D(in float4 q0, in float4 q01, in float4 q01_1q12, in float4 q12_1q23, float t)
        {
            if (t == 0)
            {
                return q0;
            }
            float4 ct = q01_1q12.Pow(t);
            float4 dtt = q12_1q23.Pow(t*t);
            float4 bctdttt = QuaternionHelper.Mul(q01, QuaternionHelper.Mul(ct, dtt)).Pow(t);
            return QuaternionHelper.Mul(q0, bctdttt);
        }
    }
}