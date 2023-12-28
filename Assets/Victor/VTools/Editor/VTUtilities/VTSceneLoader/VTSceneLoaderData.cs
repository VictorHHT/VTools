using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Victor.Tools
{
    [FilePath("ProjectSettings/VTSceneLoaderData.asset", FilePathAttribute.Location.ProjectFolder)]
    public class VTSceneLoaderData : ScriptableSingleton<VTSceneLoaderData>
    {
        // If the singleton instance of this class is null, the constructor will try to restore the data of this class from the file path specified in the attribute
        // Once the singleton gets constructed, it survives assembly reload and throughout Editor session
        [SerializeField]
        private List<VTSceneLoader.SceneInfo> m_CustomScenes = new List<VTSceneLoader.SceneInfo>();

        public List<VTSceneLoader.SceneInfo> customScenes { get { return m_CustomScenes; } }

        public void Save(List<VTSceneLoader.SceneInfo> customScenes)
        {
            m_CustomScenes = customScenes;
            Save(true);
        }
    }
}