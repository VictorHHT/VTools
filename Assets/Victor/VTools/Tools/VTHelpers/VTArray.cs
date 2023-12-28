using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Victor.Tools.VTArray;

namespace Victor.Tools
{
    public static class VTArray
    {
        /// <summary>
        /// Get the closest value in an array to the value passed in
        /// </summary>
        /// <param name="value"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ClosestArrayValue(int value, int[] array)
        {
            int minDifference = int.MaxValue;
            int closestArrayValue = array[0];

            for (int i = 0; i < array.Length; i++)
            {
                int temp = Mathf.Abs(value - array[i]);
                if (temp < minDifference)
                {
                    minDifference = temp;
                    closestArrayValue = array[i];
                }
            }

            return closestArrayValue;
        }

        public static void Swap<T>(IList<T> list, int index1, int index2)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (index1 < 0 || index1 >= list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index1));
            }

            if (index2 < 0 || index2 >= list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index2));
            }

            if (EqualityComparer<T>.Default.Equals(list[index1], list[index2]))
            {
                return;
            }

            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        /// <summary>
        /// An optimal algorithm to select the kth largest or smallest element in an array 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="k"></param>
        /// <param name="findLargest">Find kth largest or kth smallest element</param>
        /// <param name="comparer"></param>
        /// <returns>The kth largest or smallest element in the array</returns>
        public static T QuickSelect<T>(IList<T> arr, int k, bool findLargest, Comparer<T> comparer)
        {
            int adjustedK = findLargest ? arr.Count - k : k - 1;
            return QuickSelectHelper(arr, 0, arr.Count - 1, adjustedK, comparer);
        }

        private static T QuickSelectHelper<T>(IList<T> arr, int left, int right, int k, Comparer<T> comparer)
        {
            if (left == right)
                return arr[left];

            int pivotIndex = Partition(arr, left, right, comparer);

            if (k == pivotIndex)
                return arr[k];
            else if (k < pivotIndex)
                return QuickSelectHelper(arr, left, pivotIndex - 1, k, comparer);
            else
                return QuickSelectHelper(arr, pivotIndex + 1, right, k, comparer);
        }

        private static int Partition<T>(IList<T> arr, int left, int right, Comparer<T> comparer)
        {
            int pivotIndex = ChoosePivot(left, right);
            T pivotValue = arr[pivotIndex];

            VTArray.Swap(arr, pivotIndex, right);

            int partitionIndex = left;

            for (int i = left; i < right; i++)
            {
                if (comparer.Compare(arr[i], pivotValue) < 0)
                {
                    VTArray.Swap(arr, i, partitionIndex);
                    partitionIndex++;
                }
            }

            VTArray.Swap(arr, partitionIndex, right);

            return partitionIndex;
        }

        private static int ChoosePivot(int left, int right)
        {
            // Choose the middle point as pivot
            return left + (right - left) / 2;
        }
    }
}
