using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;

namespace MNP.Core.DataStruct
{
    public class MNAnimation
    {
        public List<AnimationProperty1D> AnimationProperty1DList;
        public Dictionary<string, List<Animation1D>> Animation1DDictionary;
        public List<AnimationProperty2D> AnimationProperty2DList;
        public Dictionary<string, List<Animation2D>> Animation2DDictionary;
        public List<AnimationProperty3D> AnimationProperty3DList;
        public Dictionary<string, List<Animation3D>> Animation3DDictionary;
        public List<AnimationProperty4D> AnimationProperty4DList;
        public Dictionary<string, List<Animation4D>> Animation4DDictionary;
        public List<AnimationPropertyString> AnimationPropertyStringList;
        public Dictionary<string, List<AnimationString>> AnimationStringDictionary;
        public int TotalPropertyCount
        {
            get => AnimationProperty1DList.Count + AnimationProperty2DList.Count + AnimationProperty3DList.Count + AnimationProperty4DList.Count + AnimationPropertyStringList.Count;
        }
    }
}
