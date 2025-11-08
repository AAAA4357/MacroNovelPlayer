using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Property1DComponent : IComponentData
    {
        public int PropertyIndex;
        public float Value;
    }
}
