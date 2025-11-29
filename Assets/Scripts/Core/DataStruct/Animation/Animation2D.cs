using System.Collections.Generic;
using Unity.Mathematics;

namespace MNP.Core.DataStruct.Animation
{
    public class Animation2D
    {
        public float2 StartValue;
        public float2 EndValue;
        public float2 Control0Value;
        public float2 Control1Value;
        public List<AnimationEaseKeyframe> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
        public bool Enabled;
        public Float2LerpType LerpType;
    }
}
