using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VTGameObject
{
    public static void DestroyAllChildren(GameObject parentObject)
    {
        // Check if the parent object is valid
        if (parentObject != null)
        {
            // Get the Transform component of the parent object
            Transform parentTransform = parentObject.transform;

            // Destroy all children of the parent object
            for (int i = parentTransform.childCount - 1; i >= 0; i--)
            {
                Transform childTransform = parentTransform.GetChild(i);
                Object.Destroy(childTransform.gameObject);
            }
        }
    }

    public static void DestroyAllChildrenImmediate(GameObject parentObject)
    {
        // Check if the parent object is valid
        if (parentObject != null)
        {
            // Get the Transform component of the parent object
            Transform parentTransform = parentObject.transform;

            // Destroy all children of the parent object
            for (int i = parentTransform.childCount - 1; i >= 0; i--)
            {
                Transform childTransform = parentTransform.GetChild(i);
                Object.DestroyImmediate(childTransform.gameObject);
            }
        }
    }
}
