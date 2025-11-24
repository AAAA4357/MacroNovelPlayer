using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(PipelineBufferBComponent))]
    [WithNone(typeof(PipelineBufferAComponent))]
    [WithPresent(typeof(PipelineBufferReadyComponent))]
    public partial struct SetBufferBReadyJob : IJobEntity
    {
        public void Execute(EnabledRefRW<PipelineBufferReadyComponent> readyComponent)
        {
            readyComponent.ValueRW = true;
        }
    }
}
