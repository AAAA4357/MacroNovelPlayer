using System.Collections.Generic;

namespace MNP.Core.DataStruct.Animation
{
    public class Animation1D
    {
        public List<Animation1DPathSegement> PathKeyFrameList;
        public List<AnimationEaseKeyframe> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
    }
}
