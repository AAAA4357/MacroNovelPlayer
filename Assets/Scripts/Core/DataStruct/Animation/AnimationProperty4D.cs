using System.Collections.Generic;
using UnityEngine;

namespace MNP.Core.DataStruct.Animation
{
    public class AnimationProperty4D
    {
        public string ID;
        public float StartTime;
        public float EndTime;
        public bool IsStatic;
        public Vector4? StaticValue;
        public PropertyType Type;
        public List<float> AnimationInterruptTimeList;
    }
}
