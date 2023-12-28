using UnityEditor;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTDefaultPathsEditor
    {
        [MenuItem("Tools/Victor/Default Paths/Open DataPath", false, 104)]
        private static void OpenDataPath()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
        }

        [MenuItem("Tools/Victor/Default Paths/Open PersistentDataPath", false, 105)]
        private static void OpenPersistentDataPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        [MenuItem("Tools/Victor/Default Paths/Open StreamingAssetsPath", false, 106)]
        private static void OpenStreamingAssetsPath()
        {
            EditorUtility.RevealInFinder(Application.streamingAssetsPath);
        }

        [MenuItem("Tools/Victor/Default Paths/Open ScriptAssemblies", false, 107)]
        private static void OpenScriptAssemblies()
        {
            EditorUtility.RevealInFinder(Application.dataPath + "/../Library/ScriptAssemblies");
        }

        [MenuItem("Tools/Victor/Default Paths/Open ApplicationContentsPath", false, 108)]
        private static void OpenApplicationContentsPath()
        {
            EditorUtility.RevealInFinder(EditorApplication.applicationContentsPath);
        }

        [MenuItem("Tools/Victor/Default Paths/Open TemporaryCachePath", false, 109)]
        private static void OpenTemporaryCachePath()
        {
            EditorUtility.RevealInFinder(Application.temporaryCachePath);
        }


    }

}
