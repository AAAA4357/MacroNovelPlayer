using UnityEngine;

namespace MNP.Core.DataStruct.Animation
{
    public class AnimationProperty2D
    {
        public string ID;
        public float StartTime;
        public float EndTime;
        public bool IsStatic;
        public Vector2? StaticValue;
        public PropertyType Type;
    }
}
