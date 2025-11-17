using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Transform2D;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent), typeof(LerpEnabledComponent))]
    [WithNone(typeof(PosTransform2DPropertyComponent), typeof(SclTransform2DPropertyComponent))]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct Animation2DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation2DComponent> animation2DBuffer, ref Property2DComponent property2DComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInBufferWithLength(animation2DBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation2DBuffer[animationIndex].EaseKeyframeList, fixedT);
            bool isLinear = animation2DBuffer[animationIndex].Linear;
            float2 result;
            if (isLinear)
            {
                result = PathLerpHelper.Lerp2DLinear(animation2DBuffer[animationIndex].StartValue, animation2DBuffer[animationIndex].EndValue, ease);
            }
            else
            {
                result = PathLerpHelper.GetBezierPoint2D(animation2DBuffer[animationIndex].StartValue, animation2DBuffer[animationIndex].EndValue, animation2DBuffer[animationIndex].Control0, animation2DBuffer[animationIndex].Control1, ease);
            }
            property2DComponent.Value = result;
        }
    }
}
