using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Unsafe;
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
    public unsafe partial struct Animation1DLerpUnsafeJob : IJobParallelFor
    {
        //Path
        [ReadOnly]
        public NativeList<UnsafeArray<float2>> PathArrayList;
        //EasingFunction
        [ReadOnly]
        public NativeList<UnsafeArray<float4>> EaseArrayList;
        //Time
        [ReadOnly]
        public NativeList<ChunkData<float>> TimeArray;

        //Entity
        [ReadOnly]
        public NativeArray<Entity> EntityArray;
        [ReadOnly]
        public NativeArray<Property1DComponent> PropertyArray;

        public EntityCommandBuffer.ParallelWriter Writer;

        [BurstCompile]
        public void Execute(int index)
        {
            float ease = EasingFunctionHelper.GetEaseUnsafe(EaseArrayList[index].Ptr, EaseArrayList[index].Index, EaseArrayList[index].Length, TimeArray[index].Value);
            float result = PathLerpHelper.Lerp1DLinearUnsafe(PathArrayList[index].Ptr, PathArrayList[index].Index, PathArrayList[index].Length, ease);
            //No Bezier in 1D
            Property1DComponent component = PropertyArray[index];
            component.Value = result;
            Writer.SetComponent(index, EntityArray[index], component);
        }
    }
}
