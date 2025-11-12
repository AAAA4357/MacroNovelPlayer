using System.Collections.Generic;

namespace MNP.Core.DataStruct.Animation
{
    public class Animation1D
    {
        public float StartValue;
        public float EndValue;
        public List<AnimationEaseKeyframe> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
        public bool Enabled;
    }
}
