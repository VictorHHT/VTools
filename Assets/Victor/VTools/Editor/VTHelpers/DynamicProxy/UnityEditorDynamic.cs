using System.Reflection;
using UnityEditor;

namespace Victor.Tools
{
	public static class UnityEditorDynamic
	{
		public static readonly Assembly UnityEditorAssembly;

		public static readonly dynamic EditorUtility;

		public static readonly dynamic EditorGUI;

		public static readonly dynamic EditorGUIUtility;

		public static readonly dynamic EditorWindow;

		public static readonly dynamic AddComponentWindow;

		public static readonly dynamic SceneHierarchyWindow;

		public static readonly dynamic ProjectBrowser;

		public static readonly dynamic ConsoleWindow;

		public static readonly dynamic GameView;

		public static readonly dynamic PrefabUtility;

		public static readonly dynamic WindowLayout;

		public static readonly dynamic AssetDatabase;

		public static readonly dynamic SearchFilter;

		public static readonly dynamic SearchUtility;

		public static readonly dynamic SceneView;

		public static readonly dynamic AssetPreview;

		public static readonly dynamic ScriptAttributeUtility;

		static UnityEditorDynamic()
		{
			UnityEditorAssembly = typeof(Editor).Assembly;
			EditorUtility = typeof(EditorUtility).AsDynamicType();
			EditorGUI = typeof(EditorGUI).AsDynamicType();
			EditorGUIUtility = typeof(EditorGUIUtility).AsDynamicType();
			EditorWindow = typeof(EditorWindow).AsDynamicType();
			AddComponentWindow = UnityEditorAssembly.GetType("UnityEditor.AddComponent.AddComponentWindow", true).AsDynamicType();
			SceneHierarchyWindow = UnityEditorAssembly.GetType("UnityEditor.SceneHierarchyWindow", true).AsDynamicType();
			ProjectBrowser = UnityEditorAssembly.GetType("UnityEditor.ProjectBrowser", true).AsDynamicType();
			ConsoleWindow = UnityEditorAssembly.GetType("UnityEditor.ConsoleWindow", true).AsDynamicType();
			GameView = UnityEditorAssembly.GetType("UnityEditor.GameView", true).AsDynamicType();
			PrefabUtility = typeof(PrefabUtility).AsDynamicType();
			WindowLayout = UnityEditorAssembly.GetType("UnityEditor.WindowLayout", true).AsDynamicType();
			AssetDatabase = typeof(AssetDatabase).AsDynamicType();
			SearchFilter = UnityEditorAssembly.GetType("UnityEditor.SearchFilter", true).AsDynamicType();
			SearchUtility = UnityEditorAssembly.GetType("UnityEditor.SearchUtility", true).AsDynamicType();
			SceneView = typeof(SceneView).AsDynamicType();
			AssetPreview = typeof(AssetPreview).AsDynamicType();
			ScriptAttributeUtility = UnityEditorAssembly.GetType("UnityEditor.ScriptAttributeUtility", true).AsDynamicType();
		}
	}
}