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
                ManagedAnimationPropertyListComponent component = new();
                ecb.AddComponent(entity, component);
                SeperateAnimationObject(animation, component, ecb);
                ecb.RemoveComponent<ManagedAnimationListComponent>(entity);
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
                NativeList<float2> pathKeyframeList = new(animationList.Select(x => x.PathKeyFrameList.Count).Sum(), Allocator.Temp);
                NativeList<int> pathIndexList = new(animationList.Count, Allocator.Temp);
                NativeList<float4> easeKeyframeList = new(animationList.Select(x => x.EaseKeyframeList.Count).Sum(), Allocator.Temp);
                NativeList<int> easeIndexList = new(animationList.Count, Allocator.Temp);
                NativeList<float> timeList = new(animationList.Count, Allocator.Temp);
                foreach (Animation1D animation in animationList)
                {
                    ProcessAnimationPath1D(pathKeyframeList, animation.PathKeyFrameList, animation.StartTime, animation.DurationTime);
                    pathIndexList.Add(animation.PathKeyFrameList.Count);
                    ProcessAnimationEase(easeKeyframeList, animation.EaseKeyframeList, animation.StartTime, animation.DurationTime);
                    easeIndexList.Add(animation.EaseKeyframeList.Count);
                    timeList.Add(animation.StartTime);
                }
                Animation1DArrayComponent animation1DArrayComponent = new()
                {
                    PathKeyframeArray = pathKeyframeList.ToArray(Allocator.Persistent),
                    PathIndexArray = pathIndexList.ToArray(Allocator.Persistent),
                    EaseKeyframeArray = easeKeyframeList.ToArray(Allocator.Persistent),
                    EaseIndexArray = easeIndexList.ToArray(Allocator.Persistent),
                    TimeArray = timeList.ToArray(Allocator.Persistent)
                };
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    LerpKeyArray = timeList.ToArray(Allocator.Persistent),
                    LerpEnabledArray = new(0, Allocator.Persistent),
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
                ecb.AddComponent(entity, animation1DArrayComponent);
                ecb.AddComponent(entity, managedProperty1DComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                pathKeyframeList.Dispose();
                pathIndexList.Dispose();
                easeKeyframeList.Dispose();
                easeIndexList.Dispose();
                timeList.Dispose();
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
                NativeList<float3> pathKeyframeList = new(animationList.Select(x => x.PathKeyFrameList.Count).Sum(), Allocator.Temp);
                NativeList<float2> pathControlList = new(animationList.Select(x => x.PathKeyFrameList.Count).Sum() * 2, Allocator.Temp);
                NativeList<bool> pathLinearList = new(animationList.Select(x => x.PathKeyFrameList.Count).Sum(), Allocator.Temp);
                NativeList<int> pathIndexList = new(animationList.Count, Allocator.Temp);
                NativeList<float4> easeKeyframeList = new(animationList.Select(x => x.EaseKeyframeList.Count).Sum(), Allocator.Temp);
                NativeList<int> easeIndexList = new(animationList.Count, Allocator.Temp);
                NativeList<float> timeList = new(animationList.Count, Allocator.Temp);
                foreach (Animation2D animation in animationList)
                {
                    ProcessAnimationPath2D(pathKeyframeList, pathControlList, pathLinearList, animation.PathKeyFrameList, animation.StartTime, animation.DurationTime);
                    pathIndexList.Add(animation.PathKeyFrameList.Count);
                    ProcessAnimationEase(easeKeyframeList, animation.EaseKeyframeList, animation.StartTime, animation.DurationTime);
                    easeIndexList.Add(animation.EaseKeyframeList.Count);
                    timeList.Add(animation.StartTime);
                }
                Animation2DArrayComponent animation2DArrayComponent = new()
                {
                    PathKeyframeArray = pathKeyframeList.ToArray(Allocator.Persistent),
                    PathControlArray = pathControlList.ToArray(Allocator.Persistent),
                    PathLinearLerpArray = pathLinearList.ToArray(Allocator.Persistent),
                    PathIndexArray = pathIndexList.ToArray(Allocator.Persistent),
                    EaseKeyframeArray = easeKeyframeList.ToArray(Allocator.Persistent),
                    EaseIndexArray = easeIndexList.ToArray(Allocator.Persistent),
                    TimeArray = timeList.ToArray(Allocator.Persistent)
                };
                ManagedAnimationProperty2DComponent managedProperty2DComponent = new()
                {
                    RefValue = refValue
                };
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    LerpKeyArray = timeList.ToArray(Allocator.Persistent),
                    LerpEnabledArray = new(0, Allocator.Persistent),
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
                ecb.AddComponent(entity, animation2DArrayComponent);
                ecb.AddComponent(entity, managedProperty2DComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                pathKeyframeList.Dispose();
                pathControlList.Dispose();
                pathLinearList.Dispose();
                pathIndexList.Dispose();
                easeKeyframeList.Dispose();
                easeIndexList.Dispose();
                timeList.Dispose();
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
                NativeList<float4> pathKeyframeList = new(animationList.Select(x => x.PathKeyFrameList.Count).Sum(), Allocator.Temp);
                NativeList<float3> pathControlList = new(animationList.Select(x => x.PathKeyFrameList.Count).Sum() * 2, Allocator.Temp);
                NativeList<bool> pathLinearList = new(animationList.Select(x => x.PathKeyFrameList.Count).Sum(), Allocator.Temp);
                NativeList<int> pathIndexList = new(animationList.Count, Allocator.Temp);
                NativeList<float4> easeKeyframeList = new(animationList.Select(x => x.EaseKeyframeList.Count).Sum(), Allocator.Temp);
                NativeList<int> easeIndexList = new(animationList.Count, Allocator.Temp);
                NativeList<float> timeList = new(animationList.Count, Allocator.Temp);
                foreach (Animation3D animation in animationList)
                {
                    ProcessAnimationPath3D(pathKeyframeList, pathControlList, pathLinearList, animation.PathKeyFrameList, animation.StartTime, animation.DurationTime);
                    pathIndexList.Add(animation.PathKeyFrameList.Count);
                    ProcessAnimationEase(easeKeyframeList, animation.EaseKeyframeList, animation.StartTime, animation.DurationTime);
                    easeIndexList.Add(animation.EaseKeyframeList.Count);
                    timeList.Add(animation.StartTime);
                }
                Animation3DArrayComponent animation3DArrayComponent = new()
                {
                    PathKeyframeArray = pathKeyframeList.ToArray(Allocator.Persistent),
                    PathControlArray = pathControlList.ToArray(Allocator.Persistent),
                    PathLinearLerpArray = pathLinearList.ToArray(Allocator.Persistent),
                    PathIndexArray = pathIndexList.ToArray(Allocator.Persistent),
                    EaseKeyframeArray = easeKeyframeList.ToArray(Allocator.Persistent),
                    EaseIndexArray = easeIndexList.ToArray(Allocator.Persistent),
                    TimeArray = timeList.ToArray(Allocator.Persistent)
                };
                ManagedAnimationProperty3DComponent managedProperty3DComponent = new()
                {
                    RefValue = refValue
                };
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    LerpKeyArray = timeList.ToArray(Allocator.Persistent),
                    LerpEnabledArray = new(0, Allocator.Persistent),
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
                ecb.AddComponent(entity, animation3DArrayComponent);
                ecb.AddComponent(entity, managedProperty3DComponent);
                ecb.AddComponent(entity, timeComponent);
                ecb.AddComponent(entity, new InitializedPropertyComponent());
                ecb.AddComponent(entity, new TimeEnabledComponent());
                pathKeyframeList.Dispose();
                pathControlList.Dispose();
                pathLinearList.Dispose();
                pathIndexList.Dispose();
                easeKeyframeList.Dispose();
                easeIndexList.Dispose();
                timeList.Dispose();
            }
        }

        private void ProcessAnimationPath1D(in NativeList<float2> resultList, List<Animation1DPathSegement> segementList, float startTime, float duration)
        {
            float totalWeight = segementList.Select(x => x.Weight).Sum();
            foreach (Animation1DPathSegement path in segementList)
            {
                resultList.Add(new(startTime + duration * (path.Weight / totalWeight), path.StartValue));
            }
            resultList.Add(new(startTime + duration, segementList[^1].EndValue));
        }

        private void ProcessAnimationPath2D(in NativeList<float3> resultPathList, in NativeList<float2> resultEaseList, in NativeList<bool> resultLinearList, List<Animation2DPathSegement> segementList, float startTime, float duration)
        {
            float totalWeight = segementList.Select(x => x.Weight).Sum();
            foreach (Animation2DPathSegement path in segementList)
            {
                resultPathList.Add(new(startTime + duration * (path.Weight / totalWeight), path.StartValue));
                resultEaseList.Add(new(path.Control0Value));
                resultEaseList.Add(new(path.Control1Value));
                resultLinearList.Add(path.Linear);
            }
            resultPathList.Add(new(startTime + duration, segementList[^1].EndValue));
        }

        private void ProcessAnimationPath3D(in NativeList<float4> resultPathList, in NativeList<float3> resultEaseList, in NativeList<bool> resultLinearList, List<Animation3DPathSegement> segementList, float startTime, float duration)
        {
            float totalWeight = segementList.Select(x => x.Weight).Sum();
            foreach (Animation3DPathSegement path in segementList)
            {
                resultPathList.Add(new(startTime + duration * (path.Weight / totalWeight), path.StartValue));
                resultEaseList.Add(new(path.Control0Value));
                resultEaseList.Add(new(path.Control1Value));
                resultLinearList.Add(path.Linear);
            }
            resultPathList.Add(new(startTime + duration, segementList[^1].EndValue));
        }
        
        private void ProcessAnimationEase(in NativeList<float4> resultList, List<AnimationEaseKeyframe> easeList, float startTime, float duration)
        {
            foreach (AnimationEaseKeyframe ease in easeList)
            {
                resultList.Add(new(startTime + ease.KeyTime * duration, ease.Value, ease.InTan, ease.OutTan));
            }
        }
    }
}
