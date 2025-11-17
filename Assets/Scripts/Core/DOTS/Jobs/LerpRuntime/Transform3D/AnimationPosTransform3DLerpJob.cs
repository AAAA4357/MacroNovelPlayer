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
    [WithPresent(typeof(PosTransform3DInterruptComponent))]
    public partial struct AnimationPosTransform3DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Transform3DPositionAnimationComponent> positionBuffer, in Transform3DPropertyInfoComponent transformInfoComponent, ref PosTransform3DPropertyComponent transformComponent, in PosTransform3DTimeComponent timeComponent, EnabledRefRO<PosTransform3DInterruptComponent> interruptComponent)
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
            float3 result;
            if (isLinear)
            {
                result = PathLerpHelper.Lerp3DLinear(positionBuffer[animationIndex].StartValue, positionBuffer[animationIndex].EndValue, ease);
            }
            else
            {
                result = PathLerpHelper.GetBezierPoint3D(positionBuffer[animationIndex].StartValue, positionBuffer[animationIndex].EndValue, positionBuffer[animationIndex].Control0, positionBuffer[animationIndex].Control1, ease);
            }
            transformComponent.Value = result;
        }
    }
}
