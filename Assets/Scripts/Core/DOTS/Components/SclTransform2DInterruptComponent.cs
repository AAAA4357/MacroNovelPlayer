using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct SclTransform2DInterruptComponent : IComponentData, IEnableableComponent { }
}
