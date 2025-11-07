using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class PathLerpHelper
    {
        [BurstCompile]
        public static float Lerp1D(in NativeArray<float2> pathArray, float t)
        {
            UtilityHelper.GetFloorIndexInArray(pathArray, v => v.x, t, out int index, out float fixedT);
            float start = pathArray[index].y;
            float end = pathArray[index + 1].y;
            return start * (1 - fixedT) + end * fixedT;
        }

        [BurstCompile]
        public static float2 Lerp2D(in NativeArray<float3> pathArray, in NativeArray<float2> pathControlArray, float t)
        {
            UtilityHelper.GetFloorIndexInArray(pathArray, v => v.x, t, out int index, out float fixedT);
            if (pathControlArray[index << 1].x == float.NaN || pathControlArray[index << 1].x == float.NaN)
            {
                float3 result = pathArray[index] * (1 - fixedT) + pathArray[index + 1] * t;
                return result.yz;
            }
            float2 start = pathArray[index].yz;
            float2 end = pathArray[index + 1].yz;
            float2 control1 = pathControlArray[index << 1];
            float2 control2 = pathControlArray[index << 1 + 1];
            return GetBezierPoint2D(start, control1, control2, end, fixedT);
        }

        [BurstCompile]
        public static float3 Lerp3D(in NativeArray<float4> pathArray, in NativeArray<float3> pathControlArray, float t)
        {
            UtilityHelper.GetFloorIndexInArray(pathArray, v => v.x, t, out int index, out float fixedT);
            if (pathControlArray[index << 1].x == float.NaN || pathControlArray[index << 1].x == float.NaN || pathControlArray[index << 1].z == float.NaN)
            {
                float4 result = pathArray[index] * (1 - fixedT) + pathArray[index + 1] * fixedT;
                return result.yzw;
            }
            float3 start = pathArray[index].yzw;
            float3 end = pathArray[index + 1].yzw;
            float3 control1 = pathControlArray[index << 1];
            float3 control2 = pathControlArray[index << 1 + 1];
            return GetBezierPoint3D(start, control1, control2, end, fixedT);
        }
        
        [BurstCompile]
        public static float2 GetBezierPoint2D(in float2 P0, in float2 P1, in float2 P2, in float2 P3, float t)
        {
            t = Mathf.Clamp01(t);
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
            t = Mathf.Clamp01(t);
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