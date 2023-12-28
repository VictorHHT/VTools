using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.EditorTween
{
    public class VTweenConfig
    {
        internal EaseType m_EaseType = EaseType.EaseOutQuart;
        internal PlayStyle m_PlayStyle = PlayStyle.Normal;
        internal int m_TotalLoopCount = 1;
        internal float m_InitialDelay = 0f;
        internal float m_LoopDelay = 0f;
        internal float m_HalfPingPongLoopDelay = 0f;
        internal float m_Duration = 0.5f;

        internal float m_OvershootOrAmplitude = 1f;
        internal float m_CycleDuration = 0.3f;

        internal bool m_IsInfinite = false;
        internal bool m_ApplyFirstLoopDelay = false;
        internal bool m_InvertEaseTypeOnHalfPingPongLoop = true;
        internal string m_ID = "";
        internal Action m_OnStart = null;
        internal Action m_OnComplete = null;
        internal Action m_OnLoopComplete = null;
        internal Action m_OnPingPongHalfLoopComplete = null;
        internal Action m_OnValueChanged = null;
        internal Action m_OnRewind = null;
    }
}