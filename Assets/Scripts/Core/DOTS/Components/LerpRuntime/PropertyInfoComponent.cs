using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct PropertyInfoComponent : IComponentData
    {
        public float StartTime;
        public float EndTime;
        public bool LerpEnabled;
    }
}
