using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct LerpEnabledComponent : IComponentData, IEnableableComponent { }
}
