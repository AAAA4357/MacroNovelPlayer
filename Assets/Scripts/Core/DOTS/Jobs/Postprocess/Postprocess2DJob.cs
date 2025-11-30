using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    public partial struct Postprocess2DJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<float4> OutputArray;

        [BurstCompile]
        public void Execute(in Property2DComponent property2DComponent, EnabledRefRO<TimeEnabledComponent> enabled)
        {
            if (!enabled.ValueRO)
            {
                return;
            }
            OutputArray[property2DComponent.Index] = new(property2DComponent.Value, float2.zero);
        }
    }
}
