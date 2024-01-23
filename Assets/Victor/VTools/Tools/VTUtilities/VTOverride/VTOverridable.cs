using System;

namespace Victor.Tools
{
    public class VTOverridable<T> : VTOverrideStack<T>
    {
        private T m_Value;
      
        public T value
        {
            get
            {
                return m_Getter();
            }

            internal set
            {
                m_Setter(value);
            }
        }

        public VTOverridable(T initialValue)
        {
            m_Value = initialValue;
            m_Getter = () => m_Value;
            m_Setter = newValue => m_Value = newValue;
        }
    }
}
