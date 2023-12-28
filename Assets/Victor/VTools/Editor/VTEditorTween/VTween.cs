using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Victor.Tools;

namespace Victor.EditorTween
{
    public class VTween<T1, T2, TApplier> : VTweenCore where TApplier : TweenApplier
    {
        internal T2 m_StartValue;
        internal T2 m_TargetValue;
        internal Action<T1> m_TweenSetter;
        internal Applier<T1, T2> m_Applier;

        internal VTween(Action<T1> setter, T2 startValue, T2 targetValue)
        {
            m_TweenSetter = setter;
            m_StartValue = startValue;
            m_TargetValue = targetValue;

            VTweener.AddTween(this);
            m_Applier = VTweenApplierManager.GetApplier<T1, T2, TApplier>();

            if (m_Applier == null)
            {
                Debug.LogError("[VTEditorTween] Applier type mismatch".Bold());
                VTweener.tweens.Remove(this);
            }
        }

        internal override void ApplyTween(float time)
        {
            float tweenPosition;
            tweenPosition = VTweenLibrary.GetTweenedValue01(m_EaseType, time, this);
            m_Applier.GenericTween(m_TweenSetter, m_StartValue, m_TargetValue, tweenPosition, m_ApplierSettings);
        }
    }
}
