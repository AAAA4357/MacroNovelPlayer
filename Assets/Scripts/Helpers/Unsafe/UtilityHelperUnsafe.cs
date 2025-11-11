using System;
using MNP.Core.DataStruct.Unsafe;
using Unity.Burst;
using Unity.Collections;

namespace MNP.Helpers.Unsafe
{
    [BurstCompile]
    public static class UtilityHelperUnsafe
    {
        [BurstCompile]
        public static unsafe void GetFloorIndexInArrayUnsafe<T>(T* valueArray, int startIndex, int arrayLength, Func<T, float> converter, float referenceValue, out int resultIndex, out float fixedT) where T : unmanaged
        {
            int index = 0;
            for (int i = startIndex; i < arrayLength; i++)
            {
                if (referenceValue.CompareTo(converter.Invoke(valueArray[i])) < 0)
                {
                    index = i - 1;
                    break;
                }
                else
                {
                    if (i >= arrayLength - 1)
                    {
                        index = i;
                        break;
                    }
                }
            }
            resultIndex = index;
            if (index == arrayLength - 1)
            {
                fixedT = 1;
                return;
            }
            float duration = converter.Invoke(valueArray[index + 1]) - converter.Invoke(valueArray[index]);
            fixedT = (referenceValue - converter.Invoke(valueArray[index])) / duration;
        }

        [BurstCompile]
        public static unsafe void GetFloorIndexLengthInArrayIndexedUnsafe<T>(T* valueArray, int* indexArray, int arrayLength, Func<T, float> converter, float referenceValue, out int resultIndex, out int length) where T : unmanaged
        {
            int index = 0;
            for (int i = 0; i < arrayLength; i++)
            {
                if (referenceValue.CompareTo(converter.Invoke(valueArray[indexArray[i]])) < 0)
                {
                    index = i - 1;
                    break;
                }
                else
                {
                    if (i == arrayLength - 2)
                    {
                        index = i;
                        break;
                    }
                }
            }
            resultIndex = indexArray[index];
            length = indexArray[index + 1] - indexArray[index];
        }
    }
}
