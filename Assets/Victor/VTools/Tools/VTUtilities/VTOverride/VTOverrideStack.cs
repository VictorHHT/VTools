using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public class VTOverrideStack<T>
    {
        protected Func<T> m_Getter;
        protected Action<T> m_Setter;
        protected readonly Stack<T> m_OverrideStack = new();

        // A parameterless constructor is necessary for VTOverridable constructor with only one parameter 
        public VTOverrideStack()
        {
        }

        public VTOverrideStack(Func<T> getter, Action<T> setter)
        {
            m_Getter = getter;
            m_Setter = setter;
        }

        public VTOverrideScope<T> NewOverrideScope(T overrideValue)
        {
            return new VTOverrideScope<T>(this, overrideValue);
        }

        public void BeginOverride(T overrideValue)
        {
            // Push current value to the stack so that it could be restored later
            // The benefit of using a getter is that only one parameter is required (initial value is not needed)
            // After the override stack is created, its purpose is determined, which means the retrieval of the property it contorls is determined too,
            // so there is no need to provide that information in the parameter each time. You can view it as if VTOverrideStack has learned and encapsulated that information
            m_OverrideStack.Push(m_Getter());
            m_Setter(overrideValue);
        }

        public void EndOverride()
        {
            if (m_OverrideStack.Count == 0)
            {
                throw new InvalidOperationException();
            }

            T previousValue = m_OverrideStack.Pop();
            m_Setter(previousValue);
        }
    }
}