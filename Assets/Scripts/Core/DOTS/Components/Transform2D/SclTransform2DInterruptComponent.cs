using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Transform2D
{
    [BurstCompile]
    public struct SclTransform2DInterruptComponent : IComponentData, IEnableableComponent { }
}
