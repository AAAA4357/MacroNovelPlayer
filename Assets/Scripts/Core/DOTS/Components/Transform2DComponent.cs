using MNP.Core.DataStruct;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Transform2DComponent : IComponentData
    {
        public Transform2D Transform;
    }
}
