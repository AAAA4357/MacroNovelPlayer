using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using NUnit.Framework.Interfaces;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    public partial struct Animation3DLerpJob : IJobParallelFor
    {
        //Path
        [ReadOnly]
        public NativeArray<float4> PathKeyframeArray;
        [ReadOnly]
        public NativeArray<float3> PathControlArray;
        [ReadOnly]
        public NativeArray<bool> PathLinearLerpArray;
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
        public NativeArray<Property3DComponent> PropertyArray;

        public EntityCommandBuffer.ParallelWriter Writer;

        [BurstCompile]
        public void Execute(int index)
        {
            UtilityHelper.GetFoldedArrayValue(EaseKeyframeArray, EaseIndexArray, index, out NativeArray<float4> easeKeyframeArray, Allocator.Temp);
            float ease = EasingFunctionHelper.GetEase(easeKeyframeArray, TimeArray[index]);
            UtilityHelper.GetFoldedArrayValue(PathKeyframeArray, PathIndexArray, index, out NativeArray<float4> pathKeyframeArray, Allocator.Temp);
            UtilityHelper.GetFoldedArrayValue(PathControlArray, PathIndexArray, 2, index, out NativeArray<float3> pathControlArray, Allocator.Temp);
            UtilityHelper.GetFoldedArrayValue(PathLinearLerpArray, PathIndexArray, index, out NativeArray<bool> pathLinearLerpArray, Allocator.Temp);
            float3 result;
            if (pathLinearLerpArray[index])
            {
                //Linear
                result = PathLerpHelper.Lerp3DLinear(pathKeyframeArray, ease);
            }
            else
            {
                //Bezier
                result = PathLerpHelper.Lerp3DBezier(pathKeyframeArray, pathControlArray, ease);
            }
            Property3DComponent component = PropertyArray[index];
            component.Value = result;
            Writer.SetComponent(index, EntityArray[index], component);
            easeKeyframeArray.Dispose();
            pathKeyframeArray.Dispose();
            pathControlArray.Dispose();
            pathLinearLerpArray.Dispose();
        }
    }
}
