using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Managed;
using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PropertyLerpSystemGroup))]
    partial class PostprocessingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationProperty1DComponent managedProperty1DComponent,
                                                                      in Property1DComponent property1DComponent) =>
            {
                managedProperty1DComponent.RefValue.Value = property1DComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationProperty2DComponent managedProperty2DComponent,
                                                                      in Property2DComponent property2DComponent) =>
            {
                managedProperty2DComponent.RefValue.Value = property2DComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationProperty3DComponent managedProperty3DComponent,
                                                                      in Property3DComponent property3DComponent) =>
            {
                managedProperty3DComponent.RefValue.Value = property3DComponent.Value;
            }).WithoutBurst().Run();
        }
    }
}
