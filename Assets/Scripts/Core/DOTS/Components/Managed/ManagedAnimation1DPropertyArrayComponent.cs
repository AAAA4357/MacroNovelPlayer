using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

public class ManagedAnimation1DPropertyArrayComponent : IComponentData
{
    public List<AnimationProperty1D> PropertyList;
}
