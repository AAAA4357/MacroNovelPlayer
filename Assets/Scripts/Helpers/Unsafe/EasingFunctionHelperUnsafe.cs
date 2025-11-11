using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace MNP.Helpers.Unsafe
{
    [BurstCompile]
    public static class EasingFunctionHelperUnsafe
    {
        public static unsafe float GetEaseUnsafe(float4* keyframeArrayPtr, int index, int length, float t)
        {
            if (t <= 0)
                return 0;
            if (t >= 1)
                return 1;
            UtilityHelperUnsafe.GetFloorIndexInArrayUnsafe(keyframeArrayPtr, index, length, v => v.x, t, out int easeIndex, out float fixedT);
            return EasingFunctionHelper.HermiteInterpolate(keyframeArrayPtr[index].x, keyframeArrayPtr[index + 1].y, keyframeArrayPtr[index].w, keyframeArrayPtr[index + 1].z, fixedT);
        }
    }
}
