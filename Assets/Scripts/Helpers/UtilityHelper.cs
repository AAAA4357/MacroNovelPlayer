using System;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class UtilityHelper
    {
        public const string Transorm2DPositionID = "Transform2D_Position";
        public const string Transorm2DRotationID = "Transform2D_Rotation";
        public const string Transorm2DScaleID = "Transform2D_Scale";
        public const string Transorm3DPositionID = "Transform3D_Position";
        public const string Transorm3DRotationID = "Transform3D_Rotation";
        public const string Transorm3DScaleID = "Transform3D_Scale";

        public const int Float2_LinearLerp = 0;
        public const int Float2_BezierLerp = 1;
        public const int Float2_AverageBezierLerp = 2;

        public const int Float3_LinearLerp = 0;
        public const int Float3_BezierLerp = 1;
        public const int Float3_AverageBezierLerp = 2;

        public const int Float4_LinearLerp = 0;
        public const int Float4_LinearSLerp = 1;
        public const int Float4_BezierLerp = 2;
        public const int Float4_AverageBezierLerp = 3;
        public const int Float4_SquadLerp = 4;

        public const float InterruptTorloance = 0.005f;

        [BurstCompile]
        public static void GetFloorIndexInBufferWithLength<T>(in DynamicBuffer<T> valueBuffer, Func<T, float> startConverter, Func<T, float> lengthConverter, float referenceValue, out int resultIndex, out float fixedT) where T : unmanaged
        {
            int index = 0;
            for (int i = 1; i < valueBuffer.Length; i++)
            {
                if (referenceValue.CompareTo(startConverter.Invoke(valueBuffer[i])) < 0)
                {
                    index = i - 1;
                    break;
                }
                else
                {
                    if (i >= valueBuffer.Length - 1)
                    {
                        index = i;
                        break;
                    }
                }
            }
            resultIndex = index;
            float duration = lengthConverter.Invoke(valueBuffer[index]);
            fixedT = (referenceValue - startConverter.Invoke(valueBuffer[index])) / duration;
        }
    }
}
