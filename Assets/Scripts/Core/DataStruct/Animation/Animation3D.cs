using System.Collections.Generic;

namespace MNP.Core.DataStruct.Animation
{
    public class Animation3D
    {
        public List<Animation3DPathKeyframe> PathKeyFrameList;
        public List<AnimationEaseKeyframe> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
    }
}
