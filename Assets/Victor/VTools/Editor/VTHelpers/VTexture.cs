using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTexture
    {
        private static readonly Dictionary<Color, Texture2D> s_PixelsDict;

        static VTexture()
        {
            s_PixelsDict = new Dictionary<Color, Texture2D>();
        }

        public static Texture2D GetPixel(this Color color)
        {
            if(!s_PixelsDict.ContainsKey(color))
            {
                var pixel = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                pixel.SetPixel(0, 0, color);
                pixel.hideFlags = HideFlags.HideAndDontSave;
                pixel.filterMode = FilterMode.Point;
                pixel.Apply();
                s_PixelsDict.Add(color, pixel);
            }

            return s_PixelsDict[color];
        }

        public static Texture2D GetPixel(this VTSkinnedColor skinnedColor)
        {
            return skinnedColor.color.GetPixel();
        }

        public static Texture2D CreateBox(Color fill, Color border)
        {
            var box = new Texture2D(3, 3, TextureFormat.ARGB32, false);

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    box.SetPixel(i, j, border);
                }
            }

            box.SetPixel(1, 1, fill);
            box.hideFlags = HideFlags.HideAndDontSave;
            box.filterMode = FilterMode.Point;
            box.Apply();
            return box;
        }

        public static Texture2D CreateRetinaBox(Color fill, Color border)
        {
            var box = new Texture2D(6, 6, TextureFormat.ARGB32, false);

            for (var i = 0; i < 6; i++)
            {
                for (var j = 0; j < 6; j++)
                {
                    box.SetPixel(i, j, border);
                }
            }

            for (var i = 1; i < 5; i++)
            {
                for (var j = 1; j < 5; j++)
                {
                    box.SetPixel(i, j, fill);
                }
            }

            box.hideFlags = HideFlags.HideAndDontSave;
            box.filterMode = FilterMode.Point;
            box.Apply();
            return box;
        }

        public static GUIStyle CreateBackground(this Color color)
        {
            var background = new GUIStyle();
            background.normal.background = color.GetPixel();
            return background;
        }

        public static GUIStyle CreateBackground(this VTSkinnedColor skinnedColor)
        {
            return skinnedColor.color.CreateBackground();
        }

        public static void SetTex2DColor(Texture2D texture, Color color)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }
}
