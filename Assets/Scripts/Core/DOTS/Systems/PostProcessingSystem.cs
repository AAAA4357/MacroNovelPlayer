using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Managed;
using MNP.Helpers;
using Unity.Entities;
using UnityEngine;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PropertyLerpSystem))]
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
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationPropertyListComponent managedAnimationPropertyListComponent,
                                                                      ref ElementComponent elementComponent) =>
            {
                Vector2 position = managedAnimationPropertyListComponent.Property2DList[UtilityHelper.TransormPositionID].Value;
                float rotation = managedAnimationPropertyListComponent.Property1DList[UtilityHelper.TransormRotationID].Value;
                Vector2 scale = managedAnimationPropertyListComponent.Property2DList[UtilityHelper.TransormScaleID].Value;
                elementComponent.Position = position;
                elementComponent.Rotation = rotation;
                elementComponent.Scale = scale;
                elementComponent.IsBlocked = false;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ref ElementComponent elementComponent) =>
            {
                Matrix4x4 matrix = Matrix4x4.TRS(new(elementComponent.Position.x, elementComponent.Position.y, 0), Quaternion.Euler(0, 0, elementComponent.Rotation), new(elementComponent.Scale.x, elementComponent.Scale.y, 1));
                elementComponent.TransformMatrix = matrix;
            }).WithoutBurst().Run();
        }
    }
}
