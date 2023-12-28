using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Victor.Tools
{
    // Originally implemented by http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
    [CustomPropertyDrawer(typeof(VTEnumConditionAttribute))]
    public class VTEnumConditionAttributeDrawer : PropertyDrawer
    {
        #if  UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            VTEnumConditionAttribute enumConditionAttribute = (VTEnumConditionAttribute)attribute;
            bool conditionValue = GetConditionAttributeResult(enumConditionAttribute, property);
            bool previouslyEnabled = GUI.enabled;
            GUI.enabled = conditionValue;
            if (conditionValue && enumConditionAttribute.showIf || !conditionValue && !enumConditionAttribute.showIf)
            {
                GUI.enabled = true;
                EditorGUI.PropertyField(position, property, label, true);
            }
            else if (conditionValue && !enumConditionAttribute.showIf && !enumConditionAttribute.hideInInspector || !conditionValue && enumConditionAttribute.showIf && !enumConditionAttribute.hideInInspector)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
            }

            GUI.enabled = previouslyEnabled;
        }
        #endif

        private static Dictionary<string, string> cachedPaths = new Dictionary<string, string>();

        private bool GetConditionAttributeResult(VTEnumConditionAttribute enumConditionAttribute, SerializedProperty property)
        {
            bool enabled = true;

            SerializedProperty enumProp;
            string enumPropPath = string.Empty;
            string propertyPath = property.propertyPath;

            if (!cachedPaths.TryGetValue(propertyPath, out enumPropPath))
            {
                enumPropPath = propertyPath.Replace(property.name, enumConditionAttribute.conditionEnum);
                cachedPaths.Add(propertyPath, enumPropPath);
            }

            enumProp = property.serializedObject.FindProperty(enumPropPath);

            if (enumProp != null)
            {
                int currentEnum = enumProp.enumValueIndex;
                enabled = enumConditionAttribute.ContainsBitFlag(currentEnum);
            }
            else
            {
                Debug.LogWarning("No matching boolean found for VTConditionAttribute in object: " + enumConditionAttribute.conditionEnum);
            }
            
            return enabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            VTEnumConditionAttribute enumConditionAttribute = (VTEnumConditionAttribute)attribute;
            bool conditionValue = GetConditionAttributeResult(enumConditionAttribute, property);

            if (conditionValue && enumConditionAttribute.showIf || !conditionValue && !enumConditionAttribute.showIf || conditionValue && !enumConditionAttribute.showIf && !enumConditionAttribute.hideInInspector || !conditionValue && enumConditionAttribute.showIf && !enumConditionAttribute.hideInInspector)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }

}
