using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.Transform2D
{
    [BurstCompile]
    public struct RotTransform2DInterruptTimeComponent : IBufferElementData
    {
        public float InterruptTime;
        public bool Interrupted;
    }
}
