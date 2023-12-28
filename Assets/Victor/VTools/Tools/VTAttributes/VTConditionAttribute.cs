using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Victor.Tools
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class VTConditionAttribute : PropertyAttribute
    {
        public string conditionBoolean = "";
        public bool showIf = true;
        public bool hideInInspector = true;

        public VTConditionAttribute(string conditionBoolean)
        {
            this.conditionBoolean = conditionBoolean;
        }

        public VTConditionAttribute(string conditionBoolean, bool showIf)
        {
            this.conditionBoolean = conditionBoolean;
            this.showIf = showIf;
        }

        public VTConditionAttribute(string conditionBoolean, bool showIf, bool hideInInspector)
        {
            this.conditionBoolean = conditionBoolean;
            this.showIf = showIf;
            this.hideInInspector = hideInInspector;
        }

    }

}
