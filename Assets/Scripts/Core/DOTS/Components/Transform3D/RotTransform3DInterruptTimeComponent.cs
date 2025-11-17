using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Transform3D
{
    [BurstCompile]
    public struct RotTransform3DInterruptTimeComponent : IBufferElementData
    {
        public float InterruptTime;
        public bool Interrupted;
    }
}
