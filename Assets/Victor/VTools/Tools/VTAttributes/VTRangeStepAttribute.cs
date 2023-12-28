using System;
using UnityEditor;
using UnityEngine;

// Use Example 1: [VTRangeStep(0f, 10f, 0.25f)]
// Use Example 2: [VTRangeStep(100, 1000, 25)]
namespace Victor.Tools
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class VTRangeStep : PropertyAttribute
    {
        public readonly float m_Min = 0f;
        public readonly float m_Max = 100f;
        public readonly float m_Step = 1;
        public readonly int m_Precision;
        // Whether a increase that is not the step is allowed (Occurs when we are reaching the end)
        public readonly bool m_AllowNonStepReach = true;
        public readonly bool m_IsInt = false;


        /// <summary>
        /// Allow you to increase a float value in step, make sure the type of the variable matches the the parameters
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        /// <param name="allowNonStepReach">Whether a increase that is not the step is allowed (Occurs when we are reaching the end)</param>
        public VTRangeStep(float min, float max, float step = 1f, bool allowNonStepReach = true)
        {
            m_Min = min;
            m_Max = max;
            m_Step = step;
            m_Precision = VTMath.Precision(m_Step);
            m_AllowNonStepReach = allowNonStepReach;
            m_IsInt = false;
        }

        /// <summary>
        /// Allow you to increase a int value in step, make sure the type of the variable matches the the parameters
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        /// <param name="allowNonStepReach"></param>
        public VTRangeStep(int min, int max, int step = 1, bool allowNonStepReach = true)
        {
            m_Min = min;
            m_Max = max;
            m_Step = step;
            m_AllowNonStepReach = allowNonStepReach;
            m_IsInt = true;
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(VTRangeStep))]
    public sealed class RangeStepDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rangeAttribute = (VTRangeStep)base.attribute;

            if (!rangeAttribute.m_IsInt)
            {
                float rawFloat = EditorGUI.Slider(position, label, property.floatValue, rangeAttribute.m_Min, rangeAttribute.m_Max);
                property.floatValue = Step(rawFloat, rangeAttribute);
            }
            else
            {
                int rawInt = EditorGUI.IntSlider(position, label, property.intValue, (int)rangeAttribute.m_Min, (int)rangeAttribute.m_Max);
                property.intValue = Step(rawInt, rangeAttribute);
            }
            
        }

        // This is time tested and bug free!
        // I stayed up late until 2:50 AM in September 23 2022 trying to get this right, relentless curiocity paid off
        public float Step(float rawValue, VTRangeStep range)
        {
            float f = rawValue;

            if (range.m_AllowNonStepReach)
            {
                // In order to ensure a reach, where the difference between rawValue and the max allowed value is less than step
                float topCap = (float)Math.Round(Mathf.Floor(range.m_Max / range.m_Step) * range.m_Step, range.m_Precision);
                float topRemaining = (float)Math.Round(range.m_Max - topCap, range.m_Precision);

                // If this is the special case near the top maximum
                if (topRemaining < range.m_Step && f > topCap + topRemaining / 2)
                {
                    f = range.m_Max;
                }
                else
                {
                    // Otherwise we do a regular snap
                    f = (float)Math.Round(VTMath.Snap(f, range.m_Step), range.m_Precision);
                }
            }
            else if(!range.m_AllowNonStepReach)
            {
                f = (float)Math.Round(VTMath.Snap(f, range.m_Step), range.m_Precision);
                // Make sure the value doesn't exceed the maximum allowed range
                if (f > range.m_Max)
                {
                    f -= range.m_Step;
                    f = (float)Math.Round(f, range.m_Precision);
                }
            }

            return f;
        }

        public int Step(int rawValue, VTRangeStep range)
        {
            int f = rawValue;

            if (range.m_AllowNonStepReach)
            {
                // In order to ensure a reach, where the difference between rawValue and the max allowed value is less than step
                int topCap = (int)range.m_Max / (int)range.m_Step * (int)range.m_Step;
                int topRemaining = (int)range.m_Max - topCap;

                // If this is the special case near the top maximum
                if (topRemaining < range.m_Step && f > topCap)
                {
                    f = (int)range.m_Max;
                }
                else
                {
                    // Otherwise we do a regular snap
                    f = (int)VTMath.Snap(f, range.m_Step);
                }
            }
            else if (!range.m_AllowNonStepReach)
            {
                f = (int)VTMath.Snap(f, range.m_Step);
                // Make sure the value doesn't exceed the maximum allowed range
                if (f > range.m_Max)
                {
                    f -= (int)range.m_Step;
                }
            }

            return f;
        }
    }
    #endif
}
