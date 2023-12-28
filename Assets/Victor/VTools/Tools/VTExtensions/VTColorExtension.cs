using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Victor.Tools
{
	public static class VTColorExtension
	{
		public static Color NewR(this Color color, float value)
		{
            color.r = value;
            return color;
		}

		public static Color NewG(this Color color, float value)
		{
            color.g = value;
            return color;
		}

		public static Color NewB(this Color color, float value)
		{
            color.b = value;
            return color;
		}

        /// <summary>
        /// Return a new color with hue set to value
        /// </summary>
        /// <param name="color"></param>
        /// <param name="value">[0...1]</param>
        /// <returns></returns>
        public static Color NewH(this Color color, float value)
        {
            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);
            h = value;
            return Color.HSVToRGB(h, s, v).NewA(color.a);
        }

        public static Color NewS(this Color color, float value)
        {
            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);
            s = value;
            return Color.HSVToRGB(h, s, v).NewA(color.a);
        }

        public static Color NewV(this Color color, float value)
        {
            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);
            v = value;
            return Color.HSVToRGB(h, s, v).NewA(color.a);
        }

        public static Color NewA(this Color color, float value)
        {
            color.a = value;
            return color;
        }

        public static Color MultH(this Color color, float value)
        {
            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);
            h *= value;
            return Color.HSVToRGB(h, s, v).NewA(color.a);
        }

        public static Color MultS(this Color color, float value)
        {
            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);
            s *= value;
            return Color.HSVToRGB(h, s, v).NewA(color.a);
        }

        public static Color MultV(this Color color, float value)
        {
            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);
            v *= value;
            return Color.HSVToRGB(h, s, v).NewA(color.a);
        }

        public static Color MultA(this Color color, float value)
        {
            float newA = color.a * value;
            color.a = newA;
            return color;
        }

		public static float PerceivedLuminance(this Color color)
        {
            color = color.linear;
            return Mathf.LinearToGammaSpace(0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b);
        }
	}

}

