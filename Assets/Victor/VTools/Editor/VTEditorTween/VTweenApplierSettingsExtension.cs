using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.EditorTween
{
    public static class VTweenApplierSettingsExtension
    {
        public static VTween<float, float, FloatApplier> SetFloatApplierSettings(this VTween<float, float, FloatApplier> tween, float tweenInterval, int fractionalDigitCount = 2)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            FloatApplierSettings floatApplierSettings = (FloatApplierSettings)tween.m_ApplierSettings;
            floatApplierSettings.tweenInterval = tweenInterval;
            floatApplierSettings.fractionalDigitCount = fractionalDigitCount;
            return tween;
        }

        public static VTween<int, int, IntApplier> SetIntApplierSettings(this VTween<int, int, IntApplier> tween, int tweenInterval)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            IntApplierSettings intApplierSettings = (IntApplierSettings)tween.m_ApplierSettings;
            intApplierSettings.tweenInterval = tweenInterval;
            return tween;
        }

        public static VTween<Vector2, Vector2, Vector2Applier> SetVector2ApplierSettings(this VTween<Vector2, Vector2, Vector2Applier> tween, int tweenInterval)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            Vector2ApplierSettings vector2ApplierSettings = (Vector2ApplierSettings)tween.m_ApplierSettings;
            vector2ApplierSettings.tweenInterval = tweenInterval;
            return tween;
        }

        public static VTween<Vector3, Vector3, Vector3Applier> SetVector3ApplierSettings(this VTween<Vector3, Vector3, Vector3Applier> tween, int tweenInterval)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            Vector3ApplierSettings vector3ApplierSettings = (Vector3ApplierSettings)tween.m_ApplierSettings;
            vector3ApplierSettings.tweenInterval = tweenInterval;
            return tween;
        }

        public static VTween<Quaternion, Vector3, QuaternionV3Applier> SetQuaternionV3ApplierSettings(this VTween<Quaternion, Vector3, QuaternionV3Applier> tween, RotateMode rotateMode, float tweenInterval = 0)
        {
            if (tween == null || tween.m_ModificationLocked)
            {
                return tween;
            }

            QuaternionV3ApplierSettings quaternionV3ApplierSettings = (QuaternionV3ApplierSettings)tween.m_ApplierSettings;
            quaternionV3ApplierSettings.rotateMode = rotateMode;
            quaternionV3ApplierSettings.tweenInterval = tweenInterval;
            return tween;
        }
    }
}