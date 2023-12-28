using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;
using System.Linq;

namespace Victor.Tools
{
    public class VTPrefabLoader : EditorWindow
    {
        public List<OuterListInfo> outerList;

        private const float k_PreviewWindowWidth = 190;
        private const float k_PreviewWindowHeight = 120;
        private int m_MouseDragThreshold = 2;
        private bool m_EnterPrefabStageFromPrefabLoader;
        private bool m_Loaded;
        private bool m_ShouldRepaintEveryFrame;
        private bool m_IsPreviewWindowOpen;
        private string m_LastMouseOverPreviewIdentifier;
        // The scroll position can not be static, otherwise it will be reinitialize every time domain reloads
        private Vector2 m_ScrollPosition;
        private SerializedObject m_PrefabLoaderSO;
        // In order to draw custom inspector, we need to use reorderable list in both parent list and child list
        private SerializedProperty m_OuterListProperty;
        private ReorderableList m_OuterReorderableList;
        // Preserve inner prefab list states Make inner list item selectable
        private Dictionary<string, ReorderableList> m_InnerPrefabListDict = new Dictionary<string, ReorderableList>();
        private Texture2D m_CurrentInteractedPreviewTexture = null;
        private Vector2 m_MouseDownPos;

        [Serializable]
        public class OuterListInfo
        {
            public string CategoryName;
            public bool NotFoldedOut;
            public List<GameObject> PrefabList;
        }

        [MenuItem("Tools/Victor/Dev Window/VTPrefabLoader", priority = 1)]
        private static void ShowWindow()
        {
            var window = GetWindow<VTPrefabLoader>();
            window.minSize = new Vector2(300, 200);
            window.titleContent = new GUIContent("VTPrefabLoader", VTPrefabLoaderIcons.PrefabLoaderTitleTex);
            window.Show();
        }

        private void OnEnable()
        {
            m_PrefabLoaderSO = new SerializedObject(this);
            m_OuterListProperty = m_PrefabLoaderSO.FindProperty(nameof(outerList));
            m_OuterListProperty.isExpanded = true;
            Undo.undoRedoPerformed += Repaint;
            EditorApplication.quitting += AutomaticSave;
            float spacing = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (!m_Loaded)
            {
                Load();
                m_Loaded = true;
            }

            VTPrefabLoaderIcons.LoadTextures();

            PrefabStage.prefabStageClosing += (PrefabStage stage) =>
            {
                if (m_EnterPrefabStageFromPrefabLoader)
                {
                    EditorWindow.FocusWindowIfItsOpen<VTPrefabLoader>();
                    m_EnterPrefabStageFromPrefabLoader = false;
                }
            };

            m_OuterReorderableList = new ReorderableList(m_PrefabLoaderSO, m_OuterListProperty)
            {
                multiSelect = true
            };


            m_OuterReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, new GUIContent("Prefab Lists", VTGUIIcons.PrefabIcon), VTStyles.centeredBoldLabel);
            };

            m_OuterReorderableList.drawElementCallback = (Rect rect, int outerIndex, bool isActive, bool isFocused) =>
            {
                // The serialized property of prefab list 
                SerializedProperty prefabListProperty = m_OuterListProperty.GetArrayElementAtIndex(outerIndex).FindPropertyRelative(nameof(OuterListInfo.PrefabList));
                SerializedProperty groupNameProperty = m_OuterListProperty.GetArrayElementAtIndex(outerIndex).FindPropertyRelative(nameof(OuterListInfo.CategoryName));
                SerializedProperty notFoldedOutProperty = m_OuterListProperty.GetArrayElementAtIndex(outerIndex).FindPropertyRelative(nameof(OuterListInfo.NotFoldedOut));
                ReorderableList list;
                string listKey = prefabListProperty.propertyPath;

                if (m_InnerPrefabListDict.ContainsKey(listKey))
                {
                    list = m_InnerPrefabListDict[listKey];
                }
                else
                {
                    list = new ReorderableList(m_PrefabLoaderSO, prefabListProperty)
                    {
                        multiSelect = true,

                        drawHeaderCallback = (Rect rect) =>
                        {
                            EditorGUI.LabelField(rect, $"{groupNameProperty.stringValue} List", EditorStyles.boldLabel);

                            if (rect.Contains(Event.current.mousePosition))
                            {
                                if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                                {
                                    // If the selected object is prefab but not a prefab instance
                                    GameObject[] selectedPrefabs = DragAndDrop.objectReferences.OfType<GameObject>().Where(go => (PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.Regular || PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.Model || PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.Variant) && !PrefabUtility.IsPartOfPrefabInstance(go)).ToArray();
                                    int selectedPrefabsCount = selectedPrefabs.Length;

                                    if (selectedPrefabsCount > 0)
                                    {
                                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                    }

                                    if (Event.current.type == EventType.DragPerform)
                                    {
                                        Undo.RecordObject(this, "Add To Custom Prefab List");
                                        outerList[outerIndex].PrefabList.AddRange(selectedPrefabs);
                                        DragAndDrop.AcceptDrag();
                                        GUI.changed = true;
                                    }

                                    Event.current.Use();
                                }
                            }
                        },

                        drawElementCallback = (Rect rect, int innerIndex, bool isActive, bool isFocused) =>
                        {
                            Event e = Event.current;
                            rect.y += 2;
                            float editButtonWidth = 50;
                            float previewButtonWidth = 20;
                            float padding = 3;
                            bool isModelPrefabType = false;
                            bool hasInteractivePreview = false;

                            Rect prefabPreviewButtonRect = new Rect(rect.x + 2, rect.y - 1, previewButtonWidth, previewButtonWidth);
                            
                            VTGUI.StoreGUIEnabled();
                            GameObject oldPrefabAsset = (GameObject)prefabListProperty.GetArrayElementAtIndex(innerIndex).objectReferenceValue;
                            GameObject prefabAsset = (GameObject)prefabListProperty.GetArrayElementAtIndex(innerIndex).objectReferenceValue;

                            // If the prefab asset changes, its preview texture should be updated as well
                            if (oldPrefabAsset != prefabAsset)
                            {
                                m_LastMouseOverPreviewIdentifier = null;
                            }

                            string loadButtonText = "Nope";

                            if (prefabAsset == null)
                            {
                                Rect prefabPropertyRect = new Rect(prefabPreviewButtonRect.x + previewButtonWidth + padding, rect.y, rect.width - editButtonWidth - prefabPreviewButtonRect.width - padding * 3, EditorGUIUtility.singleLineHeight);
                                prefabListProperty.GetArrayElementAtIndex(innerIndex).objectReferenceValue = EditorGUI.ObjectField(prefabPropertyRect, GUIContent.none, prefabAsset, typeof(GameObject), false);
                                GUI.enabled = false;
                                Rect editButtonRect = new Rect(prefabPropertyRect.x + prefabPropertyRect.width + padding, rect.y, editButtonWidth, EditorGUIUtility.singleLineHeight);
                                GUI.Button(editButtonRect, loadButtonText);
                                VTGUI.RevertGUIEnabled();
                            }
                            else
                            {
                                Texture2D previewTex = null;
                                previewTex = AssetPreview.GetAssetPreview(prefabAsset);

                                if (AssetPreview.IsLoadingAssetPreview(prefabAsset.GetInstanceID()))
                                {
                                    Repaint();
                                }

                                if (previewTex != null)
                                {
                                    hasInteractivePreview = true;
                                }
                                else
                                {
                                    previewTex = VTPrefabLoaderIcons.NoPreviewTex;
                                }

                                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                                string currentMouseOverPreviewIdentifier = GetMouseOverPreviewIdentifier(outerIndex, innerIndex);

                                if (prefabPreviewButtonRect.Contains(e.mousePosition) && GUIUtility.hotControl == 0 && hasInteractivePreview)
                                {
                                    if (!currentMouseOverPreviewIdentifier.Equals(m_LastMouseOverPreviewIdentifier))
                                    {
                                        DestroyImmediate(m_CurrentInteractedPreviewTexture);
                                        m_CurrentInteractedPreviewTexture = null;
                                        m_CurrentInteractedPreviewTexture = new Texture2D(previewTex.width, previewTex.height);
                                        m_CurrentInteractedPreviewTexture.hideFlags = HideFlags.HideAndDontSave;
                                        Color[] colors = previewTex.GetPixels();
                                        for (int i = 0; i < colors.Length; i++)
                                        {
                                            colors[i] = colors[i].MultS(0.88f).MultV(0.95f);
                                        }
                                        m_CurrentInteractedPreviewTexture.SetPixels(colors);
                                        m_CurrentInteractedPreviewTexture.Apply();
                                    }

                                    m_LastMouseOverPreviewIdentifier = currentMouseOverPreviewIdentifier;
                                }

                                if (e.type == EventType.MouseDown && e.button == 0 && prefabPreviewButtonRect.Contains(e.mousePosition))
                                {
                                    GUIUtility.hotControl = controlID;
                                    m_LastMouseOverPreviewIdentifier = string.Empty;

                                    if (hasInteractivePreview)
                                    {
                                        if (m_CurrentInteractedPreviewTexture != null)
                                        {
                                            DestroyImmediate(m_CurrentInteractedPreviewTexture);
                                            m_CurrentInteractedPreviewTexture = null;
                                        }

                                        m_CurrentInteractedPreviewTexture = new Texture2D(previewTex.width, previewTex.height);
                                        m_CurrentInteractedPreviewTexture.hideFlags = HideFlags.HideAndDontSave;
                                        Color[] colors = previewTex.GetPixels();

                                        for (int i = 0; i < colors.Length; i++)
                                        {
                                            colors[i] = colors[i].MultS(0.75f).MultV(0.85f);
                                        }

                                        m_CurrentInteractedPreviewTexture.SetPixels(colors);
                                        m_CurrentInteractedPreviewTexture.Apply();
                                    }

                                    m_MouseDownPos = e.mousePosition;
                                    e.Use();
                                }

                                if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == controlID)
                                {
                                    if (Vector2.Distance(e.mousePosition, m_MouseDownPos) > m_MouseDragThreshold)
                                    {
                                        DragAndDrop.PrepareStartDrag();
                                        DragAndDrop.objectReferences = new Object[] { prefabAsset };
                                        DragAndDrop.StartDrag(prefabAsset.name);
                                    }
                                }

                                if (Event.current.type == EventType.DragUpdated && GUIUtility.hotControl == controlID)
                                {
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                                }

                                if (Event.current.type == EventType.DragPerform && GUIUtility.hotControl == controlID)
                                {
                                    foreach (Object obj in DragAndDrop.objectReferences)
                                    {
                                        GameObject gameObject = obj as GameObject;

                                        if (gameObject != null)
                                        {
                                            // Check if the drop target is the scene view or the project view
                                            if (DragAndDrop.paths.Length > 0)
                                            {
                                                // Drop target is the project view
                                                // Create a new prefab from the selected GameObject and save it in the project
                                                Object prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, DragAndDrop.paths[0] + "/" + gameObject.name + ".prefab", InteractionMode.UserAction);
                                                DragAndDrop.AcceptDrag();
                                                Event.current.Use();
                                            }
                                            else if (DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0] is SceneView)
                                            {
                                                // Drop target is the scene view
                                                // Instantiate a copy of the selected GameObject in the scene view
                                                GameObject instantiatedObject = Instantiate(gameObject);
                                                instantiatedObject.name = gameObject.name;
                                                SceneView.lastActiveSceneView.AlignViewToObject(instantiatedObject.transform);
                                                DragAndDrop.AcceptDrag();
                                                Event.current.Use();
                                            }
                                        }
                                    }
                                }

                                if (e.type == EventType.DragExited && GUIUtility.hotControl == controlID)
                                {
                                    GUIUtility.hotControl = 0;
                                    DragAndDrop.visualMode = DragAndDropVisualMode.None;
                                    e.Use();
                                }

                                if (e.type == EventType.MouseUp && controlID == GUIUtility.hotControl)
                                {
                                    GUIUtility.hotControl = 0;

                                    if (hasInteractivePreview && prefabPreviewButtonRect.Contains(e.mousePosition))
                                    {
                                        Renderer[] renderers = prefabAsset.GetComponentsInChildren<Renderer>();
                                       
                                        bool hasModelPreview = false;

                                        foreach (var renderer in renderers)
                                        {
                                            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();

                                            if (renderer is MeshRenderer && meshFilter != null && meshFilter.sharedMesh != null)
                                            {
                                                hasModelPreview = true;
                                                break;
                                            }

                                            if (renderer is SkinnedMeshRenderer && ((SkinnedMeshRenderer)renderer).sharedMesh != null)
                                            {
                                                hasModelPreview = true;
                                                break;
                                            }
                                        }

                                        
                                        m_ShouldRepaintEveryFrame = true;
                                        Rect showRect = new Rect(prefabPreviewButtonRect);
                                        showRect.x -= 80;
                                        showRect.y += 5;
                                        VTPreviewWindow previewWindow = new VTPreviewWindow(prefabAsset, k_PreviewWindowWidth, k_PreviewWindowHeight, hasModelPreview);
                                        previewWindow.onOpenCallback += () => m_IsPreviewWindowOpen = true;
                                        previewWindow.onCloseCallback += () => m_IsPreviewWindowOpen = false;
                                        // The preview window could be closed by clicking outside of the window in which case, directly stop repainting every frame
                                        previewWindow.onCloseCallback += () => m_ShouldRepaintEveryFrame = false;
                                        PopupWindow.Show(showRect, previewWindow);

                                        if (!m_IsPreviewWindowOpen)
                                        {
                                            m_ShouldRepaintEveryFrame = false;
                                        }
                                    }

                                    e.Use();
                                }

                                if ((GUIUtility.hotControl == controlID || (prefabPreviewButtonRect.Contains(e.mousePosition) && GUIUtility.hotControl == 0)) && hasInteractivePreview)
                                {
                                    if (m_CurrentInteractedPreviewTexture != null)
                                    {
                                        GUI.DrawTexture(prefabPreviewButtonRect, m_CurrentInteractedPreviewTexture);
                                    }
                                }
                                else
                                {
                                    GUI.DrawTexture(prefabPreviewButtonRect, previewTex);
                                }

                                Rect prefabPropertyRect = new Rect(prefabPreviewButtonRect.x + prefabPreviewButtonRect.width + padding, rect.y, rect.width - editButtonWidth - prefabPreviewButtonRect.width - padding * 2, EditorGUIUtility.singleLineHeight);
                                prefabListProperty.GetArrayElementAtIndex(innerIndex).objectReferenceValue = EditorGUI.ObjectField(prefabPropertyRect, GUIContent.none, prefabAsset, typeof(GameObject), false);

                                Rect editButtonRect = new Rect(prefabPropertyRect.x + prefabPropertyRect.width + padding + (previewTex == null ? 2 : 0), rect.y, editButtonWidth, EditorGUIUtility.singleLineHeight);

                                isModelPrefabType = PrefabUtility.GetPrefabAssetType(prefabAsset) == PrefabAssetType.Model;

                                if (isModelPrefabType)
                                {
                                    loadButtonText = "Model";
                                    GUI.enabled = false;
                                }
                                else
                                {
                                    loadButtonText = "Edit";
                                }

                                if (GUI.Button(editButtonRect, loadButtonText))
                                {
                                    PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(prefabAsset));
                                    EditorWindow window = VTOpenDefaultWindow.GetDefaultWindow(VTOpenDefaultWindow.DefaultWindowType.Inspector);
                                    m_EnterPrefabStageFromPrefabLoader = true;
                                }

                                VTGUI.RevertGUIEnabled();
                            }
                        },

                        onAddCallback = (ReorderableList rl) =>
                        {
                            int newElementIndex = rl.serializedProperty.arraySize;
                            rl.serializedProperty.arraySize++;

                            var element = rl.serializedProperty.GetArrayElementAtIndex(newElementIndex);
                            element.objectReferenceValue = null;
                        }

                    };
                    m_InnerPrefabListDict[listKey] = list;
                };

                // Setup the inner list
                rect.y += 2;
                Rect foldOutRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                Rect nameRect = new Rect(rect.x, rect.y + spacing, rect.width, EditorGUIUtility.singleLineHeight);
                notFoldedOutProperty.boolValue = EditorGUI.BeginFoldoutHeaderGroup(foldOutRect, notFoldedOutProperty.boolValue, groupNameProperty.stringValue);

                if (notFoldedOutProperty.boolValue == true)
                {
                    var height = rect.height - EditorGUIUtility.standardVerticalSpacing - spacing - nameRect.height;

                    EditorGUI.PropertyField(nameRect, groupNameProperty);
                    list.DoList(new Rect(rect.x, nameRect.y + nameRect.height + EditorGUIUtility.standardVerticalSpacing, rect.width, height));
                }
                EditorGUI.EndFoldoutHeaderGroup();
            };

            m_OuterReorderableList.elementHeightCallback = (int index) =>
            {
                float height = 0;

                if (m_OuterListProperty.arraySize > 0)
                {
                    var element = m_OuterListProperty.GetArrayElementAtIndex(index);
                    if (element != null)
                    {
                        var prefabListProperty = element.FindPropertyRelative("PrefabList");
                        var listFoldedOutProperty = element.FindPropertyRelative(nameof(OuterListInfo.NotFoldedOut));
                        if (listFoldedOutProperty.boolValue == false)
                        {
                            height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        }
                        else
                        {
                            if (prefabListProperty.arraySize == 0 || prefabListProperty.arraySize == 1)
                            {
                                height = spacing * 6;
                            }
                            else
                            {
                                // +3 here is to take the spacing between prefab elements into consideration
                                height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 3) * (prefabListProperty.arraySize) + spacing * 5;
                            }

                        }

                        return height;
                    }

                }

                return height;
            };

            m_OuterReorderableList.onAddCallback = (ReorderableList rl) =>
            {
                int newElementIndex = rl.serializedProperty.arraySize;
                rl.serializedProperty.arraySize++;

                var element = rl.serializedProperty.GetArrayElementAtIndex(newElementIndex);
                element.FindPropertyRelative(nameof(OuterListInfo.CategoryName)).stringValue = "";
                element.FindPropertyRelative(nameof(OuterListInfo.NotFoldedOut)).boolValue = true;
                element.FindPropertyRelative(nameof(OuterListInfo.PrefabList)).ClearArray();
                
                rl.index = newElementIndex;
            };
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Repaint;
            EditorApplication.quitting -= AutomaticSave;
        }

        private void OnDestroy()
        {
            AutomaticSave();
        }

        private void OnGUI()
        {
            m_PrefabLoaderSO.Update();
            EditorGUI.BeginChangeCheck();
            DrawToolBar();
            DrawPrefabLists();

            // When a prefab preview window is open, we need to repaint every frame to give mouse over state a chance to update before the mouse enters the window
            if (m_ShouldRepaintEveryFrame)
            {
                Repaint();
            }

            if (EditorGUI.EndChangeCheck())
            {
                m_PrefabLoaderSO.ApplyModifiedProperties();
            }
        }

        private void OnInspectorUpdate()
        {
            if (mouseOverWindow != null && mouseOverWindow.ToString() == this.ToString())
            {
                Repaint();
            }
        }

        private void DrawToolBar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                {
                    ManualSave();
                }
            }
        }

        private void DrawPrefabLists()
        {
            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(m_ScrollPosition))
            {
                m_ScrollPosition = scrollViewScope.scrollPosition;
                m_OuterReorderableList.DoLayoutList();
            }
        }

        private void Load()
        {
            outerList = new List<OuterListInfo>();
            outerList.AddRange(VTPrefabLoaderData.instance.outerList);
        }

        private void AutomaticSave()
        {
            Save(true);
        }

        private void ManualSave()
        {
            Save(false);
        }

        private void Save(bool automaticSave)
        {
            VTPrefabLoaderData.instance.Save(outerList);

            if (!automaticSave)
            {
                Debug.Log($"{"[VTPrefabLoader]".Bold().Color(VTColorLibrary.victorYellowGreen)} {" Saved at ".Bold()} {DateTime.Now.ToLongTimeString().Bold().Color(VTStringExtension.RichTextBuiltInColorNames.orange)}");
            }
        }

        private string GetMouseOverPreviewIdentifier(int outerIndex, int innerIndex)
        {
            return outerIndex + "_" + innerIndex;
        }
    }
}