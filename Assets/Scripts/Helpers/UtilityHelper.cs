using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class UtilityHelper
    {
        public const string Transorm2DPositionID = "Transform2D_Position";
        public const string Transorm2DRotationID = "Transform2D_Rotation";
        public const string Transorm2DScaleID = "Transform2D_Scale";

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

        [BurstCompile]
        public static void GetFloorIndexInArray<T>(in NativeArray<T> valueSlice, Func<T, float> converter, float referenceValue, out int resultIndex, out float fixedT) where T : struct
        {
            int index = 0;
            for (int i = 1; i < valueSlice.Length - 1; i++)
            {
                if (referenceValue.CompareTo(converter.Invoke(valueSlice[i])) < 0)
                {
                    index = i - 1;
                    break;
                }
                else
                {
                    if (i >= valueSlice.Length - 2)
                    {
                        index = i;
                        break;
                    }
                }
            }
            resultIndex = index;
            if (index == valueSlice.Length - 2)
            {
                fixedT = 1;
                return;
            }
            float duration = converter.Invoke(valueSlice[index + 1]) - converter.Invoke(valueSlice[index]);
            fixedT = (referenceValue - converter.Invoke(valueSlice[index])) / duration;
        }

        [BurstCompile]
        public static void GetFloorIndexInSlice<T>(in NativeSlice<T> valueSlice, Func<T, float> converter, float referenceValue, out int resultIndex, out float fixedT) where T : struct
        {
            int index = 0;
            for (int i = 1; i < valueSlice.Length; i++)
            {
                if (referenceValue.CompareTo(converter.Invoke(valueSlice[i])) < 0)
                {
                    index = i - 1;
                    break;
                }
                else
                {
                    if (i >= valueSlice.Length - 1)
                    {
                        index = i;
                        break;
                    }
                }
            }
            resultIndex = index;
            if (index == valueSlice.Length - 1)
            {
                fixedT = 1;
                return;
            }
            float duration = converter.Invoke(valueSlice[index + 1]) - converter.Invoke(valueSlice[index]);
            fixedT = (referenceValue - converter.Invoke(valueSlice[index])) / duration;
        }
    }
}
