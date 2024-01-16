using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Victor.EditorTween;

namespace Victor.Tools
{
    public class VTCustomPreview : Editor
    {
        public float lightIntensity = 0.8f;
        public GameObject objectToPreview;
        public VTweenCore m_LightTween;

        private float m_OverallRadius;
        private bool m_HasModelPreview;
        private bool m_Initailized;
        private Vector3 m_BoundsCenter;
        // We need to instantiate a new instance of the preview object because all objects added to the preview scene will be destroyed in when PreviewRenderUtility is disposed
        private GameObject m_PreviewInstance;
        private PreviewRenderUtility m_PreviewRenderUtility;

        // Rotate
        private float m_RotAroundX;
        private float m_RotAroundY;
        // Smoother with smaller value
        private float m_RotateSmoothValue = 0.4f;
        private Quaternion m_CurrentRot;
        private Quaternion m_TargetRot;

        // Zoom
        private float m_TargetZoomLevel = 1;
        private float m_CurrentZoomLevel = 1;
        private float m_MinZoomLevel = 0.4f;
        private float m_MaxZoomLevel = 4f;
        private float m_MaxMouseDeltaZoom = 5f;
        private float m_CamToObjectCenterDistance;
        // Smoother with larger value
        private float m_DistanceChangeSmoothTime = 0.3f;
        private float m_DummyDampZoomVelocity = 0;

        // Pan
        private float m_TargetPanSpeed = 0;
        private float m_LastButtonUpTimeStamp;
        private float m_SetPanDirDelay = 0.02f;
        private bool m_IsKeyDownA;
        private bool m_IsKeyDownD;
        private bool m_IsKeyDownLeftArrow;
        private bool m_IsKeyDownRightArrow;
        private bool m_IsKeyDownW;
        private bool m_IsKeyDownS;
        private bool m_IsKeyDownUpArrow;
        private bool m_IsKeyDownDownArrow;
        private bool m_IsPanningLeft;
        private bool m_IsPanningRight;
        private bool m_IsPanningUp;
        private bool m_IsPanningDown;
        private bool m_Panning;
        private bool m_IsPanEasingToZero;
        private Vector3 m_CurrentPanVec;
        private Vector3 m_LocalRightDirBeforePannining;
        private Vector3 m_LocalUpDirBeforePanning;
        private Vector3 m_CurrentPanDelta;
        private Vector3 m_CurrentPanDir;

        private VTweenCore m_RotTween;
        private VTweenCore m_DistanceTween;
        private VTweenCore m_PanTween;
        private VTDeltaTimeTracker m_DeltaTimeTracker;

        private void OnEnable()
        {
            m_DeltaTimeTracker.Prepare();
        }

        private void OnDisable()
        {
            m_PreviewRenderUtility.Cleanup();
            DestroyImmediate(m_PreviewInstance);
        }

        private void OnDestroy()
        {
            if (m_PreviewRenderUtility != null)
            {
                m_PreviewRenderUtility.Cleanup();
            }

            DestroyImmediate(m_PreviewInstance);
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            if (m_Initailized == false)
            {
                m_PreviewRenderUtility = new PreviewRenderUtility();
                Image image = objectToPreview.GetComponent<Image>();

                // If objectToPreview is an UI object, create a new gameobject and add a sprite renderer to it
                if (image != null && image.sprite != null)
                {
                    GameObject gameObject = new GameObject();
                    SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = image.sprite;
                    m_PreviewInstance = gameObject;
                }
                else
                {
                    // Now we can be sure that objectToPreview is an GameObject with a renderer,
                    // set its position and rotation in the construction function to make it visible to the camera
                    m_PreviewInstance = Instantiate(objectToPreview, Vector3.zero, Quaternion.identity);
                    m_PreviewInstance.DisableAllScriptsRecursive();
                    // Internal interactive preview system works on local scale
                    m_PreviewInstance.transform.localScale = objectToPreview.transform.lossyScale;
                }

                m_PreviewInstance.hideFlags = HideFlags.HideAndDontSave;
                // VTRevealer might set objectToPreview to false, so just make sure the object is active 
                m_PreviewInstance.SetActive(true);
                m_PreviewRenderUtility.AddSingleGO(m_PreviewInstance);
                m_PreviewRenderUtility.camera.nearClipPlane = 0.0001f;
                m_PreviewRenderUtility.camera.farClipPlane = 10000f;
                m_PreviewRenderUtility.ambientColor = new Color(.1f, .1f, .1f, 0);
                m_CamToObjectCenterDistance = CalculateDistance(m_PreviewRenderUtility.camera);

                m_HasModelPreview = HasModelPreview(m_PreviewInstance);

                if (m_HasModelPreview)
                {
                    m_RotAroundX = 30f;
                    m_RotAroundY = 50f;
                }

                m_Initailized = true;
            }

            Event evt = Event.current;
            m_DeltaTimeTracker.Update();

            // Handle Camera Rotation
            RotatePreviewCamera(rect);

            // Handle Camera Zoom
            // This is faster than checking event type inside the function, effectively preventing unecessary function call overhead
            if (evt.type == EventType.ScrollWheel)
                ZoomPreviewModel(evt);

            // Handle Camera Pan
            InitializePanning();
            HandlePanning();

            // Remap deconstructed quaternion values to -180 - 180 so that when we drag the view while tweening, there is no jumping because of angle inconsistency
            // During deconstruction process, we are safe to remap it to -180 to 180
            if (m_RotAroundX > 180)
            {
                m_RotAroundX = m_RotAroundX - 360f;
            }

            // Remap to -180 - 180
            if (m_RotAroundY > 180)
            {
                m_RotAroundY = m_RotAroundY - 360f;
            }

            // Handle Rotate Smooth
            if (m_RotTween == null || m_RotTween != null && m_RotTween.m_Removed == true)
            {
                Vector3 targetRot = new Vector3(m_RotAroundX, m_RotAroundY, 0);
                m_TargetRot = Quaternion.Euler(targetRot);

                if (VTQuaternion.Approximately(m_CurrentRot, m_TargetRot))
                {
                    m_CurrentRot = m_TargetRot;
                }
                else
                {
                    m_CurrentRot = Quaternion.Slerp(m_CurrentRot, m_TargetRot, m_DeltaTimeTracker.deltaTime * m_RotateSmoothValue * 50);
                }
            }

            if (m_DistanceTween == null || m_DistanceTween != null && m_DistanceTween.m_Removed == true)
            {
                // Handle Zoom Smooth
                m_CurrentZoomLevel = Mathf.SmoothDamp(m_CurrentZoomLevel, m_TargetZoomLevel, ref m_DummyDampZoomVelocity, m_DistanceChangeSmoothTime);
            }

            if (m_Panning || m_PanTween != null && m_PanTween.m_Removed == false)
            {
                m_CurrentPanDelta += GetPanSpeedVec();
            }

            m_PreviewRenderUtility.BeginPreview(rect, background);

            float distance = m_CamToObjectCenterDistance * (1 / m_CurrentZoomLevel);
            m_PreviewRenderUtility.lights[0].intensity = lightIntensity;
            m_PreviewRenderUtility.lights[1].enabled = false;

            m_PreviewRenderUtility.camera.transform.rotation = m_CurrentRot;
            m_PreviewRenderUtility.camera.transform.position = m_BoundsCenter + m_PreviewRenderUtility.camera.transform.forward * (-distance) + m_CurrentPanDelta * m_OverallRadius;
            // Lights position should not be changed by panning
            m_PreviewRenderUtility.lights[0].transform.rotation = m_CurrentRot;
            m_PreviewRenderUtility.lights[0].transform.position = m_BoundsCenter + m_PreviewRenderUtility.camera.transform.forward * (-1);

            m_PreviewRenderUtility.camera.Render();
            m_PreviewRenderUtility.EndAndDrawPreview(rect);
        }

        public void RotatePreviewCamera(Rect position)
        {
            int controlID = GUIUtility.GetControlID("RotatePreviewCamera".GetHashCode(), FocusType.Passive);
            Event current = Event.current;

            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition))
                    {
                        if (GUIUtility.hotControl == 0)
                            GUIUtility.hotControl = controlID;

                        current.Use();
                        // If user rotates the model while tweening, remove the tween and give user control of the process
                        m_RotTween?.Remove();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }

                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }

                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        // Rotation around the X axis is changed by mouse dragging along the y axis
                        m_RotAroundX += current.delta.y / position.height * 80f;
                        // Rotation around the Y axis is changed by mouse dragging along the X axis
                        m_RotAroundY += current.delta.x / position.width * 100f;
                        m_RotAroundX = Mathf.Clamp(m_RotAroundX, -89f, 89f);
                        current.Use();
                        GUI.changed = true;
                    }

                    break;
            }
        }

        public void FrontView()
        {
            m_PanTween?.Remove();
            m_PanTween = VTweenCreator.TweenVector3(m_CurrentPanDelta, pan => m_CurrentPanDelta = pan, Vector3.zero).SetDuration(VTVector3.Approximately(m_CurrentPanDelta, Vector3.zero, 0.001f) ? 0f : 0.5f).OnComplete(() =>
            {
                m_RotTween?.Remove();
                m_RotTween = VTweenCreator.TweenQuaternion(m_CurrentRot, rot => m_CurrentRot = rot, new Vector3(0, 0, 0)).SetDuration(0.5f).OnValueChanged(() =>
                {
                    m_RotAroundX = m_CurrentRot.eulerAngles.x;
                    m_RotAroundY = m_CurrentRot.eulerAngles.y;
                });
            });
        }

        public void TopView()
        {
            m_PanTween?.Remove();
            m_PanTween = VTweenCreator.TweenVector3(m_CurrentPanDelta, pan => m_CurrentPanDelta = pan, Vector3.zero).SetDuration(VTVector3.Approximately(m_CurrentPanDelta, Vector3.zero, 0.001f) ? 0f : 0.5f).OnComplete(() =>
            {
                m_RotTween?.Remove();
                m_RotTween = VTweenCreator.TweenQuaternion(m_CurrentRot, rot => m_CurrentRot = rot, new Vector3(89, 0, 0)).SetDuration(0.5f).OnValueChanged(() =>
                {
                    m_RotAroundX = m_CurrentRot.eulerAngles.x;
                    m_RotAroundY = m_CurrentRot.eulerAngles.y;
                });
            });
        }

        public void LeftView()
        {
            m_PanTween?.Remove();
            m_PanTween = VTweenCreator.TweenVector3(m_CurrentPanDelta, pan => m_CurrentPanDelta = pan, Vector3.zero).SetDuration(VTVector3.Approximately(m_CurrentPanDelta, Vector3.zero, 0.001f) ? 0f : 0.5f).OnComplete(() =>
            {
                m_RotTween?.Remove();
                m_RotTween = VTweenCreator.TweenQuaternion(m_CurrentRot, rot => m_CurrentRot = rot, new Vector3(0, 89, 0)).SetDuration(0.5f).OnValueChanged(() =>
                {
                    m_RotAroundX = m_CurrentRot.eulerAngles.x;
                    m_RotAroundY = m_CurrentRot.eulerAngles.y;
                });
            });
        }

        public void ResetObjectView()
        {
            // Don't forget to set target values, otherwise when tweening is finished, current value will changing towards an old target value
            if (m_HasModelPreview)
            {
                m_PanTween?.Remove();
                m_PanTween = VTweenCreator.TweenVector3(m_CurrentPanDelta, pan => m_CurrentPanDelta = pan, Vector3.zero).SetDuration(VTVector3.Approximately(m_CurrentPanDelta, Vector3.zero, 0.001f) ? 0f : 0.5f).OnComplete(() =>
                {
                    m_DistanceTween?.Remove();
                    m_DistanceTween = VTweenCreator.TweenFloat(m_CurrentZoomLevel, zoom => m_CurrentZoomLevel = zoom, 1).SetDuration(0.75f).OnValueChanged(() =>
                    {
                        m_TargetZoomLevel = m_CurrentZoomLevel;
                    });

                    m_RotTween?.Remove();
                    m_RotTween = VTweenCreator.TweenQuaternion(m_CurrentRot, rot => m_CurrentRot = rot, new Vector3(30, 50, 0)).SetDuration(0.75f).OnValueChanged(() =>
                    {
                        m_RotAroundX = m_CurrentRot.eulerAngles.x;
                        m_RotAroundY = m_CurrentRot.eulerAngles.y;
                    });

                    m_LightTween?.Remove();
                    m_LightTween = VTweenCreator.TweenFloat(lightIntensity, intensity => lightIntensity = intensity, 0.8f).SetDuration(0.5f).OnValueChanged(Repaint);
                });
            }
            else
            {
                m_PanTween?.Remove();
                m_PanTween = VTweenCreator.TweenVector3(m_CurrentPanDelta, pan => m_CurrentPanDelta = pan, Vector3.zero).SetDuration(VTVector3.Approximately(m_CurrentPanDelta, Vector3.zero, 0.001f) ? 0f : 0.5f).OnComplete(() =>
                {
                    m_DistanceTween?.Remove();
                    m_DistanceTween = VTweenCreator.TweenFloat(m_CurrentZoomLevel, zoom => m_CurrentZoomLevel = zoom, 1).SetDuration(0.75f).OnValueChanged(() =>
                    {
                        m_TargetZoomLevel = m_CurrentZoomLevel;
                    });

                    m_RotTween?.Remove();
                    m_RotTween = VTweenCreator.TweenQuaternion(m_CurrentRot, rot => m_CurrentRot = rot, new Vector3(0, 0, 0)).SetDuration(0.75f).OnValueChanged(() =>
                    {
                        m_RotAroundX = m_CurrentRot.eulerAngles.x;
                        m_RotAroundY = m_CurrentRot.eulerAngles.y;
                    });

                    m_LightTween?.Remove();
                    m_LightTween = VTweenCreator.TweenFloat(lightIntensity, intensity => lightIntensity = intensity, 0.8f).SetDuration(0.5f).OnValueChanged(Repaint);
                });
            }
        }

        private void ZoomPreviewModel(Event evt)
        {
            // If we use apple trackpad to scroll and if speed is too high, weird jittering occurs, so we treat it with special care 
            float mouseDeltaZoom;
            float sign = Mathf.Sign(HandleUtility.niceMouseDeltaZoom);

            // To clamp a value which could be negative to its respective bound we follow this pattern:
            // Get the bigger value when delta zoom is negative and smaller value when delta zoom is positive, so checking its absolute value is a convenient way
            if (Mathf.Abs(HandleUtility.niceMouseDeltaZoom) <= m_MaxMouseDeltaZoom)
            {
                mouseDeltaZoom = HandleUtility.niceMouseDeltaZoom;
            }
            else
            {
                mouseDeltaZoom = m_MaxMouseDeltaZoom * sign;
            }

            // The system has been engineered to provide the most pleasant interaction experience, only speed of 1 is supported
            float zoomDelta = -(mouseDeltaZoom * 0.05f) * 1f;
            float distance = 1 / m_TargetZoomLevel;
            distance += zoomDelta;
            m_TargetZoomLevel = 1 / distance;
            m_TargetZoomLevel = Mathf.Clamp(m_TargetZoomLevel, m_MinZoomLevel, m_MaxZoomLevel);
            evt.Use();
            m_DistanceTween?.Remove();
            GUI.changed = true;
        }

        private Bounds CalculateOverallBounds(GameObject parentObject)
        {
            Bounds overallBounds = new Bounds();
            bool firstObject = true;
            // By using renderer bounds, we take both bounds size and object scale into consideration
            Renderer[] renderers = parentObject.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                Bounds childBounds = renderer.bounds;

                if (firstObject)
                {
                    overallBounds = childBounds;
                    firstObject = false;
                }
                else
                {
                    overallBounds.Encapsulate(childBounds);
                }
            }

            return overallBounds;
        }

        private Vector3 CalculateCenterPosition(Bounds bounds)
        {
            return (bounds.min + bounds.max) * 0.5f;
        }

        private float CalculateRadius(Bounds bounds, Vector3 center)
        {
            // Multiplied by two to leave some room on the edge of the window
            return Vector3.Distance(center, bounds.max) * 2.5f;
        }

        private float CalculateDistance(Camera previewCamera)
        {
            Bounds overallBounds = CalculateOverallBounds(m_PreviewInstance);
            m_BoundsCenter = CalculateCenterPosition(overallBounds);
            m_OverallRadius = CalculateRadius(overallBounds, m_BoundsCenter);
            return m_OverallRadius / Mathf.Sin(previewCamera.fieldOfView * Mathf.Deg2Rad);
        }

        private void InitializePanning()
        {
            int controlID = GUIUtility.GetControlID("PanPreviewCamera".GetHashCode(), FocusType.Passive);
            Event evt = Event.current;
            Transform previewCamTransform = m_PreviewRenderUtility.camera.transform;

            switch (evt.GetTypeForControl(controlID))
            {
                case EventType.KeyDown:
                    if (evt.keyCode == KeyCode.A || evt.keyCode == KeyCode.D || evt.keyCode == KeyCode.LeftArrow || evt.keyCode == KeyCode.RightArrow)
                    {
                        m_LocalRightDirBeforePannining = previewCamTransform.right;
                        // If pan keys are pressed while a pan tween is running, stop and remove the tween to give user immediate control
                        m_PanTween?.Remove();
                    }

                    if (evt.keyCode == KeyCode.W || evt.keyCode == KeyCode.S || evt.keyCode == KeyCode.UpArrow || evt.keyCode == KeyCode.DownArrow)
                    {
                        m_LocalUpDirBeforePanning = previewCamTransform.up;
                        m_PanTween?.Remove();
                    }

                    if (evt.keyCode == KeyCode.A)
                    {
                        m_IsKeyDownA = true;
                        m_IsPanningLeft = true;
                        // Set moving in the opposite direction to false
                        m_IsPanningRight = false;
                    }
                    else if (evt.keyCode == KeyCode.D)
                    {
                        m_IsKeyDownD = true;
                        m_IsPanningRight = true;
                        m_IsPanningLeft = false;
                    }
                    else if (evt.keyCode == KeyCode.LeftArrow)
                    {
                        m_IsKeyDownLeftArrow = true;
                        m_IsPanningLeft = true;
                        m_IsPanningRight = false;
                    }
                    else if (evt.keyCode == KeyCode.RightArrow)
                    {
                        m_IsKeyDownRightArrow = true;
                        m_IsPanningRight = true;
                        m_IsPanningLeft = false;
                    }
                    else if (evt.keyCode == KeyCode.W)
                    {
                        m_IsKeyDownW = true;
                        m_IsPanningUp = true;
                        m_IsPanningDown = false;
                    }
                    else if (evt.keyCode == KeyCode.S)
                    {
                        m_IsKeyDownS = true;
                        m_IsPanningDown = true;
                        m_IsPanningUp = false;
                    }
                    else if (evt.keyCode == KeyCode.UpArrow)
                    {
                        m_IsKeyDownUpArrow = true;
                        m_IsPanningUp = true;
                        m_IsPanningDown = false;
                    }
                    else if (evt.keyCode == KeyCode.DownArrow)
                    {
                        m_IsKeyDownDownArrow = true;
                        m_IsPanningDown = true;
                        m_IsPanningUp = false;
                    }

                    evt.Use();
                    break;
                case EventType.KeyUp:
                    if (evt.keyCode == KeyCode.A)
                    {
                        m_IsKeyDownA = false;

                        if (!m_IsKeyDownLeftArrow)
                        {
                            m_IsPanningLeft = false;
                            m_LastButtonUpTimeStamp = (float)EditorApplication.timeSinceStartup;

                            if (m_IsKeyDownD || m_IsKeyDownRightArrow)
                            {
                                m_IsPanningRight = true;
                            }
                        }
                    }
                    else if (evt.keyCode == KeyCode.D)
                    {
                        m_IsKeyDownD = false;

                        if (!m_IsKeyDownRightArrow)
                        {
                            m_IsPanningRight = false;
                            m_LastButtonUpTimeStamp = (float)EditorApplication.timeSinceStartup;

                            if (m_IsKeyDownA || m_IsKeyDownLeftArrow)
                            {
                                m_IsPanningLeft = true;
                            }
                        }
                    }
                    else if (evt.keyCode == KeyCode.LeftArrow)
                    {
                        m_IsKeyDownLeftArrow = false;

                        if (!m_IsKeyDownA)
                        {
                            m_IsPanningLeft = false;
                            m_LastButtonUpTimeStamp = (float)EditorApplication.timeSinceStartup;

                            if (m_IsKeyDownD || m_IsKeyDownRightArrow)
                            {
                                m_IsPanningRight = true;
                            }
                        }
                    }
                    else if (evt.keyCode == KeyCode.RightArrow)
                    {
                        m_IsKeyDownRightArrow = false;

                        if (!m_IsKeyDownD)
                        {
                            m_IsPanningRight = false;
                            m_LastButtonUpTimeStamp = (float)EditorApplication.timeSinceStartup;

                            if (m_IsKeyDownA || m_IsKeyDownLeftArrow)
                            {
                                m_IsPanningLeft = true;
                            }
                        }
                    }
                    else if (evt.keyCode == KeyCode.W)
                    {
                        m_IsKeyDownW = false;

                        if (!m_IsKeyDownUpArrow)
                        {
                            m_IsPanningUp = false;
                            m_LastButtonUpTimeStamp = (float)EditorApplication.timeSinceStartup;

                            if (m_IsKeyDownS || m_IsKeyDownDownArrow)
                            {
                                m_IsPanningDown = true;
                            }
                        }
                    }
                    else if (evt.keyCode == KeyCode.S)
                    {
                        m_IsKeyDownS = false;

                        if (!m_IsKeyDownDownArrow)
                        {
                            m_IsPanningDown = false;
                            m_LastButtonUpTimeStamp = (float)EditorApplication.timeSinceStartup;

                            if (m_IsKeyDownW || m_IsKeyDownUpArrow)
                            {
                                m_IsPanningUp = true;
                            }
                        }
                    }
                    else if (evt.keyCode == KeyCode.UpArrow)
                    {
                        m_IsKeyDownUpArrow = false;

                        if (!m_IsKeyDownW)
                        {
                            m_IsPanningUp = false;
                            m_LastButtonUpTimeStamp = (float)EditorApplication.timeSinceStartup;

                            if (m_IsKeyDownS || m_IsKeyDownDownArrow)
                            {
                                m_IsPanningDown = true;
                            }
                        }
                    }
                    else if (evt.keyCode == KeyCode.DownArrow)
                    {
                        m_IsKeyDownDownArrow = false;

                        if (!m_IsKeyDownS)
                        {
                            m_IsPanningDown = false;
                            m_LastButtonUpTimeStamp = (float)EditorApplication.timeSinceStartup;

                            if (m_IsKeyDownW || m_IsKeyDownUpArrow)
                            {
                                m_IsPanningUp = true;   
                            }
                        }
                    }

                    evt.Use();
                    break;
            }
        }

        private void HandlePanning()
        {
            m_CurrentPanDir = Vector3.zero;
            float nowTime = (float)EditorApplication.timeSinceStartup;

            // Introduce set delay so that releasing two input buttons at approximately the same time will tween the speed in that combined direction instead of the direction of either one to Vector3.zero
            // because the buttons are always being released one after another, after one is released(m_Panning is still true), if we don't have set delay, the direction will also become the direction controlled by the remaining button which would result a problematic tweening
            if (m_IsPanningLeft && nowTime - m_LastButtonUpTimeStamp > m_SetPanDirDelay)
            {
                m_CurrentPanDir -= m_LocalRightDirBeforePannining;
            }

            if (m_IsPanningRight && nowTime - m_LastButtonUpTimeStamp > m_SetPanDirDelay)
            {
                m_CurrentPanDir += m_LocalRightDirBeforePannining;
            }

            if (m_IsPanningUp && nowTime - m_LastButtonUpTimeStamp > m_SetPanDirDelay)
            {
                m_CurrentPanDir += m_LocalUpDirBeforePanning;
            }

            if (m_IsPanningDown && nowTime - m_LastButtonUpTimeStamp > m_SetPanDirDelay)
            {
                m_CurrentPanDir -= m_LocalUpDirBeforePanning;
            }

            m_Panning = m_CurrentPanDir.sqrMagnitude > 0;

            float speedModifier = 2;

            if (Event.current.shift)
            {
                speedModifier *= 2;
            }

            if (m_Panning)
            {
                // Increase the pan speed when zooming away from the object and decrease it when zooming up close
                // Use exponential growth to simulate acceleration changing faster and faster (the 'jerk' in phisics)
                m_TargetPanSpeed = m_TargetPanSpeed < Mathf.Epsilon ? 0.65f * Mathf.Min(1.5f, 1 / m_CurrentZoomLevel) : m_TargetPanSpeed * Mathf.Pow(2.75f, m_DeltaTimeTracker.deltaTime);
                // Clamp target pan speed
                m_TargetPanSpeed = Mathf.Min(2.25f, m_TargetPanSpeed);
                m_CurrentPanVec = m_TargetPanSpeed * m_CurrentPanDir.normalized * speedModifier;
                m_IsPanEasingToZero = false;
            }
            else
            {
                // If not panning, reset target pan speed to give it a chance to acquire start value on the next panning
                m_TargetPanSpeed = 0;

                // Introduce this flag to ensure that only tweening once to zero velocity when not panning
                if (!m_IsPanEasingToZero)
                {
                    m_PanTween?.Remove();
                    m_PanTween = VTweenCreator.TweenVector3(m_CurrentPanVec, vec => m_CurrentPanVec = vec, Vector3.zero).SetDuration(0.35f);
                    m_IsPanEasingToZero = true;
                }
            }
        }

        private Vector3 GetPanSpeedVec()
        {
            return m_CurrentPanVec * m_DeltaTimeTracker.deltaTime;
        }

        private static bool HasModelPreview(GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
                return false;

            foreach (Renderer renderer in renderers)
            {
                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();

                if (renderer is MeshRenderer && meshFilter != null && meshFilter.sharedMesh != null)
                {
                    return true;
                }

                if (renderer is SkinnedMeshRenderer && ((SkinnedMeshRenderer)renderer).sharedMesh != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}