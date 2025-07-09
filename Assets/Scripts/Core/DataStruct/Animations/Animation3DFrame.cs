using System.Collections.Generic;

namespace MNP.Core.DataStruct.Animations
{
    public struct Animation3DFrame
    {
        public float Time;

        public Transform3D Transform;

        public Dictionary<int, List<float>> Properties;
    }
}
