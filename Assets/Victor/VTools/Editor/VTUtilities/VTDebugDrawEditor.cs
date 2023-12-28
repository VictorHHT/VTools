using UnityEditor;
namespace Victor.Tools
{
    public class VTDebugDrawEditor
    {
        [MenuItem("Tools/Victor/Enable Debug Draws", false, 101)]
        private static void EnableDebugDraws()
        {
            VTDebugDraw.SetDebugDrawEnabled(true);
        }

        [MenuItem("Tools/Victor/Enable Debug Draws", true)]
        private static bool EnableDebugDrawsValidation()
        {
            //If it's already enabled, we grey this Enable Debug Draw Menu Item
            return !VTDebugDraw.DebugDrawEnabled;
        }

        [MenuItem("Tools/Victor/Disable Debug Draws", false, 102)]
        private static void DisableDebugDraws()
        {
            VTDebugDraw.SetDebugDrawEnabled(false);
        }

        [MenuItem("Tools/Victor/Disable Debug Draws", true)]
        private static bool DisableDebugDrawsValidation()
        {
            //If it's disabled, we grey this Enable Debug Draw Menu Item
            return VTDebugDraw.DebugDrawEnabled;
        }
    }
}

