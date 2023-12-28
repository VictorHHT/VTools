using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public class VTimeScale : MonoBehaviour
    {
        [VTInspectorButton("SetTimeScale", VTColorLibrary.VTColors.White)]
        public bool SetTimeScaleBool;
        [VTInspectorButton("ResetTimeScale", VTColorLibrary.VTColors.VictorSilver)]
        public bool ResetTimeScaleBool;
        public float TargetTimescale = 1;
        [VTReadOnly]
        public float CurrentTimescale = 1;

        void ResetTimeScale()
        {
            Time.timeScale = 1.0f;
            CurrentTimescale = 1;
            Debug.Log("Time scale has been reset");
        }

        void SetTimeScale()
        {
            Time.timeScale = TargetTimescale;
            CurrentTimescale = Time.timeScale;
            Debug.Log("Time Scale is set to:" + TargetTimescale);
        }
    }
}

