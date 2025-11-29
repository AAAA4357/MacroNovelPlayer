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
    [WithAll(typeof(TimeEnabledComponent), typeof(LerpEnabledComponent))]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct Animation4DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation4DComponent> animation4DBuffer, DynamicBuffer<AnimationBezierBakeDataComponent> bezierDataBuffer, DynamicBuffer<AnimationSquadBakeDataComponent> squadDataBuffer, ref Property4DComponent property3DComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInBufferWithLength(animation4DBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation4DBuffer[animationIndex].EaseKeyframeList, fixedT);
            float4 result;
            switch (animation4DBuffer[animationIndex].LerpType)
            {
                case Float4LerpType.Linear:
                    result = PathLerpHelper.Lerp4DLinear(animation4DBuffer[animationIndex].StartValue, animation4DBuffer[animationIndex].EndValue, ease);
                    break;
                case Float4LerpType.SLinear:
                    result = PathLerpHelper.SLerp4DLinear(animation4DBuffer[animationIndex].StartValue, animation4DBuffer[animationIndex].EndValue, ease);
                    break;
                case Float4LerpType.Bezier:
                    result = PathLerpHelper.GetBezierPoint4D(animation4DBuffer[animationIndex].StartValue, animation4DBuffer[animationIndex].Control0, animation4DBuffer[animationIndex].Control1, animation4DBuffer[animationIndex].EndValue, ease);
                    break;
                case Float4LerpType.AverageBezier:
                    result = PathLerpHelper.GetAverageBezierPoint4D(animation4DBuffer[animationIndex].StartValue, animation4DBuffer[animationIndex].Control0, animation4DBuffer[animationIndex].Control1, animation4DBuffer[animationIndex].EndValue, bezierDataBuffer[animation4DBuffer[animationIndex].BezierDataIndex].BezierLengthMap, ease);
                    break;
                case Float4LerpType.Squad:
                    int index = animation4DBuffer[animationIndex].SquadDataIndex;
                    result = PathLerpHelper.GetSquadPoint4D(squadDataBuffer[index].q0, squadDataBuffer[index].q01, squadDataBuffer[index].q01_1q12, squadDataBuffer[index].q12_1q23, ease);
                    break;
                default:
                    return;
            }
            property3DComponent.Value = result;
        }
    }
}
