using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Managed
{
    public class ManagedAnimation2DPropertyListComponent : IComponentData
    {
        public RefAnimationTransform2DProperty Transform2DProperty;
        public Dictionary<string, RefAnimationProperty1D> Property1DList;
        public Dictionary<string, RefAnimationProperty2D> Property2DList;
        public Dictionary<string, RefAnimationProperty3D> Property3DList;
        public Dictionary<string, RefAnimationProperty4D> Property4DList;
    }
}
