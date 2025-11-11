using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace MNP.Helpers.Unsafe
{
    [BurstCompile]
    public static class PathLerpHelper
    {
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
    }
}
