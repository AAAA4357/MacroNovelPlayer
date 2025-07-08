using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct LayerComponent : ISharedComponentData
    {
        public int LayerIndex;
    }
}
