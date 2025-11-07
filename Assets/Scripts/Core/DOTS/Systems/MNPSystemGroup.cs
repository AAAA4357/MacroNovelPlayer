using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class MNPSystemGroup : ComponentSystemGroup { }
}