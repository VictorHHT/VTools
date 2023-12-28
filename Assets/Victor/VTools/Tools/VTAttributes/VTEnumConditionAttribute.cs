using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Victor.Tools
{
    /// <summary>
    /// An attribute to conditionnally hide fields based on the current selection in an enum.
    /// Usage :  [VTEnumCondition("firemode", (int)FireMode.Automatic, (int)FireMode.SemiAuto)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class VTEnumConditionAttribute : PropertyAttribute
    {
        public string conditionEnum = "";
        public bool showIf = true;
        public bool hideInInspector = true;

        BitArray m_BitArray = new BitArray(32);
        public bool ContainsBitFlag(int enumValue)
        {
            return m_BitArray.Get(enumValue);
        }

        public VTEnumConditionAttribute(string conditionBoolean, params int[] enumValues)
        {
            this.conditionEnum = conditionBoolean;
            
            for (int i = 0; i < enumValues.Length; i++)
            {
                m_BitArray.Set(enumValues[i], true);
            }
        }

        public VTEnumConditionAttribute(string conditionBoolean, bool showIf, bool hideInInspector, params int[] enumValues)
        {
            this.conditionEnum = conditionBoolean;
            this.showIf = showIf;
            this.hideInInspector = hideInInspector;

            for (int i = 0; i < enumValues.Length; i++)
            {
                m_BitArray.Set(enumValues[i], true);
            }
        }
    }
}
