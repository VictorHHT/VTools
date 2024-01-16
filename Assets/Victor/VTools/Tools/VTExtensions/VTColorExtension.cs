using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Victor.Tools
{
	public static class VTColorExtension
	{
        // Because in Unity the type of Color is struct, so these extension methods only modify the copy within the scope of this method, the original value that is passed in remains unchanged

        /// <summary>
        /// Modifies the red component of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">New value for the red component.</param>
        /// <returns>New Color with the modified red component.</returns>
        public static Color NewR(this Color color, float value)
		{
            color.r = value;
            return color;
		}

        /// <summary>
        /// Modifies the green component of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">New value for the green component.</param>
        /// <returns>New Color with the modified green component.</returns>
        public static Color NewG(this Color color, float value)
		{
            color.g = value;
            return color;
		}

        /// <summary>
        /// Modifies the blue component of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">New value for the blue component.</param>
        /// <returns>New Color with the modified blue component.</returns>
        public static Color NewB(this Color color, float value)
		{
            color.b = value;
            return color;
		}

        /// <summary>
        /// Modifies the hue of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">New value for the hue.</param>
        /// <returns>New Color with the modified hue.</returns>
        public static Color NewH(this Color color, float value)
        {
            float s;
            float v;

            Color.RGBToHSV(color, out _, out s, out v);
            Color newColor = Color.HSVToRGB(value, s, v);
            newColor.a = color.a;
            return newColor;
        }

        /// <summary>
        /// Modifies the saturation of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">New value for the saturation.</param>
        /// <returns>New Color with the modified saturation.</returns>
        public static Color NewS(this Color color, float value)
        {
            float h;
            float v;

            Color.RGBToHSV(color, out h, out _, out v);
            Color newColor = Color.HSVToRGB(h, value, v);
            newColor.a = color.a;
            return newColor;
        }

        /// <summary>
        /// Modifies the brightness of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">New value for the brightness.</param>
        /// <returns>New Color with the modified brightness.</returns>

        public static Color NewV(this Color color, float value)
        {
            float h;
            float s;

            Color.RGBToHSV(color, out h, out s, out _);
            Color newColor = Color.HSVToRGB(h, s, value);
            newColor.a = color.a;
            return newColor;
        }

        /// <summary>
        /// Modifies the alpha component of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">New value for the alpha component.</param>
        /// <returns>New Color with the modified alpha component.</returns>
        public static Color NewA(this Color color, float value)
        {
            color.a = value;
            return color;
        }

        /// <summary>
        /// Multiplies the hue of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">Multiplication factor for the hue.</param>
        /// <returns>New Color with the multiplied hue.</returns>
        public static Color MultH(this Color color, float value)
        {
            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);
            h *= value;
            Color newColor = Color.HSVToRGB(h, s, v);
            newColor.a = color.a;
            return newColor;
        }

        /// <summary>
        /// Multiplies the saturation of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">Multiplication factor for the saturation.</param>
        /// <returns>New Color with the multiplied saturation.</returns>
        public static Color MultS(this Color color, float value)
        {
            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);
            s *= value;
            Color newColor = Color.HSVToRGB(h, s, v);
            newColor.a = color.a;
            return newColor;
        }

        /// <summary>
        /// Multiplies the brightness of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">Multiplication factor for the brightness.</param>
        /// <returns>New Color with the multiplied brightness.</returns>
        public static Color MultV(this Color color, float value)
        {
            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);
            v *= value;
            Color newColor = Color.HSVToRGB(h, s, v);
            newColor.a = color.a;
            return newColor;
        }

        /// <summary>
        /// Multiplies the alpha component of the color.
        /// </summary>
        /// <param name="color">Original Color to modify.</param>
        /// <param name="value">Multiplication factor for the alpha component.</param>
        /// <returns>New Color with the multiplied alpha component.</returns>
        public static Color MultA(this Color color, float value)
        {
            float newA = color.a * value;
            color.a = newA;
            return color;
        }

        /// <summary>
        /// Calculates the perceived luminance of the color.
        /// </summary>
        /// <param name="color">Original Color to calculate luminance for.</param>
        /// <returns>Perceived luminance value.</returns>
        public static float PerceivedLuminance(this Color color)
        {
            color = color.linear;
            return Mathf.LinearToGammaSpace(0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b);
        }
	}

}

