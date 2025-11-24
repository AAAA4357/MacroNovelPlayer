using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(BakeSystem))]
    partial struct PreprocessingSystem : ISystem
    {
        public JobHandle TimeHandle;
        public bool BufferB;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InitializedPropertyComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!BufferB)
            {
                state.Dependency = new PreprocessBufferAJob().ScheduleParallel(TimeHandle);
            }
            else
            {
                state.Dependency = new PreprocessBufferBJob().ScheduleParallel(TimeHandle);
            }
            
            SystemHandle handle = state.WorldUnmanaged.GetExistingUnmanagedSystem<PropertyLerpSystem>();
            ref PropertyLerpSystem system = ref state.WorldUnmanaged.GetUnsafeSystemRef<PropertyLerpSystem>(handle);
            system.PreprocessHandle = state.Dependency;
            system.BufferB = BufferB;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
