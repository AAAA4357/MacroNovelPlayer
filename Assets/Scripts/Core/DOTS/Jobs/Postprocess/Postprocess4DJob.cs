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
    public partial struct Postprocess4DJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<float4> OutputArray;

        [BurstCompile]
        public void Execute(in Property4DComponent property4DComponent)
        {
            OutputArray[property4DComponent.Index] = property4DComponent.Value;
        }
    }
}
