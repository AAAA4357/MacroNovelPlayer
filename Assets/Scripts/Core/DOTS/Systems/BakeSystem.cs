using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.LerpRuntime.Transform2D;
using MNP.Core.DOTS.Components.Managed;
using MNP.Core.DOTS.Components.Transform2D;
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
                ManagedAnimation2DPropertyListComponent component = new()
                {
                    Property1DList = new(),
                    Property2DList = new(),
                    Property3DList = new()
                };
                ecb.AddComponent(entity, component);
                SeperateAnimationObject(animation, component, ecb, entity);
                //ecb.RemoveComponent<ManagedAnimationListComponent>(entity);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
            }).WithoutBurst().Run();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void SeperateAnimationObject(ManagedAnimationListComponent animationListComponent,
                                             ManagedAnimation2DPropertyListComponent propertyListComponent,
                                             EntityCommandBuffer ecb,
                                             Entity entity)
        {
            SeperateTransformProperty(animationListComponent, propertyListComponent, ecb);
            SeperateCustom1DProperty(animationListComponent, propertyListComponent, ecb);
            SeperateCustom2DProperty(animationListComponent, propertyListComponent, ecb);
            SeperateCustom3DProperty(animationListComponent, propertyListComponent, ecb);
            ecb.AddComponent(entity, new BakeReadyComponent());
        }

        private void SeperateTransformProperty(ManagedAnimationListComponent animationListComponent,
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

        private void SeperateCustom1DProperty(ManagedAnimationListComponent animationListComponent,
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

        private void SeperateCustom2DProperty(ManagedAnimationListComponent animationListComponent,
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

        private void SeperateCustom3DProperty(ManagedAnimationListComponent animationListComponent,
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
    }
}
