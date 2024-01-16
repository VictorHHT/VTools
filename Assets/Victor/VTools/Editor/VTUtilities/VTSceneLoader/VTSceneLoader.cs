using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.IMGUI.Controls;
using Victor.EditorTween;

namespace Victor.Tools
{
    public class VTSceneLoader : EditorWindow, IHasCustomMenu
    {
        // Indicate whether VTSceneLoader is loaded
        private bool m_Loaded;
        private bool m_CanPasteAll;
        // If user has copied tag color, then he can not paste tag type
        private bool m_CanPasteTagType;
        // Same as above, even if there is data stored in m_CopiedTagInfo
        private bool m_CanPasteTagColor;
        private bool m_Searching;
        private bool m_IsTagColorNormalSet = true;
        private bool m_IsTagColorDimSet;
        private string m_SearchText;
        // The scroll position can not be static, otherwise it will be reset after assembly reload
        private Vector2 m_ScrollPosition;
        private Vector2 m_StoredSceneTagWindowScrollPosition;
        [SerializeField]
        private List<SceneInfo> m_CustomSceneInfos;
        [SerializeField]
        private List<SceneInfo> m_FilteredSceneInfos;
        private SerializedObject m_LoaderWindowSO;
        private SerializedProperty m_CustomSceneInfosProperty;
        private SerializedProperty m_FilteredSceneInfosProperty;
        private ReorderableList m_ReorderableSceneInfos;
        private ReorderableList m_FilteredReorderableSceneInfos;
        // Non-static field survives assembly reload
        private SceneTagInfo m_CopiedTagInfo = null;
        private SearchField m_SearchField;
        private PopupWindowContent m_SceneTagPopupWindowContent;
        private VTGUIStates.SmartToggleButtonState m_VTSceneLoaderSmartToggleButtonState;

        // Tween
        private float m_RemainingTagVisibility = 1f;
        private VTweenCore m_RemainingTagVisibilityTween;

        public List<SceneInfo> customSceneInfos
        {
            get { return m_CustomSceneInfos; }
            set { m_CustomSceneInfos = value; }
        }

        public SerializedObject loaderWindowSO
        {
            get { return m_LoaderWindowSO; }
            set { m_LoaderWindowSO = value; }
        }

        public SerializedProperty customSceneInfosProperty
        {
            get { return m_CustomSceneInfosProperty; }
            set { m_CustomSceneInfosProperty = value; }
        }

        public ReorderableList reorderableSceneInfos
        {
            get { return m_ReorderableSceneInfos; }
            set { m_ReorderableSceneInfos = value; }
        }

        public Vector2 storedSceneTagWindowScrollPosition
        {
            get { return m_StoredSceneTagWindowScrollPosition; }
            set { m_StoredSceneTagWindowScrollPosition = value; }
        }

        [Serializable]
        public class SceneInfo
        {
            public SceneAsset sceneAsset;
            // If this custom scene should be added to BuildSettings scenes if "Apply To Build Settings" menu item has been clicked
            public bool shouldAdd = true;
            public SceneTagInfo sceneTagInfo = new SceneTagInfo();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            if (m_CustomSceneInfos.Count == 0 || m_Searching)
            {
                menu.AddDisabledItem(new GUIContent("Sort Custom Scene List"));
            }
            else
            {
                menu.AddItem(new GUIContent("Sort Custom Scene List"), false, SortCustomSceneList);
            }

            if (m_Searching)
            {
                menu.AddDisabledItem(new GUIContent("Apply To Build Settings"));
            }
            else
            {
                menu.AddItem(new GUIContent("Apply To Build Settings"), false, ApplyCustomScenesToBuildSettings);
            }

            // Can not load if no icon show in ProjectSettings(Tag icon folder is empty)
            if (m_Searching || VTSceneLoaderSettingsProviderData.instance.customSettingsInfos == null || VTSceneLoaderSettingsProviderData.instance.customSettingsInfos.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("Load From Build Settings"));
            }
            else
            {
                menu.AddItem(new GUIContent("Load From Build Settings"), false, LoadCustomScenesFromBuildSettings);
            }
        }

        [MenuItem("Tools/Victor/Dev Window/VTSceneLoader", priority = 0)]
        private static void ShowWindow()
        {
            var window = GetWindow<VTSceneLoader>();
            window.minSize = new Vector2(300, 200);
            window.titleContent = new GUIContent("VTSceneLoader", VTSceneLoaderIcons.sceneLoaderTitleTex);
            window.Show();
        }

        private void OnEnable()
        {
            m_LoaderWindowSO = new SerializedObject(this);
            m_CustomSceneInfosProperty = m_LoaderWindowSO.FindProperty(nameof(m_CustomSceneInfos));
            m_FilteredSceneInfosProperty = m_LoaderWindowSO.FindProperty(nameof(m_FilteredSceneInfos));
            m_Searching = false;
            m_RemainingTagVisibility = 1f;

            Undo.undoRedoPerformed += Repaint;
            EditorBuildSettings.sceneListChanged += Repaint;
            EditorApplication.quitting += AutomaticSave;

            if (!m_Loaded)
            {
                Load();
                m_Loaded = true;
            }

            VTSceneLoaderIcons.LoadTextures();
            // Need to make sure the icons displayed in VTSceneTagWindow is always up to date before the window opens
            // as the 'Tag Icon' folder could be modified externally when the project is closed
            VTSceneLoaderUtility.CheckTagIconChanges();
            ValidateAndCacheTagTextures();

            if (m_ReorderableSceneInfos == null)
            {
                m_ReorderableSceneInfos = new ReorderableList(m_LoaderWindowSO, m_CustomSceneInfosProperty);
                m_FilteredReorderableSceneInfos = new ReorderableList(m_LoaderWindowSO, m_FilteredSceneInfosProperty, false, true, false, false);
                m_SearchText = null;
                m_SearchField = new SearchField();
                m_SearchField.downOrUpArrowKeyPressed += new SearchField.SearchFieldCallback(HandleSearchFieldDownOrUpArrowPressed);
                InitializeReorderableList();
                InitializeFilteredReorderableList();
            }
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Repaint;
            EditorApplication.quitting -= AutomaticSave;
            EditorBuildSettings.sceneListChanged -= Repaint;
        }

        private void OnDestroy()
        {
            AutomaticSave();
        }

        private void OnGUI()
        {
            m_LoaderWindowSO.Update();
            EditorGUI.BeginChangeCheck();
            DrawToolBar();
            HandleScrolling();
            DrawSceneList();

            // Restore dimmed tag colors back to their original
            if (!VTSceneTagWindow.isOpen && !m_IsTagColorNormalSet)
            {
                m_RemainingTagVisibilityTween?.Remove();
                m_RemainingTagVisibilityTween = VTweenCreator.TweenFloat(m_RemainingTagVisibility, newV => m_RemainingTagVisibility = newV, 1f).SetDuration(0.6f).OnValueChanged(Repaint);
                m_IsTagColorNormalSet = true;
                m_IsTagColorDimSet = false;
            }

            if (VTSceneTagWindow.isOpen && !m_IsTagColorDimSet)
            {
                m_RemainingTagVisibilityTween?.Remove();
                m_RemainingTagVisibilityTween = VTweenCreator.TweenFloat(m_RemainingTagVisibility, newV => m_RemainingTagVisibility = newV, 0.5f).SetDuration(0.6f).OnValueChanged(Repaint);
                m_IsTagColorDimSet = true;
                m_IsTagColorNormalSet = false;
            }

            if (EditorGUI.EndChangeCheck())
            {
                m_LoaderWindowSO.ApplyModifiedProperties();
            }
        }

        private void OnInspectorUpdate()
        {
            if (mouseOverWindow != null && mouseOverWindow.ToString() == this.ToString())
            {
                Repaint();
            }
        }

        // Also handles importing asset
        private void OnProjectChange()
        {
            VTSceneLoaderUtility.CheckTagIconChanges();
            ValidateAndCacheTagTextures();
        }

        private void DrawToolBar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Build Settings", EditorStyles.toolbarButton))
                {
                    EditorApplication.ExecuteMenuItem("File/Build Settings...");
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                {
                    ManualSave();
                }
            }
        }

        private void DrawSceneList()
        {
            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(m_ScrollPosition))
            {
                m_ScrollPosition = scrollViewScope.scrollPosition;
                DrawCustomSceneList();
                GUILayout.Space(50f);
                // Add a dummy GUILayout label to force horizontal scroll bar appear
                GUILayout.Label(" ", GUILayout.MinWidth(262), GUILayout.MaxHeight(0));

                if (VTSceneLoaderSettingsProviderData.instance.showBuildSettingsScenes)
                {
                    DrawBuildSettingsSceneList();
                }

                GUILayout.Space(20f);
            }
        }

        private void DrawCustomSceneList()
        {
            DrawSearchField();

            if (m_Searching)
            {
                m_FilteredReorderableSceneInfos.DoLayoutList();
            }
            else
            {
                m_ReorderableSceneInfos.DoLayoutList();
            }
        }

        private void OnDrawCustomSceneListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty sceneInfosProperty;
            List<SceneInfo> sceneInfos;

            if (m_Searching)
            {
                rect.x += 14;
                sceneInfosProperty = m_FilteredSceneInfosProperty;
                sceneInfos = m_FilteredSceneInfos;
            }
            else
            {
                sceneInfosProperty = m_CustomSceneInfosProperty;
                sceneInfos = m_CustomSceneInfos;
            }

            SerializedProperty sceneAssetProperty = sceneInfosProperty.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(SceneInfo.sceneAsset));
            SerializedProperty shouldAddProperty = sceneInfosProperty.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(SceneInfo.shouldAdd));

            float indexLabelWidth = 18;
            float customTagWidth = 18;
            float loadButtonWidth = 40;
            float shouldAddButtonWidth = 15;
            float additionalButtonWidth = 20;
            float padding = 3;
            rect.y += 2f;

            rect.height = EditorGUIUtility.singleLineHeight;

            Rect indexLabelRect = new Rect(rect.x + padding, rect.y, indexLabelWidth, indexLabelWidth);
            Rect customTagRect;

            if (VTSceneLoaderSettingsProviderData.instance.showCustomSceneIndex)
            {
                customTagRect = new Rect(indexLabelRect.x + indexLabelWidth + padding, rect.y, customTagWidth, customTagWidth);
            }
            else
            {
                customTagRect = new Rect(rect.x + padding, rect.y, customTagWidth, customTagWidth);
            }

            Rect sceneAssetRect = new Rect(customTagRect.x + customTagWidth + padding * 2, rect.y, int.MaxValue, rect.height);

            if (VTSceneLoaderSettingsProviderData.instance.showCustomSceneIndex)
            {
                sceneAssetRect.width = rect.width - indexLabelWidth - customTagWidth - loadButtonWidth - additionalButtonWidth - shouldAddButtonWidth - padding * 7 - (m_Searching ? 14 : 0);
            }
            else
            {
                sceneAssetRect.width = rect.width - customTagWidth - loadButtonWidth - additionalButtonWidth - shouldAddButtonWidth - padding * 6 - (m_Searching ? 14 : 0);
            }

            Rect loadButtonRect = new Rect(sceneAssetRect.x + sceneAssetRect.width + padding, rect.y, loadButtonWidth, rect.height);
            Rect shouldAddButtonRect = new Rect(loadButtonRect.x + loadButtonWidth + padding, rect.y + 1, shouldAddButtonWidth, shouldAddButtonWidth);
            Rect additionalRect = new Rect(shouldAddButtonRect.x + shouldAddButtonWidth + padding, rect.y, additionalButtonWidth, rect.height);

            HandleCopyAndPasteTag(index, customTagRect);

            EditorGUI.PropertyField(sceneAssetRect, sceneAssetProperty, GUIContent.none);

            VTGUI.StoreGUIEnabled();

            if (sceneAssetProperty.objectReferenceValue == null)
            {
                GUI.enabled = false;
            }

            // Tag Button
            VTGUI.StoreGUIBackgroundAndContentColor();
            Color tagColor = sceneInfos[index].sceneTagInfo.tagColor;

            // Set tag color alpha
            if (VTSceneTagWindow.openEntryIndex == index)
            {
                tagColor.a = tagColor.a * 1f;
            }
            else
            {
                tagColor.a = tagColor.a * m_RemainingTagVisibility;
            }

            if (!sceneInfos[index].sceneTagInfo.textureMissing)
            {
                GUI.contentColor = tagColor;
            }

            if (VTGUI.VTButton(customTagRect, new GUIContent(sceneInfos[index].sceneTagInfo.textureMissing ? VTSceneLoaderIcons.warningTagTex : sceneInfos[index].sceneTagInfo.tagTexture)))
            {
                customTagRect.x -= 20;
                customTagRect.y += 3;
                m_SceneTagPopupWindowContent = new VTSceneTagWindow(index, sceneInfos[index].sceneTagInfo);
                PopupWindow.Show(customTagRect, m_SceneTagPopupWindowContent);
            }

            VTGUI.RevertGUIBackgroundAndContentColor();

            if (VTSceneLoaderSettingsProviderData.instance.showCustomSceneIndex)
            {
                VTGUI.StoreGUIBackgroundAndContentColor();
                Color labelColor = sceneInfos[index].sceneTagInfo.tagColor;
                labelColor.a = 1;
                Color.RGBToHSV(labelColor, out _, out float s, out float v);
                // Make the label color looks brighter when the visibility of tag color is low
                labelColor = labelColor.NewS(Mathf.Clamp(s * 1.5f - (1 - v) * 0.25f, 0f, 1f)).NewV(Mathf.Min(1.0f, v * 1.3f + s * 0.2f + 0.1f));

                if (!sceneInfos[index].sceneTagInfo.textureMissing)
                {
                    GUI.contentColor = labelColor;
                    GUI.Label(indexLabelRect, (index + 1).ToString(), VTStyles.centeredWhiteBoldLabel);
                }
                else
                {
                    GUI.Label(indexLabelRect, (index + 1).ToString(), VTStyles.centeredBoldLabel);
                }

                VTGUI.RevertGUIBackgroundAndContentColor();
            }

            // Add to BuildSettings Button
            if (m_Searching)
            {
                m_FilteredSceneInfos[index].shouldAdd = VTGUI.SmartToggleButton(shouldAddButtonRect, rect, VTStyles.toggleButtonIncludeExclude, m_FilteredSceneInfos[index].shouldAdd, ref m_VTSceneLoaderSmartToggleButtonState, true);
            }
            else
            {
                shouldAddProperty.boolValue = VTGUI.SmartToggleButton(shouldAddButtonRect, rect, VTStyles.toggleButtonIncludeExclude, shouldAddProperty.boolValue, ref m_VTSceneLoaderSmartToggleButtonState, true);
            }

            // Additional Button
            if (GUI.Button(additionalRect, "..."))
            {
                var loaderMenu = new GenericMenu();
                SceneAsset sceneAsset = (SceneAsset)sceneAssetProperty.objectReferenceValue;
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);

                if (Application.isPlaying || sceneAsset == null || sceneAsset != null && EditorSceneManager.GetSceneByName(sceneAsset.name).isLoaded)
                {
                    loaderMenu.AddDisabledItem(new GUIContent("Load Additively"), false);
                }
                else
                {
                    loaderMenu.AddItem(new GUIContent("Load Additively"), false,
                    () =>
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                        }
                    });
                }

                loaderMenu.ShowAsContext();
            }

            if (Application.isPlaying)
            {
                GUI.enabled = false;
            }

            // Load Button
            if (GUI.Button(loadButtonRect, "Load"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    string scenePath = AssetDatabase.GetAssetPath(sceneInfos[index].sceneAsset);
                    EditorSceneManager.OpenScene(scenePath);
                }
            }

            VTGUI.RevertGUIEnabled();
        }

        private void DrawBuildSettingsSceneList()
        {
            // Get scenes property has a lot overhead, so we only get it once prior entering the loop;
            EditorBuildSettingsScene[] settingsScenes = EditorBuildSettings.scenes;

            if (settingsScenes.Length > 0)
            {
                GUILayout.Label(new GUIContent(" Build Settings Scenes", VTSceneLoaderIcons.buildSettingsTex), VTStyles.centeredBoldLabel, GUILayout.MaxHeight(17));
            }

            EditorGUILayout.Separator();

            for (int i = 0; i < settingsScenes.Length; i++)
            {
                DrawBuidlSettingsSceneListItem(i, settingsScenes[i]);
                GUILayout.Space(5);
            }
        }

        private void DrawBuidlSettingsSceneListItem(int index, EditorBuildSettingsScene scene)
        {
            var sceneName = Path.GetFileNameWithoutExtension(scene.path);

            using (new GUILayout.HorizontalScope(VTStyles.GetEvenOddBackground(index, false)))
            {
                GUILayout.Label(index.ToString(), VTStyles.centeredBoldLabel, GUILayout.Width(20));
                GUILayout.FlexibleSpace();
                GUIContent sceneNameContent = new GUIContent(sceneName, scene.path);
                Vector2 sceneNameSize = VTStyles.centeredLabel.CalcSize(sceneNameContent);

                // 180.5 is the width of the rest controls combined
                // 262 is the min width of the window before horizontal scroll bar appears
                if (sceneNameSize.x + 180.5f - position.width > 0 && sceneNameSize.x + 180.5f > 262)
                {
                    // Don't forget to specify min width of the label, otherwise the min width of the label is the width of the fully displayed text
                    GUILayout.Label(sceneNameContent, VTStyles.leftAlignedBoldLabel, GUILayout.MinWidth(90));
                }
                else
                {
                    GUILayout.Label(sceneNameContent, VTStyles.centeredBoldLabel, GUILayout.MinWidth(90));
                }

                GUILayout.FlexibleSpace();

                VTGUI.StoreGUIEnabled();

                if (Application.isPlaying)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Load"))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scene.path);
                    }
                }

                VTGUI.RevertGUIEnabled();

                if (GUILayout.Button("Locate"))
                {
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                    EditorGUIUtility.PingObject(sceneAsset);
                }

                if (GUILayout.Button("..."))
                {
                    var loaderMenu = new GenericMenu();
                    if (Application.isPlaying || SceneManager.GetActiveScene().name == sceneName)
                    {
                        loaderMenu.AddDisabledItem(new GUIContent("Load Additively"), false);
                    }
                    else
                    {
                        loaderMenu.AddItem(new GUIContent("Load Additively"), false,
                        () =>
                        {
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            {
                                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                            }
                        });
                    }
                    loaderMenu.ShowAsContext();
                }
            }
        }

        private void SortCustomSceneList()
        {
            Undo.RecordObject(this, "Sort Custom Scene List");
            List<Texture2D> tagTypeTextures = new List<Texture2D>();

            foreach (var settingsProviderInfo in VTSceneLoaderSettingsProviderData.instance.customSettingsInfos)
            {
                tagTypeTextures.Add(settingsProviderInfo.tagTexture);
            }

            m_CustomSceneInfos = m_CustomSceneInfos
                .OrderBy(sceneInfo => tagTypeTextures.IndexOf(sceneInfo.sceneTagInfo.tagTexture))
                .ThenByDescending(sceneInfo => sceneInfo.sceneTagInfo.tagColor.PerceivedLuminance())
                .ThenBy(sceneInfo => sceneInfo.sceneAsset.name)
                .ToList();
        }

        private void ApplyCustomScenesToBuildSettings()
        {
            // Invert highlight button, make yes not so obvious to instruct user
            if (EditorUtility.DisplayDialog("Caution", "Apply custom scenes to BuildSettings, continue?", "Yes", "Cancel"))
            {
                List<EditorBuildSettingsScene> newBuildSettingsScenes = new List<EditorBuildSettingsScene>();

                // Remove duplicate scenes
                List<SceneAsset> scenesToAdd = new List<SceneAsset>();

                for (int i = 0; i < m_CustomSceneInfos.Count; i++)
                {
                    if (m_CustomSceneInfos[i].shouldAdd)
                    {
                        scenesToAdd.Add(m_CustomSceneInfos[i].sceneAsset);
                    }
                }

                scenesToAdd = scenesToAdd.Distinct().ToList();

                for (int i = 0; i < scenesToAdd.Count; i++)
                {
                    SceneAsset sceneAsset = scenesToAdd[i];
                    string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                    if (!scenePath.NullOrEmpty())
                    {
                        newBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    }
                }

                EditorBuildSettings.scenes = newBuildSettingsScenes.ToArray();
            }
        }

        private void LoadCustomScenesFromBuildSettings()
        {
            if (EditorUtility.DisplayDialog("Caution", "Load scenes from BuildSettings which will override custom scenes, continue?", "Yes", "Cancel"))
            {
                Undo.RecordObject(this, "Load From BuildSettings");
                m_CustomSceneInfos.Clear();
                for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
                {
                    EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                    string scenePath = AssetDatabase.GUIDToAssetPath(scene.guid);
                    SceneInfo infoToAdd = new SceneInfo();
                    infoToAdd.sceneTagInfo = new SceneTagInfo();
                    infoToAdd.sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    infoToAdd.sceneTagInfo.tagColor = Color.white;
                    infoToAdd.sceneTagInfo.tagTexture = VTSceneLoaderSettingsProviderData.instance.customSettingsInfos[0].tagTexture;
                    m_CustomSceneInfos.Add(infoToAdd);
                }

                m_CustomSceneInfosProperty.arraySize = m_CustomSceneInfos.Count;
                // count in ReorderableList is a property, using it automatically recalculate the list count and cache result to its backing field
                // See ReorderableList.cs line 718 - 734 and related contents
                _ = m_ReorderableSceneInfos.count;
            }
        }

        private void InitializeReorderableList()
        {
            m_ReorderableSceneInfos.multiSelect = true;

            m_ReorderableSceneInfos.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, new GUIContent(" Custom Scene List", VTSceneLoaderIcons.customSceneListTex), VTStyles.centeredBoldLabel);

                if (rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                    {
                        SceneAsset[] selectedSceneAssets = DragAndDrop.objectReferences.OfType<SceneAsset>().ToArray();
                        int selectedSceneAssetsCount = selectedSceneAssets.Length;
                        
                        if (selectedSceneAssetsCount > 0)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        }

                        if (Event.current.type == EventType.DragPerform)
                        {
                            List<SceneInfo> infosToAdd = new List<SceneInfo>();
                            bool isSceneListEmpty = m_CustomSceneInfos.Count == 0;
                            Color newSceneTagColor = Color.white;
                            Texture2D newSceneTagTexture = VTSceneLoaderSettingsProviderData.instance.customSettingsInfos[0].tagTexture;

                            // Don't allow the list remembering the previous icon of a tag with missing texture
                            if (!isSceneListEmpty && !m_CustomSceneInfos[m_CustomSceneInfos.Count - 1].sceneTagInfo.textureMissing)
                            {
                                newSceneTagColor = m_CustomSceneInfos[m_CustomSceneInfos.Count - 1].sceneTagInfo.tagColor;
                                newSceneTagTexture = m_CustomSceneInfos[m_CustomSceneInfos.Count - 1].sceneTagInfo.tagTexture;
                            }

                            Undo.RecordObject(this, "Add To Custom Scene List");

                            foreach (var sceneAsset in selectedSceneAssets)
                            {
                                SceneInfo info = new SceneInfo();
                                info.sceneAsset = sceneAsset;
                                info.sceneTagInfo = new SceneTagInfo();
                                info.sceneTagInfo.tagTexture = newSceneTagTexture;
                                info.sceneTagInfo.tagColor = newSceneTagColor;
                                infosToAdd.Add(info);
                            }

                            m_CustomSceneInfos.AddRange(infosToAdd);
                            DragAndDrop.AcceptDrag();
                        }

                        Event.current.Use();
                    }
                }
            };

            m_ReorderableSceneInfos.drawElementCallback = OnDrawCustomSceneListElement;

            m_ReorderableSceneInfos.onAddCallback = (ReorderableList rl) =>
            {
                int newElementIndex = m_CustomSceneInfosProperty.arraySize;
                m_CustomSceneInfosProperty.arraySize++;
                SerializedProperty sceneAssetProperty = m_CustomSceneInfosProperty.GetArrayElementAtIndex(newElementIndex).FindPropertyRelative(nameof(SceneInfo.sceneAsset));
                SerializedProperty sceneShouldAddProperty = m_CustomSceneInfosProperty.GetArrayElementAtIndex(newElementIndex).FindPropertyRelative(nameof(SceneInfo.shouldAdd));
                SerializedProperty sceneTagProperty = m_CustomSceneInfosProperty.GetArrayElementAtIndex(newElementIndex).FindPropertyRelative(nameof(SceneInfo.sceneTagInfo));
                SerializedProperty sceneTagTypeTextureProperty = sceneTagProperty.FindPropertyRelative(nameof(SceneInfo.sceneTagInfo.tagTexture));
                SerializedProperty sceneTagColorProperty = sceneTagProperty.FindPropertyRelative(nameof(SceneInfo.sceneTagInfo.tagColor));

                sceneAssetProperty.objectReferenceValue = null;
                sceneShouldAddProperty.boolValue = true;

                SerializedProperty previousSceneTagProperty = m_CustomSceneInfosProperty.GetArrayElementAtIndex(newElementIndex).FindPropertyRelative(nameof(SceneInfo.sceneTagInfo));
                // If list is empty before adding this item, set its tag type to the first type of VTSceneLoaderSettingsProviderData
                // and if previous entry texture is missing, we set this tag type to the first in settings
                if (m_CustomSceneInfosProperty.arraySize == 1 || previousSceneTagProperty.FindPropertyRelative(nameof(SceneInfo.sceneTagInfo.textureMissing)).boolValue == true)
                {
                    // The texture won't be null, because no null texture could survive in VTSceneLoaderSettingsProviderData after CheckNewTagType() call
                    sceneTagTypeTextureProperty.objectReferenceValue = VTSceneLoaderSettingsProviderData.instance.customSettingsInfos[0].tagTexture;
                }

                if (sceneTagColorProperty.colorValue.a == 0)
                {
                    sceneTagColorProperty.colorValue = Color.white;
                }

                rl.index = newElementIndex;
            };

            // If no icon in the Tag Icons folder, add is not allowed
            m_ReorderableSceneInfos.onCanAddCallback = (ReorderableList rl) =>
            {
                return VTSceneLoaderSettingsProviderData.instance.customSettingsInfos.Count != 0;
            };
        }

        private void InitializeFilteredReorderableList()
        {
            m_FilteredReorderableSceneInfos.multiSelect = true;

            m_FilteredReorderableSceneInfos.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, new GUIContent(" Custom Scene List", VTSceneLoaderIcons.customSceneListTex), VTStyles.centeredBoldLabel);
            };

            m_FilteredReorderableSceneInfos.drawElementCallback = OnDrawCustomSceneListElement;
        }

        private void DrawSearchField()
        {
            GUILayout.Space(4);
            string oldSearchText = m_SearchText;
            m_SearchText = m_SearchField.OnGUI(m_SearchText);

            if (Event.current.type == EventType.MouseDown && m_SearchField.HasFocus())
            {
                GUIUtility.keyboardControl = 0;
                EditorGUIUtility.editingTextField = false;
            }

            if (!m_SearchText.NullOrEmpty())
            {
                if (oldSearchText != m_SearchText)
                {
                    m_Searching = true;
                    // VTSearchUtility Test
                    //VTDebug.ClearConsole();
                    FillFilteredList();
                }
            }
            else
            {
                if (oldSearchText != m_SearchText)
                {
                    m_Searching = false;
                    m_ReorderableSceneInfos.draggable = true;
                    // VTSearchUtility Test
                    //VTDebug.ClearConsole();
                    RestoreOriginal();
                }
            }
        }

        private void FillFilteredList()
        {
            m_FilteredSceneInfos = m_CustomSceneInfos
                .Where(sceneInfo =>
                {
                    return sceneInfo.sceneAsset != null && VTSearchUtility.PreFuzzyMatch(m_SearchText, sceneInfo.sceneAsset.name);
                })
                .OrderByDescending(sceneInfo =>
                {
                    int score;
                    VTSearchUtility.FuzzyMatchWithScore(m_SearchText, sceneInfo.sceneAsset.name, out score);

                    // VTSearchUtility Test
                    //Debug.Log(VTSearchUtility.GetHighlightedQuery(m_SearchText, sceneInfo.sceneAsset.name, "<size=12><color=#24ff85><b>", "</b></color></size>", false).Color(VTStringExtension.RichTextBuiltInColorNames.yellow));
                    return score;
                }).ToList();

            m_LoaderWindowSO.ApplyModifiedPropertiesWithoutUndo();
            m_LoaderWindowSO.Update();
        }

        private void RestoreOriginal()
        {
            if (m_FilteredSceneInfos != null)
            {
                m_FilteredSceneInfos.Clear();
            }

            m_LoaderWindowSO.ApplyModifiedPropertiesWithoutUndo();
            m_LoaderWindowSO.Update();
        }

        private void HandleSearchFieldDownOrUpArrowPressed()
        {
            m_ReorderableSceneInfos.GrabKeyboardFocus();
        }

        /// <summary>
        /// Display a warning icon whenever a tag's texture could not be found by the guid stored with it
        /// Only needs to call this to check if icons in the folder have been changed, call in 'OnEnable', 'OnProjectChanged' etc..
        /// </summary>
        private void ValidateAndCacheTagTextures()
        {
            foreach (var sceneInfo in m_CustomSceneInfos)
            {
                // Don't forget to set this to false when a tag with its texture missing being set to a new tag type
                sceneInfo.sceneTagInfo.textureMissing = false;
                string tagTexturePath = AssetDatabase.GetAssetPath(sceneInfo.sceneTagInfo.tagTexture);

                // Early exit if the tag icon has been deleted for good(move to the trash can)
                if (tagTexturePath.NullOrEmpty())
                {
                    sceneInfo.sceneTagInfo.textureMissing = true;
                    continue;
                }

                string[] pathNames = tagTexturePath.Split('/');

                if (pathNames[^3] != "Resources" || pathNames[^2] != VTSceneLoaderUtility.k_TagIconFolderName)
                {
                    sceneInfo.sceneTagInfo.textureMissing = true;

                    // If user has copied a tag type and it's now invalid, do not allow paste everything
                    if (m_CopiedTagInfo != null && m_CopiedTagInfo.tagTexture == sceneInfo.sceneTagInfo.tagTexture)
                    {
                        // Could do m_CanPasteTagType = false, but setting it to null is more natural
                        m_CopiedTagInfo.tagTexture = null;
                        m_CanPasteTagColor = false;
                        m_CanPasteAll = false;
                    }
                }
            }
        }

        private void HandleScrolling()
        {
            if (Event.current.type == EventType.ScrollWheel && m_SceneTagPopupWindowContent != null && m_SceneTagPopupWindowContent.editorWindow != null)
            {
                m_SceneTagPopupWindowContent.editorWindow.Close();
                // Prevent null reference exeption for whatever reason the content window becomes null
                m_SceneTagPopupWindowContent = null;
            }
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
            VTSceneLoaderData.instance.Save(m_CustomSceneInfos);

            if (!automaticSave)
            {
                Debug.Log($"{"[VTSceneLoader]".Bold().Color(Color.cyan)} {" Saved at ".Bold()} {DateTime.Now.ToLongTimeString().Bold().Color(Color.yellow)}");
            }
        }

        private void Load()
        {
            m_CustomSceneInfos = new List<SceneInfo>();
            // If I change the file externally, which will not affect the VTSceneLoaderData instance until a save command is issued,
            // so the next time the function still loads previous custom scenes into the list upon being called
            m_CustomSceneInfos.AddRange(VTSceneLoaderData.instance.customScenes);
        }

        private void HandleCopyAndPasteTag(int selectedIndex, Rect tagRect)
        {
            ReorderableList rl;
            List<SceneInfo> sceneInfos;

            if (m_Searching)
            {
                rl = m_FilteredReorderableSceneInfos;
                sceneInfos = m_FilteredSceneInfos;
            }
            else
            {
                rl = m_ReorderableSceneInfos;
                sceneInfos = m_CustomSceneInfos;
            }

            // Tag color and type copy/paste
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && tagRect.Contains(Event.current.mousePosition))
            {
                bool multiSelected = rl.selectedIndices.Count > 1 ? true : false;
                if (multiSelected)
                {
                    if (!rl.selectedIndices.Contains(selectedIndex))
                    {
                        rl.Select(selectedIndex);
                    }
                }
                else
                {
                    rl.Select(selectedIndex);
                }
            }

            if (Event.current.type == EventType.ContextClick && tagRect.Contains(Event.current.mousePosition))
            {
                var loaderMenu = new GenericMenu();

                if (sceneInfos[selectedIndex].sceneTagInfo.textureMissing)
                {
                    loaderMenu.AddDisabledItem(new GUIContent("Copy/Copy Color"), false);
                }
                else
                {
                    loaderMenu.AddItem(new GUIContent("Copy/Copy Color"), false,
                    () =>
                    {
                        if (m_CopiedTagInfo == null)
                        {
                            m_CopiedTagInfo = new SceneTagInfo();
                        }

                        m_CopiedTagInfo.tagColor = sceneInfos[selectedIndex].sceneTagInfo.tagColor;
                        m_CanPasteTagColor = true;
                        m_CanPasteTagType = false;
                        m_CanPasteAll = false;
                        rl.Select(selectedIndex);
                    });
                }

                if (sceneInfos[selectedIndex].sceneTagInfo.textureMissing)
                {
                    loaderMenu.AddDisabledItem(new GUIContent("Copy/Copy Type"), false);
                }
                else
                {
                    loaderMenu.AddItem(new GUIContent("Copy/Copy Type"), false,
                    () =>
                    {
                        if (m_CopiedTagInfo == null)
                        {
                            m_CopiedTagInfo = new SceneTagInfo();
                        }

                        m_CopiedTagInfo.tagTexture = sceneInfos[selectedIndex].sceneTagInfo.tagTexture;
                        m_CanPasteTagType = true;
                        m_CanPasteTagColor = false;
                        m_CanPasteAll = false;
                        rl.Select(selectedIndex);
                    });
                }

                loaderMenu.AddSeparator("Copy/");

                if (sceneInfos[selectedIndex].sceneTagInfo.textureMissing)
                {
                    loaderMenu.AddDisabledItem(new GUIContent("Copy/Copy All"), false);
                }
                else
                {
                    loaderMenu.AddItem(new GUIContent("Copy/Copy All"), false,
                    () =>
                    {
                        if (m_CopiedTagInfo == null)
                        {
                            m_CopiedTagInfo = new SceneTagInfo();
                        }

                        m_CopiedTagInfo.tagTexture = sceneInfos[selectedIndex].sceneTagInfo.tagTexture;
                        m_CopiedTagInfo.tagColor = sceneInfos[selectedIndex].sceneTagInfo.tagColor;
                        m_CanPasteAll = true;
                        m_CanPasteTagType = true;
                        m_CanPasteTagColor = true;
                        rl.Select(selectedIndex);
                    });
                }

                if (m_CopiedTagInfo == null || !m_CanPasteTagColor || sceneInfos[selectedIndex].sceneTagInfo.textureMissing)
                {
                    loaderMenu.AddDisabledItem(new GUIContent("Paste/Paste Color"), false);
                }
                else
                {
                    loaderMenu.AddItem(new GUIContent("Paste/Paste Color"), false,
                    () =>
                    {
                        // This records all operations done to the property in the foreach loop
                        Undo.RecordObject(this, "Paste Custom Tag Color");
                        foreach (var selectedIndex in rl.selectedIndices)
                        {
                            Color color = m_CopiedTagInfo.tagColor;

                            if (sceneInfos[selectedIndex].sceneTagInfo.tagColor != color)
                            {
                                sceneInfos[selectedIndex].sceneTagInfo.tagColor = new Color(color.r, color.g, color.b, color.a);
                            }
                        }

                        // Clear all selected indices to prevent unintentianal editing
                        rl.ClearSelection();
                        m_LoaderWindowSO.ApplyModifiedProperties();
                    });
                }

                if (m_CopiedTagInfo == null || !m_CanPasteTagType || m_CopiedTagInfo.tagTexture == null)
                {
                    loaderMenu.AddDisabledItem(new GUIContent("Paste/Paste Type"), false);
                }
                else
                {
                    loaderMenu.AddItem(new GUIContent("Paste/Paste Type"), false,
                    () =>
                    {
                        Undo.RecordObject(this, "Paste Custom Tag Type");
                        foreach (var selectedIndex in rl.selectedIndices)
                        {
                            if (sceneInfos[selectedIndex].sceneTagInfo.tagTexture != m_CopiedTagInfo.tagTexture)
                            {
                                sceneInfos[selectedIndex].sceneTagInfo.tagTexture = m_CopiedTagInfo.tagTexture;

                                // We don't want to confuse user when only pasting a type about a color option that not even shown when texture is missing
                                if (sceneInfos[selectedIndex].sceneTagInfo.textureMissing)
                                {
                                    sceneInfos[selectedIndex].sceneTagInfo.tagColor = Color.white;
                                }

                                sceneInfos[selectedIndex].sceneTagInfo.textureMissing = false;
                            }
                        }

                        rl.ClearSelection();
                        m_LoaderWindowSO.ApplyModifiedProperties();
                    });
                }

                loaderMenu.AddSeparator("Paste/");
                if (m_CopiedTagInfo == null || !m_CanPasteAll || m_CopiedTagInfo.tagTexture == null)
                {
                    loaderMenu.AddDisabledItem(new GUIContent("Paste/Paste All"), false);
                }
                else
                {
                    loaderMenu.AddItem(new GUIContent("Paste/Paste All"), false,
                    () =>
                    {
                        Undo.RecordObject(this, "Paste Custom Tag Type and Color");
                        foreach (var selectedIndex in rl.selectedIndices)
                        {
                            if (sceneInfos[selectedIndex].sceneTagInfo.tagColor != m_CopiedTagInfo.tagColor)
                            {
                                sceneInfos[selectedIndex].sceneTagInfo.tagColor = m_CopiedTagInfo.tagColor;
                            }

                            if (sceneInfos[selectedIndex].sceneTagInfo.tagTexture != m_CopiedTagInfo.tagTexture)
                            {
                                sceneInfos[selectedIndex].sceneTagInfo.tagTexture = m_CopiedTagInfo.tagTexture;
                                sceneInfos[selectedIndex].sceneTagInfo.textureMissing = false;
                            }
                        }

                        rl.ClearSelection();
                        m_LoaderWindowSO.ApplyModifiedProperties();
                    });
                }

                loaderMenu.ShowAsContext();
            }
        }
    }
}