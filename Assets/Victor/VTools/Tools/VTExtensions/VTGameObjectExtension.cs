using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTGameObjectExtension
    {
        /// <summary>
        /// Recursively set active states of childrens including self (Optional)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="active"></param>
        public static void SetActiveStateRecursive(this GameObject parent, bool active, bool includeSelf = true)
        {
            if (includeSelf)
            {
                parent.SetActive(active);
            }

            foreach (Transform child in parent.transform)
            {
                child.gameObject.SetActiveStateRecursive(active, true);
            }
        }

        /// <summary>
        /// Disable all scripts (MonoBehaviors) attached to the current GameObject
        /// </summary>
        /// <param name="obj"></param>
        public static void DisableAllScriptsRecursive(this GameObject obj)
        {
            foreach (MonoBehaviour script in obj.GetComponents<MonoBehaviour>())
            {
                script.enabled = false;
            }

            // Recursively disable scripts on children
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                DisableAllScriptsRecursive(obj.transform.GetChild(i).gameObject);
            }
        }
    }
}