using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.EditorTween
{
    public static class VTweenApplierManager
    {
        private static Dictionary<Type, TweenApplier> m_TweenAppliers = new Dictionary<Type, TweenApplier>();

        internal static Applier<T1, T2> GetApplier<T1, T2, TApplier>() where TApplier : TweenApplier
        {
            TweenApplier tweenApplier;
            Type applierType = typeof(TApplier);
            Applier<T1, T2> applier;

            if (m_TweenAppliers == null)
            {
                m_TweenAppliers = new Dictionary<Type, TweenApplier>(5);
            }
            else if (m_TweenAppliers.TryGetValue(applierType, out tweenApplier))
            {
                applier = tweenApplier as Applier<T1, T2>;

                if (applier == null)
                {
                    Debug.LogError("Applier Type Mismatch");
                    return null;
                }

                return applier;
            }

            // Create an applier of the specified custom type and add it to the dictionary for potential reuse
            tweenApplier = Activator.CreateInstance<TApplier>();
            applier = tweenApplier as Applier<T1, T2>;

            if (applier == null)
            {
                Debug.LogError("Applier Type Mismatch");
                return null;
            }

            m_TweenAppliers.Add(applierType, tweenApplier);
            return applier;
        }

        internal static void FreeAppliers()
        {
            if (m_TweenAppliers != null)
            {
                m_TweenAppliers.Clear();
                m_TweenAppliers = null;
            }
        }
    }
}