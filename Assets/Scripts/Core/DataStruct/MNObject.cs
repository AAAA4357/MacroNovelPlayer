using UnityEngine;

namespace MNP.Core.DataStruct
{
    public class MNObject
    {
        public uint ID;
        public int TextureID;
        public int Object3DMeshID;
        public Vector2 Object2DSize;
        public Vector4 Object2DUV;
        public ObjectType Type;
        public MNAnimation Animations;
        public uint? DependencyLayerID;
        public int? DependencyLayerIndex;
    }
}