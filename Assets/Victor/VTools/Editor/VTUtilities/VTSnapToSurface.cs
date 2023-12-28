using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public class VTSnapToSurface : EditorWindow
    {
        // Upper case represent positive axis, lower case for negative ones
        public enum SnapAxes { X, Y, Z, x, y, z};
        private static readonly string s_WorldSnapModePrefKey = "VTSnapToSurface_WorldSnapMode";
        private static bool s_WorldSnapMode = true;
        private static string s_SwitchButtonText;
        private static string s_SwitchButtonTooltip;
        private static Color s_WorldModeColor = VTColorLibrary.victorBlue;
        private static Color s_LocalModeColor = VTColorLibrary.victorGreen;
        private static Texture s_switchButtonImage;

        void OnEnable()
        {
            if (!EditorPrefs.HasKey(s_WorldSnapModePrefKey))
            {
                EditorPrefs.SetBool(s_WorldSnapModePrefKey, true);
            }
            s_WorldSnapMode = EditorPrefs.GetBool(s_WorldSnapModePrefKey);
        }
        
        void OnGUI()
        {
            EditorGUILayout.Space(20);

            VTGUI.StoreGUIBackgroundAndContentColor();

            if (s_WorldSnapMode)
            {
                GUI.backgroundColor = s_WorldModeColor;
            }
            else
            {
                GUI.backgroundColor = s_LocalModeColor;
            }

            // Set switch button appearance
            s_SwitchButtonText = s_WorldSnapMode ? " World Axis Snap Mode" : " Local Axis Snap Mode";
            s_SwitchButtonTooltip = s_WorldSnapMode ? "In world snap mode, press to switch to local snap mode" : "In local snap mode, press to swith to world snap mode";
            s_switchButtonImage = s_WorldSnapMode ? EditorGUIUtility.IconContent("d_ToolHandleGlobal").image : EditorGUIUtility.IconContent("d_ToolHandleLocal").image;

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent(s_SwitchButtonText, s_switchButtonImage,s_SwitchButtonTooltip), GUILayout.MaxWidth(160)))
                {
                    s_WorldSnapMode = !s_WorldSnapMode;
                    UnityEditor.Tools.pivotRotation = s_WorldSnapMode ? PivotRotation.Global : PivotRotation.Local;
                    EditorPrefs.SetBool(s_WorldSnapModePrefKey, s_WorldSnapMode);
                }
                GUILayout.FlexibleSpace();
            }
            
            VTGUI.RevertGUIBackgroundAndContentColor();
            EditorGUILayout.Space(5);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Positive Axis  ", EditorStyles.boldLabel);

                VTGUI.StoreGUIBackgroundAndContentColor();
                GUI.contentColor = s_WorldSnapMode ? s_WorldModeColor : s_LocalModeColor;
                GUILayout.Label(EditorGUIUtility.IconContent("MoveTool on"), GUILayout.MaxWidth(20));
                VTGUI.RevertGUIBackgroundAndContentColor();

                if (GUILayout.Button("X", VTStyles.buttonLeft, GUILayout.MaxWidth(position.size.x), GUILayout.MinWidth(30)))
                {
                    Snap(SnapAxes.X);
                }
                if (GUILayout.Button("Y", VTStyles.buttonMid, GUILayout.MaxWidth(position.size.x), GUILayout.MinWidth(30)))
                {
                    Snap(SnapAxes.Y);
                }
                if (GUILayout.Button("Z", VTStyles.buttonRight, GUILayout.MaxWidth(position.size.x), GUILayout.MinWidth(30)))
                {
                    Snap(SnapAxes.Z);
                }
            }

            EditorGUILayout.Space(10);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Negative Axis ", EditorStyles.boldLabel);

                VTGUI.StoreGUIBackgroundAndContentColor();
                GUI.contentColor = s_WorldSnapMode ? s_WorldModeColor : s_LocalModeColor;

                GUILayout.Label(EditorGUIUtility.IconContent("MoveTool"), GUILayout.MaxWidth(20));
                VTGUI.RevertGUIBackgroundAndContentColor();

                if (GUILayout.Button("-X", VTStyles.buttonLeft, GUILayout.MaxWidth(position.size.x), GUILayout.MinWidth(30)))
                {
                    Snap(SnapAxes.x);
                }
                if (GUILayout.Button("-Y", VTStyles.buttonMid, GUILayout.MaxWidth(position.size.x), GUILayout.MinWidth(30)))
                {
                    Snap(SnapAxes.y);
                }
                if (GUILayout.Button("-Z", VTStyles.buttonRight, GUILayout.MaxWidth(position.size.x), GUILayout.MinWidth(30)))
                {
                    Snap(SnapAxes.z);
                }
            }
        }

        [MenuItem("Tools/Victor/Dev Window/Snap to Surface", priority = 2)]
        static void ShowWindow()
        {
            var window = GetWindow<VTSnapToSurface>();
            window.minSize = new Vector2(300, 200);

            window.titleContent = new GUIContent("Snap To Surface", EditorGUIUtility.IconContent("d_SceneViewTools").image);
            window.Show();
        }

        static void Snap(SnapAxes snapAxis)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                // If the object has a collider we can do a nice sweep test for accurate placement
                var collider = go.GetComponent<Collider>();
                
                Vector3 sweepTestDirection = GetDir(snapAxis, go.transform);

                if (collider != null && !(collider is CharacterController))
                {
                    // Figure out if we need a temp rigid body and add it if needed
                    var tempRigidBody = go.GetComponent<Rigidbody>();
                    bool addedRigidBody = false;

                    if (tempRigidBody == null)
                    {
                        tempRigidBody = go.AddComponent<Rigidbody>();
                        addedRigidBody = true;
                    }

                    // Sweep the rigid body downwards and, if we hit something, move the object to the distance
                    RaycastHit[] hits;
                    float closestDistance = float.MaxValue;
                    // Minimum value recognized as a valid snap
                    float minThreshold = 0.01f;
                    Renderer renderer = new Renderer();

                    hits = tempRigidBody.SweepTestAll(sweepTestDirection, 1000f);

                    // If we added a rigid body for this test, remove it now
                    if (addedRigidBody)
                    {
                        DestroyImmediate(tempRigidBody);
                    }

                    // Necessary test, otherwise an invalid AABB test is thrown
                    if (hits.Length <= 0)
                    {
                        continue;
                    }

                    for (int index = 0; index < hits.Length; index++)
                    {
                        if (hits[index].distance < closestDistance && (hits[index].distance - 0) > minThreshold)
                        {
                            closestDistance = hits[index].distance;
                        }
                    }

                    // Necessary test, otherwise an invalid AABB test is thrown
                    if (closestDistance > 1000f)
                    {
                        continue;
                    }

                    Undo.RecordObject(go.transform, "Snap To Surface");
                    go.transform.position += sweepTestDirection * closestDistance;
                }
                // Without a collider, we do a simple raycast from the transform
                else
                {
                    // Change the object to the "ignore raycast" layer so it doesn't get hit
                    int savedLayer = go.layer;
                    go.layer = 2;

                    // Do the raycast and move the object down if it hit something
                    RaycastHit hit;

                    if (Physics.Raycast(go.transform.position, sweepTestDirection, out hit))
                    {
                        Undo.RecordObject(go.transform, "Snap To Surface");
                        go.transform.position = hit.point;
                    }

                    // Restore layer for the object
                    go.layer = savedLayer;
                }
            }
        }

        private static Vector3 GetDir(SnapAxes snapAxis, Transform goTransform)
        {
            Vector3 dir = Vector3.zero;
            switch (snapAxis)
            {
                case SnapAxes.X:
                    dir = s_WorldSnapMode ? Vector3.right : goTransform.right;
                    break;
                case SnapAxes.Y:
                    dir = s_WorldSnapMode ? Vector3.up : goTransform.up;
                    break;
                case SnapAxes.Z:
                    dir = s_WorldSnapMode ? Vector3.forward : goTransform.forward;
                    break;
                case SnapAxes.x:
                    dir = s_WorldSnapMode ? Vector3.left : -goTransform.right;
                    break;
                case SnapAxes.y:
                    dir = s_WorldSnapMode ? Vector3.down : -goTransform.up;
                    break;
                case SnapAxes.z:
                    dir = s_WorldSnapMode ? Vector3.back : -goTransform.forward;
                    break;
            }
            return dir;
        }
    }
}
