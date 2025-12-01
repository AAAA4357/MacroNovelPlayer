using System;
using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.Transform;
using MNP.Helpers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Cysharp.Threading.Tasks;


namespace MNP.Core
{
    public class SceneBaker
    {
        public int PropertyIndexCounter;

        public async UniTask BakeElements(List<MNObject> objects, IProgress<float> progress)
        {
            World world = World.DefaultGameObjectInjectionWorld;
            EntityManager manager = world.EntityManager;
            for (int i = 0; i < objects.Count; i++)
            {
                MNObject mnObject = objects[i];
                Entity entity = manager.CreateEntity();
                ElementComponent elementComponent = new()
                {
                    ID = mnObject.ID,
                    TextureID = mnObject.TextureID,
                    MeshID = mnObject.MeshID,
                    ObjectType = mnObject.Type,
                    TransformPositionIndex = -1,
                    TransformRotationIndex = -1,
                    TransformScaleIndex = -1,
                };
                switch (mnObject.Type)
                {
                    case ObjectType.Empty2D:
                    case ObjectType.Object2D:
                        SeperateAnimationObject2D(objects, mnObject.Animations, manager, ref elementComponent);
                        break;
                    case ObjectType.Empty3D:
                    case ObjectType.Object3D:
                        SeperateAnimationObject3D(objects, mnObject.Animations, manager, ref elementComponent);
                        break;
                }
                manager.AddComponentData(entity, elementComponent);
                manager.AddComponentData(entity, new BakeReadyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
                progress?.Report((float)i / objects.Count);
                await UniTask.Yield();
            }
        }

        #region 2D

        private void SeperateAnimationObject2D(List<MNObject> objects, 
                                               MNAnimation animationListComponent,
                                               EntityManager manager,
                                               ref ElementComponent element)
        {
            SeperateCustom1DProperty2D(objects, animationListComponent, manager, ref element);
            SeperateCustom2DProperty2D(objects, animationListComponent, manager, ref element);
            SeperateCustom3DProperty2D(objects, animationListComponent, manager);
            SeperateCustom4DProperty2D(objects, animationListComponent, manager);
        }

        private void SeperateCustom1DProperty2D(List<MNObject> objects, 
                                                MNAnimation animationListComponent,
                                                EntityManager manager,
                                                ref ElementComponent element)
        {
            foreach (AnimationProperty1D property in animationListComponent.AnimationProperty1DList)
            {
                Entity entity = manager.CreateEntity();
                Property1DComponent property1DComponent = new();
                if (property.IsStatic)
                {
                    property1DComponent.Value = property.StaticValue.Value;
                    property1DComponent.Index = -1;
                    manager.AddComponentData(entity, property1DComponent);
                    continue;
                }
                property1DComponent.Index = PropertyIndexCounter;
                property.Index = PropertyIndexCounter;
                PropertyIndexCounter++;

                List<Animation1D> animationList = animationListComponent.Animation1DDictionary[property.ID];

                manager.AddBuffer<Animation1DComponent>(entity);
                DynamicBuffer<Animation1DComponent> animationBuffer = manager.GetBuffer<Animation1DComponent>(entity);
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
                    animationBuffer.Add(component);
                }
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }

                manager.AddBuffer<DependencyPropertyComponent>(entity);
                DynamicBuffer<DependencyPropertyComponent> dependencyPropertyBuffer = manager.GetBuffer<DependencyPropertyComponent>(entity);
                AnimationDependencyProeprty dependencyProeprty = property.Dependency;
                if (dependencyProeprty is not null)
                {
                    property1DComponent.DependencyType = property.Dependency.Type;
                }
                while (dependencyProeprty is not null)
                {
                    MNObject mnObject = objects.Find(x => x.ID == dependencyProeprty.ObjectID);
                    AnimationProperty1D property1D = mnObject.Animations.AnimationProperty1DList.Find(x => x.ID == dependencyProeprty.PropertyID);
                    dependencyPropertyBuffer.Add(new() 
                    {
                        PropertyIndex = property1D.Index
                    });
                    dependencyProeprty = property1D.Dependency;
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

                manager.AddComponentData(entity, property1DComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new InitializedPropertyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new LerpEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);

                if (property.Type == PropertyType.Transform2DRotation)
                {
                    manager.AddComponentData(entity, new Transform2DRotationComponent());
                    element.TransformRotationIndex = property1DComponent.Index;
                }
            }
        }

        private void SeperateCustom2DProperty2D(List<MNObject> objects, 
                                                MNAnimation animationListComponent,
                                                EntityManager manager,
                                                ref ElementComponent element)
        {
            foreach (AnimationProperty2D property in animationListComponent.AnimationProperty2DList)
            {
                Entity entity = manager.CreateEntity();
                Property2DComponent property2DComponent = new();
                if (property.IsStatic)
                {
                    property2DComponent.Value = property.StaticValue.Value;
                    property2DComponent.Index = -1;
                    manager.AddComponentData(entity, property2DComponent);
                    continue;
                }
                property2DComponent.Index = PropertyIndexCounter;
                property.Index = PropertyIndexCounter;
                PropertyIndexCounter++;

                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[property.ID];

                manager.AddBuffer<Animation2DComponent>(entity);
                manager.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                DynamicBuffer<Animation2DComponent> animationBuffer = manager.GetBuffer<Animation2DComponent>(entity);
                DynamicBuffer<AnimationBezierBakeDataComponent> animationBakeDataBuffer = manager.GetBuffer<AnimationBezierBakeDataComponent>(entity);
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
                        animationBakeDataBuffer.Add(bakeDataComponent);
                        dataIndex++;
                    }
                    animationBuffer.Add(component);
                }
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }

                manager.AddBuffer<DependencyPropertyComponent>(entity);
                DynamicBuffer<DependencyPropertyComponent> dependencyPropertyBuffer = manager.GetBuffer<DependencyPropertyComponent>(entity);
                AnimationDependencyProeprty dependencyProeprty = property.Dependency;
                if (dependencyProeprty is not null)
                {
                    property2DComponent.DependencyType = property.Dependency.Type;
                }
                while (dependencyProeprty != null)
                {
                    MNObject mnObject = objects.Find(x => x.ID == dependencyProeprty.ObjectID);
                    AnimationProperty2D property2D = mnObject.Animations.AnimationProperty2DList.Find(x => x.ID == dependencyProeprty.PropertyID);
                    dependencyPropertyBuffer.Add(new() 
                    {
                        PropertyIndex = property2D.Index
                    });
                    dependencyProeprty = property2D.Dependency;
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

                manager.AddComponentData(entity, property2DComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new InitializedPropertyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new LerpEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
                

                if (property.Type == PropertyType.Transform2DPosition)
                {
                    manager.AddComponentData(entity, new Transform2DPositionComponent());
                    element.TransformPositionIndex = property2DComponent.Index;
                }
                else if (property.Type == PropertyType.Transform2DScale)
                {
                    manager.AddComponentData(entity, new Transform2DScaleComponent());
                    element.TransformScaleIndex = property2DComponent.Index;
                }
            }
        }

        private void SeperateCustom3DProperty2D(List<MNObject> objects, 
                                                MNAnimation animationListComponent,
                                                EntityManager manager)
        {
            foreach (AnimationProperty3D property in animationListComponent.AnimationProperty3DList)
            {
                Entity entity = manager.CreateEntity();
                Property3DComponent property3DComponent = new();
                if (property.IsStatic)
                {
                    property3DComponent.Value = property.StaticValue.Value;
                    property3DComponent.Index = -1;
                    manager.AddComponentData(entity, property3DComponent);
                    continue;
                }
                property3DComponent.Index = PropertyIndexCounter;
                property.Index = PropertyIndexCounter;
                PropertyIndexCounter++;

                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[property.ID];

                manager.AddBuffer<Animation3DComponent>(entity);
                manager.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                DynamicBuffer<Animation3DComponent> animationBuffer = manager.GetBuffer<Animation3DComponent>(entity);
                DynamicBuffer<AnimationBezierBakeDataComponent> animationBakeDataBuffer = manager.GetBuffer<AnimationBezierBakeDataComponent>(entity);
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
                        animationBakeDataBuffer.Add(bakeDataComponent);
                        dataIndex++;
                    }
                    animationBuffer.Add(component);
                }
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }

                manager.AddBuffer<DependencyPropertyComponent>(entity);
                DynamicBuffer<DependencyPropertyComponent> dependencyPropertyBuffer = manager.GetBuffer<DependencyPropertyComponent>(entity);
                AnimationDependencyProeprty dependencyProeprty = property.Dependency;
                if (dependencyProeprty is not null)
                {
                    property3DComponent.DependencyType = property.Dependency.Type;
                }
                while (dependencyProeprty != null)
                {
                    MNObject mnObject = objects.Find(x => x.ID == dependencyProeprty.ObjectID);
                    AnimationProperty3D property3D = mnObject.Animations.AnimationProperty3DList.Find(x => x.ID == dependencyProeprty.PropertyID);
                    dependencyPropertyBuffer.Add(new() 
                    {
                        PropertyIndex = property3D.Index
                    });
                    dependencyProeprty = property3D.Dependency;
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

                manager.AddComponentData(entity, property3DComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new InitializedPropertyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new LerpEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
            }
        }

        private void SeperateCustom4DProperty2D(List<MNObject> objects, 
                                                MNAnimation animationListComponent,
                                                EntityManager manager)
        {
            foreach (AnimationProperty4D property in animationListComponent.AnimationProperty4DList)
            {
                Entity entity = manager.CreateEntity();
                Property4DComponent property4DComponent = new();
                if (property.IsStatic)
                {
                    property4DComponent.Value = property.StaticValue.Value;
                    property4DComponent.Index = -1;
                    manager.AddComponentData(entity, property4DComponent);
                    continue;
                }
                property4DComponent.Index = PropertyIndexCounter;
                property.Index = PropertyIndexCounter;
                PropertyIndexCounter++;

                List<Animation4D> animationList = animationListComponent.Animation4DDictionary[property.ID];

                manager.AddBuffer<Animation4DComponent>(entity);
                manager.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                manager.AddBuffer<AnimationSquadBakeDataComponent>(entity);
                DynamicBuffer<Animation4DComponent> animationBuffer = manager.GetBuffer<Animation4DComponent>(entity);
                DynamicBuffer<AnimationBezierBakeDataComponent> animationBezierBakeDataBuffer = manager.GetBuffer<AnimationBezierBakeDataComponent>(entity);
                DynamicBuffer<AnimationSquadBakeDataComponent> animationSquadBakeDataBuffer = manager.GetBuffer<AnimationSquadBakeDataComponent>(entity);
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
                        animationBezierBakeDataBuffer.Add(bakeDataComponent);
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
                        animationSquadBakeDataBuffer.Add(bakeDataComponent);
                        squadDataIndex++;
                    }
                    else if (animation.LerpType == Float4LerpType.AverageSquad)
                    {
                        float4 q12 = QuaternionHelper.Mul(animation.Control0Value.Inverse(), animation.Control1Value);
                        float4 q23 = QuaternionHelper.Mul(animation.Control1Value.Inverse(), animation.EndValue);
                        float4 a = animation.StartValue; //q0
                        float4 b = QuaternionHelper.Mul(animation.StartValue.Inverse(), animation.Control0Value); //q01
                        float4 c = QuaternionHelper.Mul(b.Inverse(), q12); //q01^-1*q12
                        float4 d = QuaternionHelper.Mul(c.Inverse(), q23); //q01^-1*q12
                        AnimationSquadBakeDataComponent squadBakeDataComponent = new()
                        {
                            q0 = a,
                            q01 = b,
                            q01_1q12 = c,
                            q12_1q23 = d
                        };
                        animationSquadBakeDataBuffer.Add(squadBakeDataComponent);
                        squadDataIndex++;
                        FixedList128Bytes<float2> map = new();
                        FixedList128Bytes<float2> lengthMap = new();
                        float totalLength = PathLerpHelper.GetLengthAtSquadParameter4D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue);
                        for (int i = 0; i < map.Capacity; i++)
                        {
                            float t = (float)i / (map.Capacity - 1);
                            float curveLength = PathLerpHelper.GetLengthAtSquadParameter4D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue, 0, t);
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
                        AnimationBezierBakeDataComponent bezierBakeDataComponent = new()
                        {
                            BezierLengthMap = lengthMap
                        };
                        animationBezierBakeDataBuffer.Add(bezierBakeDataComponent);
                        bezierDataIndex++;
                    }
                    animationBuffer.Add(component);
                }
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }

                manager.AddBuffer<DependencyPropertyComponent>(entity);
                DynamicBuffer<DependencyPropertyComponent> dependencyPropertyBuffer = manager.GetBuffer<DependencyPropertyComponent>(entity);
                AnimationDependencyProeprty dependencyProeprty = property.Dependency;
                if (dependencyProeprty is not null)
                {
                    property4DComponent.DependencyType = property.Dependency.Type;
                }
                while (dependencyProeprty != null)
                {
                    MNObject mnObject = objects.Find(x => x.ID == dependencyProeprty.ObjectID);
                    AnimationProperty4D property4D = mnObject.Animations.AnimationProperty4DList.Find(x => x.ID == dependencyProeprty.PropertyID);
                    dependencyPropertyBuffer.Add(new() 
                    {
                        PropertyIndex = property4D.Index
                    });
                    dependencyProeprty = property4D.Dependency;
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

                manager.AddComponentData(entity, property4DComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new InitializedPropertyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new LerpEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
            }
        }

        #endregion


        #region 3D

        private void SeperateAnimationObject3D(List<MNObject> objects, 
                                               MNAnimation animationListComponent,
                                               EntityManager manager,
                                               ref ElementComponent element)
        {
            SeperateCustom1DProperty3D(objects, animationListComponent, manager);
            SeperateCustom2DProperty3D(objects, animationListComponent, manager);
            SeperateCustom3DProperty3D(objects, animationListComponent, manager, ref element);
            SeperateCustom4DProperty3D(objects, animationListComponent, manager, ref element);
        }

        private void SeperateCustom1DProperty3D(List<MNObject> objects, 
                                                MNAnimation animationListComponent,
                                                EntityManager manager)
        {
            foreach (AnimationProperty1D property in animationListComponent.AnimationProperty1DList)
            {
                Entity entity = manager.CreateEntity();
                Property1DComponent property1DComponent = new();
                if (property.IsStatic)
                {
                    property1DComponent.Value = property.StaticValue.Value;
                    property1DComponent.Index = -1;
                    manager.AddComponentData(entity, property1DComponent);
                    continue;
                }
                property1DComponent.Index = PropertyIndexCounter;
                property.Index = PropertyIndexCounter;
                PropertyIndexCounter++;

                List<Animation1D> animationList = animationListComponent.Animation1DDictionary[property.ID];

                manager.AddBuffer<Animation1DComponent>(entity);
                DynamicBuffer<Animation1DComponent> animationBuffer = manager.GetBuffer<Animation1DComponent>(entity);
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
                    animationBuffer.Add(component);
                }
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }

                manager.AddBuffer<DependencyPropertyComponent>(entity);
                DynamicBuffer<DependencyPropertyComponent> dependencyPropertyBuffer = manager.GetBuffer<DependencyPropertyComponent>(entity);
                AnimationDependencyProeprty dependencyProeprty = property.Dependency;
                if (dependencyProeprty is not null)
                {
                    property1DComponent.DependencyType = property.Dependency.Type;
                }
                while (dependencyProeprty != null)
                {
                    MNObject mnObject = objects.Find(x => x.ID == dependencyProeprty.ObjectID);
                    AnimationProperty1D property1D = mnObject.Animations.AnimationProperty1DList.Find(x => x.ID == dependencyProeprty.PropertyID);
                    dependencyPropertyBuffer.Add(new() 
                    {
                        PropertyIndex = property1D.Index
                    });
                    dependencyProeprty = property1D.Dependency;
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

                manager.AddComponentData(entity, property1DComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new InitializedPropertyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new LerpEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
            }
        }

        private void SeperateCustom2DProperty3D(List<MNObject> objects, 
                                                MNAnimation animationListComponent,
                                                EntityManager manager)
        {
            foreach (AnimationProperty2D property in animationListComponent.AnimationProperty2DList)
            {
                Entity entity = manager.CreateEntity();
                Property2DComponent property2DComponent = new();
                if (property.IsStatic)
                {
                    property2DComponent.Value = property.StaticValue.Value;
                    property2DComponent.Index = -1;
                    manager.AddComponentData(entity, property2DComponent);
                    continue;
                }
                property2DComponent.Index = PropertyIndexCounter;
                property.Index = PropertyIndexCounter;
                PropertyIndexCounter++;

                List<Animation2D> animationList = animationListComponent.Animation2DDictionary[property.ID];

                manager.AddBuffer<Animation2DComponent>(entity);
                manager.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                DynamicBuffer<Animation2DComponent> animationBuffer = manager.GetBuffer<Animation2DComponent>(entity);
                DynamicBuffer<AnimationBezierBakeDataComponent> animationBakeDataBuffer = manager.GetBuffer<AnimationBezierBakeDataComponent>(entity);
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
                        animationBakeDataBuffer.Add(bakeDataComponent);
                        dataIndex++;
                    }
                    animationBuffer.Add(component);
                }
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }

                manager.AddBuffer<DependencyPropertyComponent>(entity);
                DynamicBuffer<DependencyPropertyComponent> dependencyPropertyBuffer = manager.GetBuffer<DependencyPropertyComponent>(entity);
                AnimationDependencyProeprty dependencyProeprty = property.Dependency;
                if (dependencyProeprty is not null)
                {
                    property2DComponent.DependencyType = property.Dependency.Type;
                }
                while (dependencyProeprty != null)
                {
                    MNObject mnObject = objects.Find(x => x.ID == dependencyProeprty.ObjectID);
                    AnimationProperty2D property2D = mnObject.Animations.AnimationProperty2DList.Find(x => x.ID == dependencyProeprty.PropertyID);
                    dependencyPropertyBuffer.Add(new() 
                    {
                        PropertyIndex = property2D.Index
                    });
                    dependencyProeprty = property2D.Dependency;
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

                manager.AddComponentData(entity, property2DComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new InitializedPropertyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new LerpEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
            }
        }

        private void SeperateCustom3DProperty3D(List<MNObject> objects, 
                                                MNAnimation animationListComponent,
                                                EntityManager manager,
                                                ref ElementComponent element)
        {
            foreach (AnimationProperty3D property in animationListComponent.AnimationProperty3DList)
            {
                Entity entity = manager.CreateEntity();
                Property3DComponent property3DComponent = new();
                if (property.IsStatic)
                {
                    property3DComponent.Value = property.StaticValue.Value;
                    property3DComponent.Index = -1;
                    manager.AddComponentData(entity, property3DComponent);
                    continue;
                }
                property3DComponent.Index = PropertyIndexCounter;
                property.Index = PropertyIndexCounter;
                PropertyIndexCounter++;

                List<Animation3D> animationList = animationListComponent.Animation3DDictionary[property.ID];

                manager.AddBuffer<Animation3DComponent>(entity);
                manager.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                DynamicBuffer<Animation3DComponent> animationBuffer = manager.GetBuffer<Animation3DComponent>(entity);
                DynamicBuffer<AnimationBezierBakeDataComponent> animationBakeDataBuffer = manager.GetBuffer<AnimationBezierBakeDataComponent>(entity);
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
                        animationBakeDataBuffer.Add(bakeDataComponent);
                        dataIndex++;
                    }
                    animationBuffer.Add(component);
                }
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }

                manager.AddBuffer<DependencyPropertyComponent>(entity);
                DynamicBuffer<DependencyPropertyComponent> dependencyPropertyBuffer = manager.GetBuffer<DependencyPropertyComponent>(entity);
                AnimationDependencyProeprty dependencyProeprty = property.Dependency;
                if (dependencyProeprty is not null)
                {
                    property3DComponent.DependencyType = property.Dependency.Type;
                }
                while (dependencyProeprty != null)
                {
                    MNObject mnObject = objects.Find(x => x.ID == dependencyProeprty.ObjectID);
                    AnimationProperty3D property3D = mnObject.Animations.AnimationProperty3DList.Find(x => x.ID == dependencyProeprty.PropertyID);
                    dependencyPropertyBuffer.Add(new() 
                    {
                        PropertyIndex = property3D.Index
                    });
                    dependencyProeprty = property3D.Dependency;
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

                manager.AddComponentData(entity, property3DComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new InitializedPropertyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new LerpEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);

                if (property.Type == PropertyType.Transform3DPosition)
                {
                    manager.AddComponentData(entity, new Transform3DPositionComponent());
                    element.TransformPositionIndex = property3DComponent.Index;
                }
                else if (property.Type == PropertyType.Transform3DScale)
                {
                    manager.AddComponentData(entity, new Transform3DScaleComponent());
                    element.TransformScaleIndex = property3DComponent.Index;
                }
            }
        }

        private void SeperateCustom4DProperty3D(List<MNObject> objects, 
                                                MNAnimation animationListComponent,
                                                EntityManager manager,
                                                ref ElementComponent element)
        {
            foreach (AnimationProperty4D property in animationListComponent.AnimationProperty4DList)
            {
                Entity entity = manager.CreateEntity();
                Property4DComponent property4DComponent = new();
                if (property.IsStatic)
                {
                    property4DComponent.Value = property.StaticValue.Value;
                    property4DComponent.Index = -1;
                    manager.AddComponentData(entity, property4DComponent);
                    continue;
                }
                property4DComponent.Index = PropertyIndexCounter;
                property.Index = PropertyIndexCounter;
                PropertyIndexCounter++;

                List<Animation4D> animationList = animationListComponent.Animation4DDictionary[property.ID];

                manager.AddBuffer<Animation4DComponent>(entity);
                manager.AddBuffer<AnimationBezierBakeDataComponent>(entity);
                manager.AddBuffer<AnimationSquadBakeDataComponent>(entity);
                DynamicBuffer<Animation4DComponent> animationBuffer = manager.GetBuffer<Animation4DComponent>(entity);
                DynamicBuffer<AnimationBezierBakeDataComponent> animationBezierBakeDataBuffer = manager.GetBuffer<AnimationBezierBakeDataComponent>(entity);
                DynamicBuffer<AnimationSquadBakeDataComponent> animationSquadBakeDataBuffer = manager.GetBuffer<AnimationSquadBakeDataComponent>(entity);
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
                        animationBezierBakeDataBuffer.Add(bakeDataComponent);
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
                        animationSquadBakeDataBuffer.Add(bakeDataComponent);
                        squadDataIndex++;
                    }
                    else if (animation.LerpType == Float4LerpType.AverageSquad)
                    {
                        float4 q12 = QuaternionHelper.Mul(animation.Control0Value.Inverse(), animation.Control1Value);
                        float4 q23 = QuaternionHelper.Mul(animation.Control1Value.Inverse(), animation.EndValue);
                        float4 a = animation.StartValue; //q0
                        float4 b = QuaternionHelper.Mul(animation.StartValue.Inverse(), animation.Control0Value); //q01
                        float4 c = QuaternionHelper.Mul(b.Inverse(), q12); //q01^-1*q12
                        float4 d = QuaternionHelper.Mul(c.Inverse(), q23); //q01^-1*q12
                        AnimationSquadBakeDataComponent squadBakeDataComponent = new()
                        {
                            q0 = a,
                            q01 = b,
                            q01_1q12 = c,
                            q12_1q23 = d
                        };
                        animationSquadBakeDataBuffer.Add(squadBakeDataComponent);
                        squadDataIndex++;
                        FixedList128Bytes<float2> map = new();
                        FixedList128Bytes<float2> lengthMap = new();
                        float totalLength = PathLerpHelper.GetLengthAtSquadParameter4D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue);
                        for (int i = 0; i < map.Capacity; i++)
                        {
                            float t = (float)i / (map.Capacity - 1);
                            float curveLength = PathLerpHelper.GetLengthAtSquadParameter4D(animation.StartValue, animation.Control0Value, animation.Control1Value, animation.EndValue, 0, t);
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
                        AnimationBezierBakeDataComponent bezierBakeDataComponent = new()
                        {
                            BezierLengthMap = lengthMap
                        };
                        animationBezierBakeDataBuffer.Add(bezierBakeDataComponent);
                        bezierDataIndex++;
                    }
                    animationBuffer.Add(component);
                }
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }

                manager.AddBuffer<DependencyPropertyComponent>(entity);
                DynamicBuffer<DependencyPropertyComponent> dependencyPropertyBuffer = manager.GetBuffer<DependencyPropertyComponent>(entity);
                AnimationDependencyProeprty dependencyProeprty = property.Dependency;
                if (dependencyProeprty is not null)
                {
                    property4DComponent.DependencyType = property.Dependency.Type;
                }
                while (dependencyProeprty != null)
                {
                    MNObject mnObject = objects.Find(x => x.ID == dependencyProeprty.ObjectID);
                    AnimationProperty4D property4D = mnObject.Animations.AnimationProperty4DList.Find(x => x.ID == dependencyProeprty.PropertyID);
                    dependencyPropertyBuffer.Add(new() 
                    {
                        PropertyIndex = property4D.Index
                    });
                    dependencyProeprty = property4D.Dependency;
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

                manager.AddComponentData(entity, property4DComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new InitializedPropertyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new LerpEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
                
                if (property.Type == PropertyType.Transform3DRotation)
                {
                    manager.AddComponentData(entity, new Transform3DRotationComponent());
                    element.TransformRotationIndex = property4DComponent.Index;
                }
            }
        }

        #endregion
    }
}
