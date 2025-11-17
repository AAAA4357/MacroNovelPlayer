using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime.Transform2D;
using MNP.Core.DOTS.Components.Transform2D;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs.Transform2D
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    [WithPresent(typeof(RotTransform2DInterruptComponent))]
    public partial struct AnimationRotTransform2DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Transform2DRotationAnimationComponent> rotationBuffer, in Transform2DPropertyInfoComponent transformInfoComponent, ref RotTransform2DPropertyComponent transformComponent, in RotTransform2DTimeComponent timeComponent, EnabledRefRO<RotTransform2DInterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            if (!transformInfoComponent.RotationLerpEnabled)
            {
                return;
            }
            //Rotation
            UtilityHelper.GetFloorIndexInBufferWithLength(rotationBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(rotationBuffer[animationIndex].EaseKeyframeList, fixedT);
            float result = PathLerpHelper.Lerp1DLinear(rotationBuffer[animationIndex].StartValue, rotationBuffer[animationIndex].EndValue, ease);
            transformComponent.Value = result;
        }
    }
}
