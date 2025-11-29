using MNP.Core.DataStruct;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct ElementComponent : IComponentData
    {
        public uint ID;
        public int TextureID;
        public int MeshID;
        public ObjectType ObjectType;
        public int TransformPositionIndex;
        public int TransformRotationIndex;
        public int TransformScaleIndex;
        public float4x4 TransformMatrix;
    }
}
