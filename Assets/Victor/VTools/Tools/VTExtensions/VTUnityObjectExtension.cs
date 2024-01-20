using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Victor.Tools
{
    public static class VTUnityObjectExtension
    {
        /// <summary>
        /// Converts a Unity pseudo-null to a real null, facilitating the use of null conditional and null coalescing operators.
        /// </summary>
        public static T ConvertPseudoNull<T>(this T unityObject) where T : UnityObject
        {
            // This method is designed to handle Unity pseudo-nulls
            // Use example:
            // 1: renderer.GetComponent<MeshFilter>.ConvertPseudoNull()?.sharedMesh;
            // 2: <destroyedObject>.ConvertPseudoNull() ?? otherObject

            // If the UnityObject is already null, no conversion needed.
            if (unityObject == null)
            {
                return null;
            }

            return unityObject;
        }

    }
}