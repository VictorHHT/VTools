using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Victor.EditorTween;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Victor.Tools
{
    public class VTRevealerWindow : EditorWindow
    {
        private static bool s_Focusing;
        private static bool s_ShouldDrawOutline = false;
        private static Vector3 s_OutlineScale;
        private static Vector3 s_OutlinePos;
        private static GameObject s_CurrentHoveredObj;
        private static GameObject s_PreviousHoveredObj;
        private static GUIStyle s_ObjLabelStyle;
        private static GUIStyle s_RevealerCategoryStyle;
        private static VTPreviewWindow s_ObjectPreviewWindow;
        private static VTweenCore s_OutlineScaleTween;
        private static VTweenCore s_OutlinePosTween;
        private static VTweenConfig s_OutlineTweenConfig;
        private static VTHighlighter s_Highlighter;

        [SerializeField]
        private RevealerHit[] m_RevealerHits;
        // The direct hit objects, excluding children
        [SerializeField]
        private List<GameObject> m_HitObjects;
        private int m_ObjOverlapCount;
        private int m_CurrentObjOverlapCount = 0;
        private int m_CurrentUIOverlapCount = 0;
        private float m_ObjTitleColorHue = 1;
        private float m_UITitleColorHue = 0;
        private bool m_ShouldAppendObjNewLine = true;
        private bool m_ShouldAppendUINewLine = true;
        private bool m_ShouldAppendObjOverlapCount = false;
        private bool m_ShouldAppendUIOverlapCount = false;
        private string m_GameObjectTitleString = "";
        private string m_UITitleString = "";
        private Vector2 m_ScrollPosition;
        private Vector3 m_WindowScale;
        private Color m_ObjTitleGUIColor;
        private Color m_UITitleGUIColor;
        private VTweenCore m_ObjTitleColorHueTween;
        private VTweenCore m_ObjTitleGUIColorTween;
        private VTweenCore m_ObjTitleStringTween;
        private VTweenCore m_UITitleColorHueTween;
        private VTweenCore m_UITitleGUIColorTween;
        private VTweenCore m_UITitleStringTween;

        public static void Init()
        {
            Event evt = Event.current;
            RevealerHit[] revealerHits = VTOverlapRevealer.PickAll(RevealerFilter.revealerFilter, SceneView.lastActiveSceneView, evt.mousePosition, out int objOverlapCount);
            bool hasOpenedInstance = HasOpenInstances<VTRevealerWindow>();
            var window = GetWindow<VTRevealerWindow>(true, "VTRevealer", true);
            window.minSize = new Vector2(120, 140);
            // A little trick so that a non-static field could be set in a static function, because there exists only one window instance at a time
            window.m_RevealerHits = revealerHits;
            window.m_ObjOverlapCount = objOverlapCount;
            window.m_CurrentObjOverlapCount = objOverlapCount;
            window.m_CurrentUIOverlapCount = revealerHits.Length - objOverlapCount;

            if (!hasOpenedInstance)
            {
                Rect newPosition = new Rect();
                SceneView sceneView = SceneView.lastActiveSceneView;
                float sceneViewCenterX;
                bool shouldSpawnLeft = false;

                if (sceneView.maximized || !sceneView.docked)
                {
                    sceneViewCenterX = sceneView.position.center.x;
                }
                else
                {
                    sceneViewCenterX = GetSceneViewRect(sceneView).center.x;
                }

                // When scene view is maximized the view will be huge, we don't want the user navigate all the way to the opposite side to interact with the window
                // Mouse on the right side
                if (evt.mousePosition.x > sceneViewCenterX)
                {
                    if (!sceneView.maximized)
                    {
                        shouldSpawnLeft = true;
                    }
                }
                else if (sceneView.maximized) // Mouse on the left side
                {
                    shouldSpawnLeft = true;
                }

                // Spawn the window on the opposite side of the mouse position right next to the edge of the scene view
                if (shouldSpawnLeft)
                {
                    newPosition.x = sceneView.position.xMin + 2;
                    // When the scene view is maximized, move it up a little bit to avoid clipping the debug message strip
                    newPosition.y = sceneView.position.yMax - 142 - (sceneView.maximized ? 10 : 0);
                    newPosition.width = 220;
                    newPosition.height = 160;
                }
                else
                {
                    newPosition.x = sceneView.position.xMax - 221;
                    newPosition.y = sceneView.position.yMax - 142 - (sceneView.maximized ? 10 : 0);
                    newPosition.width = 220;
                    newPosition.height = 160;
                }

                bool isOSXEditor = Application.platform == RuntimePlatform.OSXEditor;

                if (isOSXEditor)
                {
                    // Window Scale Tween
                    VTweenCreator.TweenVector3(window.m_WindowScale, newScale => window.m_WindowScale = newScale, newPosition.size).SetDuration(0.6f).OnValueChanged(() =>
                    {
                        Vector3 newSize = new Vector3(Mathf.Max(1, window.m_WindowScale.x), Mathf.Max(window.m_WindowScale.y, 1));
                        newPosition.size = newSize;
                        window.position = newPosition;
                        window.Repaint();
                    });

                    // Set window initial position before doing the tweening, without this line, the window will disappear a short time then reappear at the specified position, resulting in jittery motion
                    window.position = new Rect(newPosition.x, newPosition.y, 1, 1);
                }
                else
                {
                    window.position = new Rect(newPosition.x, newPosition.y, 220, 160);
                }
                
                window.m_ObjTitleStringTween = VTweenCreator.TweenString<StringAppendApplier>(window.m_GameObjectTitleString, newString => window.m_GameObjectTitleString = newString, "GameObject").SetDuration(0.8f).SetEaseType(EaseType.EaseInOutSine).SetInitialDelay(isOSXEditor ? 0.4f : 0.1f).OnValueChanged(window.Repaint)
                    .OnComplete(() =>
                    {
                        // Don't forget the reserved newline for the title string to
                        window.m_ObjTitleStringTween = VTweenCreator.TweenString<StringAppendApplier>(window.m_GameObjectTitleString + "\n", (newString) => window.m_GameObjectTitleString = newString, "GameObject" + "\n Count: 0").SetDuration(0.9f).SetEaseType(EaseType.EaseInQuad).OnValueChanged(window.Repaint).OnStart(() =>
                        {
                            window.m_ShouldAppendObjNewLine = false;
                        })
                        .OnComplete(() =>
                        {
                            // Remove fake 0 and append tweened integer value to the title string
                            window.m_GameObjectTitleString = window.m_GameObjectTitleString.Substring(0, window.m_GameObjectTitleString.Length - 1);
                            window.m_ShouldAppendObjOverlapCount = true;
                            int targetCount = window.m_CurrentObjOverlapCount;
                            window.m_CurrentObjOverlapCount = 0;
                            VTweenCreator.TweenInt(0, (newValue) => window.m_CurrentObjOverlapCount = newValue, targetCount).SetEaseType(EaseType.EaseOutSine).SetDuration(0.75f).SetInitialDelay(0.35f).OnValueChanged(window.Repaint);
                        });
                    });

                window.m_UITitleStringTween = VTweenCreator.TweenString<StringAppendApplier>(window.m_UITitleString, newString => window.m_UITitleString = newString, "UI").SetDuration(objOverlapCount != 0 ? 0.8f : 0.5f).SetEaseType(EaseType.EaseOutCirc).SetInitialDelay(isOSXEditor ? 0.2f : 0.05f).OnValueChanged(window.Repaint)
                    .OnComplete(() =>
                    {
                        window.m_UITitleStringTween = VTweenCreator.TweenString<StringAppendApplier>(window.m_UITitleString + "\n", (newString) => window.m_UITitleString = newString, "UI" + "\n Count: 0").SetDuration(1).SetEaseType(EaseType.EaseInSine).SetInitialDelay(objOverlapCount != 0 ? 0.4f : 0.1f).OnValueChanged(window.Repaint).OnStart(() =>
                        {
                            window.m_ShouldAppendUINewLine = false;
                        })
                        .OnComplete(() =>
                        {
                            window.m_UITitleString = window.m_UITitleString.Substring(0, window.m_UITitleString.Length - 1);
                            window.m_ShouldAppendUIOverlapCount = true;
                            int targetCount = window.m_CurrentUIOverlapCount;
                            window.m_CurrentUIOverlapCount = 0;
                            VTweenCreator.TweenInt(0, (newValue) => window.m_CurrentUIOverlapCount = newValue, targetCount).SetEaseType(EaseType.EaseOutSine).SetDuration(0.65f).SetInitialDelay(0.3f).OnValueChanged(window.Repaint);
                        });
                    });
            }

            if (window.m_HitObjects != null)
            {
                foreach (var hitObject in window.m_HitObjects)
                {
                    DestroyImmediate(hitObject);
                }
            }

            window.m_HitObjects = new List<GameObject>();

            foreach (var revealerHit in window.m_RevealerHits)
            {
                GameObject obj = Instantiate(revealerHit.gameObject, Vector3.zero, Quaternion.identity);
                obj.transform.localScale = revealerHit.gameObject.transform.lossyScale;
                VTGameObject.DestroyAllChildrenImmediate(obj);

                foreach (MonoBehaviour script in obj.GetComponents<MonoBehaviour>())
                {
                    script.enabled = false;
                }

                obj.hideFlags = HideFlags.HideAndDontSave;
                obj.SetActive(false);
                window.m_HitObjects.Add(obj);
            }

            window.Show();
        }

        private void OnEnable()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;

            if (s_Highlighter == null)
            {
                s_Highlighter = new VTHighlighter();
            }

            // Tween
            s_OutlineTweenConfig = new VTweenConfig();
            s_OutlineTweenConfig.SetDuration(0.5f).OnValueChanged(() =>
            {
                foreach (SceneView sceneView in SceneView.sceneViews)
                {
                    sceneView.Repaint();
                }
            });

            m_ObjTitleColorHue = 1f;
            m_UITitleColorHue = 0f;
            VTweenConfig hueTweenConfig = new VTweenConfig();
            hueTweenConfig.SetPlayStyle(PlayStyle.Normal).SetInfinite(true);
            m_ObjTitleColorHueTween = VTweenCreator.TweenFloat(m_ObjTitleColorHue, newH => m_ObjTitleColorHue = newH, 0f).SetConfig(hueTweenConfig).SetDuration(5f).SetEaseType(EaseType.EaseInOutCirc);
            m_UITitleColorHueTween = VTweenCreator.TweenFloat(m_UITitleColorHue, newHue => m_UITitleColorHue = newHue, 1f).SetConfig(hueTweenConfig).SetDuration(7f).SetEaseType(EaseType.Linear);
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;

            // Tween
            m_ObjTitleColorHueTween?.Remove();
            m_ObjTitleGUIColorTween?.Remove();
            m_ObjTitleStringTween?.Remove();

            m_UITitleColorHueTween?.Remove();
            m_UITitleGUIColorTween?.Remove();
            m_UITitleStringTween?.Remove();
        }

        // When this window closes, also close opening preview window
        private void OnDestroy()
        {
            if (s_ObjectPreviewWindow != null && s_ObjectPreviewWindow.editorWindow != null)
            {
                s_ObjectPreviewWindow.editorWindow.Close();
                s_ObjectPreviewWindow = null;
            }

            // Destroying hit objects in OnDestroy is a must, Unity doesn't dispose instantiated objects after the window is closed.
            // And we can't rely on Init to free the instantiated objects which belongs to previously closed window,
            // because each window created is a new instance, therefore m_HitObjects references to a new list
            if (m_HitObjects != null)
            {
                foreach (var hitObject in m_HitObjects)
                {
                    DestroyImmediate(hitObject);
                }
            }

            // Tween outline scale to Vector3.zero after window closes
            CleanUp();
        }

        private void OnFocus()
        {
            FocusWindowIfItsOpen<VTRevealerWindow>();
            s_Focusing = true;
            s_ShouldDrawOutline = true;

            // Tween
            m_ObjTitleGUIColorTween?.Remove();
            m_UITitleGUIColorTween?.Remove();
            float randomObjTitleHue = Random.Range(0f, 1f);
            // Make obj title and ui title have the same gui color at begining
            float randomUITitleHue = randomObjTitleHue;

            VTweenConfig objGUIColorTweenConfig = new VTweenConfig();
            objGUIColorTweenConfig.OnComplete(() =>
            {
                // Obj title color transition loop delay is 0.75f and duration is 1.25f
                m_ObjTitleGUIColorTween = VTweenCreator.TweenColor(m_ObjTitleGUIColor, color => m_ObjTitleGUIColor = color, Color.white.NewS(0.6f).NewH(m_ObjTitleColorHue)).SetConfig(objGUIColorTweenConfig).SetInitialDelay(0.75f).SetDuration(1.25f + Random.Range(-0.2f, 0.4f)).SetEaseType(EaseType.EaseInOutSine).OnValueChanged(Repaint);
            });

            VTweenConfig uiGUIColorTweenConfig = new VTweenConfig();
            uiGUIColorTweenConfig.OnComplete(() =>
            {
                // UI title color transition loop delay is 0.35f and duration is 0.75f
                m_UITitleGUIColorTween = VTweenCreator.TweenColor(m_UITitleGUIColor, color => m_UITitleGUIColor = color, Color.white.NewS(0.8f).NewH(m_UITitleColorHue)).SetConfig(uiGUIColorTweenConfig).SetLoopDelay(0.2f).SetDuration(0.65f + Random.Range(-0.1f, 0.2f)).SetEaseType(EaseType.EaseInOutSine).OnValueChanged(Repaint);
            });

            // When the window gets focus, obj and ui title will have the same color initially
            m_ObjTitleGUIColorTween = VTweenCreator.TweenColor(m_ObjTitleGUIColor, color => m_ObjTitleGUIColor = color, Color.white.NewS(0.6f).NewH(randomObjTitleHue)).SetConfig(objGUIColorTweenConfig).SetEaseType(EaseType.EaseOutQuart).OnValueChanged(Repaint);
            m_UITitleGUIColorTween = VTweenCreator.TweenColor(m_UITitleGUIColor, color => m_UITitleGUIColor = color, Color.white.NewS(0.6f).NewH(randomUITitleHue)).SetConfig(uiGUIColorTweenConfig).SetEaseType(EaseType.EaseOutQuart).OnValueChanged(Repaint);
        }

        private void OnLostFocus()
        {
            m_ObjTitleGUIColorTween?.Remove();
            m_UITitleGUIColorTween?.Remove();
            VTweenConfig tweenConfig = new VTweenConfig().SetDuration(0.75f).SetEaseType(EaseType.EaseOutQuad).OnValueChanged(Repaint);
            m_ObjTitleGUIColorTween = VTweenCreator.TweenColor(m_ObjTitleGUIColor, color => m_ObjTitleGUIColor = color, Color.white).SetConfig(tweenConfig);
            m_UITitleGUIColorTween = VTweenCreator.TweenColor(m_UITitleGUIColor, color => m_UITitleGUIColor = color, Color.white).SetConfig(tweenConfig);
            CleanUp();
        }

        private void OnGUI()
        {
            if (s_ObjLabelStyle == null)
            {
                s_ObjLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                s_ObjLabelStyle.hover.textColor = Color.white;
                // Pressed Color
                s_ObjLabelStyle.active.textColor = VTColor.Gray(0.65f);

                s_RevealerCategoryStyle = new GUIStyle(VTStyles.centeredBoldLabel);
                s_RevealerCategoryStyle.normal.textColor = VTColor.Gray(0.95f);
            }

            Event evt = Event.current;

            if (m_RevealerHits != null && m_RevealerHits.Length > 0)
            {
                GUILayout.Space(8);

                using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(m_ScrollPosition))
                {
                    int index = 0;
                    m_ScrollPosition = scrollViewScope.scrollPosition;

                    foreach (var overlapInfo in m_RevealerHits)
                    {
                        // If the object is removed from the scene, don't draw its controls
                        if (overlapInfo.gameObject == null)
                        {
                            continue;
                        }

                        if (m_ObjOverlapCount != 0 && index == 0)
                        {
                            VTGUI.StoreGUIBackgroundAndContentColor();
                            GUI.contentColor = m_ObjTitleGUIColor;

                            // Append a new line for the title string so the content doesn't move down when the second stage of the tween starts to play
                            string titleString = m_GameObjectTitleString + (m_ShouldAppendObjNewLine ? "\n" : "") + (m_ShouldAppendObjOverlapCount ? m_CurrentObjOverlapCount : "");
                            VTGUILayout.Label(new GUIContent(titleString), s_RevealerCategoryStyle);
                            s_ObjLabelStyle.hover.textColor = m_ObjTitleGUIColor;
                            GUILayout.Space(5);

                            VTGUI.RevertGUIBackgroundAndContentColor();
                        }
                        else if (m_ObjOverlapCount != -1 && index == m_ObjOverlapCount)
                        {
                            VTGUI.StoreGUIBackgroundAndContentColor();
                            GUI.contentColor = m_UITitleGUIColor;

                            string titleString = m_UITitleString + (m_ShouldAppendUINewLine ? "\n" : "") + (m_ShouldAppendUIOverlapCount ? m_CurrentUIOverlapCount : "");
                            VTGUILayout.Label(new GUIContent(titleString), s_RevealerCategoryStyle);
                            s_ObjLabelStyle.hover.textColor = m_UITitleGUIColor;
                            GUILayout.Space(5);

                            VTGUI.RevertGUIBackgroundAndContentColor();
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        Rect previewIconRect = GUILayoutUtility.GetRect(16, 16);
                        // Temporarily set the instantiated object to true for preview
                        m_HitObjects[index].SetActive(true);
                        Texture2D texture = VTPreviewUtility.GetPreview(m_HitObjects[index], out bool hasModelPreview);
                        RectTransform rectTransform = overlapInfo.gameObject.GetComponent<RectTransform>();

                        if (texture != null)
                        {
                            GUI.DrawTexture(previewIconRect, texture, ScaleMode.ScaleToFit);

                            if (evt.type == EventType.MouseDown)
                            {
                                if (previewIconRect.Contains(evt.mousePosition))
                                {
                                    Rect showRect = new Rect(previewIconRect);
                                    showRect.x -= 40;
                                    showRect.y += 2;

                                    s_ObjectPreviewWindow = new VTPreviewWindow(m_HitObjects[index], hasModelPreview: hasModelPreview);
                                    // Need to set the instantiated object to false here, because the code that follows might repaint the scene view before it being set to false at the end of this function
                                    m_HitObjects[index].SetActive(false);
                                    PopupWindow.Show(showRect, s_ObjectPreviewWindow);
                                    evt.Use();
                                }
                            }

                            if (evt.type == EventType.ScrollWheel && s_ObjectPreviewWindow != null && s_ObjectPreviewWindow.editorWindow != null)
                            {
                                GUIUtility.hotControl = 0;
                                s_ObjectPreviewWindow.editorWindow.Close();
                                s_ObjectPreviewWindow = null;
                            }
                        }

                        // Check if the object is still in the scene
                        if (overlapInfo.gameObject != null && VTGUILayout.ClickableLabel(new GUIContent(overlapInfo.gameObject.name), s_ObjLabelStyle))
                        {
                            // If the object has been selected, zoom the scene view camera to it
                            if (Selection.activeGameObject == overlapInfo.gameObject)
                            {
                                float distance;
                                Bounds objBounds = CalculateObjectBounds(overlapInfo.gameObject, out bool isSprite);
                                Vector3 objCenter = GetObjectCenter(overlapInfo.gameObject);

                                // If object is an UI object
                                if (rectTransform != null)
                                {
                                    distance = rectTransform.rect.size.magnitude * rectTransform.lossyScale.magnitude * 0.3f;
                                }
                                else
                                {
                                    // The object is not an UI object and has at least one component attached (in addition to Transform component)
                                    distance = objBounds.size.magnitude * 0.5f;

                                    // Adjust for GameObjects which has component attached, but have no bound (Camera, Light Source etc..)
                                    if (distance <= 0)
                                    {
                                        distance = 1f;
                                    }
                                }

                                SceneView sceneView = SceneView.lastActiveSceneView;
                                // This differs from normal scene look at which focuses on the combined center of parent object and children objects
                                sceneView.LookAt(objCenter, sceneView.camera.transform.rotation, distance, sceneView.orthographic);
                                s_Highlighter.HighlightSceneObject(overlapInfo.gameObject);
                            }

                            // Clear previously selected GameObjects, because we use Cmd/Ctrl + Shift + LMB as the shortcut,
                            // there is a high probability that user click the scene view multiple times without letting go of either Shift or Cmd/Ctrl, which multiple objects will be selected
                            // And merely setting Selection.activeGameObject doesn't clear currently selected objects
                            Selection.objects = null;
                            // If the object is not being selected, select it
                            Selection.activeGameObject = overlapInfo.gameObject;
                        }

                        Rect labelRect = GUILayoutUtility.GetLastRect();

                        // Mouse hover over a name label of an object
                        if (labelRect.Contains(evt.mousePosition) && s_Focusing)
                        {
                            // If the window lost focus and regain focus or if the window is closed and opened again, reset position tween center to current object
                            if (s_CurrentHoveredObj == null)
                            {
                                s_OutlinePos = GetObjectCenter(overlapInfo.gameObject);
                            }

                            s_CurrentHoveredObj = overlapInfo.gameObject;

                            // Mouse hover on a new rect
                            if (s_CurrentHoveredObj != s_PreviousHoveredObj)
                            {
                                Bounds objBounds = CalculateObjectBounds(overlapInfo.gameObject, out bool isSprite);
                                s_OutlineScaleTween?.Remove();
                                s_OutlineScaleTween = VTweenCreator.TweenVector3(s_OutlineScale, newScale => s_OutlineScale = newScale, isSprite ? objBounds.size.NewZ(0) : objBounds.size).SetConfig(s_OutlineTweenConfig);
                                s_OutlinePosTween?.Remove();
                                s_OutlinePosTween = VTweenCreator.TweenVector3(s_OutlinePos, newPos => s_OutlinePos = newPos, GetObjectCenter(overlapInfo.gameObject)).SetConfig(s_OutlineTweenConfig);
                            }
                        }

                        s_PreviousHoveredObj = s_CurrentHoveredObj;

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(4);

                        // Hide objects in the scene
                        m_HitObjects[index].SetActive(false);
                        index++;
                    }
                }
            }

            if (s_Focusing)
            {
                Repaint();
            }

            // When there is no scene view in the editor, close current revealer window
            if (SceneView.sceneViews.Count == 0)
            {
                Close();
            }
        }

        /// <summary>
        /// Draw mouse hover object outline
        /// </summary>
        public static void DrawObjectOutline()
        {
            // If window is not in focus, doesn't hover over any rect or preview window is open, don't draw outline
            // s_OutlinePosAnimV3 may not be initialized at all (user has not hovered on a rect after the window is opened), don't draw outline
            if (!s_ShouldDrawOutline || s_OutlinePosTween == null || s_OutlineScaleTween == null)
                return;

            VTDebugDraw.HandlesDrawCube(s_OutlinePos, s_OutlineScale, VTColorLibrary.victorError, 1.5f);
        }

        /// <summary>
        /// Calculates the bounds of a GameObject and determines if it is a sprite
        /// </summary>
        /// <param name="obj">The GameObject to calculate bounds for.</param>
        /// <param name="isSprite">Indicates if the object is a SpriteRenderer.</param>
        /// <returns>The calculated bounds of the object.</returns>
        private static Bounds CalculateObjectBounds(GameObject obj, out bool isSprite)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            isSprite = false;

            if (renderer != null)
            {
                if (renderer is MeshRenderer renderer1)
                {
                    return renderer1.bounds;
                }

                if (renderer is SpriteRenderer renderer2)
                {
                    isSprite = true;
                    return renderer2.bounds;
                }

                if (renderer is SkinnedMeshRenderer renderer3)
                {
                    return renderer3.bounds;
                }

                // Objects with renderer like LineRenderer, SpriteShapeRenderer etc
                return renderer.bounds;
            }

            if (rectTransform != null)
            {
                // If the object is UI element, create bounds based on its position and size
                Vector3[] fourCorners = new Vector3[4];
                rectTransform.GetWorldCorners(fourCorners);

                // Dummy Center
                return new Bounds(Vector3.zero, new Vector3(fourCorners[2].x - fourCorners[0].x, fourCorners[3].y - fourCorners[1].y, 0));
            }

            // For camera and lights and such objects with no renderer attached, considered no bounds, thus outline size is zero
            return new Bounds();
        }

        /// <summary>
        /// Get object center, based on whether object has a renderer component
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>If object has a renderer, return renderer bounds, otherwise, return transform position</returns>
        private static Vector3 GetObjectCenter(GameObject obj)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            RectTransform rectTransform = obj.GetComponent<RectTransform>();

            if (renderer != null)
            {
                return renderer.bounds.center;
            }
            else
            {
                // UI
                if (rectTransform != null)
                {
                    Vector2 center = rectTransform.rect.center;
                    Vector3 centerInWorldSpace = rectTransform.TransformPoint(center);
                    return centerInWorldSpace;
                }

                return obj.transform.position;
            }
        }

        /// <summary>
        /// This method retrieves a docked sceneview's actual position. When a scene view is docked. its position is relative to its container (sceneView.position doesn't work). 
        /// </summary>
        /// <param name="sceneView"></param>
        /// <returns></returns>
        private static Rect GetSceneViewRect(EditorWindow sceneView)
        {
            var dynamicSceneView = sceneView.AsDynamic();
            var parent = dynamicSceneView.m_Parent;
            var containerPos = sceneView.position;

            if (parent != null && parent.GetType().Name != "DockArea")
            {
                containerPos = parent.position;
            }

            return new Rect(sceneView.position.x - containerPos.x, sceneView.position.y - containerPos.y, sceneView.position.width, sceneView.position.height);
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (mode == OpenSceneMode.Single)
            {
                Close();
            }
        }

        private void CleanUp()
        {
            s_CurrentHoveredObj = null;
            s_PreviousHoveredObj = null;
            s_OutlineScaleTween?.Remove();
            // Because of this line (s_PreviousHoveredObj = s_CurrentHoveredObj;), we have to set both current hovered object and previous hovered object to null
            s_OutlineScaleTween = VTweenCreator.TweenVector3(s_OutlineScale, newScale => s_OutlineScale = newScale, Vector3.zero).SetConfig(s_OutlineTweenConfig).OnComplete(() =>
            {
                // Use custom boolean value instead of setting current and previous hovered objects to null to prevent this bug: When the window is not focused and assembly reloaded, if mouse hover over the label and move away, outline disappears
                if (!s_Focusing)
                {
                    s_ShouldDrawOutline = false;
                }
            });
            s_Focusing = false;
        }
    }
}
