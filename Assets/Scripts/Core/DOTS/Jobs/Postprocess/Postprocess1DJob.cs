using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    public partial struct Postprocess1DJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<float4> OutputArray;

        [BurstCompile]
        public void Execute(in Property1DComponent property1DComponent)
        {
            OutputArray[property1DComponent.Index] = new(property1DComponent.Value, float3.zero);
        }
    }
}
