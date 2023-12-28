using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTIEnumerableExtension
    {
        public static string ToDelimitedString<T>(this IEnumerable<T> collection, string delimiter = ", ")
        {
            return string.Join(delimiter, collection);
        }

        public static int CountRecursively(this IEnumerable e)
        {
            int count = 0;

            foreach (object element in e)
            {
                var subCollection = element as IEnumerable;
                if (subCollection != null)
                    count += CountRecursively(subCollection);
                else
                    count++;
            }

            return count;
        }
    }
}
