using MNP.Core.DataStruct;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    public partial struct PostprocessDependency1DJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<float4> InputArray;

        [BurstCompile]
        public void Execute(DynamicBuffer<DependencyPropertyComponent> dependencyPropertyBuffer, ref Property1DComponent property1DComponent, EnabledRefRO<TimeEnabledComponent> enabled)
        {
            if (!enabled.ValueRO || property1DComponent.DependencyType is null)
            {
                return;
            }
            switch (property1DComponent.DependencyType.Value)
            {
                case DependencyPropertyType.Add:
                    foreach (DependencyPropertyComponent component in dependencyPropertyBuffer)
                    {
                        InputArray[property1DComponent.Index] += InputArray[component.PropertyIndex];
                    }
                    break;
                case DependencyPropertyType.Multiply:
                    foreach (DependencyPropertyComponent component in dependencyPropertyBuffer)
                    {
                        InputArray[property1DComponent.Index] += InputArray[component.PropertyIndex];
                    }
                    break;
            }
        }
    }
}
