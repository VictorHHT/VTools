using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.UIElements;

namespace Victor.Tools
{
    class VTSceneLoaderSettingsProvider : SettingsProvider
    {
        private SerializedObject m_CustomSettings;
        private SerializedProperty m_CustomSettingsInfoProperty;
        private ReorderableList m_ReorderableSettingsInfos;

        public VTSceneLoaderSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope){ }

        // Is called in the following senarios:
        // 1. A change in project browser
        // 2. Click on VTSceneLoader option in ProjectSettings from another option
        // 3. Open ProjectSettings and VTSceneLoader option is the selected one before previous quit
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_CustomSettings = new SerializedObject(VTSceneLoaderSettingsProviderData.instance);
            m_CustomSettingsInfoProperty = m_CustomSettings.FindProperty(nameof(VTSceneLoaderSettingsProviderData.instance.customSettingsInfos));
            EditorApplication.projectChanged += OnProjectChangeCallback;

            // If VTSceneLoader window is opening, the OnProjectChange function in VTSceneLoader class will take care of this since checking folder name recursively is relative expensive
            if (!EditorWindow.HasOpenInstances<VTSceneLoader>())
            {
                VTSceneLoaderUtility.CheckTagIconChanges();
            }

            InitializeReorderableList();
        }

        public override void OnDeactivate()
        {
            EditorApplication.projectChanged -= OnProjectChangeCallback;
            VTSceneLoaderSettingsProviderData.instance.Save();
        }

        public override void OnGUI(string searchContext)
        {
            m_CustomSettings.Update();
            GUILayout.Label(new GUIContent(" Custom Tag Icons", VTSceneLoaderIcons.projectSettingsTitleTex), VTStyles.centeredBoldLabel, GUILayout.MaxHeight(20));
            m_ReorderableSettingsInfos.DoLayoutList();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(new GUIContent("    Tag Window Row Count "), GUILayout.Width(VTGUI.k_DefaultControlWidthInSettings));
                int oldRowCount = VTSceneLoaderSettingsProviderData.instance.tagWindowRowCount;
                VTSceneLoaderSettingsProviderData.instance.tagWindowRowCount = EditorGUILayout.IntSlider(VTSceneLoaderSettingsProviderData.instance.tagWindowRowCount, 1, 10, GUILayout.ExpandWidth(true));

                if (VTSceneLoaderSettingsProviderData.instance.tagWindowRowCount != oldRowCount)
                {
                    if (EditorWindow.HasOpenInstances<VTSceneLoader>())
                    {
                        var sceneLoader = EditorWindow.GetWindow<VTSceneLoader>("VTSceneLoader", false);
                        sceneLoader.storedSceneTagWindowScrollPosition = Vector2.zero;
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(new GUIContent("    Show Custom Scene Index "), GUILayout.Width(VTGUI.k_DefaultControlWidthInSettings));
                bool oldShowSceneIndex = VTSceneLoaderSettingsProviderData.instance.showCustomSceneIndex;
                VTSceneLoaderSettingsProviderData.instance.showCustomSceneIndex = EditorGUILayout.Toggle(VTSceneLoaderSettingsProviderData.instance.showCustomSceneIndex, GUILayout.ExpandWidth(true));

                if (VTSceneLoaderSettingsProviderData.instance.showCustomSceneIndex != oldShowSceneIndex)
                {
                    if (EditorWindow.HasOpenInstances<VTSceneLoader>())
                    {
                        var sceneLoader = EditorWindow.GetWindow<VTSceneLoader>("VTSceneLoader", false);
                        sceneLoader.Repaint();
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(new GUIContent("    Show Build Settings Scenes "), GUILayout.Width(VTGUI.k_DefaultControlWidthInSettings));
                bool oldShowBuildSettings = VTSceneLoaderSettingsProviderData.instance.showBuildSettingsScenes;
                VTSceneLoaderSettingsProviderData.instance.showBuildSettingsScenes = EditorGUILayout.Toggle(VTSceneLoaderSettingsProviderData.instance.showBuildSettingsScenes, GUILayout.ExpandWidth(true));

                if (VTSceneLoaderSettingsProviderData.instance.showBuildSettingsScenes != oldShowBuildSettings)
                {
                    if (EditorWindow.HasOpenInstances<VTSceneLoader>())
                    {
                        var sceneLoader = EditorWindow.GetWindow<VTSceneLoader>("VTSceneLoader", false);
                        sceneLoader.Repaint();
                    }
                }
            }

            m_CustomSettings.ApplyModifiedProperties();
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new VTSceneLoaderSettingsProvider("Project/VTSceneLoader", SettingsScope.Project);
            return provider;
        }

        private void InitializeReorderableList()
        {
            m_ReorderableSettingsInfos = new ReorderableList(m_CustomSettings, m_CustomSettingsInfoProperty, true, true, false, false);
            m_ReorderableSettingsInfos.draggable = true;

            m_ReorderableSettingsInfos.drawHeaderCallback = (Rect rect) =>
            {
                // Draw an empty string so that default "SerializedProperty" doesn't draw
            };

            m_ReorderableSettingsInfos.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2f;

                float tagTypeIndexWidth = 20;
                float tagTypeIconWidth = 15;

                float padding = 10;
                var settingsProviderInfo = VTSceneLoaderSettingsProviderData.instance.customSettingsInfos[index];
                //Could be null if user remove tag icon from the folder
                var tagTypeTex = settingsProviderInfo.tagTexture;

                rect.height = EditorGUIUtility.singleLineHeight;

                Rect tagTypeIndexRect = new Rect(rect.x, rect.y, tagTypeIndexWidth, EditorGUIUtility.singleLineHeight);
                Rect tagTypeIconRect = new Rect(rect.width / 2 + padding - tagTypeIconWidth * 3f, rect.y + 2, tagTypeIconWidth, tagTypeIconWidth);
                Rect tagTypeNameRect = new Rect(tagTypeIconRect.x + tagTypeIconWidth + padding, rect.y, int.MaxValue, rect.height);
                tagTypeNameRect.width = VTStyles.centeredBoldLabel.CalcSize(new GUIContent(tagTypeTex.name)).x;
                GUI.Label(tagTypeIndexRect, (index + 1).ToString(), VTStyles.centeredBoldLabel);
                GUI.DrawTexture(tagTypeIconRect, tagTypeTex);
                GUI.Label(tagTypeNameRect, tagTypeTex.name, VTStyles.centeredBoldLabel);
            };
        }

        private void OnProjectChangeCallback()
        {
            m_CustomSettingsInfoProperty.arraySize = VTSceneLoaderSettingsProviderData.instance.customSettingsInfos.Count;
            _ = m_ReorderableSettingsInfos.count;
            Repaint();
        }
    }
}