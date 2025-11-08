using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Managed
{
    public class ManagedAnimationPropertyListComponent : IComponentData
    {
        public List<RefAnimationProperty1D> Property1DList;
        public List<RefAnimationProperty2D> Property2DList;
        public List<RefAnimationProperty3D> Property3DList;
    }
}
