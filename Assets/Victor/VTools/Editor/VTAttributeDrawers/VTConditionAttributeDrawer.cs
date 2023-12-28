using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEditor;

namespace Victor.Tools
{
    // Originally implemented by http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
    [CustomPropertyDrawer(typeof(VTConditionAttribute))]
    public class VTConditionAttributeDrawer : PropertyDrawer
    {
        #if  UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            VTConditionAttribute conditionAttribute = (VTConditionAttribute)attribute;
 
            bool previouslyEnabled = GUI.enabled;
            bool conditionValue = GetConditionAttributeResult(conditionAttribute, property);

            if (conditionValue && conditionAttribute.showIf || !conditionValue && !conditionAttribute.showIf)
            {
                GUI.enabled = true;
                EditorGUI.PropertyField(position, property, label, true);
            }
            else if (conditionValue && !conditionAttribute.showIf && !conditionAttribute.hideInInspector || !conditionValue && conditionAttribute.showIf && !conditionAttribute.hideInInspector)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
            }

            GUI.enabled = previouslyEnabled;
        }
        #endif
        
        private bool GetConditionAttributeResult(VTConditionAttribute condShowAtt, SerializedProperty property)
        {
            bool conditionValue = true;
            
            string propertyPath = property.propertyPath; 
            string conditionPath = propertyPath.Replace(property.name, condShowAtt.conditionBoolean); 
            SerializedProperty conditionPropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (conditionPropertyValue != null)
            {
                conditionValue = conditionPropertyValue.boolValue;
            }
            else
            {
                Debug.LogWarning("No matching boolean found for VTConditionAttribute in object: " + condShowAtt.conditionBoolean);
            }
            return conditionValue;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool conditionValue;
            VTConditionAttribute conditionAttribute = (VTConditionAttribute)attribute;
            conditionValue = GetConditionAttributeResult(conditionAttribute, property);

            // Show no matter what the hide in inspector setting is, because we need to draw the field if it needs to be drawn either enabled or disabled
            if (conditionValue && conditionAttribute.showIf || !conditionValue && !conditionAttribute.showIf || conditionValue && !conditionAttribute.showIf && !conditionAttribute.hideInInspector || !conditionValue && conditionAttribute.showIf && !conditionAttribute.hideInInspector)
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

