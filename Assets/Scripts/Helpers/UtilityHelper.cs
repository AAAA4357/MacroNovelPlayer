using System;
using Unity.Burst;
using Unity.Collections;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class UtilityHelper
    {
        [BurstCompile]
        public static void GetFloorIndexInArray<T>(in NativeArray<T> valueArray, Func<T, float> converter, float referenceValue, out int resultIndex, out float fixedT) where T : struct
        {
            int index = 0;
            for (int i = 1; i < valueArray.Length; i++)
            {
                if (referenceValue.CompareTo(converter.Invoke(valueArray[i])) < 0)
                {
                    index = i - 1;
                    break;
                }
                else
                {
                    if (i == valueArray.Length - 1)
                    {
                        index = i;
                        break;
                    }
                }
            }
            resultIndex = index;
            float duration = converter.Invoke(valueArray[index + 1]) - converter.Invoke(valueArray[index]);
            fixedT = (referenceValue - converter.Invoke(valueArray[index])) / duration;
        }

        [BurstCompile]
        public static void GetFoldedArrayValue<T>(in NativeArray<T> valueArray, in NativeArray<int> indexArray, int index, out NativeArray<T> resultArray) where T : struct
        {
            int length;
            int start;
            if (index == 0)
            {
                length = indexArray[1];
                start = 0;
            }
            else if (index == indexArray.Length - 1)
            {
                length = valueArray.Length - indexArray[index];
                start = indexArray[index];
            }
            else
            {
                length = indexArray[index + 1] - indexArray[index];
                start = indexArray[index];
            }
            NativeArray<T> results = new(length, Allocator.TempJob);
            for (int i = start, j = 0; i < start + length; i++, j++)
            {
                results[j] = valueArray[i];
            }
            resultArray = results;
        }

        [BurstCompile]
        public static void GetFoldedArrayValue<T>(in NativeArray<T> valueArray, in NativeArray<int> indexArray, int indexFactor, int index, out NativeArray<T> resultArray) where T : struct
        {
            NativeArray<int> indexs = new(indexArray.Length, Allocator.Temp);
            for (int i = 0; i < indexArray.Length; i++)
            {
                indexs[i] = indexArray[i] * indexFactor;
            }
            GetFoldedArrayValue(valueArray, indexs, index, out resultArray);
            indexs.Dispose();
        }
    }
}
