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
    public partial struct Animation3DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation3DComponent> animation3DBuffer, DynamicBuffer<AnimationBezierBakeDataComponent> bezierDataBuffer, ref Property3DComponent property3DComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInBufferWithLength(animation3DBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation3DBuffer[animationIndex].EaseKeyframeList, fixedT);
            int lerpType = animation3DBuffer[animationIndex].LerpType;
            float3 result;
            switch (lerpType)
            {
                case UtilityHelper.Float2_LinearLerp:
                    result = PathLerpHelper.Lerp3DLinear(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].EndValue, ease);
                    break;
                case UtilityHelper.Float2_BezierLerp:
                    result = PathLerpHelper.GetBezierPoint3D(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].EndValue, animation3DBuffer[animationIndex].Control0, animation3DBuffer[animationIndex].Control1, ease);
                    break;
                case UtilityHelper.Float2_AverageBezierLerp:
                    result = PathLerpHelper.GetAverageBezierPoint3D(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].EndValue, animation3DBuffer[animationIndex].Control0, animation3DBuffer[animationIndex].Control1, bezierDataBuffer[animation3DBuffer[animationIndex].BezierDataIndex].BezierLength, ease);
                    break;
                default:
                    return;
            }
            property3DComponent.Value = result;
        }
    }
}
