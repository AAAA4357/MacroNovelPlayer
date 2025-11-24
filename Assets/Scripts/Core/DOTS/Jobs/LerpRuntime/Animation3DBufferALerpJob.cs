using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent), typeof(LerpEnabledComponent), typeof(PipelineBufferAComponent))]
    [WithNone(typeof(PipelineBufferBComponent))]
    [WithPresent(typeof(InterruptComponent), typeof(PipelineBufferReadyComponent))]
    public partial struct Animation3DBufferALerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation3DComponent> animation3DBuffer, ref Property3DComponent property3DComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent, EnabledRefRO<PipelineBufferReadyComponent> readyComponent)
        {
            if (interruptComponent.ValueRO || readyComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInBufferWithLength(animation3DBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation3DBuffer[animationIndex].EaseKeyframeList, fixedT);
            bool isLinear = animation3DBuffer[animationIndex].Linear;
            float3 result;
            if (isLinear)
            {
                result = PathLerpHelper.Lerp3DLinear(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].EndValue, ease);
            }
            else
            {
                result = PathLerpHelper.GetBezierPoint3D(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].EndValue, animation3DBuffer[animationIndex].Control0, animation3DBuffer[animationIndex].Control1, ease);
            }
            property3DComponent.Value = result;
        }
    }
}
