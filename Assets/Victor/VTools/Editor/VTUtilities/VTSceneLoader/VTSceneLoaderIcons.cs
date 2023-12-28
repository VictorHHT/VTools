using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Victor.Tools.SceneTagInfo;

namespace Victor.Tools
{
    public static class VTSceneLoaderIcons
    {
        private static bool m_Loaded;
        private static Texture2D m_SceneLoaderTitleTex;
        private static Texture2D m_CustomSceneListTex;
        private static Texture2D m_BuildSettingsTex;
        private static Texture2D m_ProjectSettingsTitleTex;
        private static Texture2D m_WarningTagTex;

        public static Texture2D sceneLoaderTitleTex
        {
            get { return m_SceneLoaderTitleTex; }
        }

        public static Texture2D customSceneListTex
        {
            get { return m_CustomSceneListTex; }
        }

        public static Texture2D buildSettingsTex
        {
            get { return m_BuildSettingsTex; }
        }

        public static Texture2D warningTagTex
        {
            get { return m_WarningTagTex; }
        }

        public static Texture2D projectSettingsTitleTex
        {
            get { return m_ProjectSettingsTitleTex; }
        }

        public static void LoadTextures()
        {
            // Do not reload when the VTSceneLoader window closes and reopens, only reload textures after assembly reload
            if (m_Loaded)
            {
                return;
            }

            m_SceneLoaderTitleTex = Resources.Load<Texture2D>(VTSceneLoaderUtility.k_MainIconFolderName + '/' + "SceneLoaderTitle");
            m_CustomSceneListTex = Resources.Load<Texture2D>(VTSceneLoaderUtility.k_MainIconFolderName + '/' + "CustomSceneList");
            m_BuildSettingsTex = Resources.Load<Texture2D>(VTSceneLoaderUtility.k_MainIconFolderName + '/' + "BuildSettings");
            m_WarningTagTex = Resources.Load<Texture2D>(VTSceneLoaderUtility.k_MainIconFolderName + '/' + "Warning");
            m_ProjectSettingsTitleTex = Resources.Load<Texture2D>(VTSceneLoaderUtility.k_MainIconFolderName + '/' + "ProjectSettings");
            m_Loaded = true;
        }
    }
}
