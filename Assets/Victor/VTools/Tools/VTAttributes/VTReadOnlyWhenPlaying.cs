using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Victor.Tools
{
    public class VTReadOnlyWhenPlaying : PropertyAttribute { }


    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(VTReadOnlyWhenPlaying))]
    public class VTReadOnlyWhenPlayingDrawer : PropertyDrawer
    {        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = !Application.isPlaying;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
    #endif

}

