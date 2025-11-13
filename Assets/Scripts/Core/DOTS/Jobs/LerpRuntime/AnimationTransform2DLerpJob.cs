using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    public partial struct AnimationTransform2DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Transform2DPositionAnimationComponent> positionBuffer, DynamicBuffer<Transform2DRotationAnimationComponent> rotationBuffer, DynamicBuffer<Transform2DScaleAnimationComponent> scaleBuffer, in TransformPropertyInfoComponent transformInfoComponent, ref Transform2DPropertyComponent transformComponent, in TimeComponent timeComponent)
        {
            if (transformInfoComponent.PositionLerpEnabled)
            {
                //Position
                UtilityHelper.GetFloorIndexInBuffer(positionBuffer, v => v.StartTime, timeComponent.Time, out int animationIndex, out float fixedT);
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
                transformComponent.Position = result;
            }
            if (transformInfoComponent.RotationLerpEnabled)
            {
                //Rotation
                UtilityHelper.GetFloorIndexInBuffer(rotationBuffer, v => v.StartTime, timeComponent.Time, out int animationIndex, out float fixedT);
                float ease = EasingFunctionHelper.GetEase(rotationBuffer[animationIndex].EaseKeyframeList, fixedT);
                float result = PathLerpHelper.Lerp1DLinear(rotationBuffer[animationIndex].StartValue, rotationBuffer[animationIndex].EndValue, ease);
                transformComponent.Rotation = result;
            }
            if (transformInfoComponent.ScaleLerpEnabled)
            {
                //Scale
                UtilityHelper.GetFloorIndexInBuffer(scaleBuffer, v => v.StartTime, timeComponent.Time, out int animationIndex, out float fixedT);
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
                transformComponent.Scale = result;
            }
        }
    }
}
