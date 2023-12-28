using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Victor.Tools
{
    [CustomPropertyDrawer(typeof(VTNumberPickerAttribute))]
    public class VTNumberPickerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var numberPickerAttribute = (VTNumberPickerAttribute)attribute;

            Rect floatFieldRect = new Rect(position.x, position.y, position.width - 22, position.height);
            Rect selectButtonRect = new Rect(position.xMax - 20, position.y, 20, position.height);
            Rect nameRect = new Rect(position.xMin, position.y, position.width / 2, position.height);

            EditorGUI.PropertyField(floatFieldRect, property);
            EditorGUI.LabelField(nameRect, property.displayName);
            if (GUI.Button(selectButtonRect, "..."))
            {
                GenericMenu selectionMenu = new GenericMenu();
                if (numberPickerAttribute.floatOptions != null)
                {
                    foreach (var option in numberPickerAttribute.floatOptions)
                    {
                        selectionMenu.AddItem(new GUIContent(option.ToString()), false,
                        () =>
                        {
                            property.floatValue = option;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
                else if (numberPickerAttribute.intOptions != null)
                {
                    foreach (var option in numberPickerAttribute.intOptions)
                    {
                        selectionMenu.AddItem(new GUIContent(option.ToString()), false,
                        () =>
                        {
                            property.intValue = option;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
               
                selectionMenu.ShowAsContext();
            }
        }

    }

}
