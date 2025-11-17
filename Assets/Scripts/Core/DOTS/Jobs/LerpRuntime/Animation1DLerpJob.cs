using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Transform2D;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent), typeof(LerpEnabledComponent))]
    [WithNone(typeof(RotTransform2DPropertyComponent))]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct Animation1DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation1DComponent> animation1DBuffer, ref Property1DComponent property1DComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInBufferWithLength(animation1DBuffer, v => v.StartTime, v => v.DurationTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation1DBuffer[animationIndex].EaseKeyframeList, fixedT);
            float result = PathLerpHelper.Lerp1DLinear(animation1DBuffer[animationIndex].StartValue, animation1DBuffer[animationIndex].EndValue, ease);
            property1DComponent.Value = result;
        }
    }
}
