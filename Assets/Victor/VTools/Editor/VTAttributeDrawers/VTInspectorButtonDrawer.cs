using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Victor.Tools
{

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(VTInspectorButton))]
    public class VTInspectorButtonDrawer : PropertyDrawer
    {
        private MethodInfo _methodInfo;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            VTInspectorButton inspectorButtonAttribute = (VTInspectorButton)attribute;

            float methodWidth = GUI.skin.button.CalcSize(new GUIContent(inspectorButtonAttribute.k_MethodName)).x;
            // Make long method name button appear longer and short button shorter yet not too short
            float additionalWidth = methodWidth < 20 ? 20 : methodWidth / 4;
            float textWidth = methodWidth + additionalWidth;
            float buttonXPos = position.x + (position.xMax - position.xMin) / 2 - textWidth / 2;

            Rect buttonRect = new Rect(buttonXPos, position.y, textWidth, position.height);
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

            VTGUI.StoreGUIBackgroundAndContentColor();

            if (!inspectorButtonAttribute.startValueSet)
            {
                property.boolValue = inspectorButtonAttribute.k_DefaultValue;
                inspectorButtonAttribute.isOn = property.boolValue;
                inspectorButtonAttribute.startValueSet = true;
            }

            if (inspectorButtonAttribute.isToggleButton)
            {
                if (inspectorButtonAttribute.isOn)
                {
                    GUI.backgroundColor = VTColorLibrary.GetColor(inspectorButtonAttribute.buttonColor);
                }
                else
                {
                    GUI.backgroundColor = VTColorLibrary.GetColor(inspectorButtonAttribute.buttonOffColor);
                }
            }
            else
            {
                GUI.backgroundColor = VTColorLibrary.GetColor(inspectorButtonAttribute.buttonColor);
            }

            var button = GUI.Button(buttonRect, inspectorButtonAttribute.k_MethodName);

            if (button)
            {
                System.Type methodOwnerType = property.serializedObject.targetObject.GetType();

                string methodName = inspectorButtonAttribute.k_MethodName;

                if (_methodInfo == null)
                {
                    _methodInfo = methodOwnerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                }

                if (_methodInfo != null)
                {
                    _methodInfo.Invoke(property.serializedObject.targetObject, null);
                }
                else
                {
                    Debug.LogWarning($"VTInspectorButton: Unable to find method {methodName} in {methodOwnerType}");
                }

                if (inspectorButtonAttribute.isToggleButton)
                {
                    inspectorButtonAttribute.isOn = !inspectorButtonAttribute.isOn;
                    property.boolValue = !property.boolValue;
                }
                    
            }

            VTGUI.RevertGUIBackgroundAndContentColor();
            GUI.enabled = true;
        }


    }
#endif
}
