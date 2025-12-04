using System.Collections.Generic;

namespace MNP.Core.DataStruct.Animation
{
    public class AnimationPropertyString
    {
        public string ID;
        public float StartTime;
        public float EndTime;
        public bool IsStatic;
        public string StaticValue;
        public List<float> AnimationInterruptTimeList;
        public int Index;
    }
}
