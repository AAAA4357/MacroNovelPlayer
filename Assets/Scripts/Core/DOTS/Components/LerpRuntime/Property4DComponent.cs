using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Property4DComponent : IComponentData
    {
        public float4 Value;
    }
}
