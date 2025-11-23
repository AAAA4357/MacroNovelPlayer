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
        public void Execute(DynamicBuffer<Transform3DRotationAnimationComponent> rotationBuffer, DynamicBuffer<Transform3DRotationBakeDataComponent> rotationDataBuffer, in Transform3DPropertyInfoComponent transformInfoComponent, ref RotTransform3DPropertyComponent transformComponent, in RotTransform3DTimeComponent timeComponent, EnabledRefRO<RotTransform3DInterruptComponent> interruptComponent)
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
            int lerpType = rotationBuffer[animationIndex].LerpType;
            float4 result;
            switch (lerpType)
            {
                case UtilityHelper.Quaternion_LinearLerp:
                    result = PathLerpHelper.Lerp4DLinear(rotationBuffer[animationIndex].StartValue, rotationBuffer[animationIndex].EndValue, ease);
                    break;
                case UtilityHelper.Quaternion_LinearSLerp:
                    result = PathLerpHelper.SLerp4DLinear(rotationBuffer[animationIndex].StartValue, rotationBuffer[animationIndex].EndValue, ease);
                    break;
                case UtilityHelper.Quaternion_PathLerp:
                    int index = rotationBuffer[animationIndex].DataIndex;
                    result = PathLerpHelper.GetBezierPoint4D(rotationDataBuffer[index].q0, rotationDataBuffer[index].q01, rotationDataBuffer[index].q01_1q12, rotationDataBuffer[index].q12_1q23, ease);
                    break;
                default:
                    return;
            }
            transformComponent.Value = result;
        }
    }
}
