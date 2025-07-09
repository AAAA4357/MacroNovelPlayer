using System.Collections.Generic;

namespace MNP.Core.DataStruct.Animations
{
    public struct Animation3D
    {
        public Animation3DFrame StartFrame;

        public Animation3DFrame EndFrame;

        public Dictionary<int, AnimationProperty> PropertyMap;
    }
}
