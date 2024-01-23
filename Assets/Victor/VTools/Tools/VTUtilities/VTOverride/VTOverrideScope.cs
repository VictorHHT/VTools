using System;

namespace Victor.Tools
{
    public class VTOverrideScope<T> : IDisposable
    {
        private readonly VTOverrideStack<T> m_Stack;

        internal VTOverrideScope(VTOverrideStack<T> overrideStack, T overrideValue)
        {
            m_Stack = overrideStack;
            m_Stack.BeginOverride(overrideValue);
        }

        public void Dispose()
        {
            m_Stack.EndOverride();
        }
    }
}