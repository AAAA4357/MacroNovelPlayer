using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct ElementComponent : IComponentData
    {
        public int ID;
        public int TextureID;
        public Matrix4x4 TransformMatrix;
        public bool IsBlocked;
    }
}
