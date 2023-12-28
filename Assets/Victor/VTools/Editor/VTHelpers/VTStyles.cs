using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public static class VTStyles
    {
        // Buttons
        public static readonly GUIStyle buttonLeft;
        public static readonly GUIStyle buttonMid;
        public static readonly GUIStyle buttonRight;
        public static readonly GUIStyle paddedButton;
        public static readonly GUIStyle spawnButtonOn;

        public static readonly GUIStyle toggleButtonOnOff;
        public static readonly GUIStyle toggleButtonIncludeExclude;

        public static readonly GUIStyle buttonSmallPadding;

        // Labels
        public static readonly GUIStyle centeredLabel;
        public static readonly GUIStyle centeredBoldLabel;
        public static readonly GUIStyle centeredWhiteBoldLabel;
        public static readonly GUIStyle leftAlignedBoldLabel;
        public static readonly GUIStyle centeredBlackBoldLabel;

        // Background
        public static readonly GUIStyle evenBackgroundTight;
        public static readonly GUIStyle oddBackgroundTight;

        public static readonly GUIStyle evenBackgroundLoose;
        public static readonly GUIStyle oddBackgroundLoose;

        // Meant to work with black bold label to enhance the contrast on odd rows
        public static readonly GUIStyle oddWhiteBackgroundTight;
        public static readonly GUIStyle oddWhiteBackgroundLoose;

        static VTStyles()
        {
            // Buttons
            buttonLeft = new GUIStyle("ButtonLeft");
            buttonMid = new GUIStyle("ButtonMid");
            buttonRight = new GUIStyle("ButtonRight");

            paddedButton = new GUIStyle("Button");
            paddedButton.padding = new RectOffset(10, 10, 5, 5);

            spawnButtonOn = new GUIStyle(GUI.skin.button);
            spawnButtonOn.normal.textColor = VTColorLibrary.white;
            spawnButtonOn.hover.textColor = VTColorLibrary.victorSilver;
            spawnButtonOn.active.textColor = VTColorLibrary.victorGreen;

            toggleButtonOnOff = new GUIStyle(GUI.skin.label);
            toggleButtonOnOff.normal.background = EditorGUIUtility.FindTexture("d_winbtn_mac_close_h@2x");
            toggleButtonOnOff.active.background = EditorGUIUtility.FindTexture("d_winbtn_mac_close_a@2x");
            toggleButtonOnOff.onNormal.background = EditorGUIUtility.FindTexture("d_winbtn_mac_max_h@2x");
            toggleButtonOnOff.onActive.background = EditorGUIUtility.FindTexture("d_winbtn_mac_max_a@2x");

            toggleButtonIncludeExclude = new GUIStyle(GUI.skin.label);
            toggleButtonIncludeExclude.normal.background = EditorGUIUtility.FindTexture("d_winbtn_mac_min_h@2x");
            toggleButtonIncludeExclude.active.background = EditorGUIUtility.FindTexture("d_winbtn_mac_min_a@2x");
            toggleButtonIncludeExclude.onNormal.background = EditorGUIUtility.FindTexture("d_winbtn_mac_max_h@2x");
            toggleButtonIncludeExclude.onActive.background = EditorGUIUtility.FindTexture("d_winbtn_mac_max_a@2x");
            buttonSmallPadding = new GUIStyle(GUI.skin.button);
            buttonSmallPadding.padding = new RectOffset(2, 2, 2, 2);

            // Labels
            centeredLabel = new GUIStyle(EditorStyles.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.margin = new RectOffset(3, 3, 3, 3);

            centeredBoldLabel = new GUIStyle(EditorStyles.boldLabel);
            centeredBoldLabel.alignment = TextAnchor.MiddleCenter;
            centeredBoldLabel.margin = new RectOffset(3, 3, 3, 3);

            centeredWhiteBoldLabel = new GUIStyle(EditorStyles.boldLabel);
            centeredWhiteBoldLabel.alignment = TextAnchor.MiddleCenter;
            centeredWhiteBoldLabel.margin = new RectOffset(3, 3, 3, 3);
            centeredWhiteBoldLabel.normal.textColor = Color.white;
            centeredWhiteBoldLabel.hover.textColor = Color.white;

            leftAlignedBoldLabel = new GUIStyle(EditorStyles.boldLabel);
            leftAlignedBoldLabel.alignment = TextAnchor.MiddleLeft;
            leftAlignedBoldLabel.margin = new RectOffset(3, 3, 3, 3);

            centeredBlackBoldLabel = new GUIStyle(EditorStyles.boldLabel);
            centeredBoldLabel.margin = new RectOffset(3, 3, 3, 3);
            centeredBlackBoldLabel.alignment = TextAnchor.MiddleCenter;
            centeredBlackBoldLabel.normal.textColor = Color.black;
            centeredBlackBoldLabel.hover.textColor = Color.black;


            // Backgrounds
            evenBackgroundTight = new GUIStyle("CN EntryBackEven");
            evenBackgroundTight.padding = new RectOffset(10, 6, 0, 0);
            evenBackgroundTight.margin = new RectOffset(0, 0, -5, -5);

            oddBackgroundTight = new GUIStyle("CN EntryBackOdd");
            oddBackgroundTight.padding = new RectOffset(10, 6, 0, 0);
            oddBackgroundTight.margin = new RectOffset(0, 0, -5, -5);

            evenBackgroundLoose = new GUIStyle("CN EntryBackEven");
            evenBackgroundLoose.padding = new RectOffset(10, 6, 8, 6);
            evenBackgroundLoose.margin = new RectOffset(0, 0, 0, 0);

            oddBackgroundLoose = new GUIStyle("CN EntryBackOdd");
            oddBackgroundLoose.padding = new RectOffset(10, 6, 0, 0);
            oddBackgroundLoose.margin = new RectOffset(0, 0, 0, 0);
            oddBackgroundLoose.border = new RectOffset(0, 0, 27, 29);

            oddWhiteBackgroundTight = new GUIStyle("CN EntryBackOdd");
            oddWhiteBackgroundTight.padding = new RectOffset(10, 6, 0, 0);
            oddWhiteBackgroundTight.margin = new RectOffset(0, 0, -5, -5);
            Texture2D silverBackgroundTightTexture = new Texture2D(1, 1);
            silverBackgroundTightTexture.SetPixel(0, 0, VTColorLibrary.victorSilver);
            silverBackgroundTightTexture.Apply();
            oddWhiteBackgroundTight.normal.background = silverBackgroundTightTexture;

            oddWhiteBackgroundLoose = new GUIStyle("CN EntryBackOdd");
            oddWhiteBackgroundLoose.padding = new RectOffset(10, 6, 2, 0);
            oddWhiteBackgroundLoose.margin = new RectOffset(0, 0, 0, 0);
            oddWhiteBackgroundLoose.border = new RectOffset(0, 0, 27, 29);
            Texture2D silverBackgroundLooseTexture = new Texture2D(1, 1);
            silverBackgroundLooseTexture.SetPixel(0, 0, VTColorLibrary.victorSilver);
            silverBackgroundLooseTexture.Apply();
            oddWhiteBackgroundLoose.normal.background = silverBackgroundTightTexture;
        }

        /// <summary>
        /// Get a white color if the index is even, black if odd
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static GUIStyle GetBlackWhiteCenteredLabel(int index)
        {
            return index % 2 == 0 ? VTStyles.centeredBoldLabel : VTStyles.centeredBlackBoldLabel;
        }

        /// <summary>
        /// Get a default background style based on whether the input is even or odd
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isTight"></param>
        /// <returns></returns>
        public static GUIStyle GetEvenOddBackground(int index, bool isTight = true)
        {
            if (isTight)
            {
                return index % 2 == 0 ? VTStyles.evenBackgroundTight : VTStyles.oddBackgroundTight;
            }
            else
            {
                return index % 2 == 0 ? VTStyles.evenBackgroundLoose : VTStyles.oddBackgroundLoose;
            }
        }

        /// <summary>
        /// Get a background style black or white based on whether the input is even or odd
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isTight"></param>
        /// <returns></returns>
        public static GUIStyle GetEvenOddBlackWhiteBackground(int index, bool isTight = true)
        {
            if (isTight)
            {
                return index % 2 == 0 ? VTStyles.evenBackgroundTight : VTStyles.oddWhiteBackgroundTight;
            }
            else
            {
                return index % 2 == 0 ? VTStyles.evenBackgroundLoose : VTStyles.oddWhiteBackgroundLoose;
            }
        }
    }
}

