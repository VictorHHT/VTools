using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.EditorTween
{
    public static class VTweenManager
    {
        public static float defaultInitialDelay = 0f;
        public static float defaultIndependentUpdateScale = 1;
        public static bool defaultAutoPlay = true;
        public static bool defaultAutoRemove = false;
        public static EaseType defaultEaseType = EaseType.EaseOutQuart;
        public static PlayStyle defaultPlayStyle = PlayStyle.Normal;

        internal static void Setup(VTweenCore tweenOrSequence)
        {
            tweenOrSequence.m_InitialDelay = defaultInitialDelay;
            tweenOrSequence.m_Paused = !defaultAutoPlay;
            tweenOrSequence.m_AutoRemove = defaultAutoRemove;
            tweenOrSequence.m_EaseType = defaultEaseType;
            tweenOrSequence.m_PlayStyle = defaultPlayStyle;
        }

        internal static void RemoveFromIndependentList(VTweenCore tween)
        {
            VTweener.tweens.Remove(tween);
        }

        public static void PauseAll()
        {
            for (int i = 0; i < VTweener.tweens.Count; i++)
            {
                VTweener.tweens[i].Pause();
            }
        }

        public static void ResumeAll()
        {
            for (int i = 0; i < VTweener.tweens.Count; i++)
            {
                VTweener.tweens[i].Resume();
            }
        }

        public static void RewindAll()
        {
            for (int i = 0; i < VTweener.tweens.Count; i++)
            {
                VTweener.tweens[i].Rewind();
            }
        }

        public static void ResetAll()
        {
            for (int i = 0; i < VTweener.tweens.Count; i++)
            {
                VTweener.tweens[i].Reset();
            }
        }

        public static void CompleteAll()
        {
            for (int i = 0; i < VTweener.tweens.Count; i++)
            {
                VTweener.tweens[i].Complete();
            }
        }

        // Remove all tweens without complete them first
        public static void RemoveAll()
        {
            VTweener.tweens.Clear();
        }

        public static void PauseAllWithID(string id)
        {
            for (int i = 0; i < VTweener.tweens.Count; i++)
            {
                if (VTweener.tweens[i].m_ID == id)
                    VTweener.tweens[i].Pause();
            }
        }

        public static void ResumeAllWithID(string id)
        {
            for (int i = 0; i < VTweener.tweens.Count; i++)
            {
                if (VTweener.tweens[i].m_ID == id)
                    VTweener.tweens[i].Resume();
            }
        }

        public static void RewindAllWithID(string id)
        {
            for (int i = 0; i < VTweener.tweens.Count; i++)
            {
                if (VTweener.tweens[i].m_ID == id)
                    VTweener.tweens[i].Rewind();
            }
        }

        public static void ResetAllWithID(string id)
        {
            for (int i = 0; i < VTweener.tweens.Count; i++)
            {
                if (VTweener.tweens[i].m_ID == id)
                    VTweener.tweens[i].Reset();
            }
        }

        public static void CompleteAllWithID(string id)
        {
            // Complete in reverse to prevent modifying tweens index after removing
            for (int i = VTweener.tweens.Count - 1; i >= 0; i--)
            {
                if (VTweener.tweens[i].m_ID == id)
                    VTweener.tweens[i].Complete();
            }
        }

        // Remove all tweens without complete them first
        public static void RemoveAllWithID(string id)
        {
            // Complete in reverse to prevent modifying tweens index after removing
            for (int i = VTweener.tweens.Count - 1; i >= 0; i--)
            {
                if (VTweener.tweens[i].m_ID == id)
                    VTweener.tweens[i].Remove();
            }
        }

        public static void RemoveExistingWithUniqueID(string id)
        {
            for (int i = VTweener.tweens.Count - 1; i >= 0; i--)
            {
                if (VTweener.tweens[i].m_UniqueID == id)
                {
                    VTweener.tweens[i].Remove();
                    break;
                }
            }
        }
    }
}
