using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Victor.Tools
{
    [FilePath("ProjectSettings/VTPrefabLoaderData.asset", FilePathAttribute.Location.ProjectFolder)]
    public class VTPrefabLoaderData : ScriptableSingleton<VTPrefabLoaderData>
    {
        [SerializeField]
        private List<VTPrefabLoader.OuterListInfo> m_OuterList = new List<VTPrefabLoader.OuterListInfo>();

        public List<VTPrefabLoader.OuterListInfo> outerList { get { return m_OuterList; } }

        public void Save(List<VTPrefabLoader.OuterListInfo> outerList)
        {
            m_OuterList = outerList;
            Save(true);
        }
    }
}