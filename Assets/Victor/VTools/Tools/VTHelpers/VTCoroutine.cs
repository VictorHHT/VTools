using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTCoroutine
    {
        /// <summary>
        /// Wait a certain amount of time in seconds with a second parameter specifying unscaled time or not
        /// Use 1: yield return VTCoroutine.WaitSeconds(2);
        /// Use 2: yield return VTCoroutine.WaitSeconds(3.5f, false);
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IEnumerator WaitSeconds(float seconds, bool unscaledTime = true)
        {
            if (unscaledTime)
            {
                yield return new WaitForSecondsRealtime(seconds);
            }
            else
            {
                yield return new WaitForSeconds(seconds);
            }
        }

        /// <summary>
        /// Wait a certain amount of frames
        /// Use 1: yield return VTCoroutine.WaitFrames(600);
        /// </summary>
        /// <param name="frameCount"></param>
        /// <returns></returns>
        public static IEnumerator WaitFrames(int frameCount)
        {
            while (frameCount-- > 0)
            {
                yield return null;
            }
        }


        
    }

}
