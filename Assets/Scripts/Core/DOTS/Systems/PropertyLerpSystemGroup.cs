using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(BakeSystem))]
    public partial class PropertyLerpSystemGroup : ComponentSystemGroup { }
}
