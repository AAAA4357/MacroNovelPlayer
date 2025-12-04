using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Helpers;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [WithAll(typeof(TimeEnabledComponent))]
    [WithPresent(typeof(InterruptComponent), typeof(LerpEnabledComponent))]
    public partial struct AnimationStringJob : IJobEntity
    {
        public void Execute(AnimationStringListComponent animationStringList, PropertyStringComponent propertyStringComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent, EnabledRefRO<TimeEnabledComponent> _, EnabledRefRO<LerpEnabledComponent> lerpEnabledComponent)
        {
            if (interruptComponent.ValueRO || !lerpEnabledComponent.ValueRO) 
            {
                return;
            }
            UtilityHelper.GetFloorIndexInContainer(animationStringList.Animations, v => v.StartTime, timeComponent.Time, out int animationIndex);
            string NewValue = animationStringList.Animations[animationIndex].Value;
            propertyStringComponent.Value = NewValue;
        }
    }
}
