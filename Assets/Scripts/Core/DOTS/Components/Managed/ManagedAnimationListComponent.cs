using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Managed
{
    public class ManagedAnimationListComponent : IComponentData
    {
        public List<AnimationProperty1D> AnimationProperty1DList;
        public Dictionary<string, List<Animation1D>> Animation1DDictionary;
        public List<AnimationProperty2D> AnimationProperty2DList;
        public Dictionary<string, List<Animation2D>> Animation2DDictionary;
        public List<AnimationProperty3D> AnimationProperty3DList;
        public Dictionary<string, List<Animation3D>> Animation3DDictionary;
    }
}
