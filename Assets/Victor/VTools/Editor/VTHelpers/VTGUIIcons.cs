using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public static class VTGUIIcons
    {
        public static readonly Texture2D UnityIcon = (Texture2D)EditorGUIUtility.IconContent("UnityLogo").image;
        public static readonly Texture2D PrefabIcon = (Texture2D)EditorGUIUtility.IconContent("d_Prefab Icon").image;
        public static readonly Texture2D SettingsIcon = (Texture2D)EditorGUIUtility.IconContent("Settings").image;
        public static readonly Texture2D Arrow4DirectionsIcon = (Texture2D)EditorGUIUtility.IconContent("MoveTool on").image;
    }
}
