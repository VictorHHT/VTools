using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Victor.Tools;

namespace Victor.EditorTween
{
    public abstract class VTweenCore
    {
        internal EaseType m_EaseType = EaseType.EaseOutQuart;
        internal EaseType m_PreviousHalfPingPongLoopEaseType;
        internal EaseType m_PreviousRewindEaseType;
        // Internal so these fields can be accessed by extensions
        internal PlayDirection m_PlayDirection = PlayDirection.Forward;
        internal PlayStyle m_PlayStyle = PlayStyle.Normal;

        internal int m_TotalLoopCount = 1;
        internal int m_CurrentLoopCount = 0;
        internal float m_InitialDelay = 0f;
        internal float m_LoopDelay = 0f;
        internal float m_HalfPingPongLoopDelay = 0f;
        // duration of the actual tween, without delays
        internal float m_Duration = 0.5f;
        // In Range 0 - 1
        internal float m_Time = 0f;
        internal float m_CurrentInitialDelay = 0f;
        internal float m_CurrentLoopDelay = 0f;
        internal float m_LastUpdatedTimeStamp;

        // Custom tween configurations
        internal float m_OvershootOrAmplitude = 1f;
        internal float m_CycleDuration = 0.3f;

        internal bool m_IsInfinite = false;
        internal bool m_ApplyFirstLoopDelay = false;
        internal bool m_InvertEaseTypeOnHalfPingPongLoop = true;
        // Configurations after tween starts
        internal bool m_Initialized = false;
        internal bool m_Started = false;
        internal bool m_Paused = false;
        internal bool m_Completed = false;
        internal bool m_Removed = false;
        internal bool m_EndReached = false;
        internal bool m_AutoRemove = true;
        internal bool m_Rewinding = false;
        internal bool m_RewindFlag = false;
        // If the tween is playing, it can no longer be modified
        internal bool m_ModificationLocked = false;

        internal string m_ID = "";
        internal string m_UniqueID = "";
        internal object m_Target = null;
        internal ApplierSettings m_ApplierSettings;
        // Because VTDeltaTimeTracker is a struct, it is initialized automatically
        internal VTDeltaTimeTracker m_DeltaTimeTracker;

        // Callbacks
        internal Action m_OnStart = null;
        internal Action m_OnComplete = null;
        internal Action m_OnLoopComplete = null;
        internal Action m_OnPingPongHalfLoopComplete = null;
        internal Action m_OnValueChanged = null;
        internal Action m_OnRewind = null;

        internal float timeDelta
        {
            get
            {
                return m_DeltaTimeTracker.deltaTime;
            }
        }

        internal float directionalTimeDelta
        {
            get
            {
                return m_PlayDirection == PlayDirection.Forward ? timeDelta : -timeDelta;
            }
        }

        public virtual float TotalDuration(bool includeInitialDelay = true, bool includeLoopDelay = true)
        {
            if (m_IsInfinite)
            {
                return -1;
            }

            float totalDuration = m_Duration;

            if (m_PlayStyle == PlayStyle.Normal)
            {
                if (includeLoopDelay)
                {
                    totalDuration += m_LoopDelay;
                }

                totalDuration *= m_TotalLoopCount;
            }
            else
            {
                if (includeLoopDelay)
                {
                    // One complete pingpong loop duration
                    totalDuration = (totalDuration + m_HalfPingPongLoopDelay) + (totalDuration + m_LoopDelay);
                }

                totalDuration *= m_TotalLoopCount;
            }

            if (includeInitialDelay)
            {
                totalDuration += m_InitialDelay;
            }

            if (includeLoopDelay)
            {
                // n loops only have n - 1 loop delay
                totalDuration -= m_LoopDelay;
            }

            return totalDuration;
        }

        /// <summary>
        /// Reset the tween to its original state, the tween to reset must not has been removed
        /// </summary>
        public virtual void Reset()
        {
            Initialize();
            m_Time = 0;
            ApplyTween(m_Time);

            // Recover from complete state
            m_Completed = false;
            m_Paused = true;
            m_EndReached = false;
            m_Started = false;
            m_Rewinding = false;
            m_ModificationLocked = false;
            m_PlayDirection = PlayDirection.Forward;

            m_OnStart?.Invoke();
        }

        public void Pause()
        {
            m_Paused = true;
        }

        public void Resume()
        {
            m_Paused = false;
            // Prepare for updating after a delay
            m_DeltaTimeTracker.Prepare();
            return;
        }

        public virtual void Rewind()
        {
            if (m_Removed)
            {
                return;
            }

            m_Completed = false;
            m_Paused = false;
            m_EndReached = false;

            m_Rewinding = true;
            m_CurrentInitialDelay = 0;
            m_CurrentLoopDelay = 0;
            m_LastUpdatedTimeStamp = 0;

            if (m_Time == 1 || m_PlayDirection == PlayDirection.Forward && m_Time > 0 && m_Time != 1)
            {
                m_PlayDirection = PlayDirection.Backward;
                m_OnRewind?.Invoke();
            }
            else if (m_Time == 0 || m_PlayDirection == PlayDirection.Backward && m_Time < 1 && m_Time != 0)
            {
                m_PlayDirection = PlayDirection.Forward;
                m_OnRewind?.Invoke();
            }
        }

        public virtual void Complete()
        {
            // If play style is normal, we complete the tween by setting current time to duration
            if (m_PlayStyle == PlayStyle.Normal)
            {
                m_Time = 1;
            }
            else
            {
                m_Time = 0;
            }

            ApplyTween(m_Time);
            m_Completed = true;
            m_Paused = true;
            m_EndReached = false;

            m_OnComplete?.Invoke();

            if (m_AutoRemove)
            {
                Remove();
            }
        }

        public virtual void Remove()
        {
            if (m_Removed)
            {
                return;
            }

            VTweener.tweens.Remove(this);
            m_Removed = true;
        }

        internal virtual void Initialize()
        {
            m_CurrentInitialDelay = m_InitialDelay;
            m_CurrentLoopCount = m_TotalLoopCount;
            m_PreviousHalfPingPongLoopEaseType = m_EaseType;

            if (m_ApplyFirstLoopDelay)
            {
                m_CurrentLoopDelay = m_LoopDelay;
            }

            m_Initialized = true;
            m_ModificationLocked = true;
            // Prepare for updating delay
            m_DeltaTimeTracker.Prepare();
        }

        internal virtual void Start()
        {
            m_Time = 0;
            m_Paused = false;
            m_OnStart?.Invoke();
            m_Started = true;
            // Prepare for updating tween
            m_DeltaTimeTracker.Prepare();
        }

        internal void InvertEaseTypeOnHalfPingPongLoop()
        {
            m_EaseType = VTweenLibrary.GetInverseEaseType(m_PreviousHalfPingPongLoopEaseType);
            m_PreviousHalfPingPongLoopEaseType = m_EaseType;
        }

        // Abstract Functions
        internal abstract void ApplyTween(float easedValue);
    }
}
