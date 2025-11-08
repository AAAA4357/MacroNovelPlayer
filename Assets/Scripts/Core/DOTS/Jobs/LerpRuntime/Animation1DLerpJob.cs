using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs.LerpRuntime
{
    [BurstCompile]
    public partial struct Animation1DLerpJob : IJobParallelFor
    {
        //Path
        [ReadOnly]
        public NativeArray<float2> PathKeyframeArray;
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
        public NativeArray<Property1DComponent> PropertyArray;

        public EntityCommandBuffer.ParallelWriter Writer;

        [BurstCompile]
        public void Execute(int index)
        {
            UtilityHelper.GetFoldedArrayValue(EaseKeyframeArray, EaseIndexArray, index, out NativeArray<float4> easekeyframeArray);
            float ease = EasingFunctionHelper.GetEase(easekeyframeArray, TimeArray[index]);
            UtilityHelper.GetFoldedArrayValue(PathKeyframeArray, PathIndexArray, index, out NativeArray<float2> pathKeyframeArray);
            //No Bezier in 1D
            float result = PathLerpHelper.Lerp1DLinear(pathKeyframeArray, ease);
            Property1DComponent component = PropertyArray[index];
            component.Value = result;
            Writer.SetComponent(index, EntityArray[index], component);
            easekeyframeArray.Dispose();
            pathKeyframeArray.Dispose();
        }
    }
}
