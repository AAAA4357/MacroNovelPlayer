using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

public class ManagedAnimation2DPropertyArrayComponent : IComponentData
{
    public List<AnimationProperty2D> PropertyList;
}
