using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    public partial struct Animation1DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation1DComponent> animation1DBuffer, ref Property1DComponent property1DComponent, in TimeComponent timeComponent)
        {
            UtilityHelper.GetFloorIndexInBuffer(animation1DBuffer, v => v.StartTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation1DBuffer[animationIndex].EaseKeyframeList, fixedT);
            float result = PathLerpHelper.Lerp1DLinear(animation1DBuffer[animationIndex].StartValue, animation1DBuffer[animationIndex].EndValue, fixedT);
            property1DComponent.Value = result;
        }
    }
}
