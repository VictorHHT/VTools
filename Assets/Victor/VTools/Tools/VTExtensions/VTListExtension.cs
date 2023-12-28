using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Victor.Tools
{
    public static class VTListExtension
    {
        public static void SetNewLength<T>(this List<T> list, int newLength)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list", "List can not be null");
            }

            if (newLength < 0)
            {
                throw new ArgumentException("List length must be large than or equal to 0", "newLength");
            }

            while (list.Count < newLength)
            {
                list.Add(default(T));
            }

            while (list.Count > newLength)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        public static void SetNewLength<T>(this List<T> list, int newLength, T newElementValue)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list", "List can not be null");
            }

            if (newLength < 0)
            {
                throw new ArgumentException("List length must be large than or equal to 0", "newLength");
            }

            while (list.Count < newLength)
            {
                list.Add(newElementValue);
            }

            while (list.Count > newLength)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        public static void AddRange(this IList list, IEnumerable items)
        {
            foreach (object item in items)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// This causes all the elements between start index and target index shift to the left or right, elements moved beyond the boundaries will lost
        /// Does not support multidimensional array(works for jagged array and multidimensional list, which shifts inner array or list)
        /// </summary>
        public static IList<T> Shift<T>(this IList<T> list, int startIndex, int targetIndex)
        {
            if (startIndex == targetIndex || list == null || list.Count == 0)
            {
                return list;
            }

            IList<T> shiftedList = list.ToList();
            T value = shiftedList[startIndex];

            if (startIndex < targetIndex)
            {
                for (int i = startIndex; i < targetIndex; i++)
                {
                    shiftedList[i] = shiftedList[i + 1];
                }
            }
            else
            {
                for (int i = startIndex; i > targetIndex; i--)
                {
                    shiftedList[i] = shiftedList[i - 1];
                }
            }

            shiftedList[targetIndex] = value;

            return shiftedList;
        }

        /// <summary>
        /// Performs a cyclic shift operation on the elements of the specified list, positive values shift to the right, negative values shift to the left.
        /// Does not support multidimensional array (works for jagged array and multidimensional list, which shifts inner array or list)
        /// </summary>
        public static IList<T> CyclicShift<T>(this IList<T> list, int shiftAmount)
        {
            if (list == null || list.Count <= 1 || shiftAmount == 0)
            {
                return list;
            }

            int n = list.Count;
            int shift = shiftAmount % n;

            if (shift < 0)
            {
                shift += n;
            }

            if (shift == 0)
            {
                return list.ToList();
            }

            // By using IList<T>, we can handle both List and array inputs all at once
            IList<T> shiftedList = list.ToList();
            Reverse(shiftedList, 0, n - 1);
            Reverse(shiftedList, 0, shift - 1);
            Reverse(shiftedList, shift, n - 1);

            return shiftedList;
        }

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return list;
            }

            IList<T> shuffledList = list.ToList();
            int count = shuffledList.Count;

            // Given the count, we only need to shuffle count - 1 times to fully shuffle the list
            while (count > 1)
            {
                count--;
                int index = Random.Range(0, count + 1);

                T value = shuffledList[count];
                shuffledList[count] = shuffledList[index];
                shuffledList[index] = value;
            }

            return shuffledList;
        }

        private static void Reverse<T>(IList<T> list, int start, int end)
        {
            while (start < end)
            {
                T temp = list[start];
                list[start] = list[end];
                list[end] = temp;
                start++;
                end--;
            }
        }

        public static string ToDelimitedString<T>(this IList<T> list, int startIndex, int count, string delimiter = ", ")
        {
            // Ensure the startIndex is within a valid range.
            if (startIndex < 0 || startIndex >= list.Count)
                return string.Empty;

            // Ensure count is within a valid range.
            count = Math.Min(count, list.Count - startIndex);

            if (count <= 0)
                return string.Empty;

            // Use LINQ to select the desired subset of elements and join them with the delimiter.
            string result = string.Join(delimiter, list.Skip(startIndex).Take(count));

            return result;
        }
    }

}
