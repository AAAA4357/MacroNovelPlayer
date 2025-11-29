using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Transform;
using MNP.Helpers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core
{
    public class SceneBaker
    {
        int propertyIndexCounter;

        public async Task BakeElements(IList<AnimationElement> elements, IProgress<float> progress)
        {
            World world = World.DefaultGameObjectInjectionWorld;
            EntityManager manager = world.EntityManager;
            EntityCommandBuffer ecb = new(Allocator.Temp);
            int index = 0;
            foreach (AnimationElement element in elements)
            {
                await Task.Run(() =>
                {
                    Entity entity = ecb.CreateEntity();
                    ElementComponent elementComponent = new()
                    {
                        ID = element.ID,
                        TextureID = element.TextureID,
                        MeshID = element.MeshID,
                        ObjectType = element.Type
                    };
                    switch (element.Type)
                    {
                        case ObjectType.Object2D:
                            SeperateAnimationObject2D(element.Animations, ecb, entity, ref elementComponent);
                            break;
                        case ObjectType.Object3D:
                            SeperateAnimationObject3D(element.Animations, ecb, entity, ref elementComponent);
                            break;
                    }
                    //ecb.RemoveComponent<ManagedAnimationListComponent>(entity);
                    ecb.AddComponent(entity, new InitializedPropertyComponent());
                });
                progress?.Report(index / (elements.Count + 1));
                index++;
            }
            await Task.Run(() =>
            {
                ecb.Playback(manager);
            });
            progress?.Report(1);
            ecb.Dispose();
        }

        #region 2D

        private void SeperateAnimationObject2D(AnimationList animationListComponent,
                                               EntityCommandBuffer ecb,
                                               Entity entity,
                                               ref ElementComponent element)
        {
            SeperateCustom1DProperty2D(animationListComponent, ecb, ref element);
            SeperateCustom2DProperty2D(animationListComponent, ecb, ref element);
            SeperateCustom3DProperty2D(animationListComponent, ecb);
            SeperateCustom4DProperty2D(animationListComponent, ecb);
            ecb.AddComponent(entity, new BakeReadyComponent());
        }
        private void SeperateCustom1DProperty2D(AnimationList animationListComponent,
                                                EntityCommandBuffer ecb,
                                               ref ElementComponent element)
        {
            foreach (AnimationProperty1D property in animationListComponent.AnimationProperty1DList)
            {
                Entity entity = ecb.CreateEntity();
                Property1DComponent property1DComponent = new();
                if (property.IsStatic)
                {
                    property1DComponent.Value = property.StaticValue.Value;
                    property1DComponent.Index = -1;
                    ecb.AddComponent(entity, property1DComponent);
                    continue;
                }
                property1DComponent.Index = propertyIndexCounter;
                propertyIndexCounter++;

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
                    ecb.AddComponent(entity, new Transform2DRotationComponent());
                    element.TransformRotationIndex = property1DComponent.Index;
                }
            }
        }

        private void SeperateCustom2DProperty2D(AnimationList animationListComponent,
                                                EntityCommandBuffer ecb,
                                               ref ElementComponent element)
        {
            foreach (AnimationProperty2D property in animationListComponent.AnimationProperty2DList)
            {
                Entity entity = ecb.CreateEntity();
                Property2DComponent property2DComponent = new();
                if (property.IsStatic)
                {
                    property2DComponent.Value = property.StaticValue.Value;
                    property2DComponent.Index = -1;
                    ecb.AddComponent(entity, property2DComponent);
                    continue;
                }
                property2DComponent.Index = propertyIndexCounter;
                propertyIndexCounter++;

                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[property.ID];

                ecb.AddBuffer<Animation2DComponent>(entity);
                ecb.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                int dataIndex = 0;
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
                        DurationTime = animation.DurationTime,
                        LerpType = animation.LerpType,
                        BezierDataIndex = dataIndex
                    };
                    if (animation.LerpType == Float2LerpType.AverageBezier)
                    {
                        FixedList128Bytes<float2> map = new();
                        FixedList128Bytes<float2> lengthMap = new();
                        float totalLength = PathLerpHelper.GetLengthAtParameter2D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue);
                        for (int i = 0; i < map.Capacity; i++)
                        {
                            float t = (float)i / (map.Capacity - 1);
                            float curveLength = PathLerpHelper.GetLengthAtParameter2D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue, 0, t);
                            map.Add(new(t, curveLength / totalLength));
                        }
                        for (int i = 0; i < lengthMap.Capacity; i++)
                        {
                            float t = (float)i / (lengthMap.Capacity - 1);
                            UtilityHelper.GetFloorIndexInNativeContainer(map, x => x.y, t, out int mapIndex);
                            float2 start = map[mapIndex].yx;
                            float2 end = map[mapIndex + 1].yx;
                            float delta = end.y - start.y;
                            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
                            lengthMap.Add(new(t, averageT));
                        }
                        AnimationBezierBakeDataComponent bakeDataComponent = new()
                        {
                            BezierLengthMap = lengthMap
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

                ecb.AddComponent(entity, property2DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);
                

                if (property.Type == PropertyType.Transform2DPosition)
                {
                    ecb.AddComponent(entity, new Transform2DPositionComponent());
                    element.TransformPositionIndex = property2DComponent.Index;
                }
                else if (property.Type == PropertyType.Transform2DScale)
                {
                    ecb.AddComponent(entity, new Transform2DScaleComponent());
                    element.TransformScaleIndex = property2DComponent.Index;
                }
            }
        }

        private void SeperateCustom3DProperty2D(AnimationList animationListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty3D property in animationListComponent.AnimationProperty3DList)
            {
                Entity entity = ecb.CreateEntity();
                Property3DComponent property3DComponent = new();
                if (property.IsStatic)
                {
                    property3DComponent.Value = property.StaticValue.Value;
                    property3DComponent.Index = -1;
                    ecb.AddComponent(entity, property3DComponent);
                    continue;
                }
                property3DComponent.Index = propertyIndexCounter;
                propertyIndexCounter++;

                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[property.ID];

                ecb.AddBuffer<Animation3DComponent>(entity);
                ecb.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                int dataIndex = 0;
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
                        DurationTime = animation.DurationTime,
                        LerpType = animation.LerpType,
                        BezierDataIndex = dataIndex
                    };
                    if (animation.LerpType == Float3LerpType.AverageBezier)
                    {
                        FixedList128Bytes<float2> map = new();
                        FixedList128Bytes<float2> lengthMap = new();
                        float totalLength = PathLerpHelper.GetLengthAtParameter3D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue);
                        for (int i = 0; i < map.Capacity; i++)
                        {
                            float t = (float)i / (map.Capacity - 1);
                            float curveLength = PathLerpHelper.GetLengthAtParameter3D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue, 0, t);
                            map.Add(new(t, curveLength / totalLength));
                        }
                        for (int i = 0; i < lengthMap.Capacity; i++)
                        {
                            float t = (float)i / (lengthMap.Capacity - 1);
                            UtilityHelper.GetFloorIndexInNativeContainer(map, x => x.y, t, out int mapIndex);
                            float2 start = map[mapIndex].yx;
                            float2 end = map[mapIndex + 1].yx;
                            float delta = end.y - start.y;
                            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
                            lengthMap.Add(new(t, averageT));
                        }
                        AnimationBezierBakeDataComponent bakeDataComponent = new()
                        {
                            BezierLengthMap = lengthMap
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

                ecb.AddComponent(entity, property3DComponent);
                ecb.AddComponent(entity, propertyInfoComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                ecb.AddComponent(entity, new InterruptComponent());
                ecb.SetComponentEnabled<InterruptComponent>(entity, false);
            }
        }

        private void SeperateCustom4DProperty2D(AnimationList animationListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty4D property in animationListComponent.AnimationProperty4DList)
            {
                Entity entity = ecb.CreateEntity();
                Property4DComponent property4DComponent = new();
                if (property.IsStatic)
                {
                    property4DComponent.Value = property.StaticValue.Value;
                    property4DComponent.Index = -1;
                    ecb.AddComponent(entity, property4DComponent);
                    continue;
                }
                property4DComponent.Index = propertyIndexCounter;
                propertyIndexCounter++;

                List<Animation4D> animationList = animationListComponent.Animation4DDictionary[property.ID];

                ecb.AddBuffer<Animation4DComponent>(entity);
                ecb.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                ecb.AddBuffer<AnimationSquadBakeDataComponent>(entity);
                int bezierDataIndex = 0;
                int squadDataIndex = 0;
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
                        BezierDataIndex = bezierDataIndex,
                        SquadDataIndex = squadDataIndex
                    };
                    if (animation.LerpType == Float4LerpType.AverageBezier)
                    {
                        FixedList128Bytes<float2> map = new();
                        FixedList128Bytes<float2> lengthMap = new();
                        float totalLength = PathLerpHelper.GetLengthAtParameter4D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue);
                        for (int i = 0; i < map.Capacity; i++)
                        {
                            float t = (float)i / (map.Capacity - 1);
                            float curveLength = PathLerpHelper.GetLengthAtParameter4D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue, 0, t);
                            map.Add(new(t, curveLength / totalLength));
                        }
                        for (int i = 0; i < lengthMap.Capacity; i++)
                        {
                            float t = (float)i / (lengthMap.Capacity - 1);
                            UtilityHelper.GetFloorIndexInNativeContainer(map, x => x.y, t, out int mapIndex);
                            float2 start = map[mapIndex].yx;
                            float2 end = map[mapIndex + 1].yx;
                            float delta = end.y - start.y;
                            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
                            lengthMap.Add(new(t, averageT));
                        }
                        AnimationBezierBakeDataComponent bakeDataComponent = new()
                        {
                            BezierLengthMap = lengthMap
                        };
                        ecb.AppendToBuffer(entity, bakeDataComponent);
                        bezierDataIndex++;
                    }
                    else if (animation.LerpType == Float4LerpType.Squad)
                    {
                        float4 q12 = QuaternionHelper.Mul(animation.Control0Value.Inverse(), animation.Control1Value);
                        float4 q23 = QuaternionHelper.Mul(animation.Control1Value.Inverse(), animation.EndValue);
                        float4 a = animation.StartValue; //q0
                        float4 b = QuaternionHelper.Mul(animation.StartValue.Inverse(), animation.Control0Value); //q01
                        float4 c = QuaternionHelper.Mul(b.Inverse(), q12); //q01^-1*q12
                        float4 d = QuaternionHelper.Mul(c.Inverse(), q23); //q01^-1*q12
                        AnimationSquadBakeDataComponent bakeDataComponent = new()
                        {
                            q0 = a,
                            q01 = b,
                            q01_1q12 = c,
                            q12_1q23 = d
                        };
                        ecb.AppendToBuffer(entity, bakeDataComponent);
                        squadDataIndex++;
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
            }
        }

        #endregion


        #region 3D

        private void SeperateAnimationObject3D(AnimationList animationListComponent,
                                               EntityCommandBuffer ecb,
                                               Entity entity,
                                               ref ElementComponent element)
        {
            SeperateCustom1DProperty3D(animationListComponent, ecb);
            SeperateCustom2DProperty3D(animationListComponent, ecb);
            SeperateCustom3DProperty3D(animationListComponent, ecb, ref element);
            SeperateCustom4DProperty3D(animationListComponent, ecb, ref element);
            ecb.AddComponent(entity, new BakeReadyComponent());
        }

        private void SeperateCustom1DProperty3D(AnimationList animationListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty1D property in animationListComponent.AnimationProperty1DList)
            {
                Entity entity = ecb.CreateEntity();
                Property1DComponent property1DComponent = new();
                if (property.IsStatic)
                {
                    property1DComponent.Value = property.StaticValue.Value;
                    property1DComponent.Index = -1;
                    ecb.AddComponent(entity, property1DComponent);
                    continue;
                }
                property1DComponent.Index = propertyIndexCounter;
                propertyIndexCounter++;

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
            }
        }

        private void SeperateCustom2DProperty3D(AnimationList animationListComponent,
                                                EntityCommandBuffer ecb)
        {
            foreach (AnimationProperty2D property in animationListComponent.AnimationProperty2DList)
            {
                Entity entity = ecb.CreateEntity();
                Property2DComponent property2DComponent = new();
                if (property.IsStatic)
                {
                    property2DComponent.Value = property.StaticValue.Value;
                    property2DComponent.Index = -1;
                    ecb.AddComponent(entity, property2DComponent);
                    continue;
                }
                property2DComponent.Index = propertyIndexCounter;
                propertyIndexCounter++;

                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[property.ID];

                ecb.AddBuffer<Animation2DComponent>(entity);
                ecb.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                int dataIndex = 0;
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
                    if (animation.LerpType == Float2LerpType.AverageBezier)
                    {
                        FixedList128Bytes<float2> map = new();
                        FixedList128Bytes<float2> lengthMap = new();
                        float totalLength = PathLerpHelper.GetLengthAtParameter2D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue);
                        for (int i = 0; i < map.Capacity; i++)
                        {
                            float t = (float)i / (map.Capacity - 1);
                            float curveLength = PathLerpHelper.GetLengthAtParameter2D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue, 0, t);
                            map.Add(new(t, curveLength / totalLength));
                        }
                        for (int i = 0; i < lengthMap.Capacity; i++)
                        {
                            float t = (float)i / (lengthMap.Capacity - 1);
                            UtilityHelper.GetFloorIndexInNativeContainer(map, x => x.y, t, out int mapIndex);
                            float2 start = map[mapIndex].yx;
                            float2 end = map[mapIndex + 1].yx;
                            float delta = end.y - start.y;
                            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
                            lengthMap.Add(new(t, averageT));
                        }
                        AnimationBezierBakeDataComponent bakeDataComponent = new()
                        {
                            BezierLengthMap = lengthMap
                        };
                        ecb.AppendToBuffer(entity, bakeDataComponent);
                        dataIndex++;
                    }
                    Animation2DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        Control0 = animation.Control0Value,
                        Control1 = animation.Control1Value,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime,
                        LerpType = animation.LerpType
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
            }
        }

        private void SeperateCustom3DProperty3D(AnimationList animationListComponent,
                                                EntityCommandBuffer ecb,
                                                ref ElementComponent element)
        {
            foreach (AnimationProperty3D property in animationListComponent.AnimationProperty3DList)
            {
                Entity entity = ecb.CreateEntity();
                Property3DComponent property3DComponent = new();
                if (property.IsStatic)
                {
                    property3DComponent.Value = property.StaticValue.Value;
                    property3DComponent.Index = -1;
                    ecb.AddComponent(entity, property3DComponent);
                    continue;
                }
                property3DComponent.Index = propertyIndexCounter;
                propertyIndexCounter++;

                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[property.ID];

                ecb.AddBuffer<Animation3DComponent>(entity);
                ecb.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                int dataIndex = 0;
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
                    if (animation.LerpType == Float3LerpType.AverageBezier)
                    {
                        FixedList128Bytes<float2> map = new();
                        FixedList128Bytes<float2> lengthMap = new();
                        float totalLength = PathLerpHelper.GetLengthAtParameter3D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue);
                        for (int i = 0; i < map.Capacity; i++)
                        {
                            float t = (float)i / (map.Capacity - 1);
                            float curveLength = PathLerpHelper.GetLengthAtParameter3D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue, 0, t);
                            map.Add(new(t, curveLength / totalLength));
                        }
                        for (int i = 0; i < lengthMap.Capacity; i++)
                        {
                            float t = (float)i / (lengthMap.Capacity - 1);
                            UtilityHelper.GetFloorIndexInNativeContainer(map, x => x.y, t, out int mapIndex);
                            float2 start = map[mapIndex].yx;
                            float2 end = map[mapIndex + 1].yx;
                            float delta = end.y - start.y;
                            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
                            lengthMap.Add(new(t, averageT));
                        }
                        AnimationBezierBakeDataComponent bakeDataComponent = new()
                        {
                            BezierLengthMap = lengthMap
                        };
                        ecb.AppendToBuffer(entity, bakeDataComponent);
                        dataIndex++;
                    }
                    Animation3DComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        Control0 = animation.Control0Value,
                        Control1 = animation.Control1Value,
                        EaseKeyframeList = easeList,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime,
                        LerpType = animation.LerpType
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
                    ecb.AddComponent(entity, new Transform3DPositionComponent());
                    element.TransformPositionIndex = property3DComponent.Index;
                }
                else if (property.Type == PropertyType.Transform3DScale)
                {
                    ecb.AddComponent(entity, new Transform3DScaleComponent());
                    element.TransformScaleIndex = property3DComponent.Index;
                }
            }
        }

        private void SeperateCustom4DProperty3D(AnimationList animationListComponent,
                                                EntityCommandBuffer ecb,
                                                ref ElementComponent element)
        {
            foreach (AnimationProperty4D property in animationListComponent.AnimationProperty4DList)
            {
                Entity entity = ecb.CreateEntity();
                Property4DComponent property4DComponent = new();
                if (property.IsStatic)
                {
                    property4DComponent.Value = property.StaticValue.Value;
                    property4DComponent.Index = -1;
                    ecb.AddComponent(entity, property4DComponent);
                    continue;
                }
                property4DComponent.Index = propertyIndexCounter;
                propertyIndexCounter++;

                List<Animation4D> animationList = animationListComponent.Animation4DDictionary[property.ID];

                ecb.AddBuffer<Animation4DComponent>(entity);
                ecb.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                ecb.AddBuffer<AnimationSquadBakeDataComponent>(entity);
                int bezierDataIndex = 0;
                int squadDataIndex = 0;
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
                        BezierDataIndex = bezierDataIndex,
                        SquadDataIndex = squadDataIndex
                    };
                    if (animation.LerpType == Float4LerpType.AverageBezier)
                    {
                        FixedList128Bytes<float2> map = new();
                        FixedList128Bytes<float2> lengthMap = new();
                        float totalLength = PathLerpHelper.GetLengthAtParameter4D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue);
                        for (int i = 0; i < map.Capacity; i++)
                        {
                            float t = (float)i / (map.Capacity - 1);
                            float curveLength = PathLerpHelper.GetLengthAtParameter4D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue, 0, t);
                            map.Add(new(t, curveLength / totalLength));
                        }
                        for (int i = 0; i < lengthMap.Capacity; i++)
                        {
                            float t = (float)i / (lengthMap.Capacity - 1);
                            UtilityHelper.GetFloorIndexInNativeContainer(map, x => x.y, t, out int mapIndex);
                            float2 start = map[mapIndex].yx;
                            float2 end = map[mapIndex + 1].yx;
                            float delta = end.y - start.y;
                            float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
                            lengthMap.Add(new(t, averageT));
                        }
                        AnimationBezierBakeDataComponent bakeDataComponent = new()
                        {
                            BezierLengthMap = lengthMap
                        };
                        ecb.AppendToBuffer(entity, bakeDataComponent);
                        bezierDataIndex++;
                    }
                    else if (animation.LerpType == Float4LerpType.Squad)
                    {
                        float4 q12 = QuaternionHelper.Mul(animation.Control0Value.Inverse(), animation.Control1Value);
                        float4 q23 = QuaternionHelper.Mul(animation.Control1Value.Inverse(), animation.EndValue);
                        float4 a = animation.StartValue; //q0
                        float4 b = QuaternionHelper.Mul(animation.StartValue.Inverse(), animation.Control0Value); //q01
                        float4 c = QuaternionHelper.Mul(b.Inverse(), q12); //q01^-1*q12
                        float4 d = QuaternionHelper.Mul(c.Inverse(), q23); //q01^-1*q12
                        AnimationSquadBakeDataComponent bakeDataComponent = new()
                        {
                            q0 = a,
                            q01 = b,
                            q01_1q12 = c,
                            q12_1q23 = d
                        };
                        ecb.AppendToBuffer(entity, bakeDataComponent);
                        squadDataIndex++;
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
                    ecb.AddComponent(entity, new Transform3DRotationComponent());
                    element.TransformRotationIndex = property4DComponent.Index;
                }
            }
        }

        #endregion
    }
}
