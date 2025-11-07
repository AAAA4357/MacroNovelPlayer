using System.Collections.Generic;

namespace MNP.Core.DataStruct.Animation
{
    public class Animation2D
    {
        public List<Animation2DPathKeyframe> PathKeyFrameList;
        public List<AnimationEaseKeyframe> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
    }
}
