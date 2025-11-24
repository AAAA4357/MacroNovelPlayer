using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Managed;
using MNP.Core.DOTS.Components.Transform;
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
            Entities.WithAll<InitializedPropertyComponent, Transform2DPositionComponent>().ForEach((ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent,
                                                                                                    in Property2DComponent propertyComponent) =>
            {
                managedTransform2DComponent.RefValue.Value.Position = propertyComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent, Transform2DRotationComponent>().ForEach((ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent,
                                                                                                    in Property1DComponent propertyComponent) =>
            {
                managedTransform2DComponent.RefValue.Value.Rotation = propertyComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent, Transform2DScaleComponent>().ForEach((ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent,
                                                                                                 in Property2DComponent propertyComponent) =>
            {
                managedTransform2DComponent.RefValue.Value.Scale = propertyComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent, Transform3DPositionComponent>().ForEach((ManagedAnimationTransform3DPropertyComponent managedTransform3DComponent,
                                                                                                    in Property3DComponent propertyComponent) =>
            {
                managedTransform3DComponent.RefValue.Value.Position = propertyComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent, Transform3DRotationComponent>().ForEach((ManagedAnimationTransform3DPropertyComponent managedTransform3DComponent,
                                                                                                    in Property4DComponent propertyComponent) =>
            {
                managedTransform3DComponent.RefValue.Value.Rotation = propertyComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent, Transform3DScaleComponent>().ForEach((ManagedAnimationTransform3DPropertyComponent managedTransform3DComponent,
                                                                                                 in Property3DComponent propertyComponent) =>
            {
                managedTransform3DComponent.RefValue.Value.Scale = propertyComponent.Value;
            }).WithoutBurst().Run();
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
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationProperty4DComponent managedProperty4DComponent,
                                                                      in Property4DComponent property4DComponent) =>
            {
                managedProperty4DComponent.RefValue.Value = property4DComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimation2DPropertyListComponent managedAnimationPropertyListComponent,
                                                                      ref ElementComponent elementComponent) =>
            {
                Vector3 position = managedAnimationPropertyListComponent.Transform2DProperty.Value.Position;
                float rotation = managedAnimationPropertyListComponent.Transform2DProperty.Value.Rotation;
                Vector3 scale = managedAnimationPropertyListComponent.Transform2DProperty.Value.Scale;
                Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.Euler(0, 0, rotation), scale);
                elementComponent.TransformMatrix = matrix;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimation3DPropertyListComponent managedAnimationPropertyListComponent,
                                                                      ref ElementComponent elementComponent) =>
            {
                Vector3 position = managedAnimationPropertyListComponent.Transform3DProperty.Value.Position;
                Vector4 rotation = managedAnimationPropertyListComponent.Transform3DProperty.Value.Rotation;
                Vector3 scale = managedAnimationPropertyListComponent.Transform3DProperty.Value.Scale;
                Matrix4x4 matrix = Matrix4x4.TRS(position, new(rotation.x, rotation.y, rotation.z, rotation.w), scale);
                elementComponent.TransformMatrix = matrix;
            }).WithoutBurst().Run();
        }
    }
}
