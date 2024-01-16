using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Victor.EditorTween;

namespace Victor.Tools
{
    public class VTSceneTagWindow : PopupWindowContent
    {
        public static int openEntryIndex = 0;
        public static bool isOpen;

        private const int k_NewRowHeight = 33;
        private readonly int m_TagTypesCount;
        // (m_WindowWidth - 45 * 5 - 15) / 2 + 3 = 20.5f
        private readonly float m_WindowLeftPadding = 20.5f;
        // (m_WindowWidth / 2 - 210 / 2) = 32.5f
        private readonly float m_ColorFieldLeftPadding = 32.5f;
        private int m_WindowWidth = 275;
        private int m_CanHoldWindowWidth = 260;
        private int m_WindowHeight = 78;
        private float m_TagsSpacing = 300f;
        private float m_ColorFieldSpacing = 240f;
        private Vector2 m_ScrollPosition;
        private Vector2 m_WindowSize;
        private SceneTagInfo m_SceneTagInfo;
        private VTSceneLoader m_VTSceneLoader;
        private Texture2D[] m_TagTypeTextures;

        public VTSceneTagWindow(int openIndex, SceneTagInfo sceneTagInfo)
        {
            m_SceneTagInfo = sceneTagInfo;
            openEntryIndex = openIndex;
            m_VTSceneLoader = EditorWindow.GetWindow<VTSceneLoader>();
            m_TagTypesCount = VTSceneLoaderSettingsProviderData.instance.customSettingsInfos.Count;
            int additionalRowCount = m_TagTypesCount / 5;

            if (m_TagTypesCount % 5 != 0)
            {
                additionalRowCount++;
            }

            additionalRowCount--;
            m_WindowHeight += k_NewRowHeight * additionalRowCount;
            // 81 = 7 + 33 + 16 + 18 + 7
            // 7 is top padding
            // 33 is the height of tag icon in a line
            // 16 is the space between color field and scene tags
            // 18 is the height of color field (EditorGUIUtility.singleLineHeight)
            // 7 is the bottom padding
            int desiredWindowHeight = 81 + (VTSceneLoaderSettingsProviderData.instance.tagWindowRowCount - 1) * 33;
            // Let user override the height of the window to shrink the display area in order to make a scroll bar appear
            m_WindowHeight = Mathf.Min(desiredWindowHeight, m_WindowHeight);

            // If all tag icons could be displayed without a scroll bar, we adjust the layout
            if (additionalRowCount <= VTSceneLoaderSettingsProviderData.instance.tagWindowRowCount - 1)
            {
                m_WindowWidth = m_CanHoldWindowWidth;
                m_WindowLeftPadding = 20;
                m_ColorFieldLeftPadding = 25;
                m_WindowHeight += 5;
            }

            m_TagTypeTextures = new Texture2D[m_TagTypesCount];

            for (int i = 0; i < m_TagTypesCount; i++)
            {
                m_TagTypeTextures[i] = VTSceneLoaderSettingsProviderData.instance.customSettingsInfos[i].tagTexture;
            }
        }

        public override Vector2 GetWindowSize()
        {
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                return new Vector3(m_WindowWidth, m_WindowHeight);
            }

            return base.GetWindowSize();
        }

        public override void OnOpen()
        {
            isOpen = true;
            m_ScrollPosition = m_VTSceneLoader.storedSceneTagWindowScrollPosition;

            // We don't want to confuse the user since the color of the tag with a missing texture may have long been forgotten
            if (m_SceneTagInfo.textureMissing)
            {
                m_SceneTagInfo.tagColor = Color.white;
            }

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            InitializeTweens();
        }

        public override void OnClose()
        {
            isOpen = false;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            
            // Repaint after window is opened to remove the dim effect of other tags
            m_VTSceneLoader.Repaint();
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Space(5);
            int currentColumn = 0;

            GUILayout.BeginHorizontal();
            GUILayout.Space(m_TagsSpacing);
            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, false, false);

            // Note if the current window switches from needing a scrollbar to not needing one, the scrollPosition will be reset to Vector2.zero
            m_VTSceneLoader.storedSceneTagWindowScrollPosition = m_ScrollPosition; 

            using (new GUILayout.VerticalScope())
            {
                for (int i = 0; i < m_TagTypesCount; i++)
                {
                    var settingsProviderInfo = VTSceneLoaderSettingsProviderData.instance.customSettingsInfos[i];

                    if (currentColumn == 0)
                    {
                        if (i != 0)
                        {
                            GUILayout.Space(3);
                        }
                        
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(m_WindowLeftPadding);
                    }

                    bool isActiveTag = m_SceneTagInfo.tagTexture == settingsProviderInfo.tagTexture;
                    Rect controlRect = EditorGUILayout.GetControlRect(false, 28f, GUILayout.MaxWidth(45));
                    controlRect.width = controlRect.height;

                    if (isActiveTag)
                    {
                        VTGUI.StoreGUIBackgroundAndContentColor();
                        GUI.contentColor = m_SceneTagInfo.tagColor;
                    }

                    if (VTGUI.AnimatedButton(controlRect, new GUIContent(m_TagTypeTextures[i]), GUI.skin.button, isActiveTag, editorWindow.Repaint))
                    {
                        Undo.RecordObject(m_VTSceneLoader.loaderWindowSO.targetObject, "Set Custom Scene Tag Type");
                        m_SceneTagInfo.tagTexture = settingsProviderInfo.tagTexture;

                        // After setting tag icon, m_SceneTagInfo now has a new texture for displaying, yeah
                        m_SceneTagInfo.textureMissing = false;
                        GUI.changed = true;
                        m_VTSceneLoader.Repaint();
                    }

                    if (isActiveTag)
                    {
                        VTGUI.RevertGUIBackgroundAndContentColor();
                    }

                    currentColumn++;

                    if (currentColumn == 5)
                    {
                        GUILayout.EndHorizontal();
                        currentColumn = 0;
                    }
                }

                if (currentColumn != 0)
                {
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.Space(16);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(m_ColorFieldLeftPadding);
                GUILayout.Space(m_ColorFieldSpacing);
                Rect colorRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.MaxWidth(210));
                Color tempColor = EditorGUI.ColorField(colorRect, m_SceneTagInfo.tagColor);

                if (tempColor != m_SceneTagInfo.tagColor)
                {
                    Undo.RecordObject(m_VTSceneLoader.loaderWindowSO.targetObject, "Set Custom Scene Tag Icon");
                    m_SceneTagInfo.tagColor = tempColor;
                    m_VTSceneLoader.Repaint();
                }
            }

            GUILayout.Space(7f);

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                Rect newPosition = new Rect(editorWindow.position);
                Vector3 newSize = new Vector3(Mathf.Max(1, m_WindowSize.x), Mathf.Max(m_WindowSize.y, 1));
                newPosition.size = newSize;
                editorWindow.position = newPosition;
            }
        }

        private void OnUndoRedoPerformed()
        {
            m_VTSceneLoader.Repaint();
            editorWindow.Repaint();
        }

        private void InitializeTweens()
        {
            Vector3 targetSize = new Vector3(m_WindowWidth, m_WindowHeight);

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                VTweenCreator.TweenVector3(m_WindowSize, newSize => m_WindowSize = newSize, targetSize).SetDuration(0.65f).SetEaseType(EaseType.EaseOutBack).SetOvershootOrAmplitude(0.65f).OnValueChanged(editorWindow.Repaint);
                VTweenCreator.TweenFloat(m_TagsSpacing, newSpacing => m_TagsSpacing = newSpacing, 0f).SetDuration(0.75f).SetInitialDelay(0.15f).SetEaseType(EaseType.EaseOutBack).SetOvershootOrAmplitude(0.65f).OnValueChanged(editorWindow.Repaint);
                VTweenCreator.TweenFloat(m_ColorFieldSpacing, newSpacing => m_ColorFieldSpacing = newSpacing, 0f).SetDuration(0.675f).SetInitialDelay(0.3f).SetEaseType(EaseType.EaseOutBack).SetOvershootOrAmplitude(0.5f).OnValueChanged(editorWindow.Repaint);
            }
            else
            {
                VTweenCreator.TweenFloat(m_TagsSpacing, newSpacing => m_TagsSpacing = newSpacing, 0f).SetDuration(0.75f).SetEaseType(EaseType.EaseOutBack).SetOvershootOrAmplitude(0.65f).OnValueChanged(editorWindow.Repaint);
                VTweenCreator.TweenFloat(m_ColorFieldSpacing, newSpacing => m_ColorFieldSpacing = newSpacing, 0f).SetDuration(0.675f).SetInitialDelay(0.15f).SetEaseType(EaseType.EaseOutBack).SetOvershootOrAmplitude(0.5f).OnValueChanged(editorWindow.Repaint);
            }
        }
    }
}

