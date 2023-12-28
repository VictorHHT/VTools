using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.EditorTween
{
    public static class VTweenCreator
    {
        // Float Applier
        public static VTween<float, float, FloatApplier> TweenFloat(float floatToTween, Action<float> setter, float target)
        {
            VTween<float, float, FloatApplier> tween = new VTween<float, float, FloatApplier>(setter, floatToTween, target);
            tween.m_ApplierSettings = new FloatApplierSettings();
            return tween;
        }

        // Int Applier
        public static VTween<int, int, IntApplier> TweenInt(int intToTween, Action<int> setter, int target)
        {
            VTween<int, int, IntApplier> tween = new VTween<int, int, IntApplier>(setter, intToTween, target);
            tween.m_ApplierSettings = new IntApplierSettings();
            return tween;
        }

        // Vector2 Applier
        public static VTween<Vector2, Vector2, Vector2Applier> TweenVector2(Vector2 v2ToTween, Action<Vector2> setter, Vector2 target)
        {
            VTween<Vector2, Vector2, Vector2Applier> tween = new VTween<Vector2, Vector2, Vector2Applier>(setter, v2ToTween, target);
            tween.m_ApplierSettings = new Vector2ApplierSettings();
            return tween;
        }

        // Vector3 Applier
        public static VTween<Vector3, Vector3, Vector3Applier> TweenVector3(Vector3 v3ToTween, Action<Vector3> setter, Vector3 target)
        {
            VTween<Vector3, Vector3, Vector3Applier> tween = new VTween<Vector3, Vector3, Vector3Applier>(setter, v3ToTween, target);
            tween.m_ApplierSettings = new Vector3ApplierSettings();
            return tween;
        }

        // QuaternionV3 Applier
        public static VTween<Quaternion, Vector3, QuaternionV3Applier> TweenQuaternion(Quaternion quaternionToTween, Action<Quaternion> setter, Vector3 target)
        {
            VTween<Quaternion, Vector3, QuaternionV3Applier> tween = new VTween<Quaternion, Vector3, QuaternionV3Applier>(setter, quaternionToTween.eulerAngles, target);
            tween.m_ApplierSettings = new QuaternionV3ApplierSettings();
            return tween;
        }

        // Quaternion Applier
        public static VTween<Quaternion, Quaternion, QuaternionApplier> TweenQuaternion(Quaternion quaternionToTween, Action<Quaternion> setter, Quaternion target)
        {
            VTween<Quaternion, Quaternion, QuaternionApplier> tween = new VTween<Quaternion, Quaternion, QuaternionApplier>(setter, quaternionToTween, target);
            return tween;
        }

        // Color Applier
        public static VTween<Color, Color, ColorApplier> TweenColor(Color colorToTween, Action<Color> setter, Color target)
        {
            VTween<Color, Color, ColorApplier> tween = new VTween<Color, Color, ColorApplier>(setter, colorToTween, target);
            return tween;
        }

        // Rect Applier
        public static VTween<Rect, Rect, RectApplier> TweenRect(Rect rectToTween, Action<Rect> setter, Rect target)
        {
            VTween<Rect, Rect, RectApplier> tween = new VTween<Rect, Rect, RectApplier>(setter, rectToTween, target);
            return tween;
        }

        // String Applier
        // By using generics, we only need to implement various string tween appliers and don't need write multiple functions each with slightly different name
        public static VTween<string, string, TApplier> TweenString<TApplier>(string stringToTween, Action<string> setter, string target) where TApplier : TweenApplier
        {
            VTween<string, string, TApplier> tween = new VTween<string, string, TApplier>(setter, stringToTween, target);
            return tween;
        }
    }
}
