using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTime
    {
        /// <summary>
        /// Automatically converts float in seconds to target time string
        /// </summary>
        /// <param name="t"></param>
        /// <param name="mainSeparator"></param>
        /// <returns></returns>
        public static string FloatToTimeAuto(float t, bool displayMilliseconds = false, int milliPrecision = 2, string mainSeparator = ":", string milliSeparator = ".")
        {
            if (t < 0)
            {
                t = 0f;
            }

            if (milliPrecision < 1 && displayMilliseconds)
            {
                milliPrecision = 1;
            }
            else if (milliPrecision > 9)
            {
                milliPrecision = 9;
            }

            bool needMinutes = false;
            bool needHours = false;

            int hoursPart = 0;
            int minutesPart = 0;
            int secondsPart = (int)t % 60;
            int milliPart = 0;

            string timeString = "";
            string milliString = "";
            if (t >= 60f)
            {
                minutesPart = (int)t / 60;
                needMinutes = true;
            }

            if (t >= 3600f)
            {
                hoursPart = (int)t / 3600;
                minutesPart = (int)(t - hoursPart * 3600) / 60;
                needHours = true;
            }

            // Optimization, we don't need to calculate milli part if we are not intend to show it
            if (displayMilliseconds)
            {
                int milliMultiplier = (int)Mathf.Pow(10, milliPrecision);
                milliPart = Mathf.FloorToInt((t * milliMultiplier) % milliMultiplier);
                milliString = milliPart.ToString($"D{milliPrecision}");
            }
            else
            {
                milliSeparator = "";
                milliString = "";
            }

            if (!needMinutes)
            {
                timeString = string.Format("0{0}{1:00}{2}{3}", mainSeparator, secondsPart, milliSeparator, milliString);
            }
            else if (!needHours && needMinutes)
            {
                timeString = string.Format("{0:0}{1}{2:00}{3}{4}", minutesPart, mainSeparator, secondsPart, milliSeparator, milliString);
            }
            else if (needHours)
            {
                timeString = string.Format("{0:0}{1}{2:00}{3}{4:00}{5}{6}", hoursPart, mainSeparator, minutesPart, mainSeparator, secondsPart, milliSeparator, milliString);
            }
            
            return timeString;
        }

        public static string FloatToTime(float t, bool hours = true, bool minutes = true, bool seconds = true, bool milliseconds = false)
        {
            // Parts follow leading hours or minutes
            float wholeTimeInHours = t / 3600;
            float wholeTimeInSeconds = (int)t;
            int leadingHours = (int)t / 3600;
            float leadingMinutes = (int)t / 60;
            int minutesPart = (int)(t - leadingHours * 3600) / 60;
            int secondsPart = (int)t % 60;
            int milliSeconds = Mathf.FloorToInt((t * 10) % 10);

            string timeString = "";

            if (hours && !minutes && !seconds && !milliseconds)
            {
                timeString = string.Format("0:00", wholeTimeInHours);
            }

            if (hours && minutes && !seconds && !milliseconds)
            {
                timeString = string.Format("{0:00}:{1:00}", leadingHours, minutesPart);
            }

            if (hours && minutes && seconds)
            {
                if (milliseconds)
                {
                    timeString = string.Format("{0:00}:{1:00}:{2:00}:{3:D2}", leadingHours, minutesPart, secondsPart, milliSeconds);
                }
                else
                {
                    timeString = string.Format("{0:00}:{1:00}:{2:00}", leadingHours, minutesPart, secondsPart);
                }
            }

            if (!hours && minutes && seconds)
            {
                if (milliseconds)
                {
                    timeString = string.Format("{0:00}:{1:00}:{2:D2}", leadingMinutes, secondsPart, milliSeconds);
                }
                else
                {
                    timeString = string.Format("{0:00}:{1:00}", leadingMinutes, secondsPart);
                }
            }

            if (hours && minutes && !seconds)
            {
                timeString = string.Format("{0:00}:{1:00}", leadingHours, minutesPart);
            }

            if (!hours && minutes && !seconds)
            {
                timeString = string.Format("{0:00}", leadingMinutes);
            }

            if (!hours && !minutes && seconds)
            {
                if (milliseconds)
                {
                    timeString = string.Format("{0:00}:{1:D2}", wholeTimeInSeconds, milliSeconds);
                }
                else
                {
                    timeString = string.Format("{0:00}", wholeTimeInSeconds);
                }
            }

            return timeString;
        }
    }
}

