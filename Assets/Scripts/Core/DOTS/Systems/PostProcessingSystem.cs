using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Managed;
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
                float2 position = managedAnimationPropertyListComponent.TransformProperty.Value.Position;
                float rotation = managedAnimationPropertyListComponent.TransformProperty.Value.Rotation;
                float2 scale = managedAnimationPropertyListComponent.TransformProperty.Value.Scale;
                Matrix4x4 matrix = Matrix4x4.TRS(new(position.x, position.y, 0), Quaternion.Euler(0, 0, rotation), new(scale.x, scale.y, 1));
                elementComponent.TransformMatrix = matrix;
            }).WithoutBurst().Run();
        }
    }
}
