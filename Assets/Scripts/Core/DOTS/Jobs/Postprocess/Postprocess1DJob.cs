using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    public partial struct Postprocess1DJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<float4> OutputArray;

        [BurstCompile]
        public void Execute(in Property1DComponent property1DComponent, EnabledRefRO<TimeEnabledComponent> _)
        {
            OutputArray[property1DComponent.Index] = new(property1DComponent.Value, float3.zero);
        }
    }
}
