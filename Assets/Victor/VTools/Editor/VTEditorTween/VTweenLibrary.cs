using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Victor.Tools;

namespace Victor.EditorTween
{
    public enum EaseType
    {
        // Ease InOut
        EaseInOutQuad = 1,

        EaseInOutCubic = 2,

        EaseInOutQuart = 3,

        EaseInOutQuint = 4,

        EaseInOutExpo = 5,

        EaseInOutSine = 6,

        EaseInOutCirc = 7,

        EaseInOutBack = 8,

        EaseInOutElastic = 9,

        EaseInOutBounce = 10,

        // Ease In
        EaseInQuad = 11,

        EaseInCubic = 12,

        EaseInQuart = 13,

        EaseInQuint = 14,

        EaseInExpo = 15,

        EaseInSine = 16,

        EaseInCirc = 17,

        EaseInBack = 18,

        EaseInElastic = 19,

        EaseInBounce = 20,

        // Ease Out
        EaseOutQuad = 21,

        EaseOutCubic = 22,

        EaseOutQuart = 23,

        EaseOutQuint = 24,

        EaseOutExpo = 25,

        EaseOutSine = 26,

        EaseOutCirc = 27,

        EaseOutBack = 28,

        EaseOutElastic = 29,

        EaseOutBounce = 30,

        // Linear
        Linear = 31
    }


    public static class VTweenLibrary
    {
        public static float GetTweenedValue01(EaseType easeType, float time, VTweenCore tween)
        {
            float overshootOrAmplitude = tween.m_OvershootOrAmplitude;
            float duration = tween.m_Duration;
            float cycleDuration = tween.m_CycleDuration;

            switch (easeType)
            {
                case EaseType.EaseInOutQuad:
                    return EaseInOutQuad(time);
                case EaseType.EaseInOutCubic:
                    return EaseInOutCubic(time);
                case EaseType.EaseInOutQuart:
                    return EaseInOutQuart(time);
                case EaseType.EaseInOutQuint:
                    return EaseInOutQuint(time);
                case EaseType.EaseInOutExpo:
                    return EaseInOutExpo(time);
                case EaseType.EaseInOutSine:
                    return EaseInOutSine(time);
                case EaseType.EaseInOutCirc:
                    return EaseInOutCirc(time);
                case EaseType.EaseInOutBack:
                    return EaseInOutBack(time, overshootOrAmplitude);
                case EaseType.EaseInOutElastic:
                    return EaseInOutElastic(time, duration, overshootOrAmplitude, cycleDuration);
                case EaseType.EaseInOutBounce:
                    return EaseInOutBounce(time);
                case EaseType.EaseInQuad:
                    return EaseInQuad(time);
                case EaseType.EaseInCubic:
                    return EaseInCubic(time);
                case EaseType.EaseInQuart:
                    return EaseInQuart(time);
                case EaseType.EaseInQuint:
                    return EaseInQuint(time);
                case EaseType.EaseInExpo:
                    return EaseInExpo(time);
                case EaseType.EaseInSine:
                    return EaseInSine(time);
                case EaseType.EaseInCirc:
                    return EaseInCirc(time);
                case EaseType.EaseInBack:
                    return EaseInBack(time, overshootOrAmplitude);
                case EaseType.EaseInElastic:
                    return EaseInElastic(time, duration, overshootOrAmplitude, cycleDuration);
                case EaseType.EaseInBounce:
                    return EaseInBounce(time);
                case EaseType.EaseOutQuad:
                    return EaseOutQuad(time);
                case EaseType.EaseOutCubic:
                    return EaseOutCubic(time);
                case EaseType.EaseOutQuart:
                    return EaseOutQuart(time);
                case EaseType.EaseOutQuint:
                    return EaseOutQuint(time);
                case EaseType.EaseOutExpo:
                    return EaseOutExpo(time);
                case EaseType.EaseOutSine:
                    return EaseOutSine(time);
                case EaseType.EaseOutCirc:
                    return EaseOutCirc(time);
                case EaseType.EaseOutBack:
                    return EaseOutBack(time, overshootOrAmplitude);
                case EaseType.EaseOutElastic:
                    return EaseOutElastic(time, duration, overshootOrAmplitude, cycleDuration);
                case EaseType.EaseOutBounce:
                    return EaseOutBounce(time);
                case EaseType.Linear:
                    return Linear(time);
                default:
                    return Linear(time);
            }
        }

        public static EaseType GetInverseEaseType(EaseType easeType)
        {
            int value = (int)easeType;

            if (value > 10 && value <= 20)
            {
                return (EaseType)(value + 10);
            }
            else if (value > 20 && value <= 30)
            {
                return (EaseType)(value - 10);
            }
            else
            {
                // value >= 1 && value <= 10 || value == 31
                return easeType;
            }
        }

        // Ease InOut
        private static float EaseInOutSine(float time)
        {
            return -(Mathf.Cos(Mathf.PI * time) - 1f) / 2f;
        }

        private static float EaseInOutQuad(float time)
        {
            return time < 0.5f ? 2 * time * time : 1 - Mathf.Pow(-2 * time + 2, 2) / 2;
        }

        private static float EaseInOutCubic(float time)
        {
            return time < 0.5f ? 4 * time * time * time : 1 - Mathf.Pow(-2 * time + 2, 3) / 2;
        }

        private static float EaseInOutQuart(float time)
        {
            return time < 0.5 ? 8 * time * time * time * time : 1 - Mathf.Pow(-2 * time + 2, 4) / 2;
        }

        private static float EaseInOutQuint(float time)
        {
            return time < 0.5f ? 16 * time * time * time * time * time : 1 - Mathf.Pow(-2 * time + 2, 5) / 2;
        }

        private static float EaseInOutExpo(float time)
        {
            return time == 0 ? 0 : time == 1 ? 1 : time < 0.5 ? Mathf.Pow(2, 20 * time - 10) / 2 : (2 - Mathf.Pow(2, -20 * time + 10)) / 2;
        }

        private static float EaseInOutCirc(float time)
        {
            return time < 0.5 ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * time, 2))) / 2 : (Mathf.Sqrt(1 - Mathf.Pow(-2 * time + 2, 2)) + 1) / 2;
        }

        private static float EaseInOutBack(float time, float easeBackOvershoot)
        {
            easeBackOvershoot = Mathf.Max(0, easeBackOvershoot);
            easeBackOvershoot = VTMath.Remap(easeBackOvershoot, 0f, 1f, 0f, 0.3f);
            return time < 0.5 ?
            Mathf.Pow(time, 3f) - time * easeBackOvershoot * Mathf.Sin(time * Mathf.PI) :
            1 - (Mathf.Pow((1 - time), 3f) - (1 - time) * easeBackOvershoot * Mathf.Sin((1 - time) * Mathf.PI));
        }

        private static float EaseInOutElastic(float time, float duration, float amplitude, float oscillationCycleDuration)
        {
            if (time == 0 || time == 1)
            {
                return time;
            }

            if (oscillationCycleDuration == 0f)
            {
                oscillationCycleDuration = duration * 0.45000002f;
            }
            
            if (time < 0.5f)
            {
                amplitude = Mathf.Max(0, amplitude);
                amplitude = VTMath.Remap(amplitude, 0f, 1f, 0f, 0.2f);

                float phaseOffset = oscillationCycleDuration / 6.2831855f * (float)Math.Asin((double)(1f / amplitude));
                return -(amplitude * (float)Math.Pow(2.0, (double)(10f * time - 10f)) * (float)Math.Sin((double)((time * duration - phaseOffset) * 6.2831855f / oscillationCycleDuration)));
            }
            else
            {
                amplitude = Mathf.Max(0, amplitude);
                amplitude = VTMath.Remap(amplitude, 0f, 1f, 0f, 0.2f);

                float phaseOffset = oscillationCycleDuration / 6.2831855f * (float)Math.Asin((double)(1f / amplitude));
                return amplitude * (float)Math.Pow(2.0, (double)(-10f * time)) * (float)Math.Sin((double)((time * duration - phaseOffset) * 6.2831855f / oscillationCycleDuration)) + 1f;
            }
        }

        private static float EaseInOutBounce(float time)
        {
            return time < 0.5f ? (1 - EaseOutBounce(1 - 2 * time)) / 2 : (1 + EaseOutBounce(2 * time - 1)) / 2;
        }

        // Ease In
        private static float EaseInQuad(float time)
        {
            return time * time;
        }

        private static float EaseInCubic(float time)
        {
            return time * time * time;
        }

        private static float EaseInQuart(float time)
        {
            return time * time * time * time;
        }

        private static float EaseInQuint(float time)
        {
            return time * time * time * time * time;
        }

        private static float EaseInExpo(float time)
        {
            return time == 0 ? 0 : Mathf.Pow(2, 10 * time - 10);
        }

        private static float EaseInCirc(float time)
        {
            return 1 - Mathf.Sqrt(1 - Mathf.Pow(time, 2));
        }

        private static float EaseInBack(float time, float easeBackOvershoot)
        {
            easeBackOvershoot = Mathf.Max(0, easeBackOvershoot);
            easeBackOvershoot = VTMath.Remap(easeBackOvershoot, 0f, 1f, 0f, 0.3f);
            return Mathf.Pow(time, 3f) - time * easeBackOvershoot * Mathf.Sin(time * Mathf.PI);
        }

        private static float EaseInElastic(float time, float duration, float amplitude, float oscillationCycleDuration)
        {
            if (time == 0 || time == 1)
            {
                return time;
            }

            amplitude = Mathf.Max(1, amplitude);

            if (oscillationCycleDuration == 0f)
            {
                oscillationCycleDuration = duration * 0.3f;
            }

            float phaseOffset = oscillationCycleDuration / 6.2831855f * (float)Math.Asin((double)(1f / amplitude));
            return -(amplitude * (float)Math.Pow(2.0, (double)(10f * time - 10f)) * (float)Math.Sin((double)((time - phaseOffset) * 6.2831855f / oscillationCycleDuration)));
        }

        private static float EaseInBounce(float time)
        {
            return 1 - EaseOutBounce(1 - time);
        }

        // Ease Out
        private static float EaseOutSine(float time)
        {
            return Mathf.Sin((time * Mathf.PI) / 2f);
        }

        private static float EaseOutQuad(float time)
        {
            return 1 - (1 - time) * (1 - time);
        }

        private static float EaseOutCubic(float time)
        {
            return 1 - Mathf.Pow(1 - time, 3);
        }

        private static float EaseOutQuart(float time)
        {
            // 1 - time causes the function being evaluated from right to left,
            // because the value of the function at x = 1 is 0 and at x = 0 is 1
            return 1 - Mathf.Pow(1 - time, 4);
        }

        private static float EaseOutQuint(float time)
        {
            return 1 - Mathf.Pow(1 - time, 5);
        }

        private static float EaseOutExpo(float time)
        {
            return time == 1 ? 1 : 1 - Mathf.Pow(2, -10 * time);
        }

        private static float EaseInSine(float time)
        {
            return 1f - Mathf.Cos((time * Mathf.PI) / 2f);
        }

        private static float EaseOutCirc(float time)
        {
            return Mathf.Sqrt(1 - Mathf.Pow(time - 1, 2));
        }

        private static float EaseOutBack(float time, float easeBackOvershoot)
        {
            easeBackOvershoot = Mathf.Max(0, easeBackOvershoot);
            easeBackOvershoot = VTMath.Remap(easeBackOvershoot, 0f, 1f, 0f, 0.3f);
            return 1 - (Mathf.Pow((1 - time), 3f) - (1 - time) * easeBackOvershoot * Mathf.Sin((1 - time) * Mathf.PI));
        }

        private static float EaseOutElastic(float time, float duration, float amplitude, float oscillationCycleDuration)
        {
            if (time == 0 || time == 1)
            {
                return time;
            }

            amplitude = Mathf.Max(1, amplitude);

            if (oscillationCycleDuration == 0f)
            {
                oscillationCycleDuration = duration * 0.3f;
            }

            float phaseOffset = oscillationCycleDuration / 6.2831855f * (float)Math.Asin((double)(1f / amplitude));

            return amplitude * (float)Math.Pow(2.0, (double)(-10f * time)) * (float)Math.Sin((double)((time * duration - phaseOffset) * 6.2831855f / oscillationCycleDuration)) + 1f;
        }

        private static float EaseOutBounce(float time)
        {
            if (time < 1 / 2.75f)
                return 7.5625f * time * time;
            else if (time < 2 / 2.75f)
                return 7.5625f * (time -= 1.5f / 2.75f) * time + 0.75f;
            else if (time < 2.5f / 2.75f)
                return 7.5625f * (time -= 2.25f / 2.75f) * time + 0.9375f;
            else
                return 7.5625f * (time -= 2.625f / 2.75f) * time + 0.984375f;
        }

        private static float Linear(float time)
        {
            return time;
        }
    }
}
