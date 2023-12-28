using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
namespace Victor.Tools
{
    public static class VTOpenDefaultWindow
    {
        public enum DefaultWindowType {Inspector, Hierarychy, Project, Scene, Game, Console};

        const string inspectorName = "UnityEditor.InspectorWindow";
        const string hierarchyName = "UnityEditor.SceneHierarchyWindow";
        const string projectName = "UnityEditor.ProjectBrowser";
        const string sceneViewName = "UnityEditor.SceneView";
        const string gameViewName = "UnityEditor.GameView";
        const string consoleName = "UnityEditor.ConsoleWindow";

        public static EditorWindow GetDefaultWindow(DefaultWindowType defaultWindowType)
        {
            Type windowType;
            switch (defaultWindowType)
            {
                case DefaultWindowType.Inspector:
                    windowType = typeof(Editor).Assembly.GetType(inspectorName);
                    break;
                case DefaultWindowType.Hierarychy:
                    windowType = typeof(Editor).Assembly.GetType(hierarchyName);
                    break;
                case DefaultWindowType.Project:
                    windowType = typeof(Editor).Assembly.GetType(projectName);
                    break;
                case DefaultWindowType.Scene:
                    windowType = typeof(Editor).Assembly.GetType(sceneViewName);
                    break;
                case DefaultWindowType.Game:
                    windowType = typeof(Editor).Assembly.GetType(gameViewName);
                    break;
                case DefaultWindowType.Console:
                    windowType = typeof(Editor).Assembly.GetType(consoleName);
                    break;
                default:
                    return null;
            }
            
            return EditorWindow.GetWindow(windowType);
        }
    }
}

