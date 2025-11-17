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
    [WithPresent(typeof(PosTransform2DInterruptComponent))]
    public partial struct AnimationPosTransform2DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Transform2DPositionAnimationComponent> positionBuffer, in Transform2DPropertyInfoComponent transformInfoComponent, ref PosTransform2DPropertyComponent transformComponent, in PosTransform2DTimeComponent timeComponent, EnabledRefRO<PosTransform2DInterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            if (!transformInfoComponent.PositionLerpEnabled)
            {
                return;
            }
            //Position
            UtilityHelper.GetFloorIndexInBufferWithLength(positionBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(positionBuffer[animationIndex].EaseKeyframeList, fixedT);
            bool isLinear = positionBuffer[animationIndex].Linear;
            float2 result;
            if (isLinear)
            {
                result = PathLerpHelper.Lerp2DLinear(positionBuffer[animationIndex].StartValue, positionBuffer[animationIndex].EndValue, ease);
            }
            else
            {
                result = PathLerpHelper.GetBezierPoint2D(positionBuffer[animationIndex].StartValue, positionBuffer[animationIndex].EndValue, positionBuffer[animationIndex].Control0, positionBuffer[animationIndex].Control1, ease);
            }
            transformComponent.Value = result;
        }
    }
}
