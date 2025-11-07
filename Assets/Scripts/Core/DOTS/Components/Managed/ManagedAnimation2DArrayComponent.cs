using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

public class ManagedAnimation2DArrayComponent : IComponentData
{
    public List<Animation2D> AnimationList;
}
