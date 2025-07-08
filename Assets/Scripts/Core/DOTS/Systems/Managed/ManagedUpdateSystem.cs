using MNP.Mono;
using Unity.Entities;

namespace MNP.Core.DOTS.Systems.Managed
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(Animation3DSystem))]
    partial class ManagedUpdateSystem : SystemBase
    {
        public QuadInstance[] InstanceList;

        protected override void OnUpdate()
        {
            foreach (var quad in InstanceList)
            {
                quad.UpdateInstance();
            }
        }
    }
}
