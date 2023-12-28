using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
#endif


namespace Victor.Tools
{
#if UNITY_EDITOR
    public static class VTPackageImporter
    {
        private static ListRequest _listRequest;
        private static AddRequest _addRequest;
        // Represents the current package being installed
        private static int _currentAddIndex;

        private static string[] _packages = new string[]
        {
            "com.unity.textmeshpro"
        };

        public static void ImportAllDependencies()
        {
            _currentAddIndex = 0;
            _listRequest = null;
            _addRequest = null;

            Debug.Log("[VTPackageImporter] Package Auto Import Initializing");
            _listRequest = Client.List();
            EditorApplication.update += ListPackages;
        }

        private static void ImportNext()
        {
            if (_currentAddIndex < _packages.Length)
            {
                bool packageFound = false;
                foreach (var package in _listRequest.Result)
                {
                    if (_packages[_currentAddIndex] == package.name)
                    {
                        packageFound = true;
                        Debug.Log($"[VTPackageImporter] {_packages[_currentAddIndex]} is already installed");
                        _currentAddIndex++;
                        ImportNext();
                        return;
                    }
                }

                if (!packageFound)
                {
                    Debug.Log($"[VTPackageImporter] Installing {_packages[_currentAddIndex]}");
                    _addRequest = Client.Add(_packages[_currentAddIndex]);
                    EditorApplication.update += AddPackage;
                }
            }
            else
            {
                Debug.Log("[VTPakcageImporter] Installation Sequence Complete");
            }
        }

        private static void ListPackages()
        {
            if (_listRequest.IsCompleted)
            {
                EditorApplication.update -= ListPackages;
                if (_listRequest.Status == StatusCode.Success)
                {
                    ImportNext();
                }
                else if (_listRequest.Status == StatusCode.Failure)
                {
                    Debug.Log(_listRequest.Error.message);
                }
            }
        }

        private static void AddPackage()
        {
            if (_addRequest.IsCompleted)
            {
                if (_addRequest.Status == StatusCode.Success)
                {
                    Debug.Log($"[VTPackageImporter] {_packages[_currentAddIndex]} has been installed");
                    _currentAddIndex++;
                    ImportNext();
                }
                else if (_listRequest.Status == StatusCode.Failure)
                {
                    Debug.Log(_listRequest.Error.message);
                }
                EditorApplication.update -= AddPackage;
            }
        }
    }
# endif
}

