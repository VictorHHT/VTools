using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Victor.Tools
{
    public static class VTVector2
    {
        /// <summary>
        /// Defines the possible positions of a point relative to a line.
        /// </summary>
        public enum PointPosition { Left, Right, OnLine };

        /// <summary>
        /// Rounds the components of a 2D vector to the nearest integer values.
        /// </summary>
        /// <param name="vector">The vector to round.</param>
        /// <returns>The rounded vector.</returns>
        public static Vector2 Round(Vector2 vector)
        {
            return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
        }

        /// <summary>
        /// Rounds the components of a 2D vector to the specified number of decimal points.
        /// </summary>
        /// <param name="vector">The vector to round.</param>
        /// <param name="decimalPoints">The number of decimal points.</param>
        /// <returns>The rounded vector.</returns>
        public static Vector2 Round(Vector2 vector, int decimalPoints)
        {
            return new Vector2(VTMath.Round(vector.x, decimalPoints), VTMath.Round(vector.y, decimalPoints));
        }

        /// <summary>
        /// Snaps the components of a 2D vector to the specified interval.
        /// </summary>
        /// <param name="vector">The vector to snap.</param>
        /// <param name="interval">The interval to snap to.</param>
        /// <returns>The snapped vector.</returns>
        public static Vector2 Snap(Vector2 vector, float interval)
        {
            vector.x = VTMath.Snap(vector.x, interval);
            vector.y = VTMath.Snap(vector.y, interval);

            return vector;
        }

        /// <summary>
        /// Rounds a Vector2 up to the nearest multiple of the specified interval.
        /// </summary>
        /// <param name="vector">The Vector2 to be rounded.</param>
        /// <param name="interval">The interval to which the components should be rounded up.</param>
        /// <returns>The rounded Vector2.</returns>
        public static Vector2 CeilToInterval(Vector2 vector, float interval)
        {
            vector.x = VTMath.CeilToInterval(vector.x, interval);
            vector.y = VTMath.CeilToInterval(vector.y, interval);
            return vector;
        }

        /// <summary>
        /// Floors a Vector2 to the nearest multiple of the specified interval.
        /// </summary>
        /// <param name="vector">The Vector2 to be floored.</param>
        /// <param name="interval">The interval to which the components should be floored.</param>
        /// <returns>The floored Vector2.</returns>
        public static Vector2 FloorToInterval(Vector2 vector, float interval)
        {
            vector.x = VTMath.FloorToInterval(vector.x, interval);
            vector.y = VTMath.FloorToInterval(vector.y, interval);
            return vector;
        }

        /// <summary>
        /// Checks if two Vector2 instances are approximately equal within a specified tolerance.
        /// </summary>
        /// <param name="a">The first Vector2.</param>
        /// <param name="b">The second Vector2.</param>
        /// <param name="tolerance">The tolerance for equality. Defaults to 0.0001f.</param>
        /// <returns>True if the vectors are approximately equal; otherwise, false.</returns>
        public static bool Approximately(Vector2 a, Vector2 b, float tolerance = 0.0001f)
        {
            return Abs(a.x - b.x) <= tolerance && Abs(a.y - b.y) <= tolerance;
        }

        /// <summary>
        /// Rotates a 2D vector by the specified 2D rotation vector.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The 2D rotation vector.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2 RotateVector(Vector2 vector, Vector2 rotation)
        {
            vector = Quaternion.Euler(rotation) * vector;
            return vector;
        }

        /// <summary>
        /// Rotates a 2D vector by the specified angle in degrees.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="angle">The angle of rotation in degrees.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2 RotateVector(Vector2 vector, float angle)
        {
            vector = Quaternion.Euler(new Vector3(0, 0, angle)) * vector;
            return vector;
        }

        /// <summary>
        /// Rotates a 2D point around a pivot by the specified 2D angle vector.
        /// </summary>
        /// <param name="point">The point to rotate.</param>
        /// <param name="pivot">The pivot point for rotation.</param>
        /// <param name="angleVec">The 2D angle vector.</param>
        /// <returns>The rotated point.</returns>
        public static Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, Vector2 angleVec)
        {
            Vector3 direction = point - pivot;
            direction = Quaternion.Euler(angleVec) * direction;
            point = direction.ToV2FromXY() + pivot;
            return point;
        }

        public static Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float zAngle)
        {
            Vector3 direction = point - pivot;
            direction = Quaternion.Euler(new Vector3(0, 0, zAngle)) * direction;
            point = direction.ToV2FromXY() + pivot;
            return point;
        }

        /// <summary>
        /// Calculates the clockwise angle in degrees between two 2D vectors, vectorA and vectorB.
        /// </summary>
        /// <param name="vectorA">The first Vector2.</param>
        /// <param name="vectorB">The second Vector2.</param>
        /// <returns>
        /// The clockwise angle between vectorA and vectorB in the range [0, 360) degrees.
        /// </returns>
        public static float AngleBetween(Vector2 vectorA, Vector2 vectorB)
        {
            float angle = Vector2.Angle(vectorA, vectorB);
            Vector3 aCrossB = Vector3.Cross(vectorA, vectorB);

            if (aCrossB.z > 0)
            {
                angle = 360 - angle;
            }

            return angle;
        }

        /// <summary>
        /// Returns a vector perpendicular to the input vector. The direction can be clockwise or counterclockwise.
        /// </summary>
        /// <param name="vec">The input vector.</param>
        /// <param name="clockwise">If true, returns a clockwise perpendicular vector; otherwise, counterclockwise.</param>
        /// <returns>The normalized perpendicular vector.</returns>
        public static Vector2 GetPerpendicularDirection(Vector2 vec, bool clockwise = true)
        {
            return RotateVector(vec, clockwise ? -90 : 90).normalized;
        }

        /// <summary>
        /// Calculates the middle point between two Vector2 instances.
        /// </summary>
        /// <param name="vectorA">The first Vector2.</param>
        /// <param name="vectorB">The second Vector2.</param>
        /// <returns>The Vector2 representing the middle point between vectorA and vectorB.</returns>
        public static Vector2 MiddlePoint(Vector2 vectorA, Vector2 vectorB)
        {
            return new Vector2((vectorA.x + vectorB.x) / 2, (vectorA.y + vectorB.y) / 2);
        }

        /// <summary>
        /// Determines the side of a point relative to a directional vector in 2D space.
        /// </summary>
        /// <param name="point">The point to determine the side of.</param>
        /// <param name="direction">The directional vector.</param>
        /// <param name="directionStartPos">The starting position of the directional vector.</param>
        /// <returns>The position of the point relative to the directional vector (Left, Right, or OnLine).</returns>
        public static PointPosition DeterminePointSideOfDirection(Vector2 point, Vector2 direction, Vector2 directionStartPos)
        {
            Vector2 pointToDirStartPos = point - directionStartPos;
            float crossProduct = direction.x * pointToDirStartPos.y - direction.y * pointToDirStartPos.x;

            if (crossProduct > 0)
            {
                return PointPosition.Left;
            }
            else if (crossProduct < 0)
            {
                return PointPosition.Right;
            }
            else
            {
                return PointPosition.OnLine;
            }
        }

        /// <summary>
        /// Calculates the intersection point of two lines defined by their starting points and directions.
        /// </summary>
        /// <param name="startPos1">The starting point of the first line.</param>
        /// <param name="dir1">The direction of the first line.</param>
        /// <param name="startPos2">The starting point of the second line.</param>
        /// <param name="dir2">The direction of the second line.</param>
        /// <param name="intersection">The intersection point if the lines intersect; otherwise, Vector2.zero.</param>
        /// <returns>True if the lines intersect; otherwise, false.</returns>
        public static bool GetDirectionIntersection(Vector2 startPos1, Vector2 dir1, Vector2 startPos2, Vector2 dir2, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            // Determinant of the 2x2 matrix formed by the direction vectors
            float denominator = dir1.x * dir2.y - dir1.y * dir2.x;

            if (Mathf.Approximately(denominator, 0f))
            {
                // Lines are parallel
                return false;
            }

            // Distances along each line to the intersection point
            // Calculate the determinant of a 2x2 matrix formed by the x and y components of the direction vectors of the two lines
            // and the differences in the starting points of the two lines
            float t = ((startPos2.x - startPos1.x) * dir2.y - (startPos2.y - startPos1.y) * dir2.x) / denominator;
            float u = ((startPos2.x - startPos1.x) * dir1.y - (startPos2.y - startPos1.y) * dir1.x) / denominator;

            if (t >= 0f && u >= 0f && u <= 1f)
            {
                // if the lines intersect, then the intersection point can be expressed
                // as the sum of the starting point of one line and a multiple of its direction vector
                intersection = startPos1 + t * dir1;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Calculates the intersection point of two line segments defined by their endpoints.
        /// </summary>
        /// <param name="line1Start">The starting point of the first line segment.</param>
        /// <param name="line1End">The ending point of the first line segment.</param>
        /// <param name="line2Start">The starting point of the second line segment.</param>
        /// <param name="line2End">The ending point of the second line segment.</param>
        /// <param name="intersection">The intersection point if the line segments intersect; otherwise, Vector2.zero.</param>
        /// <returns>True if the line segments intersect; otherwise, false.</returns>
        public static bool GetLineIntersection(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            Vector2 dir1 = line1End - line1Start;
            Vector2 dir2 = line2End - line2Start;

            float a1 = -dir1.y;
            float b1 = +dir1.x;
            float d1 = -(a1 * line1Start.x + b1 * line1Start.y);

            float a2 = -dir2.y;
            float b2 = +dir2.x;
            float d2 = -(a2 * line2Start.x + b2 * line2Start.y);

            float seg1Line2Start = a2 * line1Start.x + b2 * line1Start.y + d2;
            float seg1Line2End = a2 * line1End.x + b2 * line1End.y + d2;

            float seg2Line1Start = a1 * line2Start.x + b1 * line2Start.y + d1;
            float seg2Line1End = a1 * line2End.x + b1 * line2End.y + d1;

            if (seg1Line2Start > 0f && seg1Line2End > 0f || seg1Line2Start < 0f && seg1Line2End < 0f || Mathf.Approximately(seg1Line2Start, 0f) || Mathf.Approximately(seg1Line2End, 0f))
            {
                // No intersection
                return false;
            }

            if (seg2Line1Start > 0f && seg2Line1End > 0f || seg2Line1Start < 0f && seg2Line1End < 0f || Mathf.Approximately(seg2Line1Start, 0f) || Mathf.Approximately(seg2Line1End, 0f))
            {
                // No intersection
                return false;
            }

            float u = seg1Line2Start / (seg1Line2Start - seg1Line2End);
            intersection = line1Start + u * dir1;

            return true;
        }

        /// <summary>
        /// Calculates the distance between a point and a line defined by two endpoints.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="lineStart">The starting point of the line.</param>
        /// <param name="lineEnd">The ending point of the line.</param>
        /// <returns>The distance between the point and the line.</returns>
        public static float DistanceBetweenPointAndLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            return Vector2.Distance(ProjectPointOnLine(point, lineStart, lineEnd), point);
        }

        /// <summary>
        /// Projects a point onto a line defined by two endpoints.
        /// </summary>
        /// <param name="point">The point to project.</param>
        /// <param name="lineStart">The starting point of the line.</param>
        /// <param name="lineEnd">The ending point of the line.</param>
        /// <returns>The projected point on the line.</returns>
        public static Vector3 ProjectPointOnLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 rhs = point - lineStart;
            Vector2 vector2 = lineEnd - lineStart;
            float magnitude = vector2.magnitude;
            Vector2 lhs = vector2;

            if (magnitude > 1E-06f)
            {
                lhs = (lhs / magnitude);
            }

            float num2 = Clamp(Vector2.Dot(lhs, rhs), 0f, magnitude);
            return (lineStart + (lhs * num2));
        }

        /// <summary>
        /// Finds the center of the circle passing through three given points.
        /// </summary>
        /// <param name="A">First point.</param>
        /// <param name="B">Second point.</param>
        /// <param name="C">Third point.</param>
        /// <returns>The center of the circle.</returns>
        public static Vector2 FindCircleCenter(Vector2 A, Vector2 B, Vector2 C)
        {
            // Find midpoints of two sides of the triangle
            Vector2 midAB = (A + B) / 2f;
            Vector2 midBC = (B + C) / 2f;

            // Find slopes of two sides of the triangle
            float slopeAB = (B.y - A.y) / (B.x - A.x);
            float slopeBC = (C.y - B.y) / (C.x - B.x);

            // Find the perpendicular slopes of two sides of the triangle
            float perpSlopeAB = -1f / slopeAB;
            float perpSlopeBC = -1f / slopeBC;

            // Find the y-intercepts of the perpendicular bisectors
            float yInterceptAB = midAB.y - perpSlopeAB * midAB.x;
            float yInterceptBC = midBC.y - perpSlopeBC * midBC.x;

            // Find the x and y coordinates of the center of the circle
            float centerX = (yInterceptBC - yInterceptAB) / (perpSlopeAB - perpSlopeBC);
            float centerY = perpSlopeAB * centerX + yInterceptAB;

            return new Vector2(centerX, centerY);
        }

        /// <summary>
        /// Gets the position of a point in a circle given the distance to the center and an angle in degrees.
        /// </summary>
        /// <param name="distanceToCenter">The distance from the center of the circle.</param>
        /// <param name="thetaInDegrees">The angle in degrees.</param>
        /// <returns>The position of the point in the circle.</returns>
        public static Vector2 GetPositionInCircle(float distanceToCenter, float thetaInDegrees)
        {
            float thetaInRadians = thetaInDegrees * Mathf.Deg2Rad;
            return new Vector2(Cos(thetaInRadians), Sin(thetaInRadians)) * distanceToCenter;
        }

        public static bool LessThan(Vector2 v1, Vector2 v2)
        {
            if (v1.x < v2.x && v1.y < v2.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool LessThanOrEqualTo(Vector2 v1, Vector2 v2)
        {
            if (v1.x <= v2.x && v1.y <= v2.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GreaterThan(Vector2 v1, Vector2 v2)
        {
            if (v1.x > v2.x && v1.y > v2.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GreaterThanOrEqualTo(Vector2 v1, Vector2 v2)
        {
            if (v1.x >= v2.x && v1.y >= v2.y)
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