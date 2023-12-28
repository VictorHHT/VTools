using UnityEditor;
using UnityEngine;

namespace Victor.Tools
{
    public struct VTSkinnedColor
    {
        private static readonly bool s_IsProSkin = EditorGUIUtility.isProSkin;
        private readonly Color m_LightColor;
        private readonly Color m_DarkColor;

        public Color color => s_IsProSkin ? m_DarkColor : m_LightColor;

        public VTSkinnedColor(Color lightColor, Color darkColor)
        {
            m_LightColor = lightColor;
            m_DarkColor = darkColor;
        }

        public VTSkinnedColor(Color lightAndDarkColor)
        {
            m_LightColor = lightAndDarkColor;
            m_DarkColor = lightAndDarkColor;
        }

        public static implicit operator VTSkinnedColor(Color color)
        {
            return new VTSkinnedColor(color);
        }

        public string ToHexString()
        {
            return VTColor.ColorToHex(color);
        }

        public override string ToString()
        {
            return ToHexString();
        }

        public VTSkinnedColor MultA(float alphaMultiplier)
        {
            return new VTSkinnedColor(m_LightColor.MultA(alphaMultiplier), m_DarkColor.MultA(alphaMultiplier));
        }

        public VTSkinnedColor MultA(float lightAlphaMultiplier, float darkAlphaMultiplier)
        {
            return new VTSkinnedColor(m_LightColor.MultA(lightAlphaMultiplier), m_DarkColor.MultA(darkAlphaMultiplier));
        }
    }
}
