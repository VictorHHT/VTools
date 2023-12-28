using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTDirectory
    {
        public static string FindFolderPathInAssets(string searchFolderName)
        {
            // Full path to the "Assets" folder of the project
            string startingPath = Application.dataPath;
            // This is the full path down to system root folder
            string folderPath = FindFolder(searchFolderName, startingPath);

            if (folderPath.NullOrEmpty())
            {
                throw new FileNotFoundException($"{searchFolderName} folder doesn't exist");
            }

            // Remove the "Assets" folder name from the data path
            // Use '/' since it's Unity default path separator
            startingPath = startingPath.Substring(0, startingPath.LastIndexOf('/') + 1);
            // Path relative to assets
            // To use SubString(), just pass in the length of the characters before the character you want to start with
            string relativePath = folderPath.Substring(startingPath.Length);
            return relativePath;
        }
        
        public static List<string> GetFilesWithExtensionFromDirectories(string baseFolder, string searchPattern, SearchOption searchOption, string extensionWithoutDot, bool standardizePath = true)
        {
            List<string> folderPaths = new List<string>(Directory.GetDirectories(baseFolder, searchPattern, searchOption));

            var totalFilePaths = new List<string>();
            foreach (string editorFolder in folderPaths)
            {
                string[] filePaths = Directory.GetFiles(editorFolder, "*." + extensionWithoutDot);
                totalFilePaths.AddRange(filePaths);
            }

            if (standardizePath)
            {
                for (int i = 0; i < totalFilePaths.Count; i++)
                {
                    totalFilePaths[i] = StandardizePath(totalFilePaths[i]);
                }
            }

            return totalFilePaths;
        }

        /// <summary>
        /// Append paths to original path to construct a new path with '/' as separator
        /// </summary>
        /// <param name="originalPath"></param>
        /// <param name="paths"></param>
        /// <returns>New path with paths appended to originalPath</returns>
        public static string AppendFolderOrAssetName(string originalPath, params string[] paths)
        {
            string newPath = originalPath;

            foreach (var path in paths)
            {
                newPath += ($"/{path}");
            }

            return newPath.ToString();
        }

        // In Unity, forward slash is the standard path separator
        public static string StandardizePath(string path)
        {
            return path.Replace('\\', '/');
        }

        private static string FindFolder(string searchFolderName, string startingPath)
        {
            // Check if the startingPath is the searchFolderName
            if (Path.GetFileName(startingPath) == searchFolderName)
            {
                return startingPath;
            }

            // Search all the directories in the startingPath
            foreach (string subDir in Directory.GetDirectories(startingPath))
            {
                string result = FindFolder(searchFolderName, subDir);
                if (result != null)
                {
                    // Standardize path so it behaves the same as Unity's path format (Application.dataPath)
                    result = StandardizePath(result);
                    return result;
                }
            }

            // Folder not found
            return null;
        }

    }
}
