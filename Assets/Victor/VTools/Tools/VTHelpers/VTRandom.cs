using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    // All random stuff will go here, no matter its random position, rotation, color, etc.
    public static class VTRandom
    {
         /// <summary>
        /// Generates a random Vector2 within the specified range.
        /// </summary>
        /// <param name="minVec">The minimum values for each component.</param>
        /// <param name="maxVec">The maximum values for each component.</param>
        /// <returns>A randomly generated Vector2.</returns>
        public static Vector2 RandomV2(Vector2 minVec, Vector2 maxVec)
        {
            return new Vector2(Random.Range(minVec.x, maxVec.x), Random.Range(minVec.y, maxVec.y));
        }

        /// <summary>
        /// Generates a random Vector3 within the specified range.
        /// </summary>
        /// <param name="minVec">The minimum values for each component.</param>
        /// <param name="maxVec">The maximum values for each component.</param>
        /// <returns>A randomly generated Vector3.</returns>
        public static Vector3 RandomV3(Vector3 minVec, Vector3 maxVec)
        {
            return new Vector3(Random.Range(minVec.x, maxVec.x), Random.Range(minVec.y, maxVec.y), Random.Range(minVec.z, maxVec.z));
        }

        public static Vector2 RandomPointOnCircle(float circleRadius, Vector2 circleOrigin)
        {
            return Random.insideUnitCircle.normalized * circleRadius + circleOrigin;
        }

        public static Vector2 RandomPointInsideCircle(float circleRadius, Vector2 circleOrigin)
        {
            return Random.insideUnitCircle.normalized * Random.Range(0, circleRadius) + circleOrigin;
        }

        public static Vector2 RandomPointInsideRing(float innerRadius, float outerRaidus, Vector2 ringOrigin)
        {
            if (innerRadius < outerRaidus)
            {
                return Random.insideUnitCircle.normalized * Random.Range(innerRadius, outerRaidus) + ringOrigin;
            }
            else
            {
                return Random.insideUnitCircle.normalized * Random.Range(outerRaidus, innerRadius) + ringOrigin;
            }
        }

        public static Vector2 RandomPointInsideRectangle(float width, float height, Vector2 rectangleOrigin)
        {
            float halfWidth = width / 2;
            float halfHeight = height / 2;
            return new Vector2(Random.Range(-halfWidth, halfWidth), Random.Range(-halfHeight, halfHeight)) + rectangleOrigin;
        }

        public static int RandomExclude(int leftBoundInclusive, int rightBoundExclusive, int[] excludeNumbers, int adjustAmount = 1)
        {
            int resultNumber = Random.Range(leftBoundInclusive, rightBoundExclusive);
            List<int> excludeNumbersList = excludeNumbers.ToList();
            int loops = 0;
            bool shouldAdd = VTMath.Chance(50);
            do
            {

                if (shouldAdd)
                {
                    resultNumber += adjustAmount;

                    if (resultNumber >= rightBoundExclusive)
                    {
                        resultNumber = rightBoundExclusive;
                    }
                }
                else
                {
                    resultNumber -= adjustAmount;

                    if (resultNumber <= leftBoundInclusive)
                    {
                        resultNumber = leftBoundInclusive;
                    }
                }
                loops++;
            }
            while (excludeNumbersList.Contains(resultNumber));
            Debug.Log($"Loops {loops} times");
            return resultNumber;
        }

    }
}

