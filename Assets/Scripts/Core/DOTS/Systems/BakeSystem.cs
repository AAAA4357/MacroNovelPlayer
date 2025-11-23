using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.LerpRuntime.Transform2D;
using MNP.Core.DOTS.Components.LerpRuntime.Transform3D;
using MNP.Core.DOTS.Components.Managed;
using MNP.Core.DOTS.Components.Transform2D;
using MNP.Core.DOTS.Components.Transform3D;
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
            SeperateTransform2DProperty(animationListComponent, property2DListComponent, ecb);
            SeperateCustom1DProperty2D(animationListComponent, property2DListComponent, ecb);
            SeperateCustom2DProperty2D(animationListComponent, property2DListComponent, ecb);
            SeperateCustom3DProperty2D(animationListComponent, property2DListComponent, ecb);
            SeperateCustom4DProperty2D(animationListComponent, property2DListComponent, ecb);
            ecb.AddComponent(entity, new BakeReadyComponent());
        }
        
        private void SeperateTransform2DProperty(ManagedAnimationListComponent animationListComponent,
                                               ManagedAnimation2DPropertyListComponent propertyListComponent,
                                               EntityCommandBuffer ecb)
        {
            Entity entity = ecb.CreateEntity();
            PosTransform2DPropertyComponent posPropertyComponent = new();
            ecb.AddComponent(entity, posPropertyComponent);
            RotTransform2DPropertyComponent rotPropertyComponent = new();
            ecb.AddComponent(entity, rotPropertyComponent);
            SclTransform2DPropertyComponent sclPropertyComponent = new();
            ecb.AddComponent(entity, sclPropertyComponent);

            PosTransform2DTimeComponent posTimeComponent = new()
            {
                Time = 0,
                InterrputedTime = 0
            };
            RotTransform2DTimeComponent rotTimeComponent = new()
            {
                Time = 0,
                InterrputedTime = 0
            };
            SclTransform2DTimeComponent sclTimeComponent = new()
            {
                Time = 0,
                InterrputedTime = 0
            };
            ecb.AddComponent(entity, posTimeComponent);
            ecb.AddComponent(entity, rotTimeComponent);
            ecb.AddComponent(entity, sclTimeComponent);
            
            AnimationProperty2D posProperty = animationListComponent.AnimationProperty2DList.Find(x => x.Type == PropertyType.Transform2DPosition);
            AnimationProperty1D rotProperty = animationListComponent.AnimationProperty1DList.Find(x => x.Type == PropertyType.Transform2DRotation);
            AnimationProperty2D sclProperty = animationListComponent.AnimationProperty2DList.Find(x => x.Type == PropertyType.Transform2DScale);
            
            RefAnimationTransform2DProperty refValue = new()
            {
                Value = new()
            };
            propertyListComponent.Transform2DProperty = refValue;

            if (posProperty.IsStatic)
            {
                posPropertyComponent.Value = posProperty.StaticValue.Value;
                refValue.Value.Position = posPropertyComponent.Value;
            }
            else
            {
                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[UtilityHelper.Transorm2DPositionID];

                ecb.AddBuffer<Transform2DPositionAnimationComponent>(entity);
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
                    Transform2DPositionAnimationComponent component = new()
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
            }

            if (rotProperty.IsStatic)
            {
                rotPropertyComponent.Value = rotProperty.StaticValue.Value;
                refValue.Value.Rotation = rotPropertyComponent.Value;
            }
            else
            {
                List<Animation1D> animationList = animationListComponent.Animation1DDictionary[UtilityHelper.Transorm2DRotationID];

                ecb.AddBuffer<Transform2DRotationAnimationComponent>(entity);
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
                    Transform2DRotationAnimationComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime
                    };
                    ecb.AppendToBuffer(entity, component);
                }
            }
            
            if (sclProperty.IsStatic)
            {
                sclPropertyComponent.Value = sclProperty.StaticValue.Value;
                refValue.Value.Scale = sclPropertyComponent.Value;
            }
            else
            {
                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[UtilityHelper.Transorm2DScaleID];

                ecb.AddBuffer<Transform2DScaleAnimationComponent>(entity);
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
                    Transform2DScaleAnimationComponent component = new()
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
            }
                
            ecb.AddBuffer<PosTransform2DInterruptTimeComponent>(entity);
            for (int i = 0; i < posProperty.AnimationInterruptTimeList.Count; i++)
            {
                PosTransform2DInterruptTimeComponent component = new()
                {
                    InterruptTime = posProperty.AnimationInterruptTimeList[i]
                };
                ecb.AppendToBuffer(entity, component);
            }
                
            ecb.AddBuffer<RotTransform2DInterruptTimeComponent>(entity);
            for (int i = 0; i < posProperty.AnimationInterruptTimeList.Count; i++)
            {
                RotTransform2DInterruptTimeComponent component = new()
                {
                    InterruptTime = posProperty.AnimationInterruptTimeList[i]
                };
                ecb.AppendToBuffer(entity, component);
            }
                
            ecb.AddBuffer<SclTransform2DInterruptTimeComponent>(entity);
            for (int i = 0; i < posProperty.AnimationInterruptTimeList.Count; i++)
            {
                SclTransform2DInterruptTimeComponent component = new()
                {
                    InterruptTime = posProperty.AnimationInterruptTimeList[i]
                };
                ecb.AppendToBuffer(entity, component);
            }

            ManagedAnimationTransform2DPropertyComponent managedPropertyTransform2DComponent = new()
            {
                RefValue = refValue
            };
            Transform2DPropertyInfoComponent propertyInfoComponent = new()
            {
                PositionStartTime = posProperty.StartTime,
                PositionEndTime = posProperty.EndTime,
                RotationStartTime = rotProperty.StartTime,
                RotationEndTime = rotProperty.EndTime,
                ScaleStartTime = sclProperty.StartTime,
                ScaleEndTime = sclProperty.EndTime
            };

            ecb.AddComponent(entity, propertyInfoComponent);
            ecb.AddComponent(entity, managedPropertyTransform2DComponent);
            ecb.AddComponent(entity, new InitializedPropertyComponent());
            ecb.AddComponent(entity, new TimeEnabledComponent());
            ecb.AddComponent(entity, new PosTransform2DInterruptComponent());
            ecb.AddComponent(entity, new RotTransform2DInterruptComponent());
            ecb.AddComponent(entity, new SclTransform2DInterruptComponent());
            ecb.SetComponentEnabled<PosTransform2DInterruptComponent>(entity, false);
            ecb.SetComponentEnabled<RotTransform2DInterruptComponent>(entity, false);
            ecb.SetComponentEnabled<SclTransform2DInterruptComponent>(entity, false);
        }

        private void SeperateCustom1DProperty2D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation2DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty1D property in animationListComponent.AnimationProperty1DList)
            {
                if (property.Type == PropertyType.Transform2DRotation)
                {
                    continue;
                }
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
                        if (i == animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(new(1, 1, 0, float.NaN));
                            continue;
                        }
                        else if (i > animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[0].KeyTime, animation.EaseKeyframeList[0].Value, animation.EaseKeyframeList[0].InTan, animation.EaseKeyframeList[0].OutTan));
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
            }
        }

        private void SeperateCustom2DProperty2D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation2DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty2D property in animationListComponent.AnimationProperty2DList)
            {
                if (property.Type == PropertyType.Transform2DPosition || property.Type == PropertyType.Transform2DScale)
                {
                    continue;
                }
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
                        if (i == animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(new(1, 1, 0, float.NaN));
                            continue;
                        }
                        else if (i > animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[0].KeyTime, animation.EaseKeyframeList[0].Value, animation.EaseKeyframeList[0].InTan, animation.EaseKeyframeList[0].OutTan));
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
                        if (i == animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(new(1, 1, 0, float.NaN));
                            continue;
                        }
                        else if (i > animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[0].KeyTime, animation.EaseKeyframeList[0].Value, animation.EaseKeyframeList[0].InTan, animation.EaseKeyframeList[0].OutTan));
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
                        if (i == animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(new(1, 1, 0, float.NaN));
                            continue;
                        }
                        else if (i > animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[0].KeyTime, animation.EaseKeyframeList[0].Value, animation.EaseKeyframeList[0].InTan, animation.EaseKeyframeList[0].OutTan));
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
            }
        }

        #endregion


        #region 3D

        private void SeperateAnimationObject3D(ManagedAnimationListComponent animationListComponent,
                                               ManagedAnimation3DPropertyListComponent property3DListComponent,
                                               EntityCommandBuffer ecb,
                                               Entity entity)
        {
            SeperateTransform3DProperty(animationListComponent, property3DListComponent, ecb);
            SeperateCustom1DProperty3D(animationListComponent, property3DListComponent, ecb);
            SeperateCustom2DProperty3D(animationListComponent, property3DListComponent, ecb);
            SeperateCustom3DProperty3D(animationListComponent, property3DListComponent, ecb);
            SeperateCustom4DProperty3D(animationListComponent, property3DListComponent, ecb);
            ecb.AddComponent(entity, new BakeReadyComponent());
        }


        private void SeperateTransform3DProperty(ManagedAnimationListComponent animationListComponent,
                                                 ManagedAnimation3DPropertyListComponent propertyListComponent,
                                                 EntityCommandBuffer ecb)
        {
            Entity entity = ecb.CreateEntity();
            PosTransform3DPropertyComponent posPropertyComponent = new();
            ecb.AddComponent(entity, posPropertyComponent);
            RotTransform3DPropertyComponent rotPropertyComponent = new();
            ecb.AddComponent(entity, rotPropertyComponent);
            SclTransform3DPropertyComponent sclPropertyComponent = new();
            ecb.AddComponent(entity, sclPropertyComponent);

            PosTransform3DTimeComponent posTimeComponent = new()
            {
                Time = 0,
                InterrputedTime = 0
            };
            RotTransform3DTimeComponent rotTimeComponent = new()
            {
                Time = 0,
                InterrputedTime = 0
            };
            SclTransform3DTimeComponent sclTimeComponent = new()
            {
                Time = 0,
                InterrputedTime = 0
            };
            ecb.AddComponent(entity, posTimeComponent);
            ecb.AddComponent(entity, rotTimeComponent);
            ecb.AddComponent(entity, sclTimeComponent);
            
            AnimationProperty3D posProperty = animationListComponent.AnimationProperty3DList.Find(x => x.Type == PropertyType.Transform3DPosition);
            AnimationProperty4D rotProperty = animationListComponent.AnimationProperty4DList.Find(x => x.Type == PropertyType.Transform3DRotation);
            AnimationProperty3D sclProperty = animationListComponent.AnimationProperty3DList.Find(x => x.Type == PropertyType.Transform3DScale);
            
            RefAnimationTransform3DProperty refValue = new()
            {
                Value = new()
            };
            propertyListComponent.Transform3DProperty = refValue;

            if (posProperty.IsStatic)
            {
                posPropertyComponent.Value = posProperty.StaticValue.Value;
                refValue.Value.Position = posPropertyComponent.Value;
            }
            else
            {
                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[UtilityHelper.Transorm3DPositionID];

                ecb.AddBuffer<Transform3DPositionAnimationComponent>(entity);
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
                    Transform3DPositionAnimationComponent component = new()
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
            }

            if (rotProperty.IsStatic)
            {
                rotPropertyComponent.Value = rotProperty.StaticValue.Value;
                refValue.Value.Rotation = rotPropertyComponent.Value;
            }
            else
            {
                List<Animation4D> animationList = animationListComponent.Animation4DDictionary[UtilityHelper.Transorm3DRotationID];

                ecb.AddBuffer<Transform3DRotationAnimationComponent>(entity);
                ecb.AddBuffer<Transform3DRotationBakeDataComponent>(entity);
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
                    Transform3DRotationAnimationComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
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
                        Transform3DRotationBakeDataComponent bakeDataComponent = new()
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
            }
            
            if (sclProperty.IsStatic)
            {
                sclPropertyComponent.Value = sclProperty.StaticValue.Value;
                refValue.Value.Scale = sclPropertyComponent.Value;
            }
            else
            {
                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[UtilityHelper.Transorm3DScaleID];

                ecb.AddBuffer<Transform3DScaleAnimationComponent>(entity);
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
                    Transform3DScaleAnimationComponent component = new()
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
            }
                
            ecb.AddBuffer<PosTransform3DInterruptTimeComponent>(entity);
            for (int i = 0; i < posProperty.AnimationInterruptTimeList.Count; i++)
            {
                PosTransform3DInterruptTimeComponent component = new()
                {
                    InterruptTime = posProperty.AnimationInterruptTimeList[i]
                };
                ecb.AppendToBuffer(entity, component);
            }
                
            ecb.AddBuffer<RotTransform3DInterruptTimeComponent>(entity);
            for (int i = 0; i < posProperty.AnimationInterruptTimeList.Count; i++)
            {
                RotTransform3DInterruptTimeComponent component = new()
                {
                    InterruptTime = posProperty.AnimationInterruptTimeList[i]
                };
                ecb.AppendToBuffer(entity, component);
            }
                
            ecb.AddBuffer<SclTransform3DInterruptTimeComponent>(entity);
            for (int i = 0; i < posProperty.AnimationInterruptTimeList.Count; i++)
            {
                SclTransform3DInterruptTimeComponent component = new()
                {
                    InterruptTime = posProperty.AnimationInterruptTimeList[i]
                };
                ecb.AppendToBuffer(entity, component);
            }

            ManagedAnimationTransform3DPropertyComponent managedPropertyTransform3DComponent = new()
            {
                RefValue = refValue
            };
            Transform3DPropertyInfoComponent propertyInfoComponent = new()
            {
                PositionStartTime = posProperty.StartTime,
                PositionEndTime = posProperty.EndTime,
                RotationStartTime = rotProperty.StartTime,
                RotationEndTime = rotProperty.EndTime,
                ScaleStartTime = sclProperty.StartTime,
                ScaleEndTime = sclProperty.EndTime
            };

            ecb.AddComponent(entity, propertyInfoComponent);
            ecb.AddComponent(entity, managedPropertyTransform3DComponent);
            ecb.AddComponent(entity, new InitializedPropertyComponent());
            ecb.AddComponent(entity, new TimeEnabledComponent());
            ecb.AddComponent(entity, new PosTransform3DInterruptComponent());
            ecb.AddComponent(entity, new RotTransform3DInterruptComponent());
            ecb.AddComponent(entity, new SclTransform3DInterruptComponent());
            ecb.SetComponentEnabled<PosTransform3DInterruptComponent>(entity, false);
            ecb.SetComponentEnabled<RotTransform3DInterruptComponent>(entity, false);
            ecb.SetComponentEnabled<SclTransform3DInterruptComponent>(entity, false);
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
                        if (i == animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(new(1, 1, 0, float.NaN));
                            continue;
                        }
                        else if (i > animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[0].KeyTime, animation.EaseKeyframeList[0].Value, animation.EaseKeyframeList[0].InTan, animation.EaseKeyframeList[0].OutTan));
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
                        if (i == animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(new(1, 1, 0, float.NaN));
                            continue;
                        }
                        else if (i > animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[0].KeyTime, animation.EaseKeyframeList[0].Value, animation.EaseKeyframeList[0].InTan, animation.EaseKeyframeList[0].OutTan));
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
            }
        }

        private void SeperateCustom3DProperty3D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation3DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty3D property in animationListComponent.AnimationProperty3DList)
            {
                if (property.Type == PropertyType.Transform3DPosition || property.Type == PropertyType.Transform3DScale)
                {
                    continue;
                }
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
                        if (i == animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(new(1, 1, 0, float.NaN));
                            continue;
                        }
                        else if (i > animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[0].KeyTime, animation.EaseKeyframeList[0].Value, animation.EaseKeyframeList[0].InTan, animation.EaseKeyframeList[0].OutTan));
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
            }
        }

        private void SeperateCustom4DProperty3D(ManagedAnimationListComponent animationListComponent,
                                                ManagedAnimation3DPropertyListComponent propertyListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty4D property in animationListComponent.AnimationProperty4DList)
            {
                if (property.Type == PropertyType.Transform3DRotation)
                {
                    continue;
                }
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
                        if (i == animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(new(1, 1, 0, float.NaN));
                            continue;
                        }
                        else if (i > animation.EaseKeyframeList.Count)
                        {
                            easeList.Add(float.NaN);
                            continue;
                        }
                        easeList.Add(new(animation.EaseKeyframeList[0].KeyTime, animation.EaseKeyframeList[0].Value, animation.EaseKeyframeList[0].InTan, animation.EaseKeyframeList[0].OutTan));
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
            }
        }

        #endregion
    }
}
