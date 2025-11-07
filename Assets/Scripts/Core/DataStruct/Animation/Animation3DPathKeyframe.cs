using UnityEngine;

namespace MNP.Core.DataStruct.Animation
{
    public class Animation3DPathKeyframe
    {
        public float KeyTime;
        public Vector3 Value;
        public Vector3? PrevControl;
        public Vector3? NextControl;
    }
}
