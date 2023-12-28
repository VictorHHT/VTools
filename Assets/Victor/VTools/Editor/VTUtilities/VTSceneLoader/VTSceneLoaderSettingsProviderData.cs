using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Victor.Tools
{
    [FilePath("ProjectSettings/VTSceneLoaderSettingsProviderData.asset", FilePathAttribute.Location.ProjectFolder)]
    public class VTSceneLoaderSettingsProviderData : ScriptableSingleton<VTSceneLoaderSettingsProviderData>
    {
        public List<VTSceneSettingsProviderInfo> customSettingsInfos = new List<VTSceneSettingsProviderInfo>();
        public int tagWindowRowCount = 2;
        public bool showCustomSceneIndex = true;
        public bool showBuildSettingsScenes = true;

        public void Save()
        {
            Save(true);
        }
    }
}