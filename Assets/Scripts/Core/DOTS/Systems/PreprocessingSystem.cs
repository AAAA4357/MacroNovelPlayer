using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(BakeSystem))]
    partial struct PreprocessingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InitializedPropertyComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            foreach (var (propertyInfoComponent, timeComponent, entity) in SystemAPI.Query<RefRO<PropertyInfoComponent>, RefRO<TimeComponent>>().WithEntityAccess())
            {
                UtilityHelper.GetFloorIndexInArray(propertyInfoComponent.ValueRO.LerpKeyArray,
                                                   v => v,
                                                   timeComponent.ValueRO.Time,
                                                   out int animationIndex,
                                                   out float _);
                if (timeComponent.ValueRO.Time < propertyInfoComponent.ValueRO.StartTime ||
                    timeComponent.ValueRO.Time > propertyInfoComponent.ValueRO.EndTime ||
                    !propertyInfoComponent.ValueRO.LerpEnabledArray[animationIndex])
                {
                    ecb.RemoveComponent<LerpEnabledComponent>(entity);
                }
                else
                {
                    ecb.AddComponent<LerpEnabledComponent>(entity);
                }
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
