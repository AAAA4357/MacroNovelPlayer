using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime.Transform2D;
using MNP.Core.DOTS.Components.Transform2D;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs.Transform2D
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    [WithPresent(typeof(SclTransform2DInterruptComponent))]
    public partial struct AnimationSclTransform2DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Transform2DScaleAnimationComponent> scaleBuffer, in Transform2DPropertyInfoComponent transformInfoComponent, ref SclTransform2DPropertyComponent transformComponent, in SclTransform2DTimeComponent timeComponent, EnabledRefRO<SclTransform2DInterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            if (!transformInfoComponent.ScaleLerpEnabled)
            {
                return;
            }
            //Scale
            UtilityHelper.GetFloorIndexInBufferWithLength(scaleBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(scaleBuffer[animationIndex].EaseKeyframeList, fixedT);
            bool isLinear = scaleBuffer[animationIndex].Linear;
            float2 result;
            if (isLinear)
            {
                result = PathLerpHelper.Lerp2DLinear(scaleBuffer[animationIndex].StartValue, scaleBuffer[animationIndex].EndValue, ease);
            }
            else
            {
                result = PathLerpHelper.GetBezierPoint2D(scaleBuffer[animationIndex].StartValue, scaleBuffer[animationIndex].EndValue, scaleBuffer[animationIndex].Control0, scaleBuffer[animationIndex].Control1, ease);
            }
            transformComponent.Value = result;
        }
    }
}
