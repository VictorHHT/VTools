using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public static class VTQuickActivate
    {
        [MenuItem("Tools/Victor/ToggleObjectActivation #A", false)]
        public static void ActivateGameObject()
        {
            bool hasNotActivate = false;
            foreach (var obj in Selection.gameObjects)
            {
                if (obj.activeSelf == false)
                {
                    hasNotActivate = true;
                }
            }

            if (hasNotActivate == true)
            {
                foreach (var obj in Selection.gameObjects)
                {
                    Undo.RecordObject(obj, "ToggleObjectActivation");
                    obj.SetActive(true);
                }
            }
            else
            {
                foreach (var obj in Selection.gameObjects)
                {
                    Undo.RecordObject(obj, "ToggleObjectActivation");
                    obj.SetActive(!obj.activeSelf);
                }
            }    

        }

    }
}

