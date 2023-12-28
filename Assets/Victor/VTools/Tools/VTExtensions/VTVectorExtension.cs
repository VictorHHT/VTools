using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Victor.Tools
{
    public static class VTVectorExtension
    {
        public static Vector2 Round(this Vector2 v)
        {
            return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
        }

        public static Vector3 Round(this Vector3 v)
        {
            return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        }

        public static Vector2 Snap(this Vector2 v, float xInterval, float yInterval)
        {
            return new Vector2(VTMath.Snap(v.x, xInterval), VTMath.Snap(v.y, yInterval));
        }

        public static Vector3 Snap(this Vector3 v, float xInterval, float yInterval, float zInterval)
        {
            return new Vector3(VTMath.Snap(v.x, xInterval), VTMath.Snap(v.y, yInterval), VTMath.Snap(v.z, zInterval));
        }

        public static Vector3 ToV3XY(this Vector2 v, float newZ = 0f)
        {
            return new Vector3(v.x, v.y, newZ);
        }

        public static Vector3 ToV3XZ(this Vector2 v, float newY = 0f)
        {
            return new Vector3(v.x, newY, v.y);
        }

        public static Vector2 ToV2FromXY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 ToV2FromXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector2 NewX(this Vector2 v, float newX)
        {
            return new Vector2(newX, v.y);
        }

        public static Vector2 NewY(this Vector2 v, float newY)
        {
            return new Vector2(v.x, newY);
        }

        public static Vector3 NewX(this Vector3 v, float newX)
        {
            return new Vector3(newX, v.y, v.z);
        }

        public static Vector3 NewY(this Vector3 v, float newY)
        {
            return new Vector3(v.x, newY, v.z);
        }

        public static Vector3 NewZ(this Vector3 v, float newZ)
        {
            return new Vector3(v.x, v.y, newZ);
        }

        public static Vector2 NewV(this Vector2 v, float newX, float newY)
        {
            return new Vector2(newX, newY);
        }

        public static Vector3 NewV(this Vector3 v, float newX, float newY, float newZ)
        {
            return new Vector3(newX, newY, newZ);
        }
    }

}
