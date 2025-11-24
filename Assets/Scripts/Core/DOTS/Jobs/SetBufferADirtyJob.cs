using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(PipelineBufferAComponent))]
    [WithNone(typeof(PipelineBufferBComponent))]
    [WithPresent(typeof(PipelineBufferReadyComponent))]
    public partial struct SetBufferADirtyJob : IJobEntity
    {
        public void Execute(EnabledRefRW<PipelineBufferReadyComponent> readyComponent)
        {
            readyComponent.ValueRW = false;
        }
    }
}
