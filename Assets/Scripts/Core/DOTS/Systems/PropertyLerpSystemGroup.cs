using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PreprocessingSystem))]
    public partial class PropertyLerpSystemGroup : ComponentSystemGroup { }
}
