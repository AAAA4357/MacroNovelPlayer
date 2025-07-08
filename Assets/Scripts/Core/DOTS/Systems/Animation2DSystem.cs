using System.Linq;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animations;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Jobs;
using MNP.Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(TimeSystem))]
    [UpdateBefore(typeof(Animation3DSystem))]
    partial struct Animation2DSystem : ISystem
    {
        NativeList<Entity> entities;
        NativeList<ElementComponent> elements;
        NativeList<float2x3> animationPathP0Array;
        NativeList<float2x3> animationPathP1Array;
        NativeList<float2x3> animationPathP2Array;
        NativeList<float2x3> animationPathP3Array;
        NativeList<float4> animationPositionEaseList;
        NativeList<float4> animationRotationEaseList;
        NativeList<float4> animationScaleEaseList;
        NativeList<int> positionEaseSpacingArray;
        NativeList<int> rotationEaseSpacingArray;
        NativeList<int> scaleEaseSpacingArray;
        NativeList<float> timeArray;
        NativeList<float> timeStartArray;
        NativeList<float> timeDurationArray;
        NativeList<bool> timeEndedArray;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TimeEnabledComponent>();

            entities = new(Allocator.Persistent);
            elements = new(Allocator.Persistent);
            animationPathP0Array = new(Allocator.Persistent);
            animationPathP1Array = new(Allocator.Persistent);
            animationPathP2Array = new(Allocator.Persistent);
            animationPathP3Array = new(Allocator.Persistent);
            positionEaseSpacingArray = new(Allocator.Persistent);
            rotationEaseSpacingArray = new(Allocator.Persistent);
            scaleEaseSpacingArray = new(Allocator.Persistent);
            timeArray = new(Allocator.Persistent);
            timeStartArray = new(Allocator.Persistent);
            timeDurationArray = new(Allocator.Persistent);
            timeEndedArray = new(Allocator.Persistent);
            animationPositionEaseList = new(Allocator.Persistent);
            animationRotationEaseList = new(Allocator.Persistent);
            animationScaleEaseList = new(Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            entities.Clear();
            elements.Clear();
            animationPathP0Array.Clear();
            animationPathP1Array.Clear();
            animationPathP2Array.Clear();
            animationPathP3Array.Clear();
            positionEaseSpacingArray.Clear();
            rotationEaseSpacingArray.Clear();
            scaleEaseSpacingArray.Clear();
            timeArray.Clear();
            timeStartArray.Clear();
            timeDurationArray.Clear();
            timeEndedArray.Clear();
            animationPositionEaseList.Clear();
            animationRotationEaseList.Clear();
            animationScaleEaseList.Clear();
        
            int index = 0;
            foreach (var (animationTransform2DArrayComponent, timeComponent, elementComponent, entity) in SystemAPI.Query<RefRO<AnimationTransform2DArrayComponent>, RefRO<TimeComponent>, RefRO<ElementComponent>>().WithEntityAccess())
            {
                int animationIndex = RecursionFind(animationTransform2DArrayComponent.ValueRO, 0, animationTransform2DArrayComponent.ValueRO.AnimationCount, timeComponent.ValueRO.Time);
                bool endAnimation = animationIndex == animationTransform2DArrayComponent.ValueRO.AnimationCount;
                if (endAnimation) animationIndex--;

                animationPathP0Array.Add(animationTransform2DArrayComponent.ValueRO.AnimationPathP0Array[animationIndex]);
                animationPathP1Array.Add(animationTransform2DArrayComponent.ValueRO.AnimationPathP1Array[animationIndex]);
                animationPathP2Array.Add(animationTransform2DArrayComponent.ValueRO.AnimationPathP2Array[animationIndex]);
                animationPathP3Array.Add(animationTransform2DArrayComponent.ValueRO.AnimationPathP3Array[animationIndex]);

                int easePosCount = animationTransform2DArrayComponent.ValueRO.PositionEaseSpacingArray[animationIndex];
                int easeRotCount = animationTransform2DArrayComponent.ValueRO.RotationEaseSpacingArray[animationIndex];
                int easeSclCount = animationTransform2DArrayComponent.ValueRO.ScaleEaseSpacingArray[animationIndex];

                int easePosOffset = 0;
                int easeRotOffset = 0;
                int easeSclOffset = 0;
                for (int i = 0; i < animationIndex; i++)
                {
                    easePosOffset += animationTransform2DArrayComponent.ValueRO.PositionEaseSpacingArray[i];
                    easeRotOffset += animationTransform2DArrayComponent.ValueRO.RotationEaseSpacingArray[i];
                    easeSclOffset += animationTransform2DArrayComponent.ValueRO.ScaleEaseSpacingArray[i];
                }

                for (int i = easePosOffset; i < easePosOffset + easePosCount; i++)
                {
                    animationPositionEaseList.Add(animationTransform2DArrayComponent.ValueRO.AnimationPositionEaseArray[i]);
                }
                for (int i = easeRotOffset; i < easeRotOffset + easeRotCount; i++)
                {
                    animationRotationEaseList.Add(animationTransform2DArrayComponent.ValueRO.AnimationRotationEaseArray[i]);
                }
                for (int i = easeSclOffset; i < easeSclOffset + easeSclCount; i++)
                {
                    animationScaleEaseList.Add(animationTransform2DArrayComponent.ValueRO.AnimationScaleEaseArray[i]);
                }

                positionEaseSpacingArray.Add(easePosCount);
                rotationEaseSpacingArray.Add(easeRotCount);
                scaleEaseSpacingArray.Add(easeSclCount);

                timeArray.Add(timeComponent.ValueRO.Time);
                timeStartArray.Add(animationTransform2DArrayComponent.ValueRO.AnimationFrameStartArray[animationIndex]);
                timeDurationArray.Add(animationTransform2DArrayComponent.ValueRO.AnimationFrameDurationArray[animationIndex]);
                timeEndedArray.Add(endAnimation);

                elements.Add(elementComponent.ValueRO);
                entities.Add(entity);

                index++;
            }

            EntityCommandBuffer ecb = new(Allocator.TempJob);
            AnimationTransform2DLerpJob job = new()
            {
                AnimationPathP0Array = animationPathP0Array.AsArray(),
                AnimationPathP1Array = animationPathP1Array.AsArray(),
                AnimationPathP2Array = animationPathP2Array.AsArray(),
                AnimationPathP3Array = animationPathP3Array.AsArray(),
                AnimationPositionEaseArray = animationPositionEaseList.AsArray(),
                PositionEaseSpacingArray = positionEaseSpacingArray.AsArray(),
                AnimationRotationEaseArray = animationRotationEaseList.AsArray(),
                RotationEaseSpacingArray = rotationEaseSpacingArray.AsArray(),
                AnimationScaleEaseArray = animationScaleEaseList.AsArray(),
                ScaleEaseSpacingArray = scaleEaseSpacingArray.AsArray(),
                TimeArray = timeArray.AsArray(),
                TimeStartArray = timeStartArray.AsArray(),
                TimeDurationArray = timeDurationArray.AsArray(),
                TimeEndedArray = timeEndedArray.AsArray(),
                Elements = elements.AsArray(),
                Entities = entities.AsArray(),
                EcbWriter = ecb.AsParallelWriter(),
            };

            JobHandle handle = job.Schedule(index, 32);
            handle.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            entities.Dispose();
            elements.Dispose();
            animationPathP0Array.Dispose();
            animationPathP1Array.Dispose();
            animationPathP2Array.Dispose();
            animationPathP3Array.Dispose();
            positionEaseSpacingArray.Dispose();
            rotationEaseSpacingArray.Dispose();
            scaleEaseSpacingArray.Dispose();
            timeArray.Dispose();
            timeStartArray.Dispose();
            timeDurationArray.Dispose();
            timeEndedArray.Dispose();
            animationPositionEaseList.Dispose();
            animationRotationEaseList.Dispose();
            animationScaleEaseList.Dispose();
        }
        
        [BurstCompile]
        private int RecursionFind(in AnimationTransform2DArrayComponent array, int start, int end, float time)
        {
            if (start == end)
                return start;
            int middle = (start + end) >> 1;
            float startTime = array.AnimationFrameStartArray[middle];
            float duration = array.AnimationFrameDurationArray[middle];
            if (time < startTime)
            {
                return RecursionFind(in array, start, middle, time);
            }
            if (time > startTime + duration)
            {
                return RecursionFind(in array, middle + 1, end, time);
            }
            return middle;
        }
    }
}
