using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent), typeof(LerpEnabledComponent))]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct Animation2DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation2DComponent> animation2DBuffer, DynamicBuffer<AnimationBezierBakeDataComponent> bezierDataBuffer, ref Property2DComponent property2DComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInBufferWithLength(animation2DBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation2DBuffer[animationIndex].EaseKeyframeList, fixedT);
            int lerpType = animation2DBuffer[animationIndex].LerpType;
            float2 result;
            switch (lerpType)
            {
                case UtilityHelper.Float2_LinearLerp:
                    result = PathLerpHelper.Lerp2DLinear(animation2DBuffer[animationIndex].StartValue, animation2DBuffer[animationIndex].EndValue, ease);
                    break;
                case UtilityHelper.Float2_BezierLerp:
                    result = PathLerpHelper.GetBezierPoint2D(animation2DBuffer[animationIndex].StartValue, animation2DBuffer[animationIndex].EndValue, animation2DBuffer[animationIndex].Control0, animation2DBuffer[animationIndex].Control1, ease);
                    break;
                case UtilityHelper.Float2_AverageBezierLerp:
                    result = PathLerpHelper.GetAverageBezierPoint2D(animation2DBuffer[animationIndex].StartValue, animation2DBuffer[animationIndex].EndValue, animation2DBuffer[animationIndex].Control0, animation2DBuffer[animationIndex].Control1, bezierDataBuffer[animation2DBuffer[animationIndex].BezierDataIndex].BezierLength, ease);
                    break;
                default:
                    return;
            }
            property2DComponent.Value = result;
        }
    }
}
