using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTPrefabLoaderIcons
    {
        private const string k_MainIconFolderName = "VTPrefabLoader Main Icons";
        private static bool m_Loaded;
        private static Texture2D m_PrefabLoaderTitleTex;
        private static Texture2D m_NoPreviewTex;

        public static Texture2D PrefabLoaderTitleTex
        {
            get { return m_PrefabLoaderTitleTex; }
        }

        public static Texture2D NoPreviewTex
        {
            get { return m_NoPreviewTex; }
        }

        public static void LoadTextures()
        {
            // Do not reload when the VTPrefabLoder window closes and reopens, only reload textures after assembly reload
            if (m_Loaded)
            {
                return;
            }

            m_PrefabLoaderTitleTex = Resources.Load<Texture2D>(k_MainIconFolderName + '/' + "PrefabLoaderTitle");
            m_NoPreviewTex = Resources.Load<Texture2D>(k_MainIconFolderName + '/' + "NoPreview");
            m_Loaded = true;
        }
    }
}

