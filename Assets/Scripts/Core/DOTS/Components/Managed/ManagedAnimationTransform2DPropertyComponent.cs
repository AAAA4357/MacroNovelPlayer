using MNP.Core.DataStruct.Animation;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Managed
{
    public class ManagedAnimationTransform2DPropertyComponent : IComponentData
    {
        public RefAnimationTransformProperty RefValue;
    }
}
