using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTLayer
    {
        /// <summary>
        /// Returns all the indexes of the given LayerMask, 
        /// 0 represents EveryThing
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static int[] LayerMaskDetail(LayerMask mask)
        {
            List<int> detail = new List<int>();
            int exponentOf2;
            int remaining = mask.value;

            if (mask.value == int.MaxValue)
            {
                detail.Add(0);
                return detail.ToArray();
            }

            while (remaining >= 1)
            {
                exponentOf2 = Mathf.FloorToInt(Mathf.Log(remaining, 2));
                detail.Insert(0, exponentOf2);
                int layerIndex = (int)Mathf.Pow(2, exponentOf2);
                remaining -= layerIndex;                
            }

            if (remaining == 1)
            {
                detail.Insert(1, 0);
            }

            return detail.ToArray();
        }
    }
}