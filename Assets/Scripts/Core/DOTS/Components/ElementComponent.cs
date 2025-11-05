using MNP.Core.DataStruct;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct ElementComponent : IComponentData
    {
        public int ID;
        public int TextureID;
    }
}
