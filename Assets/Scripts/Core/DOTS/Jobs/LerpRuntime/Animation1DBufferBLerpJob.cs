using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent), typeof(LerpEnabledComponent), typeof(PipelineBufferBComponent))]
    [WithNone(typeof(PipelineBufferAComponent))]
    [WithPresent(typeof(InterruptComponent), typeof(PipelineBufferReadyComponent))]
    public partial struct Animation1DBufferBLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation1DComponent> animation1DBuffer, ref Property1DComponent property1DComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent, EnabledRefRO<PipelineBufferReadyComponent> readyComponent)
        {
            if (interruptComponent.ValueRO || readyComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInBufferWithLength(animation1DBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation1DBuffer[animationIndex].EaseKeyframeList, fixedT);
            float result = PathLerpHelper.Lerp1DLinear(animation1DBuffer[animationIndex].StartValue, animation1DBuffer[animationIndex].EndValue, ease);
            property1DComponent.Value = result;
        }
    }
}
