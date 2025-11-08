using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Jobs.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Systems.LerpRuntime
{
    [UpdateInGroup(typeof(PropertyLerpSystemGroup))]
    partial struct Property1DLerpSystem : ISystem
    {
        NativeList<Entity> entities;
        NativeList<Property1DComponent> properties;
        NativeList<float2> pathKeyframeList;
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
            pathIndexList = new(Allocator.Persistent);
            easeKeyframeList = new(Allocator.Persistent);
            easeIndexList = new(Allocator.Persistent);
            timeList = new(Allocator.Persistent);
            
            query = state.GetEntityQuery(typeof(Animation1DArrayComponent), typeof(Property1DComponent), typeof(TimeComponent));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int count = query.CalculateEntityCount();

            entities.Clear();
            properties.Clear();
            pathKeyframeList.Clear();
            pathIndexList.Clear();
            easeKeyframeList.Clear();
            easeIndexList.Clear();
            timeList.Clear();
            
            entities.SetCapacity(count);
            properties.SetCapacity(count);
            pathKeyframeList.SetCapacity(count);
            pathIndexList.SetCapacity(count);
            easeKeyframeList.SetCapacity(count);
            easeIndexList.SetCapacity(count);
            timeList.SetCapacity(count);

            pathIndexList.Add(0);
            easeIndexList.Add(0);

            foreach (var (animation1DArrayComponent, property1DComponent, timeComponent, entity) in SystemAPI.Query<RefRO<Animation1DArrayComponent>, RefRO<Property1DComponent>, RefRO<TimeComponent>>().WithEntityAccess())
            {
                UtilityHelper.GetFloorIndexInArray(animation1DArrayComponent.ValueRO.TimeArray,
                                                   v => v,
                                                   timeComponent.ValueRO.Time,
                                                   out int animationIndex,
                                                   out float fixedT);
                UtilityHelper.GetFoldedArrayValue(animation1DArrayComponent.ValueRO.PathKeyframeArray,
                                                  animation1DArrayComponent.ValueRO.PathIndexArray,
                                                  animationIndex,
                                                  out NativeArray<float2> pathKeyframeArray);
                UtilityHelper.GetFoldedArrayValue(animation1DArrayComponent.ValueRO.EaseKeyframeArray,
                                                  animation1DArrayComponent.ValueRO.EaseIndexArray,
                                                  animationIndex,
                                                  out NativeArray<float4> easeKeyframeArray);
                pathKeyframeList.AddRange(pathKeyframeArray);
                pathIndexList.Add(pathKeyframeList.Length);
                easeKeyframeList.AddRange(easeKeyframeArray);
                easeIndexList.Add(easeKeyframeList.Length);
                timeList.Add(fixedT);
                entities.Add(entity);
                properties.Add(property1DComponent.ValueRO);
            }

            EntityCommandBuffer ecb = new(Allocator.TempJob);
            Animation1DLerpJob Job1D = new()
            {
                PathKeyframeArray = pathKeyframeList.AsArray(),
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
            pathIndexList.Dispose();
            easeKeyframeList.Dispose();
            easeIndexList.Dispose();
            timeList.Dispose();
        }
    }
}
