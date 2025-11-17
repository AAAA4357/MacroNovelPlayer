using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Managed;
using MNP.Core.DOTS.Components.Transform2D;
using MNP.Core.DOTS.Components.Transform3D;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PropertyLerpSystem))]
    partial class PostprocessingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent,
                                                                      in PosTransform2DPropertyComponent propertyComponent) =>
            {
                managedTransform2DComponent.RefValue.Value.Position = propertyComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent,
                                                                      in RotTransform2DPropertyComponent propertyComponent) =>
            {
                managedTransform2DComponent.RefValue.Value.Rotation = propertyComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent,
                                                                      in SclTransform2DPropertyComponent propertyComponent) =>
            {
                managedTransform2DComponent.RefValue.Value.Scale = propertyComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationTransform3DPropertyComponent managedTransform2DComponent,
                                                                      in PosTransform3DPropertyComponent propertyComponent) =>
            {
                managedTransform2DComponent.RefValue.Value.Position = propertyComponent.Value;
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationTransform3DPropertyComponent managedTransform2DComponent,
                                                                      in RotTransform3DPropertyComponent propertyComponent) =>
            {
                managedTransform2DComponent.RefValue.Value.Rotation = new(propertyComponent.Value.x, propertyComponent.Value.y, propertyComponent.Value.z, propertyComponent.Value.w);
            }).WithoutBurst().Run();
            Entities.WithAll<InitializedPropertyComponent>().ForEach((ManagedAnimationTransform3DPropertyComponent managedTransform2DComponent,
                                                                      in SclTransform3DPropertyComponent propertyComponent) =>
            {
                managedTransform2DComponent.RefValue.Value.Scale = propertyComponent.Value;
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
                Quaternion rotation = managedAnimationPropertyListComponent.Transform3DProperty.Value.Rotation;
                Vector3 scale = managedAnimationPropertyListComponent.Transform3DProperty.Value.Scale;
                Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
                elementComponent.TransformMatrix = matrix;
            }).WithoutBurst().Run();
        }
    }
}
