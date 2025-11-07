using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

public class ManagedAnimation3DArrayComponent : IComponentData
{
    public List<Animation3D> AnimationList;
}
