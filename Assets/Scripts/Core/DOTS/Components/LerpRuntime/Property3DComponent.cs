using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Property3DComponent : IComponentData
    {
        public int PropertyIndex;
        public float3 Value;
        public float StartTime;
        public float EndTime;
    }
}
