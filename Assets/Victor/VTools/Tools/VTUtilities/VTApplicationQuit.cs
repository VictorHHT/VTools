using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Victor.Tools
{
    public class VTApplicationQuit : MonoBehaviour
    {
        [VTInspectorButton("VTQuit",VTColorLibrary.VTColors.VictorRed)]
        public bool quitBool;

        public static void Quit()
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

    }

}
