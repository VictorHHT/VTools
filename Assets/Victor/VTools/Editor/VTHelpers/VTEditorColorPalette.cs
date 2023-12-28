using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    [InitializeOnLoad]
    public static class VTEditorColorPalette
    {
        public static VTSkinnedColor unityBackgroundVeryDark = new VTSkinnedColor(VTColor.Gray(0.33f), VTColor.Gray(0.10f));
        public static VTSkinnedColor unityBackgroundDark = new VTSkinnedColor(VTColor.Gray(0.64f), VTColor.Gray(0.16f));
        public static VTSkinnedColor unityBackgroundMid = new VTSkinnedColor(VTColor.Gray(0.76f), VTColor.Gray(0.22f));
        public static VTSkinnedColor unityBackgroundLight = new VTSkinnedColor(VTColor.Gray(0.87f), VTColor.Gray(0.24f));
        public static VTSkinnedColor unityBackgroundLighter = new VTSkinnedColor(VTColor.Gray(0.87f * 1.1f), VTColor.Gray(0.24f * 1.1f));
        public static VTSkinnedColor unityBackgroundPure = new VTSkinnedColor(Color.white, Color.black);
        public static VTSkinnedColor unityForeground = new VTSkinnedColor(VTColor.Gray(0.00f), VTColor.Gray(0.81f));
        public static VTSkinnedColor unityForegroundDim = unityForeground.MultA(0.40f, 0.40f);
        public static VTSkinnedColor unityForegroundSelected = new VTSkinnedColor(VTColor.Gray(1.00f), VTColor.Gray(1.00f));
        public static VTSkinnedColor unitySelectionHighlight = new VTSkinnedColor(new Color(0.24f, 0.49f, 0.91f), new Color(0.20f, 0.38f, 0.57f));
        public static VTSkinnedColor unityGraphBackground = new VTSkinnedColor(VTColor.Gray(0.36f), VTColor.Gray(0.16f));
        public static VTSkinnedColor unityHorizontalSplitter = new(new Color(0.6f, 0.6f, 0.6f, 1.333f), new Color(0.12f, 0.12f, 0.12f, 1.333f));

        // Colors
        public static VTSkinnedColor orange = new VTSkinnedColor(new Color(1.0f, 0.62f, 0.35f));
        public static VTSkinnedColor purple = new VTSkinnedColor(new Color(0.81f, 0.24f, 0.88f));
        public static VTSkinnedColor yellow = new VTSkinnedColor(new Color(1.0f, 0.90f, 0.40f));
        public static VTSkinnedColor pink = new VTSkinnedColor(new Color(0.97f, 0.38f, 0.74f));
        public static VTSkinnedColor blue = new VTSkinnedColor(new Color(0.45f, 0.78f, 1f));
        public static VTSkinnedColor teal = new VTSkinnedColor(new Color(0.45f, 1.00f, 0.82f));
        public static VTSkinnedColor green = new VTSkinnedColor(new Color(0.60f, 0.88f, 0.00f));
        public static VTSkinnedColor red = new VTSkinnedColor(new Color(0.96f, 0.35f, 0.35f));
    }
}

