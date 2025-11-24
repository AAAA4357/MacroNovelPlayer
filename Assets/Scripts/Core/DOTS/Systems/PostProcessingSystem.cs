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
        protected override void OnCreate()
        {
            RequireForUpdate<PipelineBufferReadyComponent>();
            
        }

        protected override void OnUpdate()
        {
            foreach (var (managedTransform2DComponent, propertyComponent, _) in SystemAPI.Query<ManagedAnimationTransform2DPropertyComponent, RefRO<Property2DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent, Transform2DPositionComponent>())
            {
                managedTransform2DComponent.RefValue.Value.Position = propertyComponent.ValueRO.Value;
            }
            foreach (var (managedTransform2DComponent, propertyComponent, _) in SystemAPI.Query<ManagedAnimationTransform2DPropertyComponent, RefRO<Property1DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent, Transform2DRotationComponent>())
            {
                managedTransform2DComponent.RefValue.Value.Rotation = propertyComponent.ValueRO.Value;
            }
            foreach (var (managedTransform2DComponent, propertyComponent, _) in SystemAPI.Query<ManagedAnimationTransform2DPropertyComponent, RefRO<Property2DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent, Transform2DScaleComponent>())
            {
                managedTransform2DComponent.RefValue.Value.Scale = propertyComponent.ValueRO.Value;
            }
            foreach (var (managedTransform3DComponent, propertyComponent, _) in SystemAPI.Query<ManagedAnimationTransform3DPropertyComponent, RefRO<Property3DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent, Transform3DPositionComponent>())
            {
                managedTransform3DComponent.RefValue.Value.Position = propertyComponent.ValueRO.Value;
            }
            foreach (var (managedTransform3DComponent, propertyComponent, _) in SystemAPI.Query<ManagedAnimationTransform3DPropertyComponent, RefRO<Property4DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent, Transform3DRotationComponent>())
            {
                managedTransform3DComponent.RefValue.Value.Rotation = propertyComponent.ValueRO.Value;
            }
            foreach (var (managedTransform3DComponent, propertyComponent, _) in SystemAPI.Query<ManagedAnimationTransform3DPropertyComponent, RefRO<Property3DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent, Transform3DScaleComponent>())
            {
                managedTransform3DComponent.RefValue.Value.Scale = propertyComponent.ValueRO.Value;
            }
            foreach (var (managedProperty1DComponent, property1DComponent, _) in SystemAPI.Query<ManagedAnimationProperty1DComponent, RefRO<Property1DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent>())
            {
                managedProperty1DComponent.RefValue.Value = property1DComponent.ValueRO.Value;
            }
            foreach (var (managedProperty2DComponent, property2DComponent, _) in SystemAPI.Query<ManagedAnimationProperty2DComponent, RefRO<Property2DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent>())
            {
                managedProperty2DComponent.RefValue.Value = property2DComponent.ValueRO.Value;
            }
            foreach (var (managedProperty3DComponent, property3DComponent, _) in SystemAPI.Query<ManagedAnimationProperty3DComponent, RefRO<Property3DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent>())
            {
                managedProperty3DComponent.RefValue.Value = property3DComponent.ValueRO.Value;
            }
            foreach (var (managedProperty4DComponent, property4DComponent, _) in SystemAPI.Query<ManagedAnimationProperty4DComponent, RefRO<Property4DComponent>, EnabledRefRO<PipelineBufferReadyComponent>>().WithAll<InitializedPropertyComponent>())
            {
                managedProperty4DComponent.RefValue.Value = property4DComponent.ValueRO.Value;
            }
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
