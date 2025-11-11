using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Unsafe;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Jobs.LerpRuntime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Systems.LerpRuntime
{
    [UpdateInGroup(typeof(PropertyLerpSystemGroup))]
    unsafe partial struct Property1DLerpUnsafeSystem : ISystem
    {            
        EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InitializedPropertyComponent>();
            state.RequireForUpdate<TimeEnabledComponent>();
            state.RequireForUpdate<LerpEnabledComponent>();
            
            query = state.GetEntityQuery(typeof(Animation1DArrayComponent), typeof(Property1DComponent), typeof(TimeComponent));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ComponentTypeHandle<Animation1DArrayComponent> animationTypeHandle = state.GetComponentTypeHandle<Animation1DArrayComponent>();
            ComponentTypeHandle<TimeComponent> timeTypeHandle = state.GetComponentTypeHandle<TimeComponent>();
            int count = query.CalculateEntityCount();
            NativeList<UnsafeArray<float2>> pathArrayList = new(count, Allocator.TempJob);
            NativeList<UnsafeArray<float4>> easeArrayList = new(count, Allocator.TempJob);
            NativeList<ChunkData<float>> timeList = new(count, Allocator.TempJob);
            Animation1DPacketUnsafeJob packetJob = new()
            {
                AnimationArrayHandle = animationTypeHandle,
                TimeArrayHandle = timeTypeHandle,
                ResultPathListWriter = pathArrayList.AsParallelWriter(),
                ResultEaseListWriter = easeArrayList.AsParallelWriter()
            };
            state.Dependency = packetJob.ScheduleParallel(query, state.Dependency);
            ChunkDataSortJob<UnsafeArray<float2>> pathSortJob = new()
            {
                Data = pathArrayList
            };
            state.Dependency = pathSortJob.Schedule(state.Dependency);
            ChunkDataSortJob<UnsafeArray<float4>> easeSortJob = new()
            {
                Data = easeArrayList
            };
            state.Dependency = easeSortJob.Schedule(state.Dependency);
            ChunkDataSortJob<ChunkData<float>> timeSortJob = new()
            {
                Data = timeList
            };
            state.Dependency = timeSortJob.Schedule(state.Dependency);
            state.CompleteDependency();
            EntityCommandBuffer ecb = new(Allocator.TempJob);
            Animation1DLerpUnsafeJob Job1D = new()
            {
                PathArrayList = pathArrayList,
                EaseArrayList = easeArrayList,
                TimeArray = timeList
            };
            JobHandle handle = Job1D.Schedule(count, 32);
            handle.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            pathArrayList.Dispose();
            easeArrayList.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}
