using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public static class VTGUIUtility
    {
        private static readonly GUIContent tempContent = new GUIContent();

        public static GUIContent TempContent(string text, Texture image, string tooltip)
        {
            tempContent.text = text;
            tempContent.image = image;
            tempContent.tooltip = tooltip;
            return tempContent;
        }

        public static GUIContent TempContent(string text, Texture image)
        {
            tempContent.text = text;
            tempContent.image = image;
            tempContent.tooltip = null;
            return tempContent;
        }

        public static GUIContent TempContent(string text)
        {
            tempContent.text = text;
            tempContent.image = null;
            tempContent.tooltip = null;
            return tempContent;
        }

        public static GUIContent TempContent(Texture image)
        {
            tempContent.text = null;
            tempContent.image = image;
            tempContent.tooltip = null;
            return tempContent;
        }

        public static GUIContent TempContent(Texture image, string tooltip)
        {
            tempContent.text = null;
            tempContent.image = image;
            tempContent.tooltip = tooltip;
            return tempContent;
        }

        public static GUIContent TempContent(string text, string tooltip)
        {
            tempContent.text = text;
            tempContent.image = null;
            tempContent.tooltip = tooltip;
            return tempContent;
        }

        public static Rect ExpandBy(this Rect rect, float padding)
        {
            rect.xMin -= padding;
            rect.yMin -= padding;
            rect.xMax += padding;
            rect.yMax += padding;
            return rect;
        }

        public static Rect ExpandBy(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.x -= offset.left;
            rect.y -= offset.top;
            rect.width += offset.left + offset.right;
            rect.height += offset.top + offset.bottom;
            return rect;
        }

        public static Rect ShrinkBy(this Rect rect, float padding)
        {
            return rect.ExpandBy(-padding);
        }

        public static Rect ShrinkBy(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.x += offset.left;
            rect.y += offset.top;
            rect.width -= offset.left + offset.right;
            rect.height -= offset.top + offset.bottom;
            return rect;
        }

        public static Rect ExpandByX(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.x -= offset.left;
            rect.width += offset.left + offset.right;
            return rect;
        }

        public static Rect ShrinkByX(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.x += offset.left;
            rect.width -= offset.left + offset.right;
            return rect;
        }

        public static Rect ExpandByY(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.y -= offset.top;
            rect.height += offset.top + offset.bottom;
            return rect;
        }

        public static Rect ShrinkByY(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.y += offset.top;
            rect.height -= offset.top + offset.bottom;
            return rect;
        }

        public static Rect ScaleAroundCenter(this Rect rect, float scale)
        {
            var center = rect.center;
            var size = rect.size * scale;
            rect.min = center - size / 2;
            rect.max = center + size / 2;
            return rect;
        }

        public static bool IsFocused(this EditorWindow window)
        {
            return EditorWindow.focusedWindow == window;
        }

        public static bool CtrlOrCmd(this Event e)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return e.command;
            }

            return e.control;
        }
    }
}
