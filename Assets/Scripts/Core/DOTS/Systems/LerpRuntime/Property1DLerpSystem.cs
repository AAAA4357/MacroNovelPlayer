using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Jobs.LerpRuntime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MNP.Core.DOTS.Systems.LerpRuntime
{
    [UpdateInGroup(typeof(PropertyLerpSystemGroup))]
    partial struct Property1DLerpSystem : ISystem
    {
        NativeList<Entity> entities;
        NativeList<ElementComponent> elements;
        NativeList<float> timeArray;
        NativeList<float> timeStartArray;
        NativeList<float> timeDurationArray;
        NativeList<bool> timeEndedArray;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TimeEnabledComponent>();
            state.RequireForUpdate<LerpEnabledComponent>();

            entities = new(Allocator.Persistent);
            elements = new(Allocator.Persistent);
            timeArray = new(Allocator.Persistent);
            timeStartArray = new(Allocator.Persistent);
            timeDurationArray = new(Allocator.Persistent);
            timeEndedArray = new(Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            entities.Clear();
            elements.Clear();
            timeArray.Clear();
            timeStartArray.Clear();
            timeDurationArray.Clear();
            timeEndedArray.Clear();

            int index = 0;
            foreach (var (animation1DArrayComponent, property1DArrayComponent, timeComponent, elementComponent, entity) in SystemAPI.Query<RefRO<Animation1DArrayComponent>, RefRO<Property1DComponent>, RefRO<TimeComponent>, RefRO<ElementComponent>>().WithEntityAccess())
            {

                index++;
            }
            EntityCommandBuffer ecb = new(Allocator.TempJob);
            Animation1DLerpJob Job1D = new()
            {
                Writer = ecb.AsParallelWriter()
            };
            JobHandle handle = Job1D.Schedule(100, 32);
            handle.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
