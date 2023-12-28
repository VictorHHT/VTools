using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public class VTCopyAssetName
    {
        /// <summary>
        /// Let the user copy asset name in the project panel
        /// </summary>
        [MenuItem("Assets/Copy Asset Name", false, 1)] //For Assets in the Project tab        
        public static void CopyName()
        {
            string actualName = "";

            //Get active selected objecte's path relative to the Assets folder
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            string[] pathString = path.Split(new char[] { '/' });

            //Get the name after the last '/' separater
            string name = pathString[pathString.Length - 1];

            //If the asset has a suffix followed by .  (ex: .cs)
            if (name.IndexOf('.') != -1)
            {
                actualName = name.Remove(name.IndexOf('.'));
            }
            else
            {
                actualName = name;
            }

            GUIUtility.systemCopyBuffer = actualName;
        }

        /// <summary>
        /// Let the user copy object name in the Hierarchy panel
        /// </summary>
        [MenuItem("GameObject/Copy GameObject Name", false, -1)] //For GameObjects in the Hierarchy
        public static void CopyGoName()
        {
            string name = Selection.activeGameObject.name;

            GUIUtility.systemCopyBuffer = name;
        }

        /// <summary>
        /// If the current selected asset or gameobject number is greater than 1, we disable the MenuItem
        /// </summary>
        /// <returns></returns>
        [MenuItem("Assets/Copy Asset Name", true, 1)] //For Assets in the Project tab        
        [MenuItem("GameObject/Copy GameObject Name", true, -1)]
        private static bool ValidateCopy()
        {
            return Selection.count == 1 ? true : false;
        }
    }

}
