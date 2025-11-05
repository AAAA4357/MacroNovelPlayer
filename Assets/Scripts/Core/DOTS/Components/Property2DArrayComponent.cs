using MNP.Core.DataStruct;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Property2DArrayComponent : IComponentData
    {
        public NativeArray<LerpProperty1D> Properties;
    }
}
