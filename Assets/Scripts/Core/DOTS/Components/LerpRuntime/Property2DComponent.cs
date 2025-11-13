using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Property2DComponent : IComponentData
    {
        public float2 Value;
    }
}
