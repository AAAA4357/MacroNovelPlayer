using MNP.Core.DataStruct;
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
    [WithPresent(typeof(InterruptComponent), typeof(LerpEnabledComponent))]
    public partial struct Animation3DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation3DComponent> animation3DBuffer, DynamicBuffer<AnimationBezierBakeDataComponent> bezierDataBuffer, ref Property3DComponent property3DComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent, EnabledRefRO<TimeEnabledComponent> _, EnabledRefRO<LerpEnabledComponent> lerpEnabledComponent)
        {
            if (interruptComponent.ValueRO || !lerpEnabledComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInBufferWithLength(animation3DBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation3DBuffer[animationIndex].EaseKeyframeList, fixedT);
            float3 result;
            switch (animation3DBuffer[animationIndex].LerpType)
            {
                case Float3LerpType.Linear:
                    result = PathLerpHelper.Lerp3DLinear(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].EndValue, ease);
                    break;
                case Float3LerpType.Bezier:
                    result = PathLerpHelper.GetBezierPoint3D(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].Control0, animation3DBuffer[animationIndex].Control1, animation3DBuffer[animationIndex].EndValue, ease);
                    break;
                case Float3LerpType.AverageBezier:
                    result = PathLerpHelper.GetAverageBezierPoint3D(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].Control0, animation3DBuffer[animationIndex].Control1, animation3DBuffer[animationIndex].EndValue, bezierDataBuffer[animation3DBuffer[animationIndex].BezierDataIndex].BezierLengthMap, ease);
                    break;
                default:
                    return;
            }
            property3DComponent.Value = result;
        }
    }
}
