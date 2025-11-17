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
    [WithPresent(typeof(RotTransform3DInterruptComponent))]
    public partial struct AnimationRotTransform3DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Transform3DRotationAnimationComponent> rotationBuffer, in Transform3DPropertyInfoComponent transformInfoComponent, ref RotTransform3DPropertyComponent transformComponent, in RotTransform3DTimeComponent timeComponent, EnabledRefRO<RotTransform3DInterruptComponent> interruptComponent)
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
            float4 result = PathLerpHelper.FastSquad(rotationBuffer[animationIndex].StartValue, rotationBuffer[animationIndex].Control0, rotationBuffer[animationIndex].Control1, rotationBuffer[animationIndex].EndValue, ease);
            transformComponent.Value = result;
        }
    }
}
