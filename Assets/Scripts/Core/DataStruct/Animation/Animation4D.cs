using System.Collections.Generic;
using Unity.Mathematics;

namespace MNP.Core.DataStruct.Animation
{
    public class Animation4D
    {
        public float4 StartValue;
        public float4 EndValue;
        public float4 Control0Value;
        public float4 Control1Value;
        public List<AnimationEaseKeyframe> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
        public bool Enabled;
        public Float4LerpType LerpType;
    }
}
