using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
	public static class VTColor
	{
		public static Color Gray(float brightness)
		{
			return new Color(brightness, brightness, brightness);
		}

		/// <summary>
		/// Generate random color in HSV space, allowing to choose whether to use the Hue arc that pass 0
		/// </summary>
		/// <param name="minHSV">H in [0...360], S in [0...100] V in [0...100], alpha in [0...100]</param>
		/// <param name="maxHSV"></param>
		/// <param name="alongArcPasses0"></param>
		/// <param name="alpha"></param>
		/// <returns></returns>
		public static Color RandomHSVColor(Vector3 minHSV, Vector3 maxHSV, bool alongArcPasses0 = false, float alpha = 100f)
		{
			Color resultColor;
			if (!VTVector2.LessThanOrEqualTo(minHSV, maxHSV))
			{
				Debug.LogWarning("Generating RandomHSVColor Fails because left bound is not strictly less than right bound");
				return Color.white;
			}

			if (!minHSV.x.InsideRange(0, 360f) || !maxHSV.x.InsideRange(0, 360f))
			{
				Debug.LogWarning("Min and Max Hue must reside in 0 and 360f range");
				return Color.white;
			}

			if (!minHSV.y.InsideRange(0, 100f) || !maxHSV.y.InsideRange(0, 100f))
            {
				Debug.LogWarning("Min and Max Saturation must reside in 0 and 100f range");
				return Color.white;
			}

            if (!minHSV.z.InsideRange(0, 100f) || !maxHSV.z.InsideRange(0, 100f))
            {
				Debug.LogWarning("Min and Max Value must reside in 0 and 100f range");
				return Color.white;
			}

            // Unity only accepts HSV of the range (0 - 1.0f)
            if (!alongArcPasses0)
            {
				resultColor = Color.HSVToRGB(Random.Range(minHSV.x, maxHSV.x) / 360f, Random.Range(minHSV.y, maxHSV.y) / 100f, Random.Range(minHSV.z, maxHSV.z) / 100f);
			}
			else
            {
				// Because the range is being split into two seperate range, we need to choose one fifty fifty
				bool getFromleftRange = VTMath.Chance(50);
                if (getFromleftRange)
                {
					resultColor = Color.HSVToRGB(Random.Range(0, minHSV.x) / 360f, Random.Range(minHSV.y, maxHSV.y) / 100f, Random.Range(minHSV.z, maxHSV.z) / 100f);
				}
				else
                {
					resultColor = Color.HSVToRGB(Random.Range(maxHSV.x, 360f) / 360f, Random.Range(minHSV.y, maxHSV.y) / 100f, Random.Range(minHSV.z, maxHSV.z) / 100f);
				}					
				
			}
			
			resultColor.a = alpha / 100f;
			return resultColor;
        }

        /// <summary>
        /// Convert a Hexdecimal color into RGB(RGBA) color format 
        /// with a second parameter of the color alpha channel (0-1)</summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
        {
            if (hex[0] == '#')
            {
				hex = hex.Substring(1);
            }

			bool isShortFormat = hex.Length < 6 ? true : false;

            if (isShortFormat)
            {
				float r = (HexToInt(hex[0]) + HexToInt(hex[0]) * 16) / 255f;
				float g = (HexToInt(hex[1]) + HexToInt(hex[1]) * 16) / 255f;
				float b = (HexToInt(hex[2]) + HexToInt(hex[2]) * 16) / 255f;
				float a = hex.Length == 4 ? (HexToInt(hex[3]) + HexToInt(hex[3]) * 16) / 255f : 1;
				return new Color(r, g, b, a);
			}
			else
            {
				// We build numbers from right to left
				float r = (HexToInt(hex[1]) + HexToInt(hex[0]) * 16) / 255f;
				float g = (HexToInt(hex[3]) + HexToInt(hex[2]) * 16) / 255f;
				float b = (HexToInt(hex[5]) + HexToInt(hex[4]) * 16) / 255f;
				float a = hex.Length == 8 ? (HexToInt(hex[7]) + HexToInt(hex[6]) * 16) / 255f : 1;
				return new Color(r, g, b, a);
			}
        }

		public static string ColorToHex(Color color)
		{
            int r = Mathf.RoundToInt(color.r * 255f);
            int g = Mathf.RoundToInt(color.g * 255f);
            int b = Mathf.RoundToInt(color.b * 255f);
            int a = Mathf.RoundToInt(color.a * 255f);
            string hex = r.ToString("X2") + g.ToString("X2") + b.ToString("X2");

            if (a != 255)
            {
                hex += a.ToString("X2");
            }

            return hex;
        }

        private static int HexToInt(char hex)
		{
			return int.Parse(hex.ToString(), System.Globalization.NumberStyles.HexNumber);
		}
	}
}

