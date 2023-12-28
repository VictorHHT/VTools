using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTFloatExtension
    {
        public static bool InsideRange(this float number, float leftBound, float rightBound)
        {
            if (number >= leftBound && number <= rightBound)
            {
                return true;
            }

            return false;
        }
    }
}
