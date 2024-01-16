using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.EditorTween
{
    [InitializeOnLoad]
    public static class VTweener
    {
        public static List<VTweenCore> tweens = new List<VTweenCore>();

        static VTweener()
        {
            EditorApplication.update += UpdateTweens;
        }

        public static void AddTween(VTweenCore tween)
        {
            tweens.Add(tween);
        }

        private static void UpdateTweens()
        {
            if (tweens.Count == 0)
            {
                return;
            }

            HandleTweenAndSequenceUpdation();
        }

        private static void HandleTweenAndSequenceUpdation()
        {
            for (int i = 0; i < tweens.Count; i++)
            {
                UpdateTween(tweens[i]);
            }
        }

        internal static void UpdateTween(VTweenCore tween)
        {
            if (tween == null)
            {
                tweens.Remove(tween);
                return;
            }

            if (tween.m_Initialized == false)
            {
                tween.Initialize();
            }

            if (tween.m_Paused || tween.m_Completed || tween.m_Removed)
            {
                return;
            }

            tween.m_DeltaTimeTracker.Update();

            // Initial delay pause
            if (tween.m_CurrentInitialDelay > 0)
            {
                tween.m_CurrentInitialDelay -= tween.timeDelta;
                if (tween.m_CurrentInitialDelay <= 0)
                {
                    tween.m_CurrentInitialDelay = 0;
                }
            }
            else if (tween.m_CurrentLoopDelay > 0) 
            {
                // Loop delay pause
                tween.m_CurrentLoopDelay -= tween.timeDelta;

                if (tween.m_CurrentLoopDelay <= 0)
                {
                    tween.m_CurrentLoopDelay = 0;
                }
            }
            else if (tween.m_Duration == 0) 
            {
                tween.Complete();
            }
            else 
            {
                // We are in business
                if (!tween.m_Started)
                {
                    tween.Start();
                }

                tween.m_Progress += tween.directionalTimeDelta / tween.m_Duration;
                tween.m_OnValueChanged?.Invoke();
                tween.ApplyTween(tween.m_Progress);

                if (tween.m_Progress > 1)
                {
                    tween.m_Progress = 1;
                    bool shouldApplyTween = true;
                    tween.m_OnValueChanged?.Invoke();

                    // If rewind to time >= duration
                    if (tween.m_Rewinding)
                    {
                        tween.m_Paused = true;
                        tween.m_Rewinding = false;
                    }
                    else if (tween.m_PlayStyle == PlayStyle.PingPong)
                    {
                        if (tween.m_InvertEaseTypeOnHalfPingPongLoop)
                        {
                            tween.InvertEaseTypeOnHalfPingPongLoop();
                        }

                        tween.m_PlayDirection = PlayDirection.Backward;
                        tween.m_CurrentLoopDelay = tween.m_HalfPingPongLoopDelay;
                        tween.m_OnPingPongHalfLoopComplete?.Invoke();
                    }
                    else
                    {
                        // Normal playstyle and not rewinding
                        tween.m_Progress = 0;
                        shouldApplyTween = false;
                        // End reached 
                        tween.m_EndReached = true;
                    }

                    if (shouldApplyTween)
                    {
                        tween.ApplyTween(tween.m_Progress);
                    }    
                }
                else if (tween.m_Progress < 0)
                {
                    tween.m_Progress = 0;
                    tween.m_OnValueChanged?.Invoke();

                    // If rewind to time <= 0
                    if (tween.m_Rewinding)
                    {
                        tween.m_Paused = true;
                        tween.m_Rewinding = false;
                    }
                    else if (tween.m_PlayStyle == PlayStyle.PingPong)
                    {
                        if (tween.m_InvertEaseTypeOnHalfPingPongLoop)
                        {
                            tween.InvertEaseTypeOnHalfPingPongLoop();
                        }

                        tween.m_PlayDirection = PlayDirection.Forward;
                        tween.m_OnPingPongHalfLoopComplete?.Invoke();
                        // End reached 
                        tween.m_EndReached = true;
                    }

                    tween.ApplyTween(tween.m_Progress);
                }
            }

            if (tween.m_EndReached)
            {
                tween.m_CurrentLoopDelay = tween.m_LoopDelay;
                tween.m_OnLoopComplete?.Invoke();

                if (tween.m_IsInfinite)
                {
                    tween.m_EndReached = false;
                }
                else if (tween.m_CurrentLoopCount > 1)
                {
                    tween.m_EndReached = false;
                    tween.m_CurrentLoopCount--;
                }
                else
                {
                    tween.Complete();
                }
            }
        }
    }
}
