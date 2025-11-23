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
        public void Execute(DynamicBuffer<Animation4DComponent> animation4DBuffer, DynamicBuffer<Animation4DBakeDataComponent> animation4DDataBuffer, ref Property4DComponent property3DComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInBufferWithLength(animation4DBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation4DBuffer[animationIndex].EaseKeyframeList, fixedT);
            int lerpType = animation4DBuffer[animationIndex].LerpType;
            float4 result;
            switch (lerpType)
            {
                case UtilityHelper.Quaternion_LinearLerp:
                    result = PathLerpHelper.Lerp4DLinear(animation4DBuffer[animationIndex].StartValue, animation4DBuffer[animationIndex].EndValue, ease);
                    break;
                case UtilityHelper.Quaternion_LinearSLerp:
                    result = PathLerpHelper.SLerp4DLinear(animation4DBuffer[animationIndex].StartValue, animation4DBuffer[animationIndex].EndValue, ease);
                    break;
                case UtilityHelper.Quaternion_PathLerp:
                    int index = animation4DBuffer[animationIndex].DataIndex;
                    result = PathLerpHelper.GetBezierPoint4D(animation4DDataBuffer[index].q0, animation4DDataBuffer[index].q01, animation4DDataBuffer[index].q01_1q12, animation4DDataBuffer[index].q12_1q23, ease);
                    break;
                default:
                    return;
            }
            property3DComponent.Value = result;
        }
    }
}
