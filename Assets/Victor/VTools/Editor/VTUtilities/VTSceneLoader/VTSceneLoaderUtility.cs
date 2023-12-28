using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Victor.Tools
{
    public static class VTSceneLoaderUtility
    {
        public const string k_TagIconFolderName = "VTSceneLoader Tag Icons";
        public const string k_MainIconFolderName = "VTSceneLoader Main Icons";

        /// <summary>
        /// Remove any entry in ProjectSettings if the texture of the icon displayed could not be found in tag icon folder
        /// and add any new tag type to the settings reorderable list found in the folder
        /// </summary>
        public static void CheckTagIconChanges()
        {
            var settingsProviderInfos = VTSceneLoaderSettingsProviderData.instance.customSettingsInfos;
            var tagTypesInFolder = Resources.LoadAll<Texture2D>(k_TagIconFolderName);

            // Remove missing texture guids from VTSceneLoaderSettingsProviderData if not find in folder
            for (int i = settingsProviderInfos.Count - 1; i >= 0; i--)
            {
                bool foundInFolder = false;
                var tagTypeInfo = settingsProviderInfos[i];
                foreach (var tagTexture in tagTypesInFolder)
                {
                    if (settingsProviderInfos[i].tagTexture == tagTexture)
                    {
                        foundInFolder = true;
                        break;
                    }
                }

                if (!foundInFolder)
                {
                    RemoveTagTypeFromSettings(tagTypeInfo);
                }
            }

            // Add new texture guids to VTSceneLoaderSettingsProviderData
            foreach (var tagTexture in tagTypesInFolder)
            {
                bool foundInSettings = false;
                foreach (var settingsInfo in settingsProviderInfos)
                {
                    if (tagTexture == settingsInfo.tagTexture)
                    {
                        foundInSettings = true;
                        break;
                    }
                }

                if (!foundInSettings)
                {
                    AddNewTagTypeToSettings(tagTexture);
                }
            }

            VTSceneLoaderSettingsProviderData.instance.Save();
        }

        private static void AddNewTagTypeToSettings(Texture2D newTex)
        {
            var newProviderInfo = new VTSceneSettingsProviderInfo();
            newProviderInfo.tagTexture = newTex;
            VTSceneLoaderSettingsProviderData.instance.customSettingsInfos.Add(newProviderInfo);
        }

        private static void RemoveTagTypeFromSettings(VTSceneSettingsProviderInfo tagTypeInfoToRemove)
        {
            VTSceneLoaderSettingsProviderData.instance.customSettingsInfos.Remove(tagTypeInfoToRemove);
        }
    }
}