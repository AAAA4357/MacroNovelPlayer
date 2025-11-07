using UnityEngine;

namespace MNP.Core.DataStruct.Animation
{
    public class Animation2DPathKeyframe
    {
        public float KeyTime;
        public Vector2 Value;
        public Vector2? PrevControl;
        public Vector2? NextControl;
    }
}
