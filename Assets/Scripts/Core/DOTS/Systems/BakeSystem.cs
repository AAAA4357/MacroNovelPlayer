using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Managed;
using MNP.Core.DOTS.Components.Transform;
using MNP.Helpers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(InputSystem))]
    partial class BakeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            Entities.WithNone<InitializedPropertyComponent>().ForEach((in Entity entity, in ManagedAnimationListComponent animation) =>
            {
                switch (animation.ObjectType)
                {
                    case ObjectType.Object2D:
                        ManagedAnimation2DPropertyListComponent component2D = new()
                        {
                            Property1DList = new(),
                            Property2DList = new(),
                            Property3DList = new(),
                            Property4DList = new()
                        };
                        ecb.AddComponent(entity, component2D);
                        SeperateAnimationObject2D(animation, component2D, ecb, entity);
                        break;
                    case ObjectType.Object3D:
                        ManagedAnimation3DPropertyListComponent component3D = new()
                        {
                            Property1DList = new(),
                            Property2DList = new(),
                            Property3DList = new(),
                            Property4DList = new()
                        };
                        ecb.AddComponent(entity, component3D);
                        SeperateAnimationObject3D(animation, component3D, ecb, entity);
                        break;
                }
                //ecb.RemoveComponent<ManagedAnimationListComponent>(entity);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
            }).WithoutBurst().Run();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        #region 2D

        private void SeperateAnimationObject2D(ManagedAnimationListComponent animationListComponent,
                                               ManagedAnimation2DPropertyListComponent property2DListComponent,
                                               EntityCommandBuffer ecb,
                                               Entity entity)
        {
            RefAnimationTransform2DProperty transform2DProperty = new()
            {
                Value = new()
            };
            SeperateCustom1DProperty2D(animationListComponent, property2DListComponent, transform2DProperty, ecb);
            SeperateCustom2DProperty2D(animationListComponent, property2DListComponent, transform2DProperty, ecb);
            SeperateCustom3DProperty2D(animationListComponent, property2DListComponent, ecb);
            SeperateCustom4DProperty2D(animationListComponent, property2DListComponent, ecb);
            property2DListComponent.Transform2DProperty = transform2DProperty;
            ecb.AddComponent(entity, new BakeReadyComponent());
        }

        private void SeperateCustom1DProperty2D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation2DPropertyListComponent propertyListComponent,
                                                RefAnimationTransform2DProperty refTransform2D,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty1D property in animationListComponent.AnimationProperty1DList)
            {
                Entity entityA = ecb.CreateEntity();
                RefAnimationProperty1D refValue = new();
                Property1DComponent property1DComponent = new();
                if (property.IsStatic)
                {
                    property1DComponent.Value = property.StaticValue.Value;
                    ManagedAnimationProperty1DComponent managedProperty1DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, property1DComponent);
                    ecb.AddComponent(entityA, managedProperty1DComponent);
                    ecb.AddComponent(entityA, new InitializedPropertyComponent());
                    continue;
                }
                Entity entityB = ecb.CreateEntity();
                if (property.Type != PropertyType.Transform2DRotation)
                {
                    propertyListComponent.Property1DList.Add(property.ID, refValue);
                }

                List<Animation1D> animationList = animationListComponent.Animation1DDictionary[property.ID];

                ecb.AddBuffer<Animation1DComponent>(entityA);
                ecb.AddBuffer<Animation1DComponent>(entityB);
                foreach (Animation1D animation in animationList)
                {
                    FixedList128Bytes<float4> easeList = new();
                    for (int i = 0; i < easeList.Capacity; i++)
                    {
                        if (i >= animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[i].KeyTime, animation.EaseKeyframeList[i].Value, animation.EaseKeyframeList[i].InTan, animation.EaseKeyframeList[i].OutTan));
                    }
                    Animation1DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entityA);
                ecb.AddBuffer<InterruptTimeComponent>(entityB);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                ecb.AddComponent(entityA, property1DComponent);
                ecb.AddComponent(entityA, propertyInfoComponent);
                ecb.AddComponent(entityA, timeComponent);
                ecb.AddComponent(entityA, new LerpEnabledComponent());
                ecb.AddComponent(entityA, new InitializedPropertyComponent());
                ecb.AddComponent(entityA, new TimeEnabledComponent());
                ecb.AddComponent(entityA, new InterruptComponent());
                ecb.AddComponent(entityA, new PipelineBufferAComponent());
                ecb.AddComponent(entityA, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityA, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityA, false);

                ecb.AddComponent(entityB, property1DComponent);
                ecb.AddComponent(entityB, propertyInfoComponent);
                ecb.AddComponent(entityB, timeComponent);
                ecb.AddComponent(entityB, new LerpEnabledComponent());
                ecb.AddComponent(entityB, new InitializedPropertyComponent());
                ecb.AddComponent(entityB, new TimeEnabledComponent());
                ecb.AddComponent(entityB, new InterruptComponent());
                ecb.AddComponent(entityB, new PipelineBufferBComponent());
                ecb.AddComponent(entityB, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityB, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityB, false);

                if (property.Type == PropertyType.Transform2DRotation)
                {
                    ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent = new()
                    {
                        RefValue = refTransform2D
                    };
                    ecb.AddComponent(entityA, managedTransform2DComponent);
                    ecb.AddComponent(entityA, new Transform2DRotationComponent());

                    ecb.AddComponent(entityB, managedTransform2DComponent);
                    ecb.AddComponent(entityB, new Transform2DRotationComponent());
                }
                else
                {
                    ManagedAnimationProperty1DComponent managedProperty1DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, managedProperty1DComponent);
                    ecb.AddComponent(entityB, managedProperty1DComponent);
                }
            }
        }

        private void SeperateCustom2DProperty2D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation2DPropertyListComponent propertyListComponent,
                                                RefAnimationTransform2DProperty refTransform2D,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty2D property in animationListComponent.AnimationProperty2DList)
            {
                Entity entityA = ecb.CreateEntity();
                RefAnimationProperty2D refValue = new();
                Property2DComponent property2DComponent = new();
                if (property.IsStatic)
                {
                    property2DComponent.Value = property.StaticValue.Value;
                    ManagedAnimationProperty2DComponent managedProperty2DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, property2DComponent);
                    ecb.AddComponent(entityA, managedProperty2DComponent);
                    ecb.AddComponent(entityA, new InitializedPropertyComponent());
                    continue;
                }
                Entity entityB = ecb.CreateEntity();
                if (property.Type != PropertyType.Transform2DPosition && property.Type != PropertyType.Transform2DScale)
                {
                    propertyListComponent.Property2DList.Add(property.ID, refValue);
                }

                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[property.ID];

                ecb.AddBuffer<Animation2DComponent>(entityA);
                ecb.AddBuffer<Animation2DComponent>(entityB);
                foreach (Animation2D animation in animationList)
                {
                    FixedList128Bytes<float4> easeList = new();
                    for (int i = 0; i < easeList.Capacity; i++)
                    {
                        if (i >= animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[i].KeyTime, animation.EaseKeyframeList[i].Value, animation.EaseKeyframeList[i].InTan, animation.EaseKeyframeList[i].OutTan));
                    }
                    Animation2DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        Control0 = animation.Control0Value,
                        Control1 = animation.Control1Value,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entityA);
                ecb.AddBuffer<InterruptTimeComponent>(entityB);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }

                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                ecb.AddComponent(entityA, property2DComponent);
                ecb.AddComponent(entityA, propertyInfoComponent);
                ecb.AddComponent(entityA, timeComponent);
                ecb.AddComponent(entityA, new LerpEnabledComponent());
                ecb.AddComponent(entityA, new InitializedPropertyComponent());
                ecb.AddComponent(entityA, new TimeEnabledComponent());
                ecb.AddComponent(entityA, new InterruptComponent());
                ecb.AddComponent(entityA, new PipelineBufferAComponent());
                ecb.AddComponent(entityA, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityA, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityA, false);

                ecb.AddComponent(entityB, property2DComponent);
                ecb.AddComponent(entityB, propertyInfoComponent);
                ecb.AddComponent(entityB, timeComponent);
                ecb.AddComponent(entityB, new LerpEnabledComponent());
                ecb.AddComponent(entityB, new InitializedPropertyComponent());
                ecb.AddComponent(entityB, new TimeEnabledComponent());
                ecb.AddComponent(entityB, new InterruptComponent());
                ecb.AddComponent(entityB, new PipelineBufferBComponent());
                ecb.AddComponent(entityB, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityB, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityB, false);
                

                if (property.Type == PropertyType.Transform2DPosition)
                {
                    ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent = new()
                    {
                        RefValue = refTransform2D
                    };
                    ecb.AddComponent(entityA, managedTransform2DComponent);
                    ecb.AddComponent(entityA, new Transform2DPositionComponent());
                    ecb.AddComponent(entityB, managedTransform2DComponent);
                    ecb.AddComponent(entityB, new Transform2DPositionComponent());
                }
                else if (property.Type == PropertyType.Transform2DScale)
                {
                    ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent = new()
                    {
                        RefValue = refTransform2D
                    };
                    ecb.AddComponent(entityA, managedTransform2DComponent);
                    ecb.AddComponent(entityA, new Transform2DScaleComponent());
                    ecb.AddComponent(entityB, managedTransform2DComponent);
                    ecb.AddComponent(entityB, new Transform2DScaleComponent());
                }
                else
                {
                    ManagedAnimationProperty2DComponent managedProperty2DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, managedProperty2DComponent);
                    ecb.AddComponent(entityB, managedProperty2DComponent);
                }
            }
        }

        private void SeperateCustom3DProperty2D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation2DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty3D property in animationListComponent.AnimationProperty3DList)
            {
                Entity entityA = ecb.CreateEntity();
                RefAnimationProperty3D refValue = new();
                Property3DComponent property3DComponent = new();
                if (property.IsStatic)
                {
                    property3DComponent.Value = property.StaticValue.Value;
                    ManagedAnimationProperty3DComponent managedProperty3DComponent1 = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, property3DComponent);
                    ecb.AddComponent(entityA, managedProperty3DComponent1);
                    ecb.AddComponent(entityA, new InitializedPropertyComponent());
                    continue;
                }
                Entity entityB = ecb.CreateEntity();

                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[property.ID];

                ecb.AddBuffer<Animation3DComponent>(entityA);
                ecb.AddBuffer<Animation3DComponent>(entityB);
                foreach (Animation3D animation in animationList)
                {
                    FixedList128Bytes<float4> easeList = new();
                    for (int i = 0; i < easeList.Capacity; i++)
                    {
                        if (i >= animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[i].KeyTime, animation.EaseKeyframeList[i].Value, animation.EaseKeyframeList[i].InTan, animation.EaseKeyframeList[i].OutTan));
                    }
                    Animation3DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        Control0 = animation.Control0Value,
                        Control1 = animation.Control1Value,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entityA);
                ecb.AddBuffer<InterruptTimeComponent>(entityB);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }

                ManagedAnimationProperty3DComponent managedProperty3DComponent = new()
                {
                    RefValue = refValue
                };
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                ecb.AddComponent(entityA, property3DComponent);
                ecb.AddComponent(entityA, propertyInfoComponent);
                ecb.AddComponent(entityA, managedProperty3DComponent);
                ecb.AddComponent(entityA, timeComponent);
                ecb.AddComponent(entityA, new LerpEnabledComponent());
                ecb.AddComponent(entityA, new InitializedPropertyComponent());
                ecb.AddComponent(entityA, new TimeEnabledComponent());
                ecb.AddComponent(entityA, new InterruptComponent());
                ecb.AddComponent(entityA, new PipelineBufferAComponent());
                ecb.AddComponent(entityA, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityA, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityA, false);

                ecb.AddComponent(entityB, property3DComponent);
                ecb.AddComponent(entityB, propertyInfoComponent);
                ecb.AddComponent(entityB, managedProperty3DComponent);
                ecb.AddComponent(entityB, timeComponent);
                ecb.AddComponent(entityB, new LerpEnabledComponent());
                ecb.AddComponent(entityB, new InitializedPropertyComponent());
                ecb.AddComponent(entityB, new TimeEnabledComponent());
                ecb.AddComponent(entityB, new InterruptComponent());
                ecb.AddComponent(entityB, new PipelineBufferBComponent());
                ecb.AddComponent(entityB, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityB, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityB, false);
            }
        }

        private void SeperateCustom4DProperty2D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation2DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty4D property in animationListComponent.AnimationProperty4DList)
            {
                Entity entityA = ecb.CreateEntity();
                RefAnimationProperty4D refValue = new();
                Property4DComponent property4DComponent = new();
                if (property.IsStatic)
                {
                    property4DComponent.Value = property.StaticValue.Value;
                    ManagedAnimationProperty4DComponent managedProperty4DComponent1 = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, property4DComponent);
                    ecb.AddComponent(entityA, managedProperty4DComponent1);
                    ecb.AddComponent(entityA, new InitializedPropertyComponent());
                    continue;
                }
                Entity entityB = ecb.CreateEntity();

                List<Animation4D> animationList = animationListComponent.Animation4DDictionary[property.ID];

                ecb.AddBuffer<Animation4DComponent>(entityA);
                ecb.AddBuffer<Animation4DComponent>(entityB);
                ecb.AddBuffer<Animation4DBakeDataComponent>(entityA);
                ecb.AddBuffer<Animation4DBakeDataComponent>(entityB);
                int dataIndex = 0;
                foreach (Animation4D animation in animationList)
                {
                    FixedList128Bytes<float4> easeList = new();
                    for (int i = 0; i < easeList.Capacity; i++)
                    {
                        if (i >= animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[i].KeyTime, animation.EaseKeyframeList[i].Value, animation.EaseKeyframeList[i].InTan, animation.EaseKeyframeList[i].OutTan));
                    }
                    Animation4DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        Control0 = animation.Control0Value,
                        Control1 = animation.Control1Value,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime,
                        LerpType = animation.LerpType,
                        DataIndex = dataIndex
                    };
                    if (animation.LerpType == UtilityHelper.Quaternion_PathLerp)
                    {
                        float4 q12 = QuaternionHelper.Mul(animation.Control0Value.Inverse(), animation.Control1Value);
                        float4 q23 = QuaternionHelper.Mul(animation.Control1Value.Inverse(), animation.EndValue);
                        float4 a = animation.StartValue; //q0
                        float4 b = QuaternionHelper.Mul(animation.StartValue.Inverse(), animation.Control0Value); //q01
                        float4 c = QuaternionHelper.Mul(b.Inverse(), q12); //q01^-1*q12
                        float4 d = QuaternionHelper.Mul(c.Inverse(), q23); //q01^-1*q12
                        Animation4DBakeDataComponent bakeDataComponent = new()
                        {
                            q0 = a,
                            q01 = b,
                            q01_1q12 = c,
                            q12_1q23 = d
                        };
                        ecb.AppendToBuffer(entityA, bakeDataComponent);
                        ecb.AppendToBuffer(entityB, bakeDataComponent);
                        dataIndex++;
                    }
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entityA);
                ecb.AddBuffer<InterruptTimeComponent>(entityB);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }

                ManagedAnimationProperty4DComponent managedProperty4DComponent = new()
                {
                    RefValue = refValue
                };
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                ecb.AddComponent(entityA, property4DComponent);
                ecb.AddComponent(entityA, propertyInfoComponent);
                ecb.AddComponent(entityA, managedProperty4DComponent);
                ecb.AddComponent(entityA, timeComponent);
                ecb.AddComponent(entityA, new LerpEnabledComponent());
                ecb.AddComponent(entityA, new InitializedPropertyComponent());
                ecb.AddComponent(entityA, new TimeEnabledComponent());
                ecb.AddComponent(entityA, new InterruptComponent());
                ecb.AddComponent(entityA, new PipelineBufferAComponent());
                ecb.AddComponent(entityA, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityA, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityA, false);

                ecb.AddComponent(entityB, property4DComponent);
                ecb.AddComponent(entityB, propertyInfoComponent);
                ecb.AddComponent(entityB, managedProperty4DComponent);
                ecb.AddComponent(entityB, timeComponent);
                ecb.AddComponent(entityB, new LerpEnabledComponent());
                ecb.AddComponent(entityB, new InitializedPropertyComponent());
                ecb.AddComponent(entityB, new TimeEnabledComponent());
                ecb.AddComponent(entityB, new InterruptComponent());
                ecb.AddComponent(entityB, new PipelineBufferBComponent());
                ecb.AddComponent(entityB, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityB, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityB, false);
            }
        }

        #endregion


        #region 3D

        private void SeperateAnimationObject3D(ManagedAnimationListComponent animationListComponent,
                                               ManagedAnimation3DPropertyListComponent property3DListComponent,
                                               EntityCommandBuffer ecb,
                                               Entity entity)
        {
            RefAnimationTransform3DProperty transform3DProperty = new()
            {
                Value = new()
            };
            SeperateCustom1DProperty3D(animationListComponent, property3DListComponent, ecb);
            SeperateCustom2DProperty3D(animationListComponent, property3DListComponent, ecb);
            SeperateCustom3DProperty3D(animationListComponent, property3DListComponent, transform3DProperty, ecb);
            SeperateCustom4DProperty3D(animationListComponent, property3DListComponent, transform3DProperty, ecb);
            property3DListComponent.Transform3DProperty = transform3DProperty;
            ecb.AddComponent(entity, new BakeReadyComponent());
        }

        private void SeperateCustom1DProperty3D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation3DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty1D property in animationListComponent.AnimationProperty1DList)
            {
                Entity entityA = ecb.CreateEntity();
                RefAnimationProperty1D refValue = new();
                Property1DComponent property1DComponent = new();
                if (property.IsStatic)
                {
                    property1DComponent.Value = property.StaticValue.Value;
                    ManagedAnimationProperty1DComponent managedProperty1DComponent1 = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, property1DComponent);
                    ecb.AddComponent(entityA, managedProperty1DComponent1);
                    ecb.AddComponent(entityA, new InitializedPropertyComponent());
                    continue;
                }
                Entity entityB = ecb.CreateEntity();

                List<Animation1D> animationList = animationListComponent.Animation1DDictionary[property.ID];

                ecb.AddBuffer<Animation1DComponent>(entityA);
                ecb.AddBuffer<Animation1DComponent>(entityB);
                foreach (Animation1D animation in animationList)
                {
                    FixedList128Bytes<float4> easeList = new();
                    for (int i = 0; i < easeList.Capacity; i++)
                    {
                        if (i >= animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[i].KeyTime, animation.EaseKeyframeList[i].Value, animation.EaseKeyframeList[i].InTan, animation.EaseKeyframeList[i].OutTan));
                    }
                    Animation1DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entityA);
                ecb.AddBuffer<InterruptTimeComponent>(entityB);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                ManagedAnimationProperty1DComponent managedProperty1DComponent = new()
                {
                    RefValue = refValue
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                ecb.AddComponent(entityA, property1DComponent);
                ecb.AddComponent(entityA, propertyInfoComponent);
                ecb.AddComponent(entityA, managedProperty1DComponent);
                ecb.AddComponent(entityA, timeComponent);
                ecb.AddComponent(entityA, new LerpEnabledComponent());
                ecb.AddComponent(entityA, new InitializedPropertyComponent());
                ecb.AddComponent(entityA, new TimeEnabledComponent());
                ecb.AddComponent(entityA, new InterruptComponent());
                ecb.AddComponent(entityA, new PipelineBufferAComponent());
                ecb.AddComponent(entityA, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityA, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityA, false);

                ecb.AddComponent(entityB, property1DComponent);
                ecb.AddComponent(entityB, propertyInfoComponent);
                ecb.AddComponent(entityB, managedProperty1DComponent);
                ecb.AddComponent(entityB, timeComponent);
                ecb.AddComponent(entityB, new LerpEnabledComponent());
                ecb.AddComponent(entityB, new InitializedPropertyComponent());
                ecb.AddComponent(entityB, new TimeEnabledComponent());
                ecb.AddComponent(entityB, new InterruptComponent());
                ecb.AddComponent(entityB, new PipelineBufferBComponent());
                ecb.AddComponent(entityB, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityB, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityB, false);
            }
        }

        private void SeperateCustom2DProperty3D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation3DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty2D property in animationListComponent.AnimationProperty2DList)
            {
                Entity entityA = ecb.CreateEntity();
                RefAnimationProperty2D refValue = new();
                Property2DComponent property2DComponent = new();
                if (property.IsStatic)
                {
                    property2DComponent.Value = property.StaticValue.Value;
                    ManagedAnimationProperty2DComponent managedProperty2DComponent1 = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, property2DComponent);
                    ecb.AddComponent(entityA, managedProperty2DComponent1);
                    ecb.AddComponent(entityA, new InitializedPropertyComponent());
                    continue;
                }
                Entity entityB = ecb.CreateEntity();

                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[property.ID];

                ecb.AddBuffer<Animation2DComponent>(entityA);
                ecb.AddBuffer<Animation2DComponent>(entityB);
                foreach (Animation2D animation in animationList)
                {
                    FixedList128Bytes<float4> easeList = new();
                    for (int i = 0; i < easeList.Capacity; i++)
                    {
                        if (i >= animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[i].KeyTime, animation.EaseKeyframeList[i].Value, animation.EaseKeyframeList[i].InTan, animation.EaseKeyframeList[i].OutTan));
                    }
                    Animation2DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        Control0 = animation.Control0Value,
                        Control1 = animation.Control1Value,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entityA);
                ecb.AddBuffer<InterruptTimeComponent>(entityB);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }

                ManagedAnimationProperty2DComponent managedProperty2DComponent = new()
                {
                    RefValue = refValue
                };
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                ecb.AddComponent(entityA, property2DComponent);
                ecb.AddComponent(entityA, propertyInfoComponent);
                ecb.AddComponent(entityA, managedProperty2DComponent);
                ecb.AddComponent(entityA, timeComponent);
                ecb.AddComponent(entityA, new LerpEnabledComponent());
                ecb.AddComponent(entityA, new InitializedPropertyComponent());
                ecb.AddComponent(entityA, new TimeEnabledComponent());
                ecb.AddComponent(entityA, new InterruptComponent());
                ecb.AddComponent(entityA, new PipelineBufferAComponent());
                ecb.AddComponent(entityA, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityA, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityA, false);

                ecb.AddComponent(entityB, property2DComponent);
                ecb.AddComponent(entityB, propertyInfoComponent);
                ecb.AddComponent(entityB, managedProperty2DComponent);
                ecb.AddComponent(entityB, timeComponent);
                ecb.AddComponent(entityB, new LerpEnabledComponent());
                ecb.AddComponent(entityB, new InitializedPropertyComponent());
                ecb.AddComponent(entityB, new TimeEnabledComponent());
                ecb.AddComponent(entityB, new InterruptComponent());
                ecb.AddComponent(entityB, new PipelineBufferBComponent());
                ecb.AddComponent(entityB, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityB, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityB, false);
            }
        }

        private void SeperateCustom3DProperty3D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation3DPropertyListComponent propertyListComponent,
                                                RefAnimationTransform3DProperty refTransform3D,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty3D property in animationListComponent.AnimationProperty3DList)
            {
                Entity entityA = ecb.CreateEntity();
                RefAnimationProperty3D refValue = new();
                Property3DComponent property3DComponent = new();
                if (property.IsStatic)
                {
                    property3DComponent.Value = property.StaticValue.Value;
                    ManagedAnimationProperty3DComponent managedProperty3DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, property3DComponent);
                    ecb.AddComponent(entityA, managedProperty3DComponent);
                    ecb.AddComponent(entityA, new InitializedPropertyComponent());
                    continue;
                }
                Entity entityB = ecb.CreateEntity();
                if (property.Type != PropertyType.Transform3DPosition && property.Type != PropertyType.Transform3DScale)
                {
                    propertyListComponent.Property3DList.Add(property.ID, refValue);
                }

                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[property.ID];

                ecb.AddBuffer<Animation3DComponent>(entityA);
                ecb.AddBuffer<Animation3DComponent>(entityB);
                foreach (Animation3D animation in animationList)
                {
                    FixedList128Bytes<float4> easeList = new();
                    for (int i = 0; i < easeList.Capacity; i++)
                    {
                        if (i >= animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[i].KeyTime, animation.EaseKeyframeList[i].Value, animation.EaseKeyframeList[i].InTan, animation.EaseKeyframeList[i].OutTan));
                    }
                    Animation3DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        Control0 = animation.Control0Value,
                        Control1 = animation.Control1Value,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entityA);
                ecb.AddBuffer<InterruptTimeComponent>(entityB);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }

                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                ecb.AddComponent(entityA, property3DComponent);
                ecb.AddComponent(entityA, propertyInfoComponent);
                ecb.AddComponent(entityA, timeComponent);
                ecb.AddComponent(entityA, new LerpEnabledComponent());
                ecb.AddComponent(entityA, new InitializedPropertyComponent());
                ecb.AddComponent(entityA, new TimeEnabledComponent());
                ecb.AddComponent(entityA, new InterruptComponent());
                ecb.AddComponent(entityA, new PipelineBufferAComponent());
                ecb.AddComponent(entityA, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityA, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityA, false);

                ecb.AddComponent(entityB, property3DComponent);
                ecb.AddComponent(entityB, propertyInfoComponent);
                ecb.AddComponent(entityB, timeComponent);
                ecb.AddComponent(entityB, new LerpEnabledComponent());
                ecb.AddComponent(entityB, new InitializedPropertyComponent());
                ecb.AddComponent(entityB, new TimeEnabledComponent());
                ecb.AddComponent(entityB, new InterruptComponent());
                ecb.AddComponent(entityB, new PipelineBufferBComponent());
                ecb.AddComponent(entityB, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityB, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityB, false);

                if (property.Type == PropertyType.Transform3DPosition)
                {
                    ManagedAnimationTransform3DPropertyComponent managedTransform3DComponent = new()
                    {
                        RefValue = refTransform3D
                    };
                    ecb.AddComponent(entityA, managedTransform3DComponent);
                    ecb.AddComponent(entityA, new Transform3DPositionComponent());
                    ecb.AddComponent(entityB, managedTransform3DComponent);
                    ecb.AddComponent(entityB, new Transform3DPositionComponent());
                }
                else if (property.Type == PropertyType.Transform3DScale)
                {
                    ManagedAnimationTransform3DPropertyComponent managedTransform3DComponent = new()
                    {
                        RefValue = refTransform3D
                    };
                    ecb.AddComponent(entityA, managedTransform3DComponent);
                    ecb.AddComponent(entityA, new Transform3DScaleComponent());
                    ecb.AddComponent(entityB, managedTransform3DComponent);
                    ecb.AddComponent(entityB, new Transform3DScaleComponent());
                }
                else
                {
                    ManagedAnimationProperty3DComponent managedProperty3DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, managedProperty3DComponent);
                    ecb.AddComponent(entityB, managedProperty3DComponent);
                }
            }
        }

        private void SeperateCustom4DProperty3D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation3DPropertyListComponent propertyListComponent,
                                                RefAnimationTransform3DProperty refTransform3D,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty4D property in animationListComponent.AnimationProperty4DList)
            {
                Entity entityA = ecb.CreateEntity();
                RefAnimationProperty4D refValue = new();
                Property4DComponent property4DComponent = new();
                if (property.IsStatic)
                {
                    property4DComponent.Value = property.StaticValue.Value;
                    ManagedAnimationProperty4DComponent managedProperty4DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, property4DComponent);
                    ecb.AddComponent(entityA, managedProperty4DComponent);
                    ecb.AddComponent(entityA, new InitializedPropertyComponent());
                    continue;
                }
                Entity entityB = ecb.CreateEntity();
                if (property.Type != PropertyType.Transform3DPosition && property.Type != PropertyType.Transform3DScale)
                {
                    propertyListComponent.Property4DList.Add(property.ID, refValue);
                }

                List<Animation4D> animationList = animationListComponent.Animation4DDictionary[property.ID];

                ecb.AddBuffer<Animation4DComponent>(entityA);
                ecb.AddBuffer<Animation4DBakeDataComponent>(entityA);
                ecb.AddBuffer<Animation4DComponent>(entityB);
                ecb.AddBuffer<Animation4DBakeDataComponent>(entityB);
                int dataIndex = 0;
                foreach (Animation4D animation in animationList)
                {
                    FixedList128Bytes<float4> easeList = new();
                    for (int i = 0; i < easeList.Capacity; i++)
                    {
                        if (i >= animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[i].KeyTime, animation.EaseKeyframeList[i].Value, animation.EaseKeyframeList[i].InTan, animation.EaseKeyframeList[i].OutTan));
                    }
                    Animation4DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        Control0 = animation.Control0Value,
                        Control1 = animation.Control1Value,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime,
                        LerpType = animation.LerpType,
                        DataIndex = dataIndex
                    };
                    if (animation.LerpType == UtilityHelper.Quaternion_PathLerp)
                    {
                        float4 q12 = QuaternionHelper.Mul(animation.Control0Value.Inverse(), animation.Control1Value);
                        float4 q23 = QuaternionHelper.Mul(animation.Control1Value.Inverse(), animation.EndValue);
                        float4 a = animation.StartValue; //q0
                        float4 b = QuaternionHelper.Mul(animation.StartValue.Inverse(), animation.Control0Value); //q01
                        float4 c = QuaternionHelper.Mul(b.Inverse(), q12); //q01^-1*q12
                        float4 d = QuaternionHelper.Mul(c.Inverse(), q23); //q01^-1*q12
                        Animation4DBakeDataComponent bakeDataComponent = new()
                        {
                            q0 = a,
                            q01 = b,
                            q01_1q12 = c,
                            q12_1q23 = d
                        };
                        ecb.AppendToBuffer(entityA, bakeDataComponent);
                        ecb.AppendToBuffer(entityB, bakeDataComponent);
                        dataIndex++;
                    }
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entityA);
                ecb.AddBuffer<InterruptTimeComponent>(entityB);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entityA, component);
                    ecb.AppendToBuffer(entityB, component);
                }

                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                ecb.AddComponent(entityA, property4DComponent);
                ecb.AddComponent(entityA, propertyInfoComponent);
                ecb.AddComponent(entityA, timeComponent);
                ecb.AddComponent(entityA, new LerpEnabledComponent());
                ecb.AddComponent(entityA, new InitializedPropertyComponent());
                ecb.AddComponent(entityA, new TimeEnabledComponent());
                ecb.AddComponent(entityA, new InterruptComponent());
                ecb.AddComponent(entityA, new PipelineBufferAComponent());
                ecb.AddComponent(entityA, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityA, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityA, false);

                ecb.AddComponent(entityB, property4DComponent);
                ecb.AddComponent(entityB, propertyInfoComponent);
                ecb.AddComponent(entityB, timeComponent);
                ecb.AddComponent(entityB, new LerpEnabledComponent());
                ecb.AddComponent(entityB, new InitializedPropertyComponent());
                ecb.AddComponent(entityB, new TimeEnabledComponent());
                ecb.AddComponent(entityB, new InterruptComponent());
                ecb.AddComponent(entityB, new PipelineBufferBComponent());
                ecb.AddComponent(entityB, new PipelineBufferReadyComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entityB, false);
                ecb.SetComponentEnabled<PipelineBufferReadyComponent>(entityB, false);
                
                if (property.Type == PropertyType.Transform3DRotation)
                {
                    ManagedAnimationTransform3DPropertyComponent managedTransform3DComponent = new()
                    {
                        RefValue = refTransform3D
                    };
                    ecb.AddComponent(entityA, managedTransform3DComponent);
                    ecb.AddComponent(entityA, new Transform3DRotationComponent());
                    ecb.AddComponent(entityB, managedTransform3DComponent);
                    ecb.AddComponent(entityB, new Transform3DRotationComponent());
                }
                else
                {
                    ManagedAnimationProperty4DComponent managedProperty3DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entityA, managedProperty3DComponent);
                    ecb.AddComponent(entityB, managedProperty3DComponent);
                }
            }
        }

        #endregion
    }
}
