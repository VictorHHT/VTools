using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.EditorTween
{
    public abstract class ApplierSettings
    {
    }

    public class FloatApplierSettings : ApplierSettings
    {
        public float tweenInterval = 0f;
        public int fractionalDigitCount = 2;
    }

    public class IntApplierSettings : ApplierSettings
    {
        public int tweenInterval = 0;
    }

    public class Vector2ApplierSettings : ApplierSettings
    {
        public float tweenInterval = 0f;
    }

    public class Vector3ApplierSettings : ApplierSettings
    {
        public float tweenInterval = 0f;
    }

    public class QuaternionV3ApplierSettings : ApplierSettings
    {
        public float tweenInterval = 0f;
        public RotateMode rotateMode;
    }

}
