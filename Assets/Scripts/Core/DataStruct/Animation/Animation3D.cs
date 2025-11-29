using System.Collections.Generic;
using Unity.Mathematics;

namespace MNP.Core.DataStruct.Animation
{
    public class Animation3D
    {
        public float3 StartValue;
        public float3 EndValue;
        public float3 Control0Value;
        public float3 Control1Value;
        public List<AnimationEaseKeyframe> EaseKeyframeList;
        public float StartTime;
        public float DurationTime;
        public bool Enabled;
        public Float3LerpType LerpType;
    }
}
