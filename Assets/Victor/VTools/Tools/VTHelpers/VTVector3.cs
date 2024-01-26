using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Victor.Tools
{
    public static class VTVector3
    {
        /// <summary>
        /// Defines the possible positions of a point relative to a line.
        /// </summary>
        public enum PointPosition { Left, Right, OnLine };

        /// <summary>
        /// Rounds the components of a Vector3 to the nearest integer.
        /// </summary>
        /// <param name="vector">The Vector3 to round.</param>
        /// <returns>The rounded Vector3.</returns>
        public static Vector3 Round(Vector3 vector)
        {
            return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
        }

        /// <summary>
        /// Rounds the components of a 3D vector to the specified number of decimal points.
        /// </summary>
        /// <param name="vector">The vector to round.</param>
        /// <param name="decimalPoints">The number of decimal points.</param>
        /// <returns>The rounded vector.</returns>
        public static Vector3 Round(Vector3 vector, int decimalPoints)
        {
            return new Vector3(VTMath.Round(vector.x, decimalPoints), VTMath.Round(vector.y, decimalPoints), VTMath.Round(vector.z, decimalPoints));
        }

        /// <summary>
        /// Snaps the components of a Vector3 to the specified interval.
        /// </summary>
        /// <param name="vector">The Vector3 to snap.</param>
        /// <param name="interval">The interval to snap to.</param>
        /// <returns>The snapped Vector3.</returns>
        public static Vector3 Snap(Vector3 vector, float interval)
        {
            vector.x = VTMath.Snap(vector.x, interval);
            vector.y = VTMath.Snap(vector.y, interval);
            vector.z = VTMath.Snap(vector.z, interval);

            return vector;
        }

        /// <summary>
        /// Rounds a Vector3 up to the nearest multiple of the specified interval.
        /// </summary>
        /// <param name="vector">The Vector3 to be rounded.</param>
        /// <param name="interval">The interval to which the components should be rounded up.</param>
        /// <returns>The rounded Vector3.</returns>
        public static Vector3 CeilToInterval(Vector3 vector, float interval)
        {
            vector.x = VTMath.CeilToInterval(vector.x, interval);
            vector.y = VTMath.CeilToInterval(vector.y, interval);
            vector.z = VTMath.CeilToInterval(vector.z, interval);
            return vector;
        }

        /// <summary>
        /// Floors a Vector3 to the nearest multiple of the specified interval.
        /// </summary>
        /// <param name="vector">The Vector3 to be floored.</param>
        /// <param name="interval">The interval to which the components should be floored.</param>
        /// <returns>The floored Vector3.</returns>
        public static Vector3 FloorToInterval(Vector3 vector, float interval)
        {
            vector.x = VTMath.FloorToInterval(vector.x, interval);
            vector.y = VTMath.FloorToInterval(vector.y, interval);
            vector.z = VTMath.FloorToInterval(vector.z, interval);
            return vector;
        }

        /// <summary>
        /// Checks if two Vector3 objects are approximately equal within a given tolerance.
        /// </summary>
        /// <param name="a">The first Vector3.</param>
        /// <param name="b">The second Vector3.</param>
        /// <param name="tolerance">The allowable tolerance for equality.</param>
        /// <returns>True if approximately equal, false otherwise.</returns>
        public static bool Approximately(Vector3 a, Vector3 b, float tolerance = 0.0001f)
        {
            return Abs(a.x - b.x) <= tolerance && Abs(a.y - b.y) <= tolerance && Abs(a.z - b.z) <= tolerance;
        }

        /// <summary>
        /// Rotates a 3D vector by the specified euler angles.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The euler angles for rotation.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector3 RotateVector(Vector3 vector, Vector3 rotation)
        {
            vector = Quaternion.Euler(rotation) * vector;
            return vector;
        }

        /// <summary>
        /// Rotates a point around a pivot in 3D space using euler angles.
        /// </summary>
        /// <param name="point">The point to rotate.</param>
        /// <param name="pivot">The pivot point.</param>
        /// <param name="angleVec">The euler angles for rotation.</param>
        /// <returns>The rotated point.</returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angleVec)
        {
            Vector3 direction = point - pivot;
            direction = Quaternion.Euler(angleVec) * direction;
            point = direction + pivot;
            return point;
        }

        /// <summary>
        /// Rotates a point around a pivot in 3D space using specified euler angles.
        /// </summary>
        /// <param name="point">The point to rotate.</param>
        /// <param name="pivot">The pivot point.</param>
        /// <param name="xAngle">Rotation angle around the x-axis.</param>
        /// <param name="yAngle">Rotation angle around the y-axis.</param>
        /// <param name="zAngle">Rotation angle around the z-axis.</param>
        /// <returns>The rotated point.</returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float xAngle = 0, float yAngle = 0, float zAngle = 0)
        {
            Vector3 direction = point - pivot;
            direction = Quaternion.Euler(new Vector3(xAngle, yAngle, zAngle)) * direction;
            //We plus origin to direction to get final point position in world space
            point = direction + pivot;
            return point;
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }

        /// <summary>
        /// Rotates a vector around a specified direction in 3D space.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="direction">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector3 RotateVectorAroundDirection(Vector3 vector, Vector3 direction, float angle)
        {
            float angleInRad = -Deg2Rad * angle;
            direction.Normalize();

            Vector3 result;
            result = Cos(angleInRad) * (vector - (Vector3.Dot(vector, direction) * direction)) + Sin(angleInRad) * (Vector3.Cross(direction, vector) + (Vector3.Dot(vector, direction) * direction));
            return result;
        }

        /// <summary>
        /// Calculates the middle point between two Vector3 points.
        /// </summary>
        /// <param name="vectorA">The first Vector3.</param>
        /// <param name="vectorB">The second Vector3.</param>
        /// <returns>The middle point between the two vectors.</returns>
        public static Vector3 MiddlePoint(Vector3 vectorA, Vector3 vectorB)
        {
            return new Vector3((vectorA.x + vectorB.x) / 2, (vectorA.y + vectorB.y) / 2, (vectorA.z + vectorB.z) / 2);
        }

        /// <summary>
        /// Determines the position of a point relative to a direction in 3D space.
        /// </summary>
        /// <param name="point">The point to determine the position of.</param>
        /// <param name="direction">The direction in 3D space.</param>
        /// <param name="directionStartPos">The starting position of the direction.</param>
        /// <returns>The position of the point relative to the direction.</returns>
        public static PointPosition DeterminePointSideOfDirection(Vector3 point, Vector3 direction, Vector3 directionStartPos)
        {
            Vector3 pointToDirStartPos = point - directionStartPos;
            Vector3 crossProduct = Vector3.Cross(direction, pointToDirStartPos);

            if (crossProduct.y > 0)
            {
                return PointPosition.Left;
            }
            else if (crossProduct.y < 0)
            {
                return PointPosition.Right;
            }
            else
            {
                return PointPosition.OnLine;
            }
        }

        /// <summary>
        /// Calculates the distance between a point and a line defined by two Vector3 points.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="lineStart">The starting point of the line.</param>
        /// <param name="lineEnd">The ending point of the line.</param>
        /// <returns>The distance between the point and the line.</returns>
        public static float DistanceBetweenPointAndLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Distance(ProjectPointOnLine(point, lineStart, lineEnd), point);
        }

        /// <summary>
        /// Projects a point onto a line defined by two Vector3 points.
        /// </summary>
        /// <param name="point">The point to project.</param>
        /// <param name="lineStart">The starting point of the line.</param>
        /// <param name="lineEnd">The ending point of the line.</param>
        /// <returns>The projected point on the line.</returns>
        public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 rhs = point - lineStart;
            Vector3 vector2 = lineEnd - lineStart;
            float magnitude = vector2.magnitude;
            Vector3 lhs = vector2;

            if (magnitude > 1E-06f)
            {
                lhs = lhs / magnitude;
            }

            float num2 = Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
            return (lineStart + (lhs * num2));
        }

        public static bool LessThan(Vector3 v1, Vector3 v2)
        {
            if (v1.x < v2.x && v1.y < v2.y && v1.z < v2.z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool LessThanOrEqualTo(Vector3 v1, Vector3 v2)
        {
            if (v1.x <= v2.x && v1.y <= v2.y && v1.z <= v2.z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GreaterThan(Vector3 v1, Vector3 v2)
        {
            if (v1.x > v2.x && v1.y > v2.y && v1.z > v2.z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GreaterThanOrEqualTo(Vector3 v1, Vector3 v2)
        {
            if (v1.x >= v2.x && v1.y >= v2.y && v1.z >= v2.z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}