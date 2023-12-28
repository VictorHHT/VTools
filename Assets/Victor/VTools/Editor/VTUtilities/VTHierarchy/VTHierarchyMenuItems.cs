using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Victor.Tools
{
    public static class VTHierarchyMenuItems
    {
        [MenuItem("GameObject/Select Objects in Same Level", false, 0)]
        private static void SelectGameObjectsOfSameLevel()
        {
            List<GameObject> objectsInSameLevel = new List<GameObject>();

            foreach (GameObject obj in Selection.gameObjects)
            {
                int childCount = obj.transform.childCount;
                bool hasParent = obj.transform.parent == null ? false : true;
                int depth = obj.transform.GetHierarchyDepth();
                Transform parent = obj.transform.parent;
                var siblingIndex = VTHierarchyUtility.GetObjectSelfSiblingIndex(obj.transform);
                VTSceneObjectCache sceneGameObjectCache = new VTSceneObjectCache(obj.transform, obj.GetInstanceID(), childCount, depth, siblingIndex, hasParent, parent);
                objectsInSameLevel.AddRange(SelectSameLevel(sceneGameObjectCache));
            }

            objectsInSameLevel.AddRange(Selection.gameObjects);
            Selection.objects = objectsInSameLevel.ToArray();
        }

        [MenuItem("GameObject/Select Objects of Same Level", true, 0)]
        private static bool ValidateSelectGameObjectsOfSameLevel()
        {
            if (Selection.count == 0)
            {
                return false;
            }

            return true;
        }

        static GameObject[] SelectSameLevel(VTSceneObjectCache objectCache)
        {
            List<GameObject> selectedObjects = new List<GameObject>();
           
            foreach (VTSceneObjectCache cache in VTHierarchyView.sceneObjCacheDict.Values.ToList())
            {
                if (objectCache.parent == cache.parent && objectCache.depth == cache.depth)
                {
                    selectedObjects.Add(cache.objTransform.gameObject);
                }
            }

            return selectedObjects.ToArray();
        }
    }
}