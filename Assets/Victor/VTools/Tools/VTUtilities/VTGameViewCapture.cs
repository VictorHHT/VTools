using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public class VTGameViewCapture : MonoBehaviour
    {
        public string fileName;

        public bool useSuperSize = false;
        public bool addDateTimeAsSuffix = true;
        [VTCondition("useSuperSize")]
        [VTRangeStep(2, 4)]
        public int superSizeRatio = 2;

        [SerializeField]
        [VTReadOnlyInEditorMode]
        [VTInspectorButton("CaptureGameView")]
        private bool CaptureGameViewButton;

        private void CaptureGameView()
        {
            StartCoroutine(CaptureGameViewCo());
        }

        private IEnumerator CaptureGameViewCo()
        {
            yield return new WaitForEndOfFrame();

            string outputFileName = null;

            if (addDateTimeAsSuffix)
            {
                outputFileName = fileName + " " + System.DateTime.Now.ToString("M:dd:yy (HH-mm-ss tt)") + ".png";
            }
            else
            {
                outputFileName = fileName + ".png";
            }

            if (!useSuperSize)
            {
                ScreenCapture.CaptureScreenshot(outputFileName);
            }
            else
            {
                ScreenCapture.CaptureScreenshot(outputFileName, superSizeRatio);
            }
        }
    }
}
