using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public static class VTRect
    {
        /// <summary>
        /// Add a simple button effect to the rect that we are interacting with
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="e"></param>
        /// <param name="pressingDown"></param>
        /// <param name="hoverSizeMultiplier"></param>
        /// <param name="pressSizeMultiplier"></param>
        /// <param name="addHover"></param>
        /// <returns></returns>
        public static Rect AddSimpleButtonEffect(Rect rect, Event e , bool pressingDown, float hoverSizeMultiplier, float pressSizeMultiplier, bool addHover = false)
        {
            bool hovering = rect.Contains(e.mousePosition);
            if (GUI.enabled && addHover && !pressingDown && hovering)
            {
                Vector2 originalSize = rect.size;
                rect.size = rect.size * hoverSizeMultiplier;
                rect.position -= (rect.size - originalSize) * 0.5f;
            }
            else if (GUI.enabled && pressingDown)
            {
                Vector2 originalSize = rect.size;
                rect.size = rect.size * pressSizeMultiplier;
                rect.position -= (rect.size - originalSize) * 0.5f;
            }
            return rect;
        }
    }

}
