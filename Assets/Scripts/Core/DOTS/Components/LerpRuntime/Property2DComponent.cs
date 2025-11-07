using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Property2DComponent : IComponentData
    {
        public int PropertyIndex;
        public float2 Value;
        public float StartTime;
        public float EndTime;
    }
}
