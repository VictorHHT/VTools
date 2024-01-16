using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Victor.Tools;

namespace Victor.EditorTween
{
    public static class VTweenExtension
    {
        public static VTweenCore SetPlayStyle(this VTweenCore tween, PlayStyle playStyle)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_PlayStyle = playStyle;
            return tween;
        }

        public static VTweenCore SetDuration(this VTweenCore tween, float duration)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_Duration = duration;
            return tween;
        }

        public static VTweenCore SetLoopCount(this VTweenCore tween, int loopCount)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_TotalLoopCount = loopCount;
            return tween;
        }

        // For those extensions which modifys total duration of the tween, we don't allow modification after the tween has been created and running
        // Because VTween is an abstract class, so we have to use extension methods
        public static VTweenCore SetInitialDelay(this VTweenCore tween, float delay)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_InitialDelay = delay;
            return tween;
        }

        public static VTweenCore SetLoopDelay(this VTweenCore tween, float loopDelay)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_LoopDelay = loopDelay;
            return tween;
        }

        public static VTweenCore SetEaseType(this VTweenCore tween, EaseType ease)
        {
            if (tween == null)
            {
                return tween;
            }

            tween.m_EaseType = ease;
            return tween;
        }

        public static VTweenCore SetProgress(this VTweenCore tween, float progress)
        {
            if (tween == null)
            {
                return tween;
            }

            tween.Pause();
            tween.m_Progress = progress;
            tween.ApplyTween(tween.m_Progress);
            return tween;
        }

        public static VTweenCore SetHalfPingPongLoopDelay(this VTweenCore tween, float delay)
        {
            if (tween == null)
            {
                return tween;
            }

            tween.m_HalfPingPongLoopDelay = delay;
            return tween;
        }

        public static VTweenCore SetInfinite(this VTweenCore tween, bool condition)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_IsInfinite = condition;
            return tween;
        }

        public static VTweenCore SetAutoRemove(this VTweenCore tween, bool condition)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_AutoRemove = condition;
            return tween;
        }

        public static VTweenCore SetID(this VTweenCore tween, string id)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_ID = id;
            return tween;
        }

        public static VTweenCore SetUniqueID(this VTweenCore tween, string uniqueID)
        {
            if (tween == null || tween.m_ModificationLocked || uniqueID.NullOrEmpty())
            {
                return tween;
            }

            // Remove any existing tween with unique ID
            VTweenManager.RemoveExistingWithUniqueID(uniqueID);
            tween.m_UniqueID = uniqueID;
            return tween;
        }

        public static VTweenCore SetOvershootOrAmplitude(this VTweenCore tween, float overshootOrAmplitude)
        {
            if (tween == null)
            {
                return tween;
            }

            tween.m_OvershootOrAmplitude = overshootOrAmplitude;
            return tween;
        }

        public static VTweenCore SetCycleDuration(this VTweenCore tween, float cycleDuration)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_CycleDuration = cycleDuration;
            return tween;
        }

        public static VTweenCore ApplyFirstLoopDelay(this VTweenCore tween, bool condition)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            tween.m_ApplyFirstLoopDelay = condition;
            return tween;
        }

        public static VTweenCore InvertEaseTypeOnHalfPingPongLoop(this VTweenCore tween, bool condition)
        {
            if (tween == null)
            {
                return tween;
            }

            tween.m_InvertEaseTypeOnHalfPingPongLoop = condition;
            return tween;
        }

        public static VTweenCore OnStart(this VTweenCore tween, Action onStart)
        {
            tween.m_OnStart = onStart;
            return tween;
        }

        public static VTweenCore OnRewind(this VTweenCore tween, Action onRewind)
        {
            tween.m_OnRewind = onRewind;
            return tween;
        }

        public static VTweenCore OnValueChanged(this VTweenCore tween, Action onValueChanged)
        {
            tween.m_OnValueChanged = onValueChanged;
            return tween;
        }

        public static VTweenCore OnLoopComplete(this VTweenCore tween, Action onLoopComplete)
        {
            tween.m_OnLoopComplete = onLoopComplete;
            return tween;
        }

        public static VTweenCore OnPingPongHalfLoopComplete(this VTweenCore tween, Action onLoopComplete)
        {
            tween.m_OnPingPongHalfLoopComplete = onLoopComplete;
            return tween;
        }

        public static VTweenCore OnComplete(this VTweenCore tween, Action onComplete)
        {
            tween.m_OnComplete = onComplete;
            return tween;
        }

        public static VTweenCore SetConfig(this VTweenCore tweenCore, VTweenConfig tweenConfig)
        {
            tweenCore.m_EaseType = tweenConfig.m_EaseType;
            tweenCore.m_PlayStyle = tweenConfig.m_PlayStyle;
            tweenCore.m_Duration = tweenConfig.m_Duration;
            tweenCore.m_TotalLoopCount = tweenConfig.m_TotalLoopCount;
            tweenCore.m_InitialDelay = tweenConfig.m_InitialDelay;
            tweenCore.m_LoopDelay = tweenConfig.m_LoopDelay;
            tweenCore.m_HalfPingPongLoopDelay = tweenConfig.m_HalfPingPongLoopDelay;
            tweenCore.m_OvershootOrAmplitude = tweenConfig.m_OvershootOrAmplitude;
            tweenCore.m_CycleDuration = tweenConfig.m_CycleDuration;
            tweenCore.m_IsInfinite = tweenConfig.m_IsInfinite;
            tweenCore.m_ApplyFirstLoopDelay = tweenConfig.m_ApplyFirstLoopDelay;
            tweenCore.m_InvertEaseTypeOnHalfPingPongLoop = tweenConfig.m_InvertEaseTypeOnHalfPingPongLoop;
            tweenCore.m_ID = tweenConfig.m_ID;
            tweenCore.m_OnStart = tweenConfig.m_OnStart;
            tweenCore.m_OnComplete = tweenConfig.m_OnComplete;
            tweenCore.m_OnLoopComplete = tweenConfig.m_OnLoopComplete;
            tweenCore.m_OnPingPongHalfLoopComplete = tweenConfig.m_OnPingPongHalfLoopComplete;
            tweenCore.m_OnValueChanged = tweenConfig.m_OnValueChanged;
            tweenCore.m_OnRewind = tweenConfig.m_OnRewind;
            return tweenCore;
        }

        public static VTweenConfig SetEaseType(this VTweenConfig tweenConfig, EaseType easeType)
        {
            tweenConfig.m_EaseType = easeType;
            return tweenConfig;
        }

        public static VTweenConfig SetPlayStyle(this VTweenConfig tweenConfig, PlayStyle playStyle)
        {
            tweenConfig.m_PlayStyle = playStyle;
            return tweenConfig;
        }

        public static VTweenConfig SetDuration(this VTweenConfig tweenConfig, float duration)
        {
            tweenConfig.m_Duration = duration;
            return tweenConfig;
        }

        public static VTweenConfig SetLoopCount(this VTweenConfig tweenConfig, int totalLoopCount)
        {
            tweenConfig.m_TotalLoopCount = totalLoopCount;
            return tweenConfig;
        }

        public static VTweenConfig SetInitialDelay(this VTweenConfig tweenConfig, float initialDelay)
        {
            tweenConfig.m_InitialDelay = initialDelay;
            return tweenConfig;
        }

        public static VTweenConfig SetLoopDelay(this VTweenConfig tweenConfig, float loopDelay)
        {
            tweenConfig.m_LoopDelay = loopDelay;
            return tweenConfig;
        }

        public static VTweenConfig SetHalfPingPongLoopDelay(this VTweenConfig tweenConfig, float delay)
        {
            tweenConfig.m_HalfPingPongLoopDelay = delay;
            return tweenConfig;
        }

        public static VTweenConfig SetOvershootOrAmplitude(this VTweenConfig tweenConfig, float overshootOrAmplitude)
        {
            tweenConfig.m_OvershootOrAmplitude = overshootOrAmplitude;
            return tweenConfig;
        }

        public static VTweenConfig SetCycleDuration(this VTweenConfig tweenConfig, float cycleDuration)
        {
            tweenConfig.m_CycleDuration = cycleDuration;
            return tweenConfig;
        }

        public static VTweenConfig SetInfinite(this VTweenConfig tweenConfig, bool condition)
        {
            tweenConfig.m_IsInfinite = condition;
            return tweenConfig;
        }

        public static VTweenConfig ApplyFirstLoopDelay(this VTweenConfig tweenConfig, bool condition)
        {
            tweenConfig.m_ApplyFirstLoopDelay = condition;
            return tweenConfig;
        }

        public static VTweenConfig InvertEaseTypeOnHalfPingPongLoop(this VTweenConfig tweenConfig, bool condition)
        {
            tweenConfig.m_InvertEaseTypeOnHalfPingPongLoop = condition;
            return tweenConfig;
        }

        public static VTweenConfig SetID(this VTweenConfig tweenConfig, string id)
        {
            tweenConfig.m_ID = id;
            return tweenConfig;
        }

        public static VTweenConfig OnStart(this VTweenConfig tweenConfig, Action onStart)
        {
            tweenConfig.m_OnStart = onStart;
            return tweenConfig;
        }

        public static VTweenConfig OnComplete(this VTweenConfig tweenConfig, Action onComplete)
        {
            tweenConfig.m_OnComplete = onComplete;
            return tweenConfig;
        }

        public static VTweenConfig OnLoopComplete(this VTweenConfig tweenConfig, Action onLoopComplete)
        {
            tweenConfig.m_OnLoopComplete = onLoopComplete;
            return tweenConfig;
        }

        public static VTweenConfig OnPingPongHalfLoopComplete(this VTweenConfig tweenConfig, Action action)
        {
            tweenConfig.m_OnPingPongHalfLoopComplete = action;
            return tweenConfig;
        }

        public static VTweenConfig OnValueChanged(this VTweenConfig tweenConfig, Action onValueChanged)
        {
            tweenConfig.m_OnValueChanged = onValueChanged;
            return tweenConfig;
        }

        public static VTweenConfig OnRewind(this VTweenConfig tweenConfig, Action onRewind)
        {
            tweenConfig.m_OnRewind = onRewind;
            return tweenConfig;
        }

        // Float Applier
        public static VTween<float, float, FloatApplier> TweenFloat(this float floatToTween, Action<float> setter, float target)
        {
            VTween<float, float, FloatApplier> tween = new VTween<float, float, FloatApplier>(setter, floatToTween, target);
            tween.m_ApplierSettings = new FloatApplierSettings();
            return tween;
        }

        // Int Applier
        public static VTween<int, int, IntApplier> TweenInt(this int intToTween, Action<int> setter, int target)
        {
            VTween<int, int, IntApplier> tween = new VTween<int, int, IntApplier>(setter, intToTween, target);
            tween.m_ApplierSettings = new IntApplierSettings();
            return tween;
        }

        // Vector2 Applier
        public static VTween<Vector2, Vector2, Vector2Applier> TweenVector2(this Vector2 v2ToTween, Action<Vector2> setter, Vector2 target)
        {
            VTween<Vector2, Vector2, Vector2Applier> tween = new VTween<Vector2, Vector2, Vector2Applier>(setter, v2ToTween, target);
            tween.m_ApplierSettings = new Vector2ApplierSettings();
            return tween;
        }

        // Vector3 Applier
        public static VTween<Vector3, Vector3, Vector3Applier> TweenVector3(this Vector3 v3ToTween, Action<Vector3> setter, Vector3 target)
        {
            VTween<Vector3, Vector3, Vector3Applier> tween = new VTween<Vector3, Vector3, Vector3Applier>(setter, v3ToTween, target);
            tween.m_ApplierSettings = new Vector3ApplierSettings();
            return tween;
        }

        // QuaternionV3 Applier
        public static VTween<Quaternion, Vector3, QuaternionV3Applier> TweenQuaternion(this Quaternion quaternionToTween, Action<Quaternion> setter, Vector3 target)
        {
            VTween<Quaternion, Vector3, QuaternionV3Applier> tween = new VTween<Quaternion, Vector3, QuaternionV3Applier>(setter, quaternionToTween.eulerAngles, target);
            tween.m_ApplierSettings = new QuaternionV3ApplierSettings();
            return tween;
        }

        // Quaternion Applier
        public static VTween<Quaternion, Quaternion, QuaternionApplier> TweenQuaternion(this Quaternion quaternionToTween, Action<Quaternion> setter, Quaternion target)
        {
            VTween<Quaternion, Quaternion, QuaternionApplier> tween = new VTween<Quaternion, Quaternion, QuaternionApplier>(setter, quaternionToTween, target);
            return tween;
        }

        // Color Applier
        public static VTween<Color, Color, ColorApplier> TweenColor(this Color colorToTween, Action<Color> setter, Color target)
        {
            VTween<Color, Color, ColorApplier> tween = new VTween<Color, Color, ColorApplier>(setter, colorToTween, target);
            return tween;
        }

        // Rect Applier
        public static VTween<Rect, Rect, RectApplier> TweenRect(this Rect rectToTween, Action<Rect> setter, Rect target)
        {
            VTween<Rect, Rect, RectApplier> tween = new VTween<Rect, Rect, RectApplier>(setter, rectToTween, target);
            return tween;
        }

        // String Applier
        public static VTween<string, string, TApplier> TweenString<TApplier>(this string stringToTween, Action<string> setter, string target) where TApplier : TweenApplier
        {
            VTween<string, string, TApplier> tween = new VTween<string, string, TApplier>(setter, stringToTween, target);
            return tween;
        }
    }
}

