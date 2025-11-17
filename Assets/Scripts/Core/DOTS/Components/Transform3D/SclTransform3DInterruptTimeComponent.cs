using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Transform3D
{
    [BurstCompile]
    public struct SclTransform3DInterruptTimeComponent : IBufferElementData
    {
        public float InterruptTime;
        public bool Interrupted;
    }
}
