using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTColorLibrary
    {
        public enum VTColors { White, Black, VictorBlue, VictorRed, VictorGreen, VictorYellow, VictorGold, VictorSilver, VictorPink, VictorYellowGreen, VictorWarning, VictorError, DefaultBackground};

        public static Color white = VTColor.HexToColor("ffffff");
        public static Color black = VTColor.HexToColor("201E1F");
        public static Color victorWarning = VTColor.HexToColor("FFC009");
        public static Color victorError = VTColor.HexToColor("FF6E3F");
        public static Color victorBlue = VTColor.HexToColor("249fff");
        public static Color victorRed = VTColor.HexToColor("ff2431");
        public static Color victorGreen = VTColor.HexToColor("24ff85");
        public static Color victorYellow = VTColor.HexToColor("fff204");
        public static Color victorGold = VTColor.HexToColor("ffd700");
        public static Color victorSilver = VTColor.HexToColor("c0c0c0");
        public static Color victorPink = VTColor.HexToColor("ffc0cb");
        public static Color victorYellowGreen = VTColor.HexToColor("9acd32");

        public static Color defaultBackGround = new Color(0.76f, 0.76f, 0.76f, 1);
        // _END -------------------------------------------------------------------------------------------------

        public static Color GetRandomVTColor()
        {
            int colorIndex = Random.Range(0, (int)VTColors.DefaultBackground);
            return GetColor((VTColors)colorIndex);
        }

        public static Color GetColor(VTColors vtColor)
        {
            switch (vtColor)
            {
                case VTColors.White: return white;
                case VTColors.Black: return black;
                case VTColors.VictorBlue: return victorBlue;
                case VTColors.VictorRed: return victorRed;
                case VTColors.VictorGreen: return victorGreen;
                case VTColors.VictorYellow: return victorYellow;
                case VTColors.VictorGold: return victorGold;
                case VTColors.VictorSilver: return victorSilver;
                case VTColors.VictorPink: return victorPink;
                case VTColors.VictorYellowGreen: return victorYellowGreen;
                case VTColors.VictorWarning: return victorWarning;
                case VTColors.VictorError: return victorError;
                case VTColors.DefaultBackground: return defaultBackGround;
                default: return white;
            }
        }

    }
}