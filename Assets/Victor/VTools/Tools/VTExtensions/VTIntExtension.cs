using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTIntExtension
    {
        public static bool InsideRange(this int number, int leftBound, int rightBound, bool rightExclusive = true)
        {
            if (!rightExclusive)
            {
                if (number >= leftBound && number <= rightBound)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                if (number >= leftBound && number < rightBound)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
