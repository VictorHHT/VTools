using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTransformExtension
    {
        public static void SetGlobalScale(this Transform transform, Vector3 newGlobalScale)
        {
            // Get global scale up to the parent
            transform.localScale = Vector3.one;
            var matrix = transform.worldToLocalMatrix;
            matrix.SetColumn(0, new Vector4(matrix.GetColumn(0).magnitude, 0, 0, 0));
            matrix.SetColumn(1, new Vector4(0, matrix.GetColumn(1).magnitude, 0, 0));
            matrix.SetColumn(2, new Vector4(0, 0, matrix.GetColumn(2).magnitude, 0));
            matrix.SetColumn(3, new Vector4(0, 0, 0, 1));
            transform.localScale = matrix.MultiplyPoint(newGlobalScale);
        }

        /// <summary>
        /// Rotate angle degrees around the X axis, relative to self or world space
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="relativeSpace"></param>
        public static void RotateX(this Transform transform, float angle, Space relativeSpace = Space.World)
        {
            Vector3 rotateVec = new Vector3(angle, 0, 0);
            transform.Rotate(rotateVec, relativeSpace);
        }

        /// <summary>
        /// Rotate angle degrees around the Y axis, relative to self or world space
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="relativeSpace"></param>
        public static void RotateY(this Transform transform, float angle, Space relativeSpace = Space.World)
        {
            Vector3 rotateVec = new Vector3(0, angle, 0);
            transform.Rotate(rotateVec, relativeSpace);
        }

        /// <summary>
        /// Rotate angle degrees around the Z axis, relative to self or world space
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="relativeSpace"></param>
        public static void RotateZ(this Transform transform, float angle, Space relativeSpace = Space.World)
        {
            Vector3 rotateVec = new Vector3(0, 0, angle);
            transform.Rotate(rotateVec, relativeSpace);
        }

        /// <summary>
        /// Set X rotation to angle realative to self or world space
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="relativeSpace"></param>
        public static void SetRotationX(this Transform transform, float angle, Space relativeSpace = Space.World)
        {
            Vector3 rotationVec;
            // If set rotation in world space
            if (relativeSpace == Space.World)
            {
                rotationVec = new Vector3(angle, transform.rotation.y, transform.rotation.z);
                Quaternion q = Quaternion.Euler(rotationVec);
                transform.rotation = q;
            }
            else // If set rotation in local space
            {
                rotationVec = new Vector3(angle, transform.localRotation.y, transform.localRotation.z);
                Quaternion q = Quaternion.Euler(rotationVec);
                transform.localRotation = q;
            }
            
        }

        /// <summary>
        /// Set Y rotation value to angle realative to self or world space
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="relativeSpace"></param>
        public static void SetRotationY(this Transform transform, float angle, Space relativeSpace = Space.World)
        {
            Vector3 rotationVec;
            if(relativeSpace == Space.World)
            {
                rotationVec = new Vector3(transform.rotation.x, angle, transform.rotation.z);
            }
            else
            {
                rotationVec = new Vector3(transform.localRotation.x, angle, transform.localRotation.z);
            }
            Quaternion q = Quaternion.Euler(rotationVec);
            transform.rotation = q;
        }

        /// <summary>
        /// Set Z rotation value to angle realative to self or world space
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="relativeSpace"></param>
        public static void SetRotationZ(this Transform transform, float angle, Space relativeSpace = Space.World)
        {
            Vector3 rotationVec;
            if (relativeSpace == Space.World)
            {
                rotationVec = new Vector3(transform.rotation.x, transform.rotation.y, angle);
            }
            else
            {
                rotationVec = new Vector3(transform.localRotation.x, transform.localRotation.y, angle);
            }
            Quaternion q = Quaternion.Euler(rotationVec);
            transform.rotation = q;
        }

        public static int GetHierarchyDepth(this Transform transform)
        {
            int depth = 0;
            Transform parent = transform.parent;

            while (parent != null)
            {
                depth++;
                parent = parent.parent;
            }

            return depth;
        }
    }
}

