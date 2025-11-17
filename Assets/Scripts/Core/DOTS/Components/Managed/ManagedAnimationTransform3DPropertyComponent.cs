using MNP.Core.DataStruct.Animation;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Managed
{
    public class ManagedAnimationTransform3DPropertyComponent : IComponentData
    {
        public RefAnimationTransform3DProperty RefValue;
    }
}
