using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Victor.Tools
{   
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class VTInspectorButton : PropertyAttribute
    {
        public readonly string k_MethodName;
        public readonly string k_ConditionName;
        public bool isToggleButton;
        public bool isOn;
        public bool startValueSet;
        // For some reason, assign the backing property initial value doesn't work, we have to work around this limitation
        public readonly bool k_DefaultValue;
        public readonly VTColorLibrary.VTColors buttonColor;
        public readonly VTColorLibrary.VTColors buttonOffColor;

        public VTInspectorButton(string methodName)
        {
            k_MethodName = methodName;
            isToggleButton = false;
            buttonColor = VTColorLibrary.VTColors.White;
        }

        public VTInspectorButton(string methodName, VTColorLibrary.VTColors buttonColor)
        {
            k_MethodName = methodName;
            isToggleButton = false;
            this.buttonColor = buttonColor;
        }

        /// <summary>
        /// If we provide two Colors, the inspector button is recognized as a switch button, which changes its color as user clicks it
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="buttonOnColor"></param>
        /// <param name="buttonOffColor"></param>
        public VTInspectorButton(string methodName, VTColorLibrary.VTColors buttonOnColor, VTColorLibrary.VTColors buttonOffColor, bool defaultValue = false)
        {
            k_MethodName = methodName;
            isToggleButton = true;
            buttonColor = buttonOnColor;
            this.buttonOffColor = buttonOffColor;
            k_DefaultValue = defaultValue;
        }
    }
}


