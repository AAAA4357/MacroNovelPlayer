using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct ElementComponent : IComponentData
    {
        public uint ID;
        public int Object3DTextureID;
        public int Object3DMeshID;
        public float2 Object2DSize;
        public int TransformPositionIndex;
        public int TransformRotationIndex;
        public int TransformScaleIndex;
        public float4x4 TransformMatrix;
    }
}
