using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    public class AnimationStringListComponent : IComponentData
    {
        public List<AnimationString> Animations;
    }
}
