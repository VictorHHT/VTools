using System;
using UnityEngine;

namespace Victor.Tools
{
    [Flags]
    public enum ShortcutModifiers
    {
        None = 0,

        Action = 1 << 0,

        Shift = 1 << 1,

        Alt = 1 << 2,
    }

    public static class VTShortcutModifiers
    {
        public static bool HasModifiers(this ShortcutModifiers modifiers, ShortcutModifiers modifiersToCheck)
        {
            return (modifiers & modifiersToCheck) != 0;
        }

        public static bool HasAction(this ShortcutModifiers modifiers) => modifiers.HasModifiers(ShortcutModifiers.Action);

        public static bool HasShift(this ShortcutModifiers modifiers) => modifiers.HasModifiers(ShortcutModifiers.Shift);

        public static bool HasAlt(this ShortcutModifiers modifiers) => modifiers.HasModifiers(ShortcutModifiers.Alt);

        public static EventModifiers ToEventModifiers(this ShortcutModifiers modifiers)
        {
            var em = default(EventModifiers);

            if (modifiers.HasAction())
            {
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    em |= EventModifiers.Command;
                }
                else
                {
                    em |= EventModifiers.Control;
                }
            }

            if (modifiers.HasShift())
            {
                em |= EventModifiers.Shift;
            }

            if (modifiers.HasAlt())
            {
                em |= EventModifiers.Alt;
            }

            return em;
        }
    }
}