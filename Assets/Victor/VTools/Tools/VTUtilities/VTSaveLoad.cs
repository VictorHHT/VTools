using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    public static class VTSaveLoad
    {
        public static void SaveToJson(string savePath, string fileName, object saveObject, bool formatJson = true)
        {
            const string fileExtension = ".json";
            // Create directory
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            // Create file stream to save file
            FileStream saveStream = File.Create(Path.Combine(savePath, fileName) + fileExtension);
            var streamWriter = new StreamWriter(saveStream);
            try
            {
                // Convert object to json string using JsonUtility API from UnityEngine
                string json = JsonUtility.ToJson(saveObject, formatJson);
                // Create stream writer to write to the file
                streamWriter.Write(json);
                // On comment the following line to view saved json content in the console
                //Debug.Log($"{fileName} new saved content: {json}");
            }
            catch (IOException e)
            {
                throw new IOException(e.Message);
            }
            finally
            {
                streamWriter.Close();
                saveStream.Close();
            }            
        }

        public static object LoadFromJson<T>(string pathName, string fileName)
        {
            const string fileExtension = ".json";
            string filePath = Path.Combine(pathName, fileName) + fileExtension;
            if (!Directory.Exists(pathName))
            {
                Debug.Log($"[VTSaveLoad] Directory \"{pathName} \" not exist");
                return null;
            }

            if (!File.Exists(filePath))
            {
                Debug.Log($"[VTSaveLoad] File at path \"{filePath} \" not exist");
                return null;
            }

            FileStream loadStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            object savedObject;
            var loadReader = new StreamReader(filePath, Encoding.UTF8);
            string json = loadReader.ReadToEnd();
            savedObject = JsonUtility.FromJson<T>(json);
            loadReader.Close();
            loadStream.Close();
            return savedObject;

        }

        public static void ArraySaveToJson<T>(string savePath, string fileName, T[] saveArray, bool formatJson = true)
        {
            const string fileExtension = ".json";
            // Create directory
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            // Create file stream to save file
            FileStream saveStream = File.Create(Path.Combine(savePath, fileName) + fileExtension);
            var streamWriter = new StreamWriter(saveStream);
            try
            {
                // Convert object to json string using JsonUtility API from UnityEngine
                Wrapper<T> wrapper = new Wrapper<T>();
                wrapper.Items = saveArray;
                string json = JsonUtility.ToJson(wrapper, formatJson);
                // Create stream writer to write to the file
                streamWriter.Write(json);
                // Uncomment the following line to view saved json content in the console
                //Debug.Log($"{fileName} new saved content: {json}");
            }
            catch (IOException e)
            {
                throw new IOException(e.Message);
            }
            finally
            {
                streamWriter.Close();
                saveStream.Close();
            }
        }

        public static T[] ArrayLoadFromJson<T>(string pathName, string fileName)
        {
            const string fileExtension = ".json";
            string filePath = Path.Combine(pathName, fileName) + fileExtension;
            if (!Directory.Exists(pathName))
            {
                Debug.Log($"[VTSaveLoad] Directory \"{pathName} \" not exist");
                return null;
            }

            if (!File.Exists(filePath))
            {
                Debug.Log($"[VTSaveLoad] File at path \"{filePath} \" not exist");
                return null;
            }

            FileStream loadStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var loadReader = new StreamReader(filePath, Encoding.UTF8);
            string json = loadReader.ReadToEnd();
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);

            loadReader.Close();
            loadStream.Close();
            return wrapper.Items;

        }

#if UNITY_EDITOR
        public static void EditorPrefsArraySaveToJson<T>(string editorPrefsKey, T[] saveArray, bool formatJson = false)
        {
            try
            {
                // Convert object to json string using JsonUtility API from UnityEngine
                Wrapper<T> wrapper = new Wrapper<T>();
                wrapper.Items = saveArray;
                string loadedJson = EditorJsonUtility.ToJson(wrapper);
                EditorPrefs.SetString(editorPrefsKey, loadedJson);
            }
            catch (IOException e)
            {
                throw new IOException(e.Message);
            }
        }

        public static T[] EditorPrefsArrayLoadFromJson<T>(string editorPrefsKey)
        {
            if (!EditorPrefs.HasKey(editorPrefsKey))
            {
                Debug.LogError($"Fail to get the key {editorPrefsKey} in EditorPrefs");
            }

            Wrapper<T> wrapper = new Wrapper<T>();
            string loadedJson = EditorPrefs.GetString(editorPrefsKey);
            EditorJsonUtility.FromJsonOverwrite(loadedJson, wrapper);
            return wrapper.Items;
        }
#endif
        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

}
