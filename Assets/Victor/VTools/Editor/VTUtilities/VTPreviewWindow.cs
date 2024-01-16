using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Victor.EditorTween;
using Object = UnityEngine.Object;

namespace Victor.Tools
{
    public class VTPreviewWindow : PopupWindowContent
    {
        public event Action onOpenCallback;
        public event Action onCloseCallback;

        private readonly float m_WindowWidth;
        private readonly float m_WindowHeight;
        private readonly float m_FixedControlWidth = 60;
        private readonly Object m_ObjectToPreview;
        private bool m_HasModelPreview;
        private bool m_SizeAnimFirstStageComplete;
        private VTCustomPreview m_PreviewEditor;

        // Tween
        private Vector3 m_PreviewRectSize;
        private Vector3 m_WindowSize;

        public VTPreviewWindow(Object objectToPreview, float windowWidth = 190, float windowHeight = 120, bool hasModelPreview = false)
        {
            m_ObjectToPreview = objectToPreview;
            m_WindowWidth = windowWidth;
            m_WindowHeight = windowHeight;
            m_HasModelPreview = hasModelPreview;
        }

        public override Vector2 GetWindowSize()
        {
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                return new Vector3(m_WindowWidth + m_FixedControlWidth, m_WindowHeight);
            }

            return base.GetWindowSize();
        }

        public override void OnGUI(Rect rect)
        {
            if (m_PreviewEditor == null)
            {
                m_PreviewEditor = (VTCustomPreview)Editor.CreateEditor(m_ObjectToPreview, typeof(VTCustomPreview));
            }

            m_PreviewEditor.objectToPreview = (GameObject)m_ObjectToPreview;

            GUILayout.BeginHorizontal();
            m_PreviewEditor.OnPreviewGUI(GUILayoutUtility.GetRect(Mathf.Max(m_PreviewRectSize.x, 1), Mathf.Max(m_PreviewRectSize.y, 1)), GUIStyle.none);

            GUILayout.BeginVertical();

            if (GUILayout.Button("Front"))
            {
                m_PreviewEditor.FrontView();
            }

            if (GUILayout.Button("Top"))
            {
                m_PreviewEditor.TopView();
            }

            if (GUILayout.Button("Left"))
            {
                m_PreviewEditor.LeftView();
            }

            // Only draw light intensity slider if an object with mesh renderer exists
            // sprites can't receive
            if (m_HasModelPreview)
            {
                GUILayout.Label("Light", VTStyles.centeredBoldLabel);
                Undo.RecordObject(m_PreviewEditor, "Modify Preview Light Intensity");

                bool oldChanged = GUI.changed;
                GUI.changed = false;
                m_PreviewEditor.lightIntensity = GUILayout.HorizontalSlider(m_PreviewEditor.lightIntensity, 0.1f, 1.5f);

                // Check if user interacted with horizontal slider
                if (GUI.changed)
                {
                    m_PreviewEditor.m_LightTween?.Remove();
                }

                GUI.changed |= oldChanged;
            }

            GUILayout.FlexibleSpace();

            VTGUI.StoreGUIBackgroundAndContentColor();
            GUI.contentColor = VTColorLibrary.victorYellow;

            if (GUILayout.Button("Reset"))
            {
                m_PreviewEditor.ResetObjectView();
            }

            VTGUI.RevertGUIBackgroundAndContentColor();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                Rect newPosition = new Rect(editorWindow.position);
                Vector3 newSize = new Vector3(Mathf.Max(1, m_WindowSize.x), Mathf.Max(m_WindowSize.y, 1));
                newPosition.size = newSize;
                editorWindow.position = newPosition;
            }
        }

        public override void OnOpen()
        {
            EditorApplication.update += editorWindow.Repaint;

            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                Rect newPosition = new Rect(editorWindow.position);
                Vector3 newSize = new Vector3(m_WindowWidth + m_FixedControlWidth, m_WindowHeight);
                newPosition.size = newSize;
                editorWindow.position = newPosition;
            }

            InitializeWindowSizeTweens();
            onOpenCallback?.Invoke();
        }

        public override void OnClose()
        {
            EditorApplication.update -= editorWindow.Repaint;

            onCloseCallback?.Invoke();
            Object.DestroyImmediate(m_PreviewEditor);
            m_PreviewEditor = null;
        }

        private void InitializeWindowSizeTweens()
        {
            Vector3 targetWindowSize = new Vector3(m_WindowWidth, m_WindowHeight);

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                VTweenCreator.TweenVector3(m_PreviewRectSize, newSize => m_PreviewRectSize = newSize, targetWindowSize).SetDuration(0.4f).SetEaseType(EaseType.EaseOutCubic).OnValueChanged(editorWindow.Repaint);
                VTweenCore m_WindowSizeTween = null;

                m_WindowSizeTween = VTweenCreator.TweenVector3(m_WindowSize, newSize => m_WindowSize = newSize, targetWindowSize).SetDuration(0.5f).OnValueChanged(() =>
                {
                    editorWindow.Repaint();

                    if (m_WindowSizeTween.m_Progress > 0.4f && !m_SizeAnimFirstStageComplete)
                    {
                        m_WindowSizeTween.Remove();
                        m_SizeAnimFirstStageComplete = true;
                        Vector3 newWindowSizeTarget = new Vector3(m_WindowWidth + m_FixedControlWidth, m_WindowHeight);
                        m_WindowSizeTween = VTweenCreator.TweenVector3(m_WindowSize, newSize => m_WindowSize = newSize, newWindowSizeTarget).SetDuration(0.6f).SetEaseType(EaseType.EaseOutExpo).OnValueChanged(editorWindow.Repaint);
                    }
                });
            }
            else
            {
                Vector3 initialPreviewRectSize = new Vector3(m_WindowWidth + m_FixedControlWidth + 50f, m_WindowHeight);
                m_PreviewRectSize = initialPreviewRectSize;
                VTweenCreator.TweenVector3(m_PreviewRectSize, newSize => m_PreviewRectSize = newSize, targetWindowSize).SetDuration(0.65f).SetEaseType(EaseType.EaseOutBack).SetOvershootOrAmplitude(0.65f).OnValueChanged(editorWindow.Repaint);
            }          
        }
    }
}