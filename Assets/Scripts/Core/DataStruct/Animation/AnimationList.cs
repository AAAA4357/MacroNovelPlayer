using System.Collections.Generic;

namespace MNP.Core.DataStruct.Animation
{
    public class AnimationList
    {
        public List<AnimationProperty1D> AnimationProperty1DList;
        public Dictionary<string, List<Animation1D>> Animation1DDictionary;
        public List<AnimationProperty2D> AnimationProperty2DList;
        public Dictionary<string, List<Animation2D>> Animation2DDictionary;
        public List<AnimationProperty3D> AnimationProperty3DList;
        public Dictionary<string, List<Animation3D>> Animation3DDictionary;
        public List<AnimationProperty4D> AnimationProperty4DList;
        public Dictionary<string, List<Animation4D>> Animation4DDictionary;
    }
}
