using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Property1DComponent : IComponentData
    {
        public float Value;
        public int Index;
    }
}
