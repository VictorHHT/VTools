using System;
using UnityEditor;
using UnityEngine;

// Example with two arguments, giving it initial min and max values:
// [VTMinMaxSlider(0f, 100f)]
// public Vector2 Randomness = new Vector2(20, 80);
namespace Victor.Tools
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class VTMinMaxSlider : PropertyAttribute
    {
        public Vector2 m_MinmaxLimit;
        public Vector2 m_MinmaxValue;

        public VTMinMaxSlider(float minLimit, float maxLimit)
        {
            m_MinmaxLimit.x = minLimit;
            m_MinmaxLimit.y = maxLimit;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(VTMinMaxSlider))]
    public sealed class MinMaxSliderDrawer : PropertyDrawer
    {
        float m_MinValue;
        float m_MaxValue;
        float m_MinLimit;
        float m_MaxLimit;
        float m_MinValueAtMouseDown;
        float m_MaxValueAtMouseDown;
        float m_MinValueAtReached;
        float m_MaxValueAtReached;
        float m_MouseXPosAtRached;
        bool m_MaxReached;
        bool m_MinReached;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Event e = Event.current;
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                throw new ArgumentException("VTMinMaxSlider underlying value type must be vector2");
            }

            var minmaxAttribute = (VTMinMaxSlider)base.attribute;
            m_MinLimit = minmaxAttribute.m_MinmaxLimit.x;
            m_MaxLimit = minmaxAttribute.m_MinmaxLimit.y;

            if (m_MinLimit >= m_MaxLimit)
            {
                throw new ArgumentOutOfRangeException("VTMinMaxSlider min limit can not greater than or equal to max limit");
            }

            float padding = 2;
            EditorGUI.PrefixLabel(position, new GUIContent(property.name));

            // Each int field occupies 1/5 of the remaining rect area
            float intFieldWidth = ((position.xMax - position.xMin) - EditorGUIUtility.labelWidth - padding * 3) / 5;
            // We Clamp its width to prevent is from getting too small
            intFieldWidth = Mathf.Max(intFieldWidth, 30);

            Rect minFieldRect = new Rect(padding + position.x + EditorGUIUtility.labelWidth, position.y, intFieldWidth, position.height);
            // The three paddings are min rect padding, padding of itself, max rect padding
            float sliderWidth = position.width - intFieldWidth * 2 - EditorGUIUtility.labelWidth - padding * 3;
            Rect sliderRect = new Rect(minFieldRect.position.x + minFieldRect.width + padding, position.y, sliderWidth, position.height);

            // If user starts to drag the slider, we clear the current keyboard focus
            if (e.type == EventType.MouseDown && e.button == 0 && sliderRect.Contains(e.mousePosition))
            {
                GUIUtility.keyboardControl = 0;
                m_MinValueAtMouseDown = m_MinValue;
                m_MaxValueAtMouseDown = m_MaxValue;
            }

            // Make sure the min and max value set by the underlying field is greater than min limit,
            // and maxValue less than max limit
            m_MinValue = Mathf.Max(property.vector2Value.x, m_MinLimit);
            m_MaxValue = Mathf.Min(Mathf.Max(property.vector2Value.y, m_MinLimit), m_MaxLimit);
            // We want a delayed field because we don't want to change another bound value immediately when we type a number
            m_MinValue = EditorGUI.DelayedFloatField(minFieldRect, m_MinValue);

            EditorGUI.BeginChangeCheck();

            if (e.type == EventType.MouseDrag && e.button == 0)
            {
                // Mouse dragging when the max value reaches the max limit
                if (m_MaxValue == m_MaxLimit)
                {
                    // Make sure the the following property set only once
                    if (m_MaxReached == false)
                    {
                        m_MaxReached = true;

                        // When max is reached and min is not reached we set minReachFlag to false
                        // This is important because min and max value could be reached (The Slider "Max Out") at the same time
                        if (m_MinValue > m_MinLimit)
                        {
                            m_MinReached = false;
                        }

                        m_MouseXPosAtRached = e.mousePosition.x;
                        m_MinValueAtReached = m_MinValue;
                    }

                    if (e.mousePosition.x >= m_MouseXPosAtRached)
                    {
                        // Shrink to the right
                        float totalRange = m_MaxLimit - m_MinLimit;
                        // Take slider width into account
                        float minValueIncreaseAmount = (e.mousePosition.x - m_MouseXPosAtRached) / sliderWidth * totalRange;
                        m_MinValue = m_MinValueAtReached + minValueIncreaseAmount;
                    }
                    else
                    {
                        // Prevent number error
                        m_MinValue = m_MinValueAtReached;
                    }
                }

                if (m_MinValue == m_MinLimit)
                {
                    // Make sure the mouse x position at reached only set once
                    if (m_MinReached == false)
                    {
                        m_MinReached = true;

                        // Vice versa
                        if (m_MaxValue < m_MaxLimit)
                        {
                            m_MaxReached = false;
                        }

                        m_MouseXPosAtRached = e.mousePosition.x;
                        m_MaxValueAtReached = m_MaxValue;
                    }

                    if (e.mousePosition.x < m_MouseXPosAtRached)
                    {
                        // Shrink to the left
                        float totalRange = m_MaxLimit - m_MinLimit;
                        float maxValueDecreaseAmount = (m_MouseXPosAtRached - e.mousePosition.x) / sliderWidth * totalRange;
                        m_MaxValue = m_MaxValueAtReached - maxValueDecreaseAmount;
                    }
                    else
                    {
                        // Prevent number error
                        m_MaxValue = m_MaxValueAtReached;
                    }
                }
            }

            if (e.type == EventType.MouseUp && e.button == 0)
            {
                m_MaxReached = false;
                m_MinReached = false;
                m_MouseXPosAtRached = 0;
                m_MinValueAtReached = 0;
                m_MaxValueAtReached = 0;
            }

            EditorGUI.MinMaxSlider(sliderRect, ref m_MinValue, ref m_MaxValue, m_MinLimit, m_MaxLimit);

            Rect maxFieldRect = new Rect(sliderRect.position.x + sliderRect.width + padding, position.y, intFieldWidth, position.height);
            m_MaxValue = EditorGUI.DelayedFloatField(maxFieldRect, m_MaxValue);

            if (EditorGUI.EndChangeCheck())
            {
                // Clamp min value between min limit and max value
                m_MinValue = Mathf.Clamp(m_MinValue, m_MinLimit, m_MaxValue);
                // Clamp max value between min value and max limit
                m_MaxValue = Mathf.Clamp(m_MaxValue, m_MinValue, m_MaxLimit);
                // Clamp min value between min limit and max value
                m_MinValue = Mathf.Clamp(m_MinValue, m_MinLimit, m_MaxValue);

                minmaxAttribute.m_MinmaxValue.x = (float)Math.Round(m_MinValue, 1);
                minmaxAttribute.m_MinmaxValue.y = (float)Math.Round(m_MaxValue, 1);

                // Assign underlying variable value
                property.vector2Value = minmaxAttribute.m_MinmaxValue;
            }
        }
    }
#endif

}
