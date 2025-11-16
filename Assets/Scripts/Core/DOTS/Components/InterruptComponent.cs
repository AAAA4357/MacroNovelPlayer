using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct InterruptComponent : IComponentData, IEnableableComponent { }
}
