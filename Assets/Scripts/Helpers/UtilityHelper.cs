using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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
        
        public const string Text2DID = "2D_Text";
        public const string Text3DID = "3D_Text";

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

        [BurstCompile]
        public static void GetFloorIndexInNativeContainer<T>(in INativeList<T> valueContainer, Func<T, float> converter, float referenceValue, out int resultIndex) where T : unmanaged
        {
            int index = 0;
            for (int i = 1; i < valueContainer.Length; i++)
            {
                if (referenceValue.CompareTo(converter.Invoke(valueContainer[i])) < 0)
                {
                    index = i - 1;
                    break;
                }
                else
                {
                    if (i >= valueContainer.Length - 2)
                    {
                        index = i;
                        break;
                    }
                }
            }
            resultIndex = index;
        }

        public static void GetFloorIndexInContainer<T>(IList<T> valueContainer, Func<T, float> converter, float referenceValue, out int resultIndex)
        {
            int index = 0;
            for (int i = 1; i < valueContainer.Count; i++)
            {
                if (referenceValue.CompareTo(converter.Invoke(valueContainer[i])) < 0)
                {
                    index = i - 1;
                    break;
                }
                else
                {
                    if (i >= valueContainer.Count - 1)
                    {
                        index = i;
                        break;
                    }
                }
            }
            resultIndex = index;
        }
        
        [BurstCompile]
        public static float4 CharToFloat4(char character)
        {
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(new[] { character });
            float4 result = float4.zero;
            for (int i = 0; i < math.min(3, utf8Bytes.Length); i++)
            {
                result[i] = utf8Bytes[i];
            }
            result.w = utf8Bytes.Length;
            return result;
        }
        
        [BurstCompile]
        public static char Float4ToChar(float4 encoded)
        {
            int byteLength = (int)encoded.w;
            byteLength = math.clamp(byteLength, 1, 4);
            byte[] utf8Bytes = new byte[byteLength];
            for (int i = 0; i < byteLength; i++)
            {
                utf8Bytes[i] = (byte)encoded[i];
            }
            string result = System.Text.Encoding.UTF8.GetString(utf8Bytes);
            return result.Length > 0 ? result[0] : '\0';
        }
    }
}
