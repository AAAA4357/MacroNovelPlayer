using MNP.Core.DataStruct;
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
        public NativeArray<float2> AnchorArray;
        [ReadOnly]
        public NativeArray<int> IndexArray;

        //EasingFunction
        [ReadOnly]
        public NativeArray<float4> EasingKeyFrameArray;
        [ReadOnly]
        public NativeArray<int> EasingKeyFrameIndexArray;

        //Time
        [ReadOnly]
        public NativeArray<float> TimeCurrentArray;
        [ReadOnly]
        public NativeArray<float> TimeStartArray;
        [ReadOnly]
        public NativeArray<float> TimeDurationArray;

        //Entity
        [ReadOnly]
        public NativeArray<Entity> EntityArray;
        [ReadOnly]
        public NativeArray<Property1DComponent> PropertyArray;

        public EntityCommandBuffer.ParallelWriter Writer;

        [BurstCompile]
        public void Execute(int index)
        {
            UtilityHelper.GetFoldedArrayValue(EasingKeyFrameArray, EasingKeyFrameIndexArray, index, out NativeArray<float4> keyframeArray);
            float ease = EasingFunctionHelper.GetEase(keyframeArray, (TimeCurrentArray[index] - TimeStartArray[index]) / TimeDurationArray[index]);
            UtilityHelper.GetFoldedArrayValue(AnchorArray, IndexArray, index, out NativeArray<float2> pathArray);
            float result = PathLerpHelper.Lerp1D(pathArray, ease);
            Property1DComponent component = PropertyArray[index];
            component.Value = result;
            Writer.SetComponent(index, EntityArray[index], component);
            keyframeArray.Dispose();
            pathArray.Dispose();
        }
    }
}
