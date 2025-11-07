using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

public class ManagedAnimation1DArrayComponent : IComponentData
{
    public Dictionary<string, List<Animation1D>> AnimationListDictionary;
}
