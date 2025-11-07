using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Systems.LerpRuntime
{
    [UpdateInGroup(typeof(PropertyLerpSystemGroup))]
    [UpdateAfter(typeof(Property1DLerpSystem))]
    partial struct Property3DLerpSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
