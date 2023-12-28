using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Victor.Tools
{
    #if UNITY_EDITOR
    public static class VTDebugDraw
    {

        //reset every session
        private static bool _debugDrawEnabledSet = false;

        private static bool _debugDrawEnabled = false;

        private const string _editorPrefsDebugDraws = "VTDebugDrawsEnabled";

        //The question is when does this property being accessed
        public static bool DebugDrawEnabled
        {
            get
            {
                #if UNITY_EDITOR
                    //If this is true, meaning user has clicked the Disabling Button in the Editor
                    if(_debugDrawEnabledSet)
                    {
                        //If the first time user sees the Enable and Disable MenuItems
                        //We don't rely on memory because it's volatile 
                        if (PlayerPrefs.HasKey(_editorPrefsDebugDraws))
                        {
                            _debugDrawEnabled = (PlayerPrefs.GetInt(_editorPrefsDebugDraws) == 0) ? false : true;
                        }
                        else
                        {
                            _debugDrawEnabled = true;
                        }
                        _debugDrawEnabledSet = true;
                        return _debugDrawEnabled;
                    }
                    else //Otherwise by DEFAULT, DebugDraw is enabled in every session
                    {
                        _debugDrawEnabled = true;
                        return _debugDrawEnabled;
                    }
                    #else
                        return false;
                    #endif
            }
            private set { }
        }

        //Being called when user Enabling and Disabling the DrawGizmo in the Editor
        public static void SetDebugDrawEnabled(bool status)
        {
            DebugDrawEnabled = status;
            _debugDrawEnabled = status;
            //If we click disable DebugDraw MenuItem in the Editor the first time, this will be set to true
            _debugDrawEnabledSet = true;

            int newStatus = status ? 1 : 0;
            PlayerPrefs.SetInt(_editorPrefsDebugDraws, newStatus);
        }

        // Handles Draw --------------------------------------------------------------------------------------

        public static void HandlesDrawWireCircle(Vector3 center, Vector3 normal, float radius, Color color, float thickness = 2)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Handles.DrawWireDisc(center, normal, radius, thickness);
            }
        }

        public static void HandlesDrawSolidCircle(Vector3 center, Vector3 normal, float radius, Color color, float thickness = 2, float solidTransparency = 0.3f, float solidVisibility = 0.75f)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Handles.DrawWireDisc(center, normal, radius, thickness);
            }

            Color solidColor = color.MultV(solidVisibility);
            solidColor.a *= solidTransparency;

            using (new Handles.DrawingScope(solidColor, Handles.matrix))
            {
                Handles.DrawSolidArc(center, normal, Vector3.right, 360f, radius);
            }
        }

        public static void HandlesDrawRectangle(Vector3 center, float width, float height, Color color, float thickness = 2)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            float halfWidth = width / 2;
            float halfHeight = height / 2;

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Vector3 topLeft = new Vector3(center.x - halfWidth, center.y + halfHeight);
                Vector3 topRight = new Vector3(center.x + halfWidth, center.y + halfHeight);
                Vector3 bottomLeft = new Vector3(center.x - halfWidth, center.y - halfHeight);
                Vector3 bottomRight = new Vector3(center.x + halfWidth, center.y - halfHeight);

                Handles.DrawLine(topLeft, topRight, thickness);
                Handles.DrawLine(topLeft, bottomLeft, thickness);
                Handles.DrawLine(bottomRight, bottomLeft, thickness);
                Handles.DrawLine(bottomRight, topRight, thickness);
            }
        }

        public static void HandlesDrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowLength = 1f, float arrowHeadLength = 0.3f, float thickness = 2)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Handles.DrawLine(origin, direction.normalized * arrowLength + origin);
                HandlesDrawArrowEnd(origin, direction.normalized * arrowLength, color, arrowHeadLength, 30f, thickness);
            }
        }

        /// <summary>
		/// Draws a cube at the specified position, and of the specified color and size
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		public static void HandlesDrawCube(Vector3 position, Vector3 size, Color color, float thickness = 2)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Vector3 halfSize = size / 2f;

                Vector3[] points = new Vector3[]
                {
                    position + new Vector3(halfSize.x,halfSize.y,halfSize.z),
                    position + new Vector3(-halfSize.x,halfSize.y,halfSize.z),
                    position + new Vector3(-halfSize.x,-halfSize.y,halfSize.z),
                    position + new Vector3(halfSize.x,-halfSize.y,halfSize.z),
                    position + new Vector3(halfSize.x,halfSize.y,-halfSize.z),
                    position + new Vector3(-halfSize.x,halfSize.y,-halfSize.z),
                    position + new Vector3(-halfSize.x,-halfSize.y,-halfSize.z),
                    position + new Vector3(halfSize.x,-halfSize.y,-halfSize.z),
                };

                for (int i = 0; i < 4; i++)
                {
                    Handles.DrawLine(points[i], points[(i + 1) % 4], thickness);
                    Handles.DrawLine(points[i + 4], points[((i + 1) % 4) + 4], thickness);
                    Handles.DrawLine(points[i], points[i + 4], thickness);
                }
            }
        }

        public static void HandlesDrawCrossXY(Vector3 position, float crossSize, Color color, float thickness = 2)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Vector3 origin = Vector3.zero;
                Vector3 direction = Vector3.zero;


                origin.x = position.x - crossSize / 2;
                origin.y = position.y - crossSize / 2;
                origin.z = position.z;

                direction.x = 1;
                direction.y = 1;
                direction.z = 0;

                Handles.DrawLine(origin, direction * crossSize, thickness);

                origin.x = position.x + crossSize / 2;
                direction.x = -1;
                direction.y = 1;
                direction.z = 0;

                Handles.DrawLine(origin, direction * crossSize, thickness);
            }
        }

        public static void HandlesDrawCrossXZ(Vector3 position, float crossSize, Color color, float thickness = 2)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Vector3 origin = Vector3.zero;
                Vector3 direction = Vector3.zero;


                origin.x = position.x - crossSize / 2;
                origin.y = position.y;
                origin.z = position.z - crossSize / 2;

                direction.x = 1;
                direction.y = 0;
                direction.z = 1;

                Handles.DrawLine(origin, direction * crossSize, thickness);

                origin.x = position.x + crossSize / 2;
                direction.x = -1;
                direction.y = 0;
                direction.z = 1;

                Handles.DrawLine(origin, direction * crossSize, thickness);
            }
        }

        public static void HandlesDrawCrossYZ(Vector3 position, float crossSize, Color color, float thickness = 2)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Vector3 origin = Vector3.zero;
                Vector3 direction = Vector3.zero;


                origin.x = position.x;
                origin.y = position.y - crossSize / 2;
                origin.z = position.z - crossSize / 2;

                direction.x = 0;
                direction.y = 1;
                direction.z = 1;

                Handles.DrawLine(origin, direction * crossSize, thickness);

                origin.z = position.z + crossSize / 2;
                direction.y = 1;
                direction.z = -1;

                Handles.DrawLine(origin, direction * crossSize, thickness);
            }
        }

        private static void HandlesDrawArrowEnd(Vector3 arrowOriginPos, Vector3 direction, Color color, float arrowHeadLength, float arrowHeadAngle, float thickness)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            if (direction == Vector3.zero)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Vector3 top = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
                Vector3 rightBottom = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, -30, 0) * Vector3.back;
                Vector3 leftBottom = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 30, 0) * Vector3.back;

                Handles.DrawLine(arrowOriginPos + direction, rightBottom * arrowHeadLength + arrowOriginPos + direction, thickness);
                Handles.DrawLine(arrowOriginPos + direction, leftBottom * arrowHeadLength + arrowOriginPos + direction, thickness);
                Handles.DrawLine(arrowOriginPos + direction, top * arrowHeadLength + arrowOriginPos + direction, thickness);
            }
        }

        /// <summary>
        /// Use handles to draw the bounds of an object.
        /// </summary>
        /// <param name="bounds">Bounds.</param>
        /// <param name="color">Color.</param>
        public static void HandlesDrawBounds(Bounds bounds, Color color, float thickness = 2)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                Vector3 boundsCenter = bounds.center;
                Vector3 boundsExtents = bounds.extents;

                Vector3 v3FrontTopLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front top left corner
                Vector3 v3FrontTopRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front top right corner
                Vector3 v3FrontBottomLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front bottom left corner
                Vector3 v3FrontBottomRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front bottom right corner
                Vector3 v3BackTopLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back top left corner
                Vector3 v3BackTopRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back top right corner
                Vector3 v3BackBottomLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back bottom left corner
                Vector3 v3BackBottomRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back bottom right corner

                Handles.DrawLine(v3FrontTopLeft, v3FrontTopRight, thickness);
                Handles.DrawLine(v3FrontTopRight, v3FrontBottomRight, thickness);
                Handles.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, thickness);
                Handles.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, thickness);

                Handles.DrawLine(v3BackTopLeft, v3BackTopRight, thickness);
                Handles.DrawLine(v3BackTopRight, v3BackBottomRight, thickness);
                Handles.DrawLine(v3BackBottomRight, v3BackBottomLeft, thickness);
                Handles.DrawLine(v3BackBottomLeft, v3BackTopLeft, thickness);

                Handles.DrawLine(v3FrontTopLeft, v3BackTopLeft, thickness);
                Handles.DrawLine(v3FrontTopRight, v3BackTopRight, thickness);
                Handles.DrawLine(v3FrontBottomRight, v3BackBottomRight, thickness);
                Handles.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, thickness);
            }
        }

        public static void HandlesDrawPoint(Vector3 position, float size, Color color, float thickness = 2)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            using (new Handles.DrawingScope(color, Handles.matrix))
            {
                //Draw Debug Pyramid
                Vector3[] points = new Vector3[]
                {
                    position + (Vector3.up * size),
                    position - (Vector3.up * size),
                    position + (Vector3.right * size),
                    position - (Vector3.right * size),
                    position + (Vector3.forward * size),
                    position - (Vector3.forward * size)
                };

                Vector3[] additionalPoints = new Vector3[]
                {
                    points[0] / 2 + (points[2] + points[4]) / 4,
                    points[0] / 2 + (points[2] + points[5]) / 4,
                    points[0] / 2 + (points[3] + points[4]) / 4,
                    points[0] / 2 + (points[3] + points[5]) / 4,
                    points[1] / 2 + (points[3] + points[4]) / 4,
                    points[1] / 2 + (points[3] + points[5]) / 4,
                    points[1] / 2 + (points[2] + points[4]) / 4,
                    points[1] / 2 + (points[2] + points[5]) / 4,
                };

                Handles.DrawLine(points[0], points[1], thickness);
                Handles.DrawLine(points[2], points[3], thickness);
                Handles.DrawLine(points[4], points[5], thickness);
                Handles.DrawLine(points[2], points[4], thickness);
                Handles.DrawLine(points[2], points[5], thickness);
                Handles.DrawLine(points[3], points[4], thickness);
                Handles.DrawLine(points[3], points[5], thickness);
                Handles.DrawLine(points[0], points[2], thickness);
                Handles.DrawLine(points[0], points[3], thickness);
                Handles.DrawLine(points[0], points[4], thickness);
                Handles.DrawLine(points[0], points[5], thickness);
                Handles.DrawLine(points[1], points[2], thickness);
                Handles.DrawLine(points[1], points[3], thickness);
                Handles.DrawLine(points[1], points[4], thickness);
                Handles.DrawLine(points[1], points[5], thickness);


                //Draw minor points
                Handles.DrawLine(points[0], additionalPoints[0], thickness);
                Handles.DrawLine(points[2], additionalPoints[0], thickness);
                Handles.DrawLine(points[4], additionalPoints[0], thickness);
                Handles.DrawLine(points[0], additionalPoints[1], thickness);
                Handles.DrawLine(points[2], additionalPoints[1], thickness);
                Handles.DrawLine(points[5], additionalPoints[1], thickness);
                Handles.DrawLine(points[0], additionalPoints[2], thickness);
                Handles.DrawLine(points[3], additionalPoints[2], thickness);
                Handles.DrawLine(points[4], additionalPoints[2], thickness);
                Handles.DrawLine(points[0], additionalPoints[3], thickness);
                Handles.DrawLine(points[3], additionalPoints[3], thickness);
                Handles.DrawLine(points[5], additionalPoints[3], thickness);
                Handles.DrawLine(points[1], additionalPoints[4], thickness);
                Handles.DrawLine(points[3], additionalPoints[4], thickness);
                Handles.DrawLine(points[4], additionalPoints[4], thickness);
                Handles.DrawLine(points[1], additionalPoints[5], thickness);
                Handles.DrawLine(points[3], additionalPoints[5], thickness);
                Handles.DrawLine(points[5], additionalPoints[5], thickness);
                Handles.DrawLine(points[1], additionalPoints[6], thickness);
                Handles.DrawLine(points[2], additionalPoints[6], thickness);
                Handles.DrawLine(points[4], additionalPoints[6], thickness);
                Handles.DrawLine(points[1], additionalPoints[7], thickness);
                Handles.DrawLine(points[2], additionalPoints[7], thickness);
                Handles.DrawLine(points[5], additionalPoints[7], thickness);
            }
        }

        // Debug Draw --------------------------------------------------------------------------------------

        /// <summary>
		/// Draws a cube at the specified position, and of the specified color and size
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		public static void DebugDrawCube(Vector3 position, Color color, Vector3 size)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            Vector3 halfSize = size / 2f;

            Vector3[] points = new Vector3[]
            {
                position + new Vector3(halfSize.x,halfSize.y,halfSize.z),
                position + new Vector3(-halfSize.x,halfSize.y,halfSize.z),
                position + new Vector3(-halfSize.x,-halfSize.y,halfSize.z),
                position + new Vector3(halfSize.x,-halfSize.y,halfSize.z),
                position + new Vector3(halfSize.x,halfSize.y,-halfSize.z),
                position + new Vector3(-halfSize.x,halfSize.y,-halfSize.z),
                position + new Vector3(-halfSize.x,-halfSize.y,-halfSize.z),
                position + new Vector3(halfSize.x,-halfSize.y,-halfSize.z),
            };

            Debug.DrawLine(points[0], points[1], color);
            Debug.DrawLine(points[1], points[2], color);
            Debug.DrawLine(points[2], points[3], color);
            Debug.DrawLine(points[3], points[0], color);
        }


        public static void DebugDrawCrossXY(Vector3 position, float crossSize, Color color)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            Vector3 origin = Vector3.zero;
            Vector3 direction = Vector3.zero;


            origin.x = position.x - crossSize / 2;
            origin.y = position.y - crossSize / 2;
            origin.z = position.z;

            direction.x = 1;
            direction.y = 1;
            direction.z = 0;
            Debug.DrawRay(origin, direction * crossSize, color);

            origin.x = position.x + crossSize / 2;

            direction.x = -1;
            direction.y = 1;
            direction.z = 0;
            Debug.DrawRay(origin, direction * crossSize, color);
        }

        public static void DebugDrawCrossXZ(Vector3 position, float crossSize, Color color)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            Vector3 origin = Vector3.zero;
            Vector3 direction = Vector3.zero;


            origin.x = position.x - crossSize / 2;
            origin.y = position.y;
            origin.z = position.z - crossSize / 2;

            direction.x = 1;
            direction.y = 0;
            direction.z = 1;
            Debug.DrawRay(origin, direction * crossSize, color);

            origin.x = position.x + crossSize / 2;

            direction.x = -1;
            direction.y = 0;
            direction.z = 1;
            Debug.DrawRay(origin, direction * crossSize, color);
        }

        public static void DebugDrawCrossYZ(Vector3 position, float crossSize, Color color)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            Vector3 origin = Vector3.zero;
            Vector3 direction = Vector3.zero;


            origin.x = position.x;
            origin.y = position.y - crossSize / 2;
            origin.z = position.z - crossSize / 2;

            direction.x = 0;
            direction.y = 1;
            direction.z = 1;
            Debug.DrawRay(origin, direction * crossSize, color);

            origin.z = position.z + crossSize / 2;

            direction.y = 1;
            direction.z = -1;
            Debug.DrawRay(origin, direction * crossSize, color);
        }

        public static void DebugDrawPoint(Vector3 position, float size, Color color)
        {
            if (!DebugDrawEnabled)
            {
                return;
            }

            //Draw Gizmo Sphere
            Gizmos.color = color;
            Gizmos.DrawWireSphere(position, size);

            //Draw Debug Pyramid
            Vector3[] points = new Vector3[]
            {
                position + (Vector3.up * size),
                position - (Vector3.up * size),
                position + (Vector3.right * size),
                position - (Vector3.right * size),
                position + (Vector3.forward * size),
                position - (Vector3.forward * size)

            };

            Vector3[] additionalPoints = new Vector3[]
            {
                points[0] / 2 + (points[2] + points[4]) / 4,
                points[0] / 2 + (points[2] + points[5]) / 4,
                points[0] / 2 + (points[3] + points[4]) / 4,
                points[0] / 2 + (points[3] + points[5]) / 4,
                points[1] / 2 + (points[3] + points[4]) / 4,
                points[1] / 2 + (points[3] + points[5]) / 4,
                points[1] / 2 + (points[2] + points[4]) / 4,
                points[1] / 2 + (points[2] + points[5]) / 4,


            };

            Debug.DrawLine(points[0], points[1], color);
            Debug.DrawLine(points[2], points[3], color);
            Debug.DrawLine(points[4], points[5], color);
            Debug.DrawLine(points[2], points[4], color);
            Debug.DrawLine(points[2], points[5], color);
            Debug.DrawLine(points[3], points[4], color);
            Debug.DrawLine(points[3], points[5], color);
            Debug.DrawLine(points[0], points[2], color);
            Debug.DrawLine(points[0], points[3], color);
            Debug.DrawLine(points[0], points[4], color);
            Debug.DrawLine(points[0], points[5], color);
            Debug.DrawLine(points[1], points[2], color);
            Debug.DrawLine(points[1], points[3], color);
            Debug.DrawLine(points[1], points[4], color);
            Debug.DrawLine(points[1], points[5], color);


            //Draw minor points
            Debug.DrawLine(points[0], additionalPoints[0], color);
            Debug.DrawLine(points[2], additionalPoints[0], color);
            Debug.DrawLine(points[4], additionalPoints[0], color);
            Debug.DrawLine(points[0], additionalPoints[1], color);
            Debug.DrawLine(points[2], additionalPoints[1], color);
            Debug.DrawLine(points[5], additionalPoints[1], color);
            Debug.DrawLine(points[0], additionalPoints[2], color);
            Debug.DrawLine(points[3], additionalPoints[2], color);
            Debug.DrawLine(points[4], additionalPoints[2], color);
            Debug.DrawLine(points[0], additionalPoints[3], color);
            Debug.DrawLine(points[3], additionalPoints[3], color);
            Debug.DrawLine(points[5], additionalPoints[3], color);
            Debug.DrawLine(points[1], additionalPoints[4], color);
            Debug.DrawLine(points[3], additionalPoints[4], color);
            Debug.DrawLine(points[4], additionalPoints[4], color);
            Debug.DrawLine(points[1], additionalPoints[5], color);
            Debug.DrawLine(points[3], additionalPoints[5], color);
            Debug.DrawLine(points[5], additionalPoints[5], color);
            Debug.DrawLine(points[1], additionalPoints[6], color);
            Debug.DrawLine(points[2], additionalPoints[6], color);
            Debug.DrawLine(points[4], additionalPoints[6], color);
            Debug.DrawLine(points[1], additionalPoints[7], color);
            Debug.DrawLine(points[2], additionalPoints[7], color);
            Debug.DrawLine(points[5], additionalPoints[7], color);
        }

        public static void SetHandlesColor(Color color)
        {
            Handles.color = color;
        }

        public static void SetHandlesDefaultColor()
        {
            Handles.color = Color.white;
        }
    }
    #endif
}
