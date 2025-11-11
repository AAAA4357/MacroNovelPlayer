using MNP.Helpers.Unsafe;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class PathLerpHelper
    {
        [BurstCompile]
        public static float Lerp1DLinear(in NativeArray<float2> pathArray, float t)
        {
            float start;
            float end;
            if (t < 0)
            {
                start = pathArray[0].y;
                end = pathArray[1].y;
                return start * (1 - t) + end * t;
            }
            else if (t > 1)
            {
                start = pathArray[^2].y;
                end = pathArray[^1].y;
                return start * (1 - t) + end * t;
            }
            UtilityHelper.GetFloorIndexInArray(pathArray, v => v.x, t, out int index, out float fixedT);
            start = pathArray[index].y;
            end = pathArray[index + 1].y;
            return start * (1 - fixedT) + end * fixedT;
        }

        [BurstCompile]
        public static unsafe float Lerp1DLinearUnsafe(float2* pathArray, int startIndex, int length, float t)
        {
            float start;
            float end;
            if (t < 0)
            {
                start = pathArray[0].y;
                end = pathArray[1].y;
                return start * (1 - t) + end * t;
            }
            else if (t > 1)
            {
                start = pathArray[length - 2].y;
                end = pathArray[length - 1].y;
                return start * (1 - t) + end * t;
            }
            UtilityHelperUnsafe.GetFloorIndexInArrayUnsafe(pathArray, startIndex, length, v => v.x, t, out int index, out float fixedT);
            start = pathArray[index].y;
            end = pathArray[index + 1].y;
            return start * (1 - fixedT) + end * fixedT;
        }

        [BurstCompile]
        public static float2 Lerp2DLinear(in NativeArray<float3> pathArray, float t)
        {
            float2 start;
            float2 end;
            if (t < 0)
            {
                start = pathArray[0].yz;
                end = pathArray[1].yz;
                return start * (1 - t) + end * t;
            }
            else if (t > 1)
            {
                start = pathArray[^2].yz;
                end = pathArray[^1].yz;
                return start * (1 - t) + end * t;
            }
            UtilityHelper.GetFloorIndexInArray(pathArray, v => v.x, t, out int index, out float fixedT);
            start = pathArray[index].yz;
            end = pathArray[index + 1].yz;
            return start * (1 - fixedT) + end * t;
        }

        public static float2 Lerp2DBezier(in NativeArray<float3> pathArray, in NativeArray<float2> pathControlArray, float t)
        {
            UtilityHelper.GetFloorIndexInArray(pathArray, v => v.x, t, out int index, out float fixedT);
            float2 start = pathArray[index].yz;
            float2 end = pathArray[index + 1].yz;
            float2 control1 = pathControlArray[index << 1];
            float2 control2 = pathControlArray[(index << 1) + 1];
            return GetBezierPoint2D(start, control1, control2, end, fixedT);
        }

        [BurstCompile]
        public static float3 Lerp3DLinear(in NativeArray<float4> pathArray, float t)
        {
            float3 start;
            float3 end;
            if (t < 0)
            {
                start = pathArray[0].yzw;
                end = pathArray[1].yzw;
                return start * (1 - t) + end * t;
            }
            else if (t > 1)
            {
                start = pathArray[^2].yzw;
                end = pathArray[^1].yzw;
                return start * (1 - t) + end * t;
            }
            UtilityHelper.GetFloorIndexInArray(pathArray, v => v.x, t, out int index, out float fixedT);
            start = pathArray[index].yzw;
            end = pathArray[index + 1].yzw;
            return start * (1 - fixedT) + end * t;
        }

        [BurstCompile]
        public static float3 Lerp3DBezier(in NativeArray<float4> pathArray, in NativeArray<float3> pathControlArray, float t)
        {
            UtilityHelper.GetFloorIndexInArray(pathArray, v => v.x, t, out int index, out float fixedT);
            float3 start = pathArray[index].yzw;
            float3 end = pathArray[index + 1].yzw;
            float3 control1 = pathControlArray[index << 1];
            float3 control2 = pathControlArray[index << 1 + 1];
            return GetBezierPoint3D(start, control1, control2, end, fixedT);
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