using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Victor.Tools;

namespace Victor.EditorTween
{
    public abstract class TweenApplier
    {
    }

    public abstract class Applier<T1, T2> : TweenApplier
    {
        public abstract void GenericTween(Action<T1> setter, T2 startValue, T2 targetValue, float tweenPosition, ApplierSettings applierSettings);
    }

    public class FloatApplier : Applier<float, float>
    {
        public override void GenericTween(Action<float> setter, float startValue, float targetValue, float tweenPosition, ApplierSettings applierSettings)
        {
            FloatApplierSettings floatApplierSettings = (FloatApplierSettings)applierSettings;
            float tweenedValue;

            if (floatApplierSettings.tweenInterval > 0)
            {
                tweenedValue = VTMath.Round(VTMath.Snap(Mathf.LerpUnclamped(startValue, targetValue, tweenPosition), floatApplierSettings.tweenInterval), floatApplierSettings.fractionalDigitCount);
            }
            else
            {
                tweenedValue = VTMath.Round(Mathf.LerpUnclamped(startValue, targetValue, tweenPosition), floatApplierSettings.fractionalDigitCount);
            }

            setter(tweenedValue);
        }
    }

    public class IntApplier : Applier<int, int>
    {
        public override void GenericTween(Action<int> setter, int startValue, int targetValue, float tweenPosition, ApplierSettings applierSettings)
        {
            int tweenedValue;
            IntApplierSettings intApplierSettings = (IntApplierSettings)applierSettings;

            if (intApplierSettings.tweenInterval > 0)
            {
                tweenedValue = Mathf.RoundToInt(VTMath.Snap(Mathf.LerpUnclamped(startValue, targetValue, tweenPosition), intApplierSettings.tweenInterval));
            }
            else
            {
                tweenedValue = Mathf.RoundToInt(Mathf.LerpUnclamped(startValue, targetValue, tweenPosition));
            }

            setter(tweenedValue);
        }
    }

    public class Vector2Applier : Applier<Vector2, Vector2>
    {
        public override void GenericTween(Action<Vector2> setter, Vector2 startValue, Vector2 targetValue, float tweenPosition, ApplierSettings applierSettings)
        {
            Vector2ApplierSettings vector2ApplierSettings = (Vector2ApplierSettings)applierSettings;
            Vector2 tweenedValue = Vector2.LerpUnclamped(startValue, targetValue, tweenPosition);

            if (vector2ApplierSettings.tweenInterval > 0)
            {
                tweenedValue = VTVector2.Snap(tweenedValue, vector2ApplierSettings.tweenInterval);
            }

            setter(tweenedValue);
        }
    }

    public class Vector3Applier : Applier<Vector3, Vector3>
    {
        public override void GenericTween(Action<Vector3> setter, Vector3 startValue, Vector3 targetValue, float tweenPosition, ApplierSettings applierSettings)
        {
            Vector3ApplierSettings vector3ApplierSettings = (Vector3ApplierSettings)applierSettings;
            Vector3 tweenedValue = Vector3.LerpUnclamped(startValue, targetValue, tweenPosition);

            if (vector3ApplierSettings.tweenInterval > 0)
            {
                tweenedValue = VTVector3.Snap(tweenedValue, vector3ApplierSettings.tweenInterval);
            }

            setter(tweenedValue);
        }
    }

    public class QuaternionV3Applier : Applier<Quaternion, Vector3>
    {
        public override void GenericTween(Action<Quaternion> setter, Vector3 startValue, Vector3 targetValue, float tweenPosition, ApplierSettings applierSettings)
        {
            QuaternionV3ApplierSettings quaternionV3ApplierSettings = (QuaternionV3ApplierSettings)applierSettings;

            // Fast Rotate Mode
            if (quaternionV3ApplierSettings.rotateMode == RotateMode.Fast)
            {
                Vector3 newTarget = ClampRotationValue(targetValue);
                Vector3 rotationChange = newTarget - startValue;
                rotationChange = AdjustForShortestRotation(rotationChange);

                if (quaternionV3ApplierSettings.tweenInterval > 0)
                {
                    Vector3 snappedRotation = VTVector3.Snap(startValue + rotationChange * tweenPosition, quaternionV3ApplierSettings.tweenInterval);
                    setter(Quaternion.Euler(snappedRotation));
                }
                else
                {
                    setter(Quaternion.Euler(startValue + rotationChange * tweenPosition));
                }
            }
            else // Slow Rotate Mode
            {
                Vector3 rotationChange = targetValue - startValue;

                if (quaternionV3ApplierSettings.tweenInterval > 0)
                {
                    Vector3 snappedRotation = VTVector3.Snap(startValue + rotationChange * tweenPosition, quaternionV3ApplierSettings.tweenInterval);
                    setter(Quaternion.Euler(snappedRotation));
                }
                else
                {
                    setter(Quaternion.Euler(startValue + rotationChange * tweenPosition));
                }
            }
        }

        private Vector3 ClampRotationValue(Vector3 rotation)
        {
            rotation.x = VTMath.ClampAngle(rotation.x, 0, 360);
            rotation.y = VTMath.ClampAngle(rotation.y, 0, 360);
            rotation.z = VTMath.ClampAngle(rotation.z, 0, 360);

            return rotation;
        }

        private Vector3 AdjustForShortestRotation(Vector3 rotation)
        {
            rotation.x = ShortestRotation(rotation.x);
            rotation.y = ShortestRotation(rotation.y);
            rotation.z = ShortestRotation(rotation.z);
            return rotation;
        }

        private float ShortestRotation(float angle)
        {
            float absoluteAngle = Mathf.Abs(angle);

            if (absoluteAngle > 180f)
            {
                angle = (angle > 0f) ? -(360f - absoluteAngle) : (360f - absoluteAngle);
            }

            return angle;
        }
    }

    public class QuaternionApplier : Applier<Quaternion, Quaternion>
    {
        public override void GenericTween(Action<Quaternion> setter, Quaternion startValue, Quaternion targetValue, float tweenPosition, ApplierSettings applierSettings)
        {
            Quaternion tweenedValue = Quaternion.LerpUnclamped(startValue, targetValue, tweenPosition);
            setter(tweenedValue);
        }
    }

    public class ColorApplier : Applier<Color, Color>
    {
        public override void GenericTween(Action<Color> setter, Color startValue, Color targetValue, float tweenPosition, ApplierSettings applierSettings)
        {
            Color tweenedValue = Color.LerpUnclamped(startValue, targetValue, tweenPosition);
            setter(tweenedValue);
        }
    }

    public class RectApplier : Applier<Rect, Rect>
    {
        public override void GenericTween(Action<Rect> setter, Rect startValue, Rect targetValue, float tweenPosition, ApplierSettings applierSettings)
        {
            float tweenedWidth = Mathf.LerpUnclamped(startValue.width, targetValue.width, tweenPosition);
            float tweenedHeight = Mathf.LerpUnclamped(startValue.height, targetValue.height, tweenPosition);
            float tweenedX = Mathf.LerpUnclamped(startValue.x, targetValue.x, tweenPosition);
            float tweenedY = Mathf.LerpUnclamped(startValue.y, targetValue.y, tweenPosition);
            Rect tweenedValue = new Rect(tweenedX, tweenedY, tweenedWidth, tweenedHeight);
            setter(tweenedValue);
        }
    }

    public class StringAppendApplier : Applier<string, string>
    {
        public override void GenericTween(Action<string> setter, string startValue, string targetValue, float tweenPosition, ApplierSettings applierSettings)
        {
            int startLength = startValue.Length;
            int targetLength = targetValue.Length;
            int tweenedLength = Mathf.FloorToInt(Mathf.LerpUnclamped(startLength, targetLength, tweenPosition));
            string tweenedValue = targetValue.Substring(0, tweenedLength);
            setter(tweenedValue);
        }
    }
}

