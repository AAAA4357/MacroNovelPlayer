using Unity.Burst;
using Unity.Collections;
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
        public static float4 Lerp4DLinear(float4 start, float4 end, float t)
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
        public static float4 GetBezierPoint4D(in float4 P0, in float4 P1, in float4 P2, in float4 P3, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            float4 result = P0 * uuu;
            result += 3 * t * uu * P1;
            result += 3 * tt * u * P2;
            result += P3 * ttt;
            return result;
        }
        
        [BurstCompile]
        public static float2 GetAverageBezierPoint2D(in float2 P0, in float2 P1, in float2 P2, in float2 P3, in FixedList128Bytes<float2> lengthMap, float t)
        {
            UtilityHelper.GetFloorIndexInNativeContainer(lengthMap, x => x.x, t, out int mapIndex);
            float2 start = lengthMap[mapIndex];
            float2 end = lengthMap[mapIndex + 1];
            float delta = end.y - start.y;
            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
            return GetBezierPoint2D(P0, P1, P2, P3, averageT);
        }
        
        [BurstCompile]
        public static float3 GetAverageBezierPoint3D(in float3 P0, in float3 P1, in float3 P2, in float3 P3, in FixedList128Bytes<float2> lengthMap, float t)
        {
            UtilityHelper.GetFloorIndexInNativeContainer(lengthMap, x => x.x, t, out int mapIndex);
            float2 start = lengthMap[mapIndex];
            float2 end = lengthMap[mapIndex + 1];
            float delta = end.y - start.y;
            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
            return GetBezierPoint3D(P0, P1, P2, P3, averageT);
        }
        
        [BurstCompile]
        public static float4 GetAverageBezierPoint4D(in float4 P0, in float4 P1, in float4 P2, in float4 P3, in FixedList128Bytes<float2> lengthMap, float t)
        {
            UtilityHelper.GetFloorIndexInNativeContainer(lengthMap, x => x.x, t, out int mapIndex);
            float2 start = lengthMap[mapIndex];
            float2 end = lengthMap[mapIndex + 1];
            float delta = end.y - start.y;
            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
            return GetBezierPoint4D(P0, P1, P2, P3, averageT);
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
        public static float4 GetSquadPoint4D(in float4 q0, in float4 q01, in float4 q01_1q12, in float4 q12_1q23, float t)
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
        
        [BurstCompile]
        public static float4 GetAverageSquadPoint4D(in float4 q0, in float4 q01, in float4 q01_1q12, in float4 q12_1q23, in FixedList128Bytes<float2> lengthMap, float t)
        {
            UtilityHelper.GetFloorIndexInNativeContainer(lengthMap, x => x.x, t, out int mapIndex);
            float2 start = lengthMap[mapIndex];
            float2 end = lengthMap[mapIndex + 1];
            float delta = end.y - start.y;
            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
            return GetSquadPoint4D(q0, q01, q01_1q12, q12_1q23, averageT);
        }

        public static float4 GetSquadPoint4DRaw(in float4 q0, in float4 q1, in float4 q2, in float4 q3, float t)
        {
            float4 q01 = SLerp4DLinear(q0, q1, t);
            float4 q12 = SLerp4DLinear(q1, q2, t);
            float4 q23 = SLerp4DLinear(q2, q3, t);

            float4 q01_12 = SLerp4DLinear(q01, q12, t);
            float4 q12_23 = SLerp4DLinear(q12, q23, t);

            float4 result = SLerp4DLinear(q01_12, q12_23, t);
            return result;
        }

        [BurstCompile]
        public static float GetLengthAtParameter2D(in float2 P0, in float2 P1, in float2 P2, in float2 P3, float fromT = 0f, float toT = 1f)
        {
            const int segments = 20;
            float length = 0f;
            float2 prevPoint = GetBezierPoint2D(P0, P1, P2, P3, fromT);
            float2 currentPoint;
            for (int i = 1; i <= segments; i++)
            {
                float t = fromT + (toT - fromT) * i / segments;
                currentPoint = GetBezierPoint2D(P0, P1, P2, P3, t);
                length += math.length(currentPoint - prevPoint);
                prevPoint = currentPoint;
            }
            return length;
        }

        [BurstCompile]
        public static float GetLengthAtParameter3D(in float3 P0, in float3 P1, in float3 P2, in float3 P3, float fromT = 0f, float toT = 1f)
        {
            const int segments = 20;
            float length = 0f;
            float3 prevPoint = GetBezierPoint3D(P0, P1, P2, P3, fromT);
            float3 currentPoint;
            for (int i = 1; i <= segments; i++)
            {
                float t = fromT + (toT - fromT) * i / segments;
                currentPoint = GetBezierPoint3D(P0, P1, P2, P3, t);
                length += math.length(currentPoint - prevPoint);
                prevPoint = currentPoint;
            }
            return length;
        }

        [BurstCompile]
        public static float GetLengthAtParameter4D(in float4 P0, in float4 P1, in float4 P2, in float4 P3, float fromT = 0f, float toT = 1f)
        {
            const int segments = 20;
            float length = 0f;
            float4 prevPoint = GetBezierPoint4D(P0, P1, P2, P3, fromT);
            float4 currentPoint;
            for (int i = 1; i <= segments; i++)
            {
                float t = fromT + (toT - fromT) * i / segments;
                currentPoint = GetBezierPoint4D(P0, P1, P2, P3, t);
                length += math.length(currentPoint - prevPoint);
                prevPoint = currentPoint;
            }
            return length;
        }

        [BurstCompile]
        public static float GetLengthAtSquadParameter4D(in float4 P0, in float4 P1, in float4 P2, in float4 P3, float fromT = 0f, float toT = 1f)
        {
            const int segments = 20;
            float length = 0f;
            float4 prevPoint = GetSquadPoint4DRaw(P0, P1, P2, P3, fromT);
            float4 currentPoint;
            for (int i = 1; i <= segments; i++)
            {
                float t = fromT + (toT - fromT) * i / segments;
                currentPoint = GetSquadPoint4DRaw(P0, P1, P2, P3, t);
                length += math.length(currentPoint - prevPoint);
                prevPoint = currentPoint;
            }
            return length;
        }
    }
}