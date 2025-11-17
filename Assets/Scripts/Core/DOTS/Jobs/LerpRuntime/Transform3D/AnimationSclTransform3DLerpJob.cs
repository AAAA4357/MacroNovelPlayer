using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime.Transform3D;
using MNP.Core.DOTS.Components.Transform3D;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs.Transform3D
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    [WithPresent(typeof(SclTransform3DInterruptComponent))]
    public partial struct AnimationSclTransform3DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Transform3DScaleAnimationComponent> scaleBuffer, in Transform3DPropertyInfoComponent transformInfoComponent, ref SclTransform3DPropertyComponent transformComponent, in SclTransform3DTimeComponent timeComponent, EnabledRefRO<SclTransform3DInterruptComponent> interruptComponent)
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
            float3 result;
            if (isLinear)
            {
                result = PathLerpHelper.Lerp3DLinear(scaleBuffer[animationIndex].StartValue, scaleBuffer[animationIndex].EndValue, ease);
            }
            else
            {
                result = PathLerpHelper.GetBezierPoint3D(scaleBuffer[animationIndex].StartValue, scaleBuffer[animationIndex].EndValue, scaleBuffer[animationIndex].Control0, scaleBuffer[animationIndex].Control1, ease);
            }
            transformComponent.Value = result;
        }
    }
}
