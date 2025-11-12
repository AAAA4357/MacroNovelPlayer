using System.Collections.Generic;
using System.Linq;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Managed;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(TimeSystem))]
    partial class BakeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            Entities.WithNone<InitializedPropertyComponent>().ForEach((in Entity entity, in ManagedAnimationListComponent animation) =>
            {
                ManagedAnimationPropertyListComponent component = new()
                {
                    Property1DList = new(),
                    Property2DList = new(),
                    Property3DList = new()
                };
                ecb.AddComponent(entity, component);
                SeperateAnimationObject(animation, component, ecb);
                //ecb.RemoveComponent<ManagedAnimationListComponent>(entity);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
            }).WithoutBurst().Run();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void SeperateAnimationObject(ManagedAnimationListComponent animationListComponent,
                                             ManagedAnimationPropertyListComponent propertyListComponent,
                                             EntityCommandBuffer ecb)
        {
            Seperate1DProperty(animationListComponent, propertyListComponent, ecb);
            Seperate2DProperty(animationListComponent, propertyListComponent, ecb);
            Seperate3DProperty(animationListComponent, propertyListComponent, ecb);
        }

        private void Seperate1DProperty(ManagedAnimationListComponent animationListComponent,
                                        ManagedAnimationPropertyListComponent propertyListComponent,
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
                using NativeList<bool> lerpEnabledList = new(Allocator.Temp);
                using NativeList<float> timeList = new(Allocator.Temp);

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
                    timeList.Add(animation.StartTime);
                    lerpEnabledList.Add(animation.Enabled);
                }
                
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    LerpKeyArray = timeList.ToArray(Allocator.Persistent),
                    LerpEnabledArray = lerpEnabledList.ToArray(Allocator.Persistent),
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
            }
        }

        private void Seperate2DProperty(ManagedAnimationListComponent animationListComponent,
                                        ManagedAnimationPropertyListComponent propertyListComponent,
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
                using NativeList<bool> lerpEnabledList = new(Allocator.Temp);
                using NativeList<float> timeList = new(Allocator.Temp);

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
                    timeList.Add(animation.StartTime);
                    lerpEnabledList.Add(animation.Enabled);
                }

                ManagedAnimationProperty2DComponent managedProperty2DComponent = new()
                {
                    RefValue = refValue
                };
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    LerpKeyArray = timeList.ToArray(Allocator.Persistent),
                    LerpEnabledArray = lerpEnabledList.ToArray(Allocator.Persistent),
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
            }
        }

        private void Seperate3DProperty(ManagedAnimationListComponent animationListComponent,
                                        ManagedAnimationPropertyListComponent propertyListComponent,
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
                using NativeList<bool> lerpEnabledList = new(Allocator.Temp);
                using NativeList<float> timeList = new(Allocator.Temp);

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
                    timeList.Add(animation.StartTime);
                    lerpEnabledList.Add(animation.Enabled);
                }

                ManagedAnimationProperty3DComponent managedProperty3DComponent = new()
                {
                    RefValue = refValue
                };
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    LerpKeyArray = timeList.ToArray(Allocator.Persistent),
                    LerpEnabledArray = lerpEnabledList.ToArray(Allocator.Persistent),
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
