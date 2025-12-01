using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct DependencyPropertyComponent : IBufferElementData
    {
        public int PropertyIndex;
    }
}
