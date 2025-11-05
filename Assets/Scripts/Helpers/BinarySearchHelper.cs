using System;
using System.Collections.Generic;

namespace MNP.Helpers
{
    public static class BinarySearchHelper
    {
        public static int SearchFloorIndex<T>(IList<T> orderedList, T reference) where T : IComparable<T>
        {
            return orderedList.IndexOf(RecursiveSearchFloor(orderedList, 0, orderedList.Count, reference));
        }

        //[,)
        private static T RecursiveSearchFloor<T>(IList<T> list, int left, int right, T value) where T : IComparable<T>
        {
            if (right - 1 == left)
                return list[left];
            int sum = left + right;
            int mid = sum >> 1 - 0b1 & sum;
            int result = list[mid].CompareTo(value);
            if (result > 0)
            {
                return RecursiveSearchFloor(list, left, mid, value);
            }
            if (result < 0)
            {
                return RecursiveSearchFloor(list, mid, right, value);
            }
            return list[mid];
        }
        
        //(,]
        private static T RecursiveSearchCeiling<T>(IList<T> list, int left, int right, T value) where T : IComparable<T>
        {
            if (right - 1 == left)
                return list[left];
            int sum = left + right;
            int mid = sum >> 1 - 0b1 ^ sum;
            int result = list[mid].CompareTo(value);
            if (result > 0)
            {
                return RecursiveSearchCeiling(list, left, mid, value);
            }
            if (result < 0)
            {
                return RecursiveSearchCeiling(list, mid, right, value);
            }
            return list[mid];
        }
    }
}
