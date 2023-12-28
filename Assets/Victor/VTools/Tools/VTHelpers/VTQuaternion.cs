using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTQuaternion
    {
        /// <summary>
        /// Checks if two Quaternions are approximately equal within a specified maximum angle difference.
        /// </summary>
        /// <param name="a">The first Quaternion.</param>
        /// <param name="b">The second Quaternion.</param>
        /// <param name="maxAngleDifference">The maximum allowable angle difference for equality.</param>
        /// <returns>True if approximately equal, false otherwise.</returns>
        public static bool Approximately(Quaternion a, Quaternion b, float maxAngleDifference = 0.0001f)
        {
            float angle = Quaternion.Angle(a, b);
            return Mathf.Abs(angle) <= maxAngleDifference;
        }
    }
}
