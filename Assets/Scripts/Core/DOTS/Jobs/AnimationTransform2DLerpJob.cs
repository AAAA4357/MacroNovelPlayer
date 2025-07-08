using MNP.Core.DOTS.Components;
using MNP.Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    public struct AnimationTransform2DLerpJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float2x3> AnimationPathP0Array;
        [ReadOnly]
        public NativeArray<float2x3> AnimationPathP1Array;
        [ReadOnly]
        public NativeArray<float2x3> AnimationPathP2Array;
        [ReadOnly]
        public NativeArray<float2x3> AnimationPathP3Array;
        [ReadOnly]
        public NativeArray<float4> AnimationPositionEaseArray;
        [ReadOnly]
        public NativeArray<int> PositionEaseSpacingArray;
        [ReadOnly]
        public NativeArray<float4> AnimationRotationEaseArray;
        [ReadOnly]
        public NativeArray<int> RotationEaseSpacingArray;
        [ReadOnly]
        public NativeArray<float4> AnimationScaleEaseArray;
        [ReadOnly]
        public NativeArray<int> ScaleEaseSpacingArray;

        [ReadOnly]
        public NativeArray<float> TimeArray;
        [ReadOnly]
        public NativeArray<float> TimeStartArray;
        [ReadOnly]
        public NativeArray<float> TimeDurationArray;

        [ReadOnly]
        public NativeArray<bool> TimeEndedArray;

        [ReadOnly]
        public NativeArray<ElementComponent> Elements;
        [ReadOnly]
        public NativeArray<Entity> Entities;

        public EntityCommandBuffer.ParallelWriter EcbWriter;

        [BurstCompile]
        public void Execute(int index)
        {
            float2x3 PathP0 = AnimationPathP0Array[index];
            float2x3 PathP1 = AnimationPathP1Array[index];
            float2x3 PathP2 = AnimationPathP2Array[index];
            float2x3 PathP3 = AnimationPathP3Array[index];

            float2 PosP0 = PathP0.c0;
            float2 PosP1 = PathP1.c0;
            float2 PosP2 = PathP2.c0;
            float2 PosP3 = PathP3.c0;

            float2 RotP0 = PathP0.c1;
            float2 RotP1 = PathP1.c1;
            float2 RotP2 = PathP2.c1;
            float2 RotP3 = PathP3.c1;

            float2 SclP0 = PathP0.c2;
            float2 SclP1 = PathP1.c2;
            float2 SclP2 = PathP2.c2;
            float2 SclP3 = PathP3.c2;

            int PosEaseLen = PositionEaseSpacingArray[index];
            int RotEaseLen = RotationEaseSpacingArray[index];
            int SclEaseLen = ScaleEaseSpacingArray[index];

            int PosEaseOff = 0;
            int RotEaseOff = 0;
            int SclEaseOff = 0;
            for (int i = 0; i < index; i++)
            {
                PosEaseOff += PositionEaseSpacingArray[i];
                RotEaseOff += RotationEaseSpacingArray[i];
                SclEaseOff += ScaleEaseSpacingArray[i];
            }

            NativeArray<float4> PositionEase = new(PosEaseLen, Allocator.Temp);
            NativeArray<float4> RotationEase = new(RotEaseLen, Allocator.Temp);
            NativeArray<float4> ScaleEase = new(SclEaseLen, Allocator.Temp);
            for (int i = 0; i < PosEaseLen; i++)
                PositionEase[i] = AnimationPositionEaseArray[PosEaseOff + i];
            for (int i = 0; i < RotEaseLen; i++)
                RotationEase[i] = AnimationRotationEaseArray[RotEaseOff + i];
            for (int i = 0; i < SclEaseLen; i++)
                ScaleEase[i] = AnimationScaleEaseArray[SclEaseOff + i];

            float time = TimeArray[index];
            float startTime = TimeStartArray[index];
            float durationTime = TimeDurationArray[index];

            time -= startTime;

            float ePos = EasingFunctionHelper.GetEase(PositionEase, time / durationTime);
            float eRot = EasingFunctionHelper.GetEase(RotationEase, time / durationTime);
            float eScl = EasingFunctionHelper.GetEase(ScaleEase, time / durationTime);

            float2 resultPos = new();
            BezierCurveHelper.GetPoint(ref resultPos, PosP0, PosP1, PosP2, PosP3, ePos);
            float2 resultRot = new();
            BezierCurveHelper.GetPoint(ref resultRot, RotP0, RotP1, RotP2, RotP3, eRot);
            float2 resultScl = new();
            BezierCurveHelper.GetPoint(ref resultScl, SclP0, SclP1, SclP2, SclP3, eScl);

            ElementComponent element = Elements[index];
            element.Transform = new()
            {
                Position = resultPos,
                Rotation = resultRot.x,
                Scale = resultScl
            };
            EcbWriter.SetComponent(index, Entities[index], element);

            if (TimeEndedArray[index])
                EcbWriter.RemoveComponent(index, Entities[index], typeof(TimeEnabledComponent));

            PositionEase.Dispose();
            RotationEase.Dispose();
            ScaleEase.Dispose();
        }
    }
}
