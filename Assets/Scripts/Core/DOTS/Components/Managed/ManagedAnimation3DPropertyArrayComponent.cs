using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

public class ManagedAnimation3DPropertyArrayComponent : IComponentData
{
    public List<AnimationProperty3D> PropertyList;
}
