using MNP.Core.DataStruct;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct ElementComponent : ICleanupComponentData
    {
        public int ID;

        public Transform2D Transform;
    }
}
