using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct InterruptTimeComponent : IBufferElementData
    {
        public float InterruptTime;
        public bool Interrupted;
    }
}
