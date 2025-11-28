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
        public bool IsBlocked;
        public ObjectType ObjectType;
        public int TransformPositionIndex;
        public int TransformRotationIndex;
        public int TransformScaleIndex;
    }
}
