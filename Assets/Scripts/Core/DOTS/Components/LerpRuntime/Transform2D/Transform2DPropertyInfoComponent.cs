using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime.Transform2D
{
    [BurstCompile]
    public struct Transform2DPropertyInfoComponent : IComponentData
    {
        public float PositionStartTime;
        public float PositionEndTime;
        public bool PositionLerpEnabled;
        public float RotationStartTime;
        public float RotationEndTime;
        public bool RotationLerpEnabled;
        public float ScaleStartTime;
        public float ScaleEndTime;
        public bool ScaleLerpEnabled;
    }
}
