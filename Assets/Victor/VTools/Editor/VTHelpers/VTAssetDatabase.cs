using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTAssetDatabase
    {
        public static GUID GetGUIDFromAsset(Object asset)
        {
            return AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(asset));
        }

        public static string GetGUIDStringFromAsset(Object asset)
        {
            return AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(asset)).ToString();
        }

        public static bool TryGetAssetFromGUID<T>(GUID guid, out T asset) where T : Object
        {
            asset = (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T));
            return asset != null;
        }

        public static T GetAssetFromGUID<T>(GUID guid) where T : Object
        {
            return (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T));
        }

        public static bool TryGetAssetFromGUIDString<T>(string guidString, out T asset) where T : Object
        {
            GUID.TryParse(guidString, out GUID result);
            asset = (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(result), typeof(T));
            return asset != null;
        }

        public static T GetAssetFromGUIDString<T>(string guidString) where T: Object
        {
            GUID.TryParse(guidString, out GUID result);
            return (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(result), typeof(T));
        }
    }
}