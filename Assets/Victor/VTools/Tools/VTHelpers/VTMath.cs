using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace Victor.Tools
{	
    public static class VTMath
    {
        /// <summary>
        /// Rounds a floating-point value to a specified number of decimal points.
        /// </summary>
        /// <param name="value">The value to be rounded.</param>
        /// <param name="decimalPoints">The number of decimal points to round to.</param>
        /// <returns>The rounded value.</returns>
        public static float Round(float value, int decimalPoints)
        {
            float multiplier = Mathf.Pow(10f, decimalPoints);
            return Mathf.Round(value * multiplier) / multiplier;
        }

        /// <summary>
        /// Round a value to the nearest multiple of the specified snap interval.
        /// </summary>
        /// <param name="value">The value to be rounded.</param>
        /// <param name="snapInterval">The interval to which the value should be snapped.</param>
        /// <returns>The rounded value.</returns>
        public static float Snap(float value, float snapInterval)
        {
            return Mathf.Round(value / snapInterval) * snapInterval;
        }

        /// <summary>
        /// Round an integer value to the nearest multiple of the specified snap interval.
        /// </summary>
        /// <param name="value">The integer value to be rounded.</param>
        /// <param name="snapInterval">The interval to which the value should be snapped.</param>
        /// <returns>The rounded integer value.</returns>
        public static int Snap(int value, int snapInterval)
        {
            return Mathf.RoundToInt((float)value / snapInterval) * snapInterval;
        }

        /// <summary>
        /// Rounds a floating-point value up to the nearest multiple of the specified interval.
        /// </summary>
        /// <param name="value">The value to be rounded.</param>
        /// <param name="interval">The interval to which the value should be rounded up.</param>
        /// <returns>The rounded value.</returns>
        public static float CeilToInterval(float value, float interval)
        {
            value += interval * 0.51f;
            value = Snap(value, interval);
            return value;
        }

        /// <summary>
        /// Floors a floating-point value to the nearest multiple of the specified interval.
        /// </summary>
        /// <param name="value">The value to be floored.</param>
        /// <param name="interval">The interval to which the value should be floored.</param>
        /// <returns>The floored value.</returns>
        public static float FloorToInterval(float value, float interval)
        {
            value -= interval * 0.51f;
            value = Snap(value, interval);
            return value;
        }

        /// <summary>
        /// Checks if two float values are approximately equal within a given tolerance.
        /// </summary>
        /// <param name="a">The first float value.</param>
        /// <param name="b">The second float value.</param>
        /// <param name="tolerance">The allowable tolerance for equality.</param>
        /// <returns>True if approximately equal, false otherwise.</returns>
        public static bool Approximately(float a, float b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a - b) <= tolerance;
        }

        /// <summary>
        /// Determines the precision of a floating-point number.
        /// </summary>
        /// <param name="f">The floating-point number.</param>
        /// <returns>The number of decimal places in the input number, up to a maximum of 7.</returns>
        public static int Precision(float f)
        {
            string str = f.ToString();

            int decimalPointPos = str.IndexOf('.');

            if (decimalPointPos < 0)
            {
                return 0;
            }

            int decimalPlaces = str.Length - decimalPointPos - 1;

            return Mathf.Min(decimalPlaces, 7);
        }

        /// <summary>
        /// Returns the closest power of ten to the given positive number.
        /// If we want to change the value in range R by approximately 1/100, we could do the following
        /// value = GetClosestPowerOfTen(Mathf.Abs(R.max - R.min) * 0.01f)
        /// So if R.max is 120 and R.min is 0, each step will change the value by 1 (not 1.2),
        /// if multiply by 0.1f instead, the result value will be 10
        /// </summary>
        /// <param name="positiveNumber">The positive number for which to find the closest power of ten.</param>
        /// <returns>The closest power of ten to the input positive number.</returns>
        public static float GetClosestPowerOfTen(float positiveNumber)
        {
            if (positiveNumber <= 0)
                return 1;
            return Mathf.Pow(10, Mathf.RoundToInt(Mathf.Log10(positiveNumber)));
        }

        /// <summary>
        /// Clamps a float value between specified minimum and maximum bounds, with optional clamping for each bound.
        /// </summary>
        /// <param name="value">The float value to be clamped.</param>
        /// <param name="min">The minimum bound. If clampMin is true, the value will not go below this bound.</param>
        /// <param name="max">The maximum bound. If clampMax is true, the value will not exceed this bound.</param>
        /// <param name="clampMin">Flag indicating whether to clamp the value to the minimum bound.</param>
        /// <param name="clampMax">Flag indicating whether to clamp the value to the maximum bound.</param>
        /// <returns>The clamped float value.</returns>
        public static float Clamp(float value, float min, float max, bool clampMin, bool clampMax)
        {
            float returnValue = value;

            // Clamp to the minimum bound if clampMin is true
            if (clampMin && (returnValue < min))
            {
                returnValue = min;
            }

            // Clamp to the maximum bound if clampMax is true
            if (clampMax && (returnValue > max))
            {
                returnValue = max;
            }

            return returnValue;
        }

        /// <summary>
        /// Return the result of rolling a 6 sided dice of the specified number of times
        /// </summary>
        /// <returns>The result of the dice roll.</returns>
        /// <param name="numberOfSides">Number of sides of the dice.</param>
        public static int RollADice(int numberOfTimes)
        {
            int result = 0;

            for(int i = 0; i < numberOfTimes; i++)
            {
                result += UnityEngine.Random.Range(1, 6 + 1);
            }

            return result;
        }

        /// <summary>
        /// Return a random success based on X% of chance.
        /// Example : I have 20% of chance to do X, Chance(20) > true, yay!
        /// </summary>
        /// <param name="percent">Percent of chance.</param>
        public static bool Chance(int percent)
        {
            return UnityEngine.Random.Range(0,100) <= percent;
        }

        /// <summary>
        /// Moves from "from" to "to" by the specified amount and returns the corresponding value
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="amount">Amount.</param>
        public static float Approach(float from, float to, float amount)
        {
            if (from < to)
            {
                from += amount;
                if (from > to)
                {
                    return to;
                }
            }
            else
            {
                from -= amount;
                if (from < to)
                {
                    return to;
                }
            }
            return from;
        }

        /// <summary>
        /// Remaps a value 'x' from the interval [A, B] to its proportional value in the target interval [C, D].
        /// </summary>
        /// <param name="x">The value to remap.</param>
        /// <param name="A">The minimum bound of the source interval [A, B] that contains the 'x' value.</param>
        /// <param name="B">The maximum bound of the source interval [A, B] that contains the 'x' value.</param>
        /// <param name="C">The minimum bound of the target interval [C, D].</param>
        /// <param name="D">The maximum bound of the target interval [C, D].</param>
        /// <returns>The remapped value in the target interval [C, D].</returns>
        public static float Remap(float x, float A, float B, float C, float D)
        {
            // Formula for remapping the value proportionally to the target interval
            float remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }

        /// <summary>
        /// Clamp the angle in parameters between a minimum and maximum angle (all angles expressed in degrees)
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="minimumAngle"></param>
        /// <param name="maximumAngle"></param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float minimumAngle, float maximumAngle)
        {
            if (angle < -360)
            {
                angle += 360;
            }

            if (angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, minimumAngle, maximumAngle);
        }


        /// <summary>
        /// Wraps the angle difference between angleA and angleB (B - A) to the range between -180 to +180 degrees.
        /// </summary>
        /// <param name="angle">The angle in degrees to be wrapped.</param>
        /// <returns>The wrapped angle in the range between -180 to +180 degrees.</returns>
        public static float WrapPI(float angle)
        {
            if (Mathf.Abs(angle) > Mathf.PI * Mathf.Rad2Deg)
            {
                float revolution = Mathf.Floor(((angle + Mathf.PI * Mathf.Rad2Deg) / (Mathf.PI * 2 * Mathf.Rad2Deg)));
                angle -= Mathf.PI * 2 * Mathf.Rad2Deg * revolution;
            }

            return angle;
        }

        /// <summary>
        /// Wraps the angular difference between two given angles (B - A) to the range between -180 to +180 degrees.
        /// This function determines how to move from angle A to angle B, considering clockwise or counter-clockwise direction.
        /// </summary>
        /// <param name="angleA">The starting angle in degrees.</param>
        /// <param name="angleB">The target angle in degrees.</param>
        /// <returns>The wrapped angular difference between angleA and angleB in the range -180 to +180 degrees.</returns>
        public static float WrapPI(float angleA, float angleB)
        {
            float difference = angleB - angleA;
            return WrapPI(difference);
        }

        /// <summary>
        /// Wraps an angle to the range [0, 360) degrees, ensuring cyclic behavior.
        /// </summary>
        /// <param name="angleToWrap">The angle to be wrapped.</param>
        /// <returns>The wrapped angle within the range [0, 360) degrees.</returns>
        public static float WrapAngle(float angleToWrap)
        {
            // Use modulo to wrap the angle within the range [0, 360)
            return (angleToWrap % 360.0f + 360.0f) % 360.0f;
        }

        /// <summary>
        /// Finds the mid angle between two angles along the smaller arc.
        /// </summary>
        /// <param name="angleA">The first angle.</param>
        /// <param name="angleB">The second angle.</param>
        /// <returns>The mid angle along the smaller arc between the two input angles.</returns>
        public static float SmallerArcMidAngle(float angleA, float angleB)
        {
            float smallerMidAngle;
            float wrappedAngleA = WrapPI(angleA);
            float wrappedAngleB = WrapPI(angleB);

            // Meanning the two angles are in the second and third section of the number axis, where need to take separately
            if (Mathf.Abs(wrappedAngleA - wrappedAngleB) > 180)
            {
                float positiveDifference = Mathf.Sign(wrappedAngleA) == 1 ? 180 - wrappedAngleA : 180 - wrappedAngleB;
                float negativeDifference = Mathf.Sign(wrappedAngleA) == -1 ? Mathf.Abs(-180 - wrappedAngleA) : Mathf.Abs(-180 - wrappedAngleB);

                float midRange = (positiveDifference + negativeDifference) / 2;

                if (Mathf.Sign(wrappedAngleA) == 1)
                {
                    smallerMidAngle = midRange + wrappedAngleA;
                }
                else
                {
                    smallerMidAngle = midRange + wrappedAngleB;
                }
            }
            else
            {
                smallerMidAngle = (wrappedAngleA + wrappedAngleB) / 2;
            }

            smallerMidAngle = WrapPI(smallerMidAngle);
            return smallerMidAngle;
        }

        /// <summary>
        /// Rounds a given value to the closest value among an array of possible candidates.
        /// </summary>
        /// <param name="valueToRound">The value to be rounded.</param>
        /// <param name="candidates">An array of potential rounded values.</param>
        /// <param name="preferSmallestDistance">
        ///   If true, selects the candidate with the smallest absolute distance in case of a tie.
        ///   If false, selects the candidate with the largest absolute distance in case of a tie.
        /// </param>
        /// <returns>The rounded value selected from the array of candidates.</returns>
        public static float RoundToClosest(float valueToRound, float[] candidates, bool preferSmallestDistance = false)
        {
            // Return 0 if the array of candidates is empty
            if (candidates.Length == 0)
            {
                return 0f;
            }

            // Initialize with the first candidate in the array
            float closestCandidate = candidates[0];

            foreach (float candidate in candidates)
            {
                float currentClosestDistance = Mathf.Abs(closestCandidate - valueToRound);
                float candidateDistance = Mathf.Abs(candidate - valueToRound);

                // Update closestCandidate if the current candidate is closer
                if (currentClosestDistance > candidateDistance)
                {
                    closestCandidate = candidate;
                }
                // Handle tie-breaking situations
                else if (currentClosestDistance == candidateDistance)
                {
                    // Choose the smaller or larger candidate based on the tie-breaking rule
                    if ((preferSmallestDistance && closestCandidate > candidate) || (!preferSmallestDistance && closestCandidate < candidate))
                    {
                        closestCandidate = (valueToRound < 0) ? closestCandidate : candidate;
                    }
                }
            }

            return closestCandidate;
        }
    }
}