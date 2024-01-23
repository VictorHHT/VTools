using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Victor.EditorTween;

namespace Victor.Tools
{
    public static class VTGUI
    {
        // Don't forget to set another control in the same Horizontal scope ExpandWidth to true
        public const float k_DefaultControlWidthInSettings = 254;
        private static bool s_StoredGUIEnabledResult;
        private static Color s_StoredGUIColor;
        private static Color s_StoredGUIBackgroundColor;
        private static Color s_StoredGUIContentColor;

        // Example usage:
        // using (VTGUI.GUIEnabledStack.NewOverrideScope(Color.green))
        // {
        //    
        // }
        public static readonly VTOverrideStack<bool> GUIEnabledStack = new
        (
            () => GUI.enabled,
            value => GUI.enabled = value
        );

        public static readonly VTOverrideStack<Color> GUIColorStack = new
        (
            () => GUI.color,
            value => GUI.color = value
        );

        public static readonly VTOverrideStack<Color> GUIBackgroundColorStack = new
        (
            () => GUI.backgroundColor,
            value => GUI.backgroundColor = value
        );

        public static readonly VTOverrideStack<Color> GUIContentColorStack = new
        (
            () => GUI.contentColor,
            value => GUI.contentColor = value
        );

        public static readonly VTOverrideStack<(Color, Color)> GUIContentColorAndBackgroundStack = new
        (
            () => (GUI.contentColor, GUI.backgroundColor),

            value =>
            {
                GUI.contentColor = value.Item1;
                GUI.backgroundColor = value.Item2;
            }
        );

        public static void StoreGUIColor()
        {
            s_StoredGUIColor = GUI.color;
        }

        // Meant to be called before changing GUI background color and content color
        public static void StoreGUIBackgroundAndContentColor()
        {
            s_StoredGUIBackgroundColor = GUI.backgroundColor;
            s_StoredGUIContentColor = GUI.contentColor;
        }

        public static void StoreGUIBackgroundColor()
        {
            s_StoredGUIBackgroundColor = GUI.backgroundColor;
        }

        public static void StoreGUIContentColor()
        {
            s_StoredGUIContentColor = GUI.contentColor;
        }

        public static void StoreGUIEnabled()
        {
            s_StoredGUIEnabledResult = GUI.enabled;
        }

        public static void RevertGUIColor()
        {
            GUI.color = s_StoredGUIColor;
        }

        public static void RevertGUIBackgroundAndContentColor()
        {
            GUI.backgroundColor = s_StoredGUIBackgroundColor;
            GUI.contentColor = s_StoredGUIContentColor;
        }

        public static void RevertGUIBackgroundColor()
        {
            GUI.backgroundColor = s_StoredGUIBackgroundColor;
        }

        public static void RevertGUIContentColor()
        {
            GUI.contentColor = s_StoredGUIContentColor;
        }

        public static void RevertGUIEnabled()
        {
            GUI.enabled = s_StoredGUIEnabledResult;
        }

        // Various Scopes
        public class BackgroundColorScope : GUI.Scope
        {
            private Color m_PreviousColor;

            public BackgroundColorScope(Color newBackgroundColor)
            {
                m_PreviousColor = GUI.backgroundColor;
                GUI.backgroundColor = newBackgroundColor;
            }

            public BackgroundColorScope(float r, float g, float b, float a = 1.0f) : this(new Color(r, g, b, a))
            {
            }

            protected override void CloseScope()
            {
                GUI.backgroundColor = m_PreviousColor;
            }
        }

        public class ContentColorScope : GUI.Scope
        {
            private Color m_PreviousColor;

            public ContentColorScope(Color newContentColor)
            {
                m_PreviousColor = GUI.contentColor;
                GUI.contentColor = newContentColor;
            }

            public ContentColorScope(float r, float g, float b, float a = 1.0f) : this(new Color(r, g, b, a))
            {
            }

            protected override void CloseScope()
            {
                GUI.contentColor = m_PreviousColor;
            }
        }

        public class GUIColorScope : GUI.Scope
        {
            private Color m_PreviousBackgroundColor;
            private Color m_PreviousContentColor;

            public GUIColorScope(Color newBackgroundColor, Color newContentColor)
            {
                m_PreviousBackgroundColor = GUI.backgroundColor;
                m_PreviousContentColor = GUI.contentColor;
                GUI.backgroundColor = newBackgroundColor;
                GUI.contentColor = newContentColor;
            }

            protected override void CloseScope()
            {
                GUI.backgroundColor = m_PreviousBackgroundColor;
                GUI.contentColor = m_PreviousContentColor;
            }
        }

        public class GUIEnabledScope : GUI.Scope
        {
            private bool m_PreviousEnabled;

            public GUIEnabledScope(bool newEnabled)
            {
                m_PreviousEnabled = GUI.enabled;
                GUI.enabled = newEnabled;
            }

            protected override void CloseScope()
            {
                GUI.enabled = m_PreviousEnabled;
            }
        }

        public static bool ClickableLabel(Rect rect, GUIContent content, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;
            bool clicked = false;

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    {
                        if (e.button == 0 && rect.Contains(e.mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            GUI.changed = true;
                            e.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            if (GUIUtility.hotControl == controlID)
                            {
                                clicked = true;
                                GUIUtility.hotControl = 0;
                                GUI.changed = true;
                                e.Use();
                            }
                        }
                        break;
                    }
                case EventType.Repaint:
                    {
                        style.Draw(rect, content.text, rect.Contains(e.mousePosition) && GUIUtility.hotControl == 0, GUIUtility.hotControl == controlID, false, false);
                        break;
                    }
            }

            return clicked;
        }

        public static void Label(Rect rect, GUIContent content, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.Repaint:
                    {
                        style.Draw(rect, content.text, false, false, false, false);
                        break;
                    }
            }
        }

        public static bool AnimatedButton(Rect rect, GUIContent content, GUIStyle style, bool isOn, Action repaintCallback, float downSizeMultiplier = 0.925f, float downColorVisibilityMultiplier = 0.9f, float downTweenDuration = 0.5f, EaseType downTweenEaseType = EaseType.EaseOutQuart, float upTweenDuration = 0.4f, EaseType upTweenEaseType = EaseType.EaseOutQuart, float hoverSizeMultiplier = 1.075f, float hoverColorVisibilityMultiplier = 1.2f, float hoverEnterTweenDuration = 0.4f, float hoverExitTweenDuration = 0.4f)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    {
                        if (e.button == 0 && rect.Contains(e.mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            VTGUIStates.AnimatedButtonState buttonState = (VTGUIStates.AnimatedButtonState)GUIUtility.GetStateObject(typeof(VTGUIStates.AnimatedButtonState), controlID);

                            buttonState.initialRect = rect;
                            buttonState.currentRect = rect;

                            buttonState.initialColor = GUI.backgroundColor;
                            buttonState.currentColor = GUI.backgroundColor;

                            buttonState.rectTween?.Remove();
                            buttonState.animationStarted = true;
                            buttonState.rectTween = VTweenCreator.TweenRect(buttonState.currentRect, rect => buttonState.currentRect = rect, rect.ScaleAroundCenter(0.925f)).OnValueChanged(() =>
                            {
                                repaintCallback?.Invoke();
                            });

                            buttonState.colorTween?.Remove();
                            buttonState.colorTween = VTweenCreator.TweenColor(buttonState.currentColor, color => buttonState.currentColor = color, buttonState.initialColor.NewV(0.9f)).OnValueChanged(() =>
                            {
                                repaintCallback?.Invoke();
                            });

                            GUI.changed = true;
                            e.Use();
                            return true;
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl == controlID)
                        {
                            VTGUIStates.AnimatedButtonState buttonState = (VTGUIStates.AnimatedButtonState)GUIUtility.GetStateObject(typeof(VTGUIStates.AnimatedButtonState), controlID);

                            buttonState.rectTween?.Remove();
                            buttonState.rectTween = VTweenCreator.TweenRect(buttonState.currentRect, rect => buttonState.currentRect = rect, buttonState.initialRect).SetDuration(0.5f).SetEaseType(EaseType.EaseOutQuint).OnValueChanged(() =>
                            {
                                repaintCallback?.Invoke();
                            });

                            buttonState.colorTween?.Remove();
                            buttonState.colorTween = VTweenCreator.TweenColor(buttonState.currentColor, color => buttonState.currentColor = color, buttonState.initialColor).OnValueChanged(() =>
                            {
                                repaintCallback?.Invoke();
                            });

                            GUIUtility.hotControl = 0;
                            GUI.changed = true;
                            e.Use();
                        }
                        break;
                    }
                case EventType.Repaint:
                    {
                        VTGUIStates.AnimatedButtonState buttonState = (VTGUIStates.AnimatedButtonState)GUIUtility.GetStateObject(typeof(VTGUIStates.AnimatedButtonState), controlID);

                        if (!buttonState.initialized)
                        {
                            buttonState.initialRect = rect;
                            buttonState.currentRect = rect;

                            buttonState.initialColor = GUI.backgroundColor;
                            buttonState.currentColor = GUI.backgroundColor;

                            buttonState.initialized = true;
                        }

                        if (!buttonState.hoverTweenSet && rect.Contains(e.mousePosition))
                        {
                            buttonState.rectTween?.Remove();
                            buttonState.rectTween = VTweenCreator.TweenRect(buttonState.currentRect, rect => buttonState.currentRect = rect, buttonState.initialRect.ScaleAroundCenter(1.075f)).SetDuration(0.4f).SetEaseType(EaseType.EaseOutQuart).OnValueChanged(() =>
                            {
                                repaintCallback?.Invoke();
                            });

                            buttonState.colorTween?.Remove();
                            buttonState.colorTween = VTweenCreator.TweenColor(buttonState.currentColor, color => buttonState.currentColor = color, buttonState.initialColor.NewV(1.2f)).OnValueChanged(() =>
                            {
                                repaintCallback?.Invoke();
                            });

                            buttonState.hoverTweenSet = true;
                        }

                        if (buttonState.hoverTweenSet && !rect.Contains(e.mousePosition))
                        {
                            buttonState.rectTween?.Remove();
                            buttonState.rectTween = VTweenCreator.TweenRect(buttonState.currentRect, rect => buttonState.currentRect = rect, buttonState.initialRect).SetDuration(0.4f).SetEaseType(EaseType.EaseOutQuart).OnValueChanged(() =>
                            {
                                repaintCallback?.Invoke();
                            });

                            buttonState.colorTween?.Remove();
                            buttonState.colorTween = VTweenCreator.TweenColor(buttonState.currentColor, color => buttonState.currentColor = color, buttonState.initialColor).OnValueChanged(() =>
                            {
                                repaintCallback?.Invoke();
                            });

                            buttonState.hoverTweenSet = false;
                        }

                        StoreGUIBackgroundColor();

                        // Use initial value 
                        if (buttonState.animationStarted || buttonState.hoverTweenSet && rect.Contains(e.mousePosition) || buttonState.hoverTweenSet == false && !rect.Contains(e.mousePosition))
                        {
                            rect = buttonState.currentRect;
                            GUI.backgroundColor = buttonState.currentColor;
                        }

                        style.Draw(rect, content, false, controlID == GUIUtility.hotControl, isOn, false);
                        RevertGUIBackgroundColor();
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// A interactable button displays an icon and reacts to user click
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool VTButton(Rect rect, GUIContent content, bool addHoverEffect = false, float hoverSizeMultiplier = 1.065f, float pressSizeMultiplier = 0.935f)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    {
                        if (e.button == 0 && rect.Contains(e.mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            e.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            if (GUIUtility.hotControl == controlID)
                            {
                                GUIUtility.hotControl = 0;
                                // This is essential, otherwise the change to isOn doesn't reflect back
                                GUI.changed = true;
                                e.Use();
                                return true;
                            }
                        }
                        break;
                    }
                case EventType.Repaint:
                    {
                        GUIStyle style = GUI.skin.label;
                        rect = VTRect.AddSimpleButtonEffect(rect, e, GUIUtility.hotControl == controlID, hoverSizeMultiplier, pressSizeMultiplier, addHoverEffect);
                        style.Draw(rect, content, controlID, false, rect.Contains(e.mousePosition));
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// Allow user to quickly set a lot of entries in one mouse drag
        /// Use Example (See VTSceneLoader class):
        /// 1: Create a field in your Editor class that wants to include the button
        /// private SmartToggleButtonState m_VTSceneLoaderSmartToggleButtonState = new SmartToggleButtonState();
        /// 2: Pass that field as the parameter
        /// sceneShouldAddProperty.boolValue = VTGUI.SmartToggleButton(shouldAddButtonRect, rect, VTStyles.toggleButtonIncludeExcludeStyle, sceneShouldAddProperty.boolValue, ref m_VTSceneLoaderSmartToggleButtonState, true);
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="containingRect"></param>
        /// <param name="style"></param>
        /// <param name="enabled"></param>
        /// <param name="state"></param>
        /// <param name="addHoverEffect"></param>
        /// <param name="hoverSizeMultiplier"></param>
        /// <param name="pressSizeMultiplier"></param>
        /// <returns></returns>
        public static bool SmartToggleButton(Rect rect, Rect containingRect, GUIStyle style, bool enabled, ref VTGUIStates.SmartToggleButtonState state, bool addHoverEffect = false, float hoverSizeMultiplier = 1.065f, float pressSizeMultiplier = 0.935f)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;
            bool isOn = enabled;

            switch (e.type)
            {
                case EventType.MouseDown:
                    {
                        if (e.button == 0 && rect.Contains(e.mousePosition))
                        {
                            GUIUtility.hotControl = controlID;

                            // The reason why we do things this way rather than getting a state object with id of the hot control is to prevent multiple smart toggle button serve different purposes interfere each other
                            state = new VTGUIStates.SmartToggleButtonState();
                            state.setDownBotBoundary = containingRect.yMax;
                            state.setUpTopBoundary = containingRect.yMin;
                            state.hotControlIsSmartToggleButton = true;
                            isOn = !isOn;
                            // After negate isOn, we set it to state.SetTo to set other toggles
                            state.setTo = isOn;
                            
                            // This is essential, otherwise the change to isOn doesn't reflect back
                            GUI.changed = true;
                            e.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl == controlID)
                        {
                            if (rect.Contains(e.mousePosition))
                            {
                                state.hasSetDirection = false;
                                GUIUtility.hotControl = 0;

                                GUI.changed = true;
                                e.Use();
                            }
                            state = null;
                        }
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        if (state == null)
                        {
                            break;
                        }

                        if (!state.hasSetDirection && GUIUtility.hotControl != controlID && state.hotControlIsSmartToggleButton)
                        {
                            if (e.mousePosition.y > state.setDownBotBoundary)
                            {
                                state.setDirection = VTGUIStates.SmartToggleButtonState.SetDirection.Down;
                                state.hasSetDirection = true;

                                GUI.changed = true;
                                e.Use();
                            }
                            else if (e.mousePosition.y < state.setUpTopBoundary)
                            {
                                state.setDirection = VTGUIStates.SmartToggleButtonState.SetDirection.Up;
                                state.hasSetDirection = true;
                               
                                GUI.changed = true;
                                e.Use();
                            }
                            
                        }

                        if (GUIUtility.hotControl != controlID && state.hasSetDirection)
                        {
                            // We check if containing rect's yMin GREATER than bottom boundary and LESS than mouse position to determin whether to set this toggle or not 
                            if (state.setDirection == VTGUIStates.SmartToggleButtonState.SetDirection.Down && e.mousePosition.y > containingRect.yMin && containingRect.yMin > state.setDownBotBoundary)
                            {
                                isOn = state.setTo;
                                state.setDownBotBoundary = containingRect.yMax;
                                GUI.changed = true;
                                e.Use();
                            }
                            else if (state.setDirection == VTGUIStates.SmartToggleButtonState.SetDirection.Up && e.mousePosition.y < containingRect.yMax && containingRect.yMax < state.setUpTopBoundary)
                            {
                                isOn = state.setTo;
                                state.setUpTopBoundary = containingRect.yMin;
                                GUI.changed = true;
                                e.Use();
                            }
                        }

                        break;
                    }
                case EventType.Repaint:
                    {
                        rect = VTRect.AddSimpleButtonEffect(rect, e, GUIUtility.hotControl == controlID, hoverSizeMultiplier, pressSizeMultiplier, addHoverEffect);

                        style.Draw(rect, new GUIContent(""), controlID, isOn);
                        break;
                    }
            }
            return isOn;
        }

        /// Use Example: BoolValue = VTStyles.ToggleButton(shouldAddButtonRect, BoolValue);
        /// <summary>
        /// Make a toggle button using built in icons, optimal rect width is 25
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="enabled"></param>
        /// <param name="useExclude">Use yellow exclude button or red error button to represent off state</param>
        /// <returns></returns>
        public static bool ToggleButton(Rect rect, bool enabled, GUIStyle style, bool addHoverEffect = false, float hoverSizeMultiplier = 1.065f, float pressSizeMultiplier = 0.935f)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;
            bool isOn = enabled;

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    {
                        if (e.button == 0 && rect.Contains(e.mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            e.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            if (GUIUtility.hotControl == controlID)
                            {
                                isOn = !isOn;
                                GUIUtility.hotControl = 0;
                                // This is essential, otherwise the change to isOn doesn't reflect back
                                GUI.changed = true;
                                e.Use();
                            }
                        }
                        break;
                    }
                case EventType.Repaint:
                    {
                        rect = VTRect.AddSimpleButtonEffect(rect, e, (GUIUtility.hotControl == controlID), hoverSizeMultiplier, pressSizeMultiplier, addHoverEffect);

                        style.Draw(rect, new GUIContent(""), controlID, isOn, rect.Contains(e.mousePosition));
                        break;
                    }
            }
            return isOn;
        }

        /// <summary>
        /// Usage: Rect dragRect = new Rect(m_SidePanelWidth, 0, 20, position.height);
        /// dragRect = VTGUI.HandleHorizontalSplitter(dragRect, position.width, m_SidePanelMinWidth, 100f);
        /// VTGUI.DrawHorizontalSplitter(dragRect);
        /// m_SidePanelWidth = dragRect.x;
        /// </summary>
        /// <param name="dragRect"></param>
        /// <param name="containingRectWidth"></param>
        /// <param name="minLeftSideWidth"></param>
        /// <param name="minRightSideWidth"></param>
        /// <returns></returns>
        public static Rect HandleHorizontalSplitter(Rect dragRect, float containingRectWidth, float minLeftSideWidth, float minRightSideWidth)
        {
            // Shift dragRect to the left to make the draggable area on either side of dragRect (drag area is defined by dragRect.width)
            dragRect.x -= dragRect.width / 2;

            // Add a cursor rect indicating we can drag this area
            if (Event.current.type == EventType.Repaint)
                EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.SplitResizeLeftRight);

            float deltaX = UnityEditorDynamic.EditorGUI.MouseDeltaReader(dragRect, true).x;
            dragRect.x += deltaX;
            dragRect.x = Mathf.Clamp(dragRect.x, minLeftSideWidth, containingRectWidth - minRightSideWidth);

            // Restore dragRect original position so it is drawn on the correct position
            dragRect.x += dragRect.width / 2;
            return dragRect;
        }

        public static void DrawHorizontalSplitter(Rect dragRect)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            StoreGUIColor();
            Color tintColor = VTEditorColorPalette.unityHorizontalSplitter.color;
            GUI.color = GUI.color * tintColor;
            Rect splitterRect = new Rect(dragRect.x - 1, dragRect.y, 1, dragRect.height);
            GUI.DrawTexture(splitterRect, EditorGUIUtility.whiteTexture);
            RevertGUIColor();
        }
    }

}
