using Unity.Burst;
using Unity.Mathematics;

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
    }
}