using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    public partial struct Animation3DLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<Animation3DComponent> animation3DBuffer, ref Property3DComponent property3DComponent, in TimeComponent timeComponent)
        {
            UtilityHelper.GetFloorIndexInBuffer(animation3DBuffer, v => v.StartTime, timeComponent.Time, out int animationIndex, out float fixedT);
            float ease = EasingFunctionHelper.GetEase(animation3DBuffer[animationIndex].EaseKeyframeList, fixedT);
            bool isLinear = animation3DBuffer[animationIndex].Linear;
            float3 result;
            if (isLinear)
            {
                result = PathLerpHelper.Lerp3DLinear(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].EndValue, fixedT);
            }
            else
            {
                result = PathLerpHelper.GetBezierPoint3D(animation3DBuffer[animationIndex].StartValue, animation3DBuffer[animationIndex].EndValue, animation3DBuffer[animationIndex].Control0, animation3DBuffer[animationIndex].Control1, fixedT);
            }
            property3DComponent.Value = result;
        }
    }
}
