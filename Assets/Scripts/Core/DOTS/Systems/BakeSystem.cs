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
                Entity entity = ecb.CreateEntity();
                RefAnimationProperty1D refValue = new();
                Property1DComponent property1DComponent = new();
                propertyListComponent.Property1DList.Add(property.ID, refValue);
                if (property.IsStatic)
                {
                    property1DComponent.Value = property.StaticValue.Value;
                    ecb.AddComponent(entity, property1DComponent);
                    refValue.Value = property1DComponent.Value;
                    continue;
                }

                List<Animation1D> animationList = animationListComponent.Animation1DDictionary[property.ID];

                ecb.AddBuffer<Animation1DComponent>(entity);
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
                    ecb.AppendToBuffer(entity, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entity, component);
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

                ecb.AddComponent(entity, property1DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);

                if (property.Type == PropertyType.Transform2DRotation)
                {
                    ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent = new()
                    {
                        RefValue = refTransform2D
                    };
                    ecb.AddComponent(entity, managedTransform2DComponent);
                    ecb.AddComponent(entity, new Transform2DRotationComponent());
                }
                else
                {
                    ManagedAnimationProperty1DComponent managedProperty1DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entity, managedProperty1DComponent);
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
                Entity entity = ecb.CreateEntity();
                RefAnimationProperty2D refValue = new();
                Property2DComponent property2DComponent = new();
                propertyListComponent.Property2DList.Add(property.ID, refValue);
                if (property.IsStatic)
                {
                    property2DComponent.Value = property.StaticValue.Value;
                    ecb.AddComponent(entity, property2DComponent);
                    refValue.Value = property2DComponent.Value;
                    continue;
                }

                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[property.ID];

                ecb.AddBuffer<Animation2DComponent>(entity);
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
                    ecb.AppendToBuffer(entity, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entity, component);
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

                ecb.AddComponent(entity, property2DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);
                

                if (property.Type == PropertyType.Transform2DPosition)
                {
                    ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent = new()
                    {
                        RefValue = refTransform2D
                    };
                    ecb.AddComponent(entity, managedTransform2DComponent);
                    ecb.AddComponent(entity, new Transform2DPositionComponent());
                }
                else if (property.Type == PropertyType.Transform2DScale)
                {
                    ManagedAnimationTransform2DPropertyComponent managedTransform2DComponent = new()
                    {
                        RefValue = refTransform2D
                    };
                    ecb.AddComponent(entity, managedTransform2DComponent);
                    ecb.AddComponent(entity, new Transform2DScaleComponent());
                }
                else
                {
                    ManagedAnimationProperty2DComponent managedProperty2DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entity, managedProperty2DComponent);
                }
            }
        }

        private void SeperateCustom3DProperty2D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation2DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty3D property in animationListComponent.AnimationProperty3DList)
            {
                Entity entity = ecb.CreateEntity();
                RefAnimationProperty3D refValue = new();
                Property3DComponent property3DComponent = new();
                propertyListComponent.Property3DList.Add(property.ID, refValue);
                if (property.IsStatic)
                {
                    property3DComponent.Value = property.StaticValue.Value;
                    ecb.AddComponent(entity, property3DComponent);
                    refValue.Value = property3DComponent.Value;
                    continue;
                }

                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[property.ID];

                ecb.AddBuffer<Animation3DComponent>(entity);
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
                    ecb.AppendToBuffer(entity, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entity, component);
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

                ecb.AddComponent(entity, property3DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, managedProperty3DComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);
            }
        }

        private void SeperateCustom4DProperty2D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation2DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty4D property in animationListComponent.AnimationProperty4DList)
            {
                Entity entity = ecb.CreateEntity();
                RefAnimationProperty4D refValue = new();
                Property4DComponent property4DComponent = new();
                propertyListComponent.Property4DList.Add(property.ID, refValue);
                if (property.IsStatic)
                {
                    property4DComponent.Value = property.StaticValue.Value;
                    ecb.AddComponent(entity, property4DComponent);
                    refValue.Value = property4DComponent.Value;
                    continue;
                }

                List<Animation4D> animationList = animationListComponent.Animation4DDictionary[property.ID];

                ecb.AddBuffer<Animation4DComponent>(entity);
                ecb.AddBuffer<Animation4DBakeDataComponent>(entity);
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
                        ecb.AppendToBuffer(entity, bakeDataComponent);
                        dataIndex++;
                    }
                    ecb.AppendToBuffer(entity, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entity, component);
                }

                ManagedAnimationProperty4DComponent managedProperty3DComponent = new()
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

                ecb.AddComponent(entity, property4DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, managedProperty3DComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);
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
                Entity entity = ecb.CreateEntity();
                RefAnimationProperty1D refValue = new();
                Property1DComponent property1DComponent = new();
                propertyListComponent.Property1DList.Add(property.ID, refValue);
                if (property.IsStatic)
                {
                    property1DComponent.Value = property.StaticValue.Value;
                    ecb.AddComponent(entity, property1DComponent);
                    refValue.Value = property1DComponent.Value;
                    continue;
                }

                List<Animation1D> animationList = animationListComponent.Animation1DDictionary[property.ID];

                ecb.AddBuffer<Animation1DComponent>(entity);
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
                    ecb.AppendToBuffer(entity, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entity, component);
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

                ecb.AddComponent(entity, property1DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, managedProperty1DComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);
            }
        }

        private void SeperateCustom2DProperty3D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation3DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty2D property in animationListComponent.AnimationProperty2DList)
            {
                Entity entity = ecb.CreateEntity();
                RefAnimationProperty2D refValue = new();
                Property2DComponent property2DComponent = new();
                propertyListComponent.Property2DList.Add(property.ID, refValue);
                if (property.IsStatic)
                {
                    property2DComponent.Value = property.StaticValue.Value;
                    ecb.AddComponent(entity, property2DComponent);
                    refValue.Value = property2DComponent.Value;
                    continue;
                }

                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[property.ID];

                ecb.AddBuffer<Animation2DComponent>(entity);
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
                    ecb.AppendToBuffer(entity, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entity, component);
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

                ecb.AddComponent(entity, property2DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, managedProperty2DComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);
            }
        }

        private void SeperateCustom3DProperty3D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation3DPropertyListComponent propertyListComponent,
                                                RefAnimationTransform3DProperty refTransform3D,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty3D property in animationListComponent.AnimationProperty3DList)
            {
                Entity entity = ecb.CreateEntity();
                RefAnimationProperty3D refValue = new();
                Property3DComponent property3DComponent = new();
                propertyListComponent.Property3DList.Add(property.ID, refValue);
                if (property.IsStatic)
                {
                    property3DComponent.Value = property.StaticValue.Value;
                    ecb.AddComponent(entity, property3DComponent);
                    refValue.Value = property3DComponent.Value;
                    continue;
                }

                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[property.ID];

                ecb.AddBuffer<Animation3DComponent>(entity);
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
                    ecb.AppendToBuffer(entity, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entity, component);
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

                ecb.AddComponent(entity, property3DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);

                if (property.Type == PropertyType.Transform3DPosition)
                {
                    ManagedAnimationTransform3DPropertyComponent managedTransform3DComponent = new()
                    {
                        RefValue = refTransform3D
                    };
                    ecb.AddComponent(entity, managedTransform3DComponent);
                    ecb.AddComponent(entity, new Transform3DPositionComponent());
                }
                else if (property.Type == PropertyType.Transform3DScale)
                {
                    ManagedAnimationTransform3DPropertyComponent managedTransform3DComponent = new()
                    {
                        RefValue = refTransform3D
                    };
                    ecb.AddComponent(entity, managedTransform3DComponent);
                    ecb.AddComponent(entity, new Transform3DScaleComponent());
                }
                else
                {
                    ManagedAnimationProperty3DComponent managedProperty3DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entity, managedProperty3DComponent);
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
                Entity entity = ecb.CreateEntity();
                RefAnimationProperty4D refValue = new();
                Property4DComponent property4DComponent = new();
                propertyListComponent.Property4DList.Add(property.ID, refValue);
                if (property.IsStatic)
                {
                    property4DComponent.Value = property.StaticValue.Value;
                    ecb.AddComponent(entity, property4DComponent);
                    refValue.Value = property4DComponent.Value;
                    continue;
                }

                List<Animation4D> animationList = animationListComponent.Animation4DDictionary[property.ID];

                ecb.AddBuffer<Animation4DComponent>(entity);
                ecb.AddBuffer<Animation4DBakeDataComponent>(entity);
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
                        ecb.AppendToBuffer(entity, bakeDataComponent);
                        dataIndex++;
                    }
                    ecb.AppendToBuffer(entity, component);
                }
                
                ecb.AddBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    ecb.AppendToBuffer(entity, component);
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

                ecb.AddComponent(entity, property4DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);
                
                if (property.Type == PropertyType.Transform3DRotation)
                {
                    ManagedAnimationTransform3DPropertyComponent managedTransform3DComponent = new()
                    {
                        RefValue = refTransform3D
                    };
                    ecb.AddComponent(entity, managedTransform3DComponent);
                    ecb.AddComponent(entity, new Transform3DRotationComponent());
                }
                else
                {
                    ManagedAnimationProperty4DComponent managedProperty3DComponent = new()
                    {
                        RefValue = refValue
                    };
                    ecb.AddComponent(entity, managedProperty3DComponent);
                }
            }
        }

        #endregion
    }
}
