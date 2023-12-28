using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public static class VTGUILayout
    {
        public static void DrawTexture(Texture texture, float width, float height)
        {
            Rect r = GUILayoutUtility.GetRect(width, height);
            GUI.DrawTexture(r, texture);
        }

        public static bool VTButton(GUIContent content, bool addHoverEffect = false, float hoverSizeMultiplier = 1.065f, float pressSizeMultiplier = 0.935f)
        {
            Rect rect = GUILayoutUtility.GetRect(content, GUI.skin.label);
            return VTGUI.VTButton(rect, content, addHoverEffect, hoverSizeMultiplier, pressSizeMultiplier);
        }

        public static bool ClickableLabel(GUIContent content, GUIStyle style, params GUILayoutOption[] layoutOptions)
        {
            Rect rect = GUILayoutUtility.GetRect(content, style, layoutOptions);
            return VTGUI.ClickableLabel(rect, content, style);
        }

        public static void Label(GUIContent content, GUIStyle style, params GUILayoutOption[] layoutOptions)
        {
            Rect rect = GUILayoutUtility.GetRect(content, style, layoutOptions);
            VTGUI.Label(rect, content, style);
        }
    }
}

