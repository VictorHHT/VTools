﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEditor;

namespace Victor.Tools
{
    [CustomPropertyDrawer(typeof(VTReadOnly))]
    public class VTReadOnlyDrawer : PropertyDrawer
    {
        // Make sure properties don't collapse smaller than their content
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        #if  UNITY_EDITOR
        // Draw a disabled property field
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // Disable fields
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true; // Enable fields
        }
        #endif
    }
}
