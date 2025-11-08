using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Jobs;
using MNP.Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Systems.LerpRuntime
{
    [UpdateInGroup(typeof(PropertyLerpSystemGroup))]
    [UpdateAfter(typeof(Property1DLerpSystem))]
    partial struct Property2DLerpSystem : ISystem
    {
        NativeList<Entity> entities;
        NativeList<Property2DComponent> properties;
        NativeList<float3> pathKeyframeList;
        NativeList<float2> pathControlList;
        NativeList<bool> pathLinearList;
        NativeList<int> pathLinearIndexList;
        NativeList<int> pathIndexList;
        NativeList<float4> easeKeyframeList;
        NativeList<int> easeIndexList;
        NativeList<float> timeList;

        EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InitializedPropertyComponent>();
            state.RequireForUpdate<TimeEnabledComponent>();
            state.RequireForUpdate<LerpEnabledComponent>();

            entities = new(Allocator.Persistent);
            properties = new(Allocator.Persistent);
            pathKeyframeList = new(Allocator.Persistent);
            pathControlList = new(Allocator.Persistent);
            pathLinearList = new(Allocator.Persistent);
            pathLinearIndexList = new(Allocator.Persistent);
            pathIndexList = new(Allocator.Persistent);
            easeKeyframeList = new(Allocator.Persistent);
            easeIndexList = new(Allocator.Persistent);
            timeList = new(Allocator.Persistent);

            query = state.GetEntityQuery(typeof(Animation2DArrayComponent), typeof(Property2DComponent), typeof(TimeComponent));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int count = query.CalculateEntityCount();
            
            entities.Clear();
            properties.Clear();
            pathKeyframeList.Clear();
            pathControlList.Clear();
            pathLinearList.Clear();
            pathLinearIndexList.Clear();
            pathIndexList.Clear();
            easeKeyframeList.Clear();
            easeIndexList.Clear();
            timeList.Clear();
            
            entities.SetCapacity(count);
            properties.SetCapacity(count);
            pathKeyframeList.SetCapacity(count);
            pathControlList.SetCapacity(count);
            pathLinearList.SetCapacity(count);
            pathLinearIndexList.SetCapacity(count);
            pathIndexList.SetCapacity(count);
            easeKeyframeList.SetCapacity(count);
            easeIndexList.SetCapacity(count);
            timeList.SetCapacity(count);

            pathIndexList.Add(0);
            easeIndexList.Add(0);
            pathLinearIndexList.Add(0);

            int counter = 1;
            foreach (var (animation2DArrayComponent, property2DComponent, timeComponent, entity) in SystemAPI.Query<RefRO<Animation2DArrayComponent>, RefRO<Property2DComponent>, RefRO<TimeComponent>>().WithEntityAccess())
            {
                UtilityHelper.GetFloorIndexInArray(animation2DArrayComponent.ValueRO.TimeArray,
                                                   v => v,
                                                   timeComponent.ValueRO.Time,
                                                   out int animationIndex,
                                                   out float fixedT);
                UtilityHelper.GetFoldedArrayValue(animation2DArrayComponent.ValueRO.PathKeyframeArray,
                                                  animation2DArrayComponent.ValueRO.PathIndexArray,
                                                  animationIndex,
                                                  out NativeArray<float3> pathKeyframeArray);
                UtilityHelper.GetFoldedArrayValue(animation2DArrayComponent.ValueRO.PathControlArray,
                                                  animation2DArrayComponent.ValueRO.PathLinearIndexArray,
                                                  2,
                                                  animationIndex,
                                                  out NativeArray<float2> pathControlArray);
                UtilityHelper.GetFoldedArrayValue(animation2DArrayComponent.ValueRO.PathLinearLerpArray,
                                                  animation2DArrayComponent.ValueRO.PathLinearIndexArray,
                                                  animationIndex,
                                                  out NativeArray<bool> pathLinearLerpArray);
                UtilityHelper.GetFoldedArrayValue(animation2DArrayComponent.ValueRO.EaseKeyframeArray,
                                                  animation2DArrayComponent.ValueRO.EaseIndexArray,
                                                  animationIndex,
                                                  out NativeArray<float4> easeKeyframeArray);
                pathKeyframeList.AddRange(pathKeyframeArray);
                pathControlList.AddRange(pathControlArray);
                pathLinearList.AddRange(pathLinearLerpArray);
                pathIndexList.Add(pathKeyframeList.Length);
                pathLinearIndexList.Add(pathKeyframeList.Length - counter);
                easeKeyframeList.AddRange(easeKeyframeArray);
                easeIndexList.Add(easeKeyframeList.Length);
                timeList.Add(fixedT);
                entities.Add(entity);
                properties.Add(property2DComponent.ValueRO);
                counter++;
            }
            
            EntityCommandBuffer ecb = new(Allocator.TempJob);
            Animation2DLerpJob Job1D = new()
            {
                PathKeyframeArray = pathKeyframeList.AsArray(),
                PathControlArray = pathControlList.AsArray(),
                PathLinearLerpArray = pathLinearList.AsArray(),
                PathLinearIndexArray = pathLinearIndexList.AsArray(),
                PathIndexArray = pathIndexList.AsArray(),
                EaseKeyframeArray = easeKeyframeList.AsArray(),
                EaseIndexArray = easeIndexList.AsArray(),
                TimeArray = timeList.AsArray(),
                EntityArray = entities.AsArray(),
                PropertyArray = properties.AsArray(),
                Writer = ecb.AsParallelWriter()
            };
            JobHandle handle = Job1D.Schedule(count, 32);
            handle.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            entities.Dispose();
            properties.Dispose();
            pathKeyframeList.Dispose();
            pathControlList.Dispose();
            pathLinearList.Dispose();
            pathLinearIndexList.Dispose();
            pathIndexList.Dispose();
            easeKeyframeList.Dispose();
            easeIndexList.Dispose();
            timeList.Dispose();
        }
    }
}
