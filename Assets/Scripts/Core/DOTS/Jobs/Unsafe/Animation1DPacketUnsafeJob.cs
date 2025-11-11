using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Unsafe;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers.Unsafe;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs.LerpRuntime
{
    [BurstCompile]
    public unsafe partial struct Animation1DPacketUnsafeJob : IJobChunk
    {
        public ComponentTypeHandle<Animation1DArrayComponent> AnimationArrayHandle;
        public ComponentTypeHandle<TimeComponent> TimeArrayHandle;

        public NativeList<UnsafeArray<float2>>.ParallelWriter ResultPathListWriter;
        public NativeList<UnsafeArray<float4>>.ParallelWriter ResultEaseListWriter;
        public NativeList<ChunkData<float>>.ParallelWriter ResultTimeListWriter;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            Animation1DArrayComponent* animationArray = (Animation1DArrayComponent*)chunk.GetNativeArray(ref AnimationArrayHandle).GetUnsafeReadOnlyPtr();
            TimeComponent* timeArray = (TimeComponent*)chunk.GetNativeArray(ref TimeArrayHandle).GetUnsafeReadOnlyPtr();
            for (int i = 0; i < chunk.Count; i++)
            {
                UtilityHelperUnsafe.GetFloorIndexLengthInArrayIndexedUnsafe((float2*)animationArray[i].PathKeyframeArray.GetUnsafeReadOnlyPtr(), (int*)animationArray[i].PathIndexArray.GetUnsafeReadOnlyPtr(), animationArray[i].PathIndexArray.Length, v => v.x, timeArray[i].Time, out int animationPathIndex, out int pathLength);
                ResultPathListWriter.AddNoResize(new UnsafeArray<float2>()
                {
                    Ptr = (float2*)animationArray[i].PathKeyframeArray.GetUnsafeReadOnlyPtr(),
                    Index = animationPathIndex,
                    Length = pathLength,
                    ChunkIndex = unfilteredChunkIndex,
                    EntityIndex = i
                });
                UtilityHelperUnsafe.GetFloorIndexLengthInArrayIndexedUnsafe((float4*)animationArray[i].EaseKeyframeArray.GetUnsafeReadOnlyPtr(), (int*)animationArray[i].EaseIndexArray.GetUnsafeReadOnlyPtr(), animationArray[i].EaseIndexArray.Length, v => v.x, timeArray[i].Time, out int animationEaseIndex, out int easeLength);
                ResultEaseListWriter.AddNoResize(new UnsafeArray<float4>()
                {
                    Ptr = (float4*)animationArray[i].EaseKeyframeArray.GetUnsafeReadOnlyPtr(),
                    Index = animationEaseIndex,
                    Length = easeLength,
                    ChunkIndex = unfilteredChunkIndex,
                    EntityIndex = i
                });
                ResultTimeListWriter.AddNoResize(new ChunkData<float>()
                {
                    Value = timeArray[i].Time,
                    ChunkIndex = unfilteredChunkIndex,
                    EntityIndex = i
                });
            }
        }
    }
}
