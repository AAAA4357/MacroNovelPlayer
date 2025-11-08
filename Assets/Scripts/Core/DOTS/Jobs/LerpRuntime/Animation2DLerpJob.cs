using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    public partial struct Animation2DLerpJob : IJobParallelFor
    {
        //Path
        [ReadOnly]
        public NativeArray<float3> PathKeyframeArray;
        [ReadOnly]
        public NativeArray<float2> PathControlArray;
        [ReadOnly]
        public NativeArray<bool> PathLinearLerpArray;
        [ReadOnly]
        public NativeArray<int> PathLinearIndexArray;
        [ReadOnly]
        public NativeArray<int> PathIndexArray;

        //EasingFunction
        [ReadOnly]
        public NativeArray<float4> EaseKeyframeArray;
        [ReadOnly]
        public NativeArray<int> EaseIndexArray;

        //Time
        [ReadOnly]
        public NativeArray<float> TimeArray;

        //Entity
        [ReadOnly]
        public NativeArray<Entity> EntityArray;
        [ReadOnly]
        public NativeArray<Property2DComponent> PropertyArray;

        public EntityCommandBuffer.ParallelWriter Writer;

        [BurstCompile]
        public void Execute(int index)
        {
            UtilityHelper.GetFloorIndexInArray(PathKeyframeArray, v => v.x, TimeArray[index], out int segmentIndex, out _);
            UtilityHelper.GetFoldedArrayValue(EaseKeyframeArray, EaseIndexArray, index, out NativeArray<float4> easeKeyframeArray, Allocator.Temp);
            float ease = EasingFunctionHelper.GetEase(easeKeyframeArray, TimeArray[index]);
            UtilityHelper.GetFoldedArrayValue(PathKeyframeArray, PathIndexArray, index, out NativeArray<float3> pathKeyframeArray, Allocator.Temp);
            UtilityHelper.GetFoldedArrayValue(PathControlArray, PathLinearIndexArray, 2, index, out NativeArray<float2> pathControlArray, Allocator.Temp);
            UtilityHelper.GetFoldedArrayValue(PathLinearLerpArray, PathLinearIndexArray, index, out NativeArray<bool> pathLinearLerpArray, Allocator.Temp);
            float2 result;
            if (pathLinearLerpArray[segmentIndex])
            {
                //Linear
                result = PathLerpHelper.Lerp2DLinear(pathKeyframeArray, ease);
            }
            else
            {
                //Bezier
                result = PathLerpHelper.Lerp2DBezier(pathKeyframeArray, pathControlArray, ease);
            }
            Property2DComponent component = PropertyArray[index];
            component.Value = result;
            Writer.SetComponent(index, EntityArray[index], component);
            easeKeyframeArray.Dispose();
            pathKeyframeArray.Dispose();
            pathControlArray.Dispose();
            pathLinearLerpArray.Dispose();
        }
    }
}
