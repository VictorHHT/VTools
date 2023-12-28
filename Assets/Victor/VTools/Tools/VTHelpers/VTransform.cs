using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Victor.Tools
{
    public static class VTransform
    {
        public enum TraverseTechniques { Breadth, Depth };

        public static GameObject TraverseAndGetGameObject(Predicate<Transform> callback = null, Transform startTransform = null, TraverseTechniques traverseTechnique = TraverseTechniques.Breadth)
        {
            if (callback == null)
            {
                return null;
            }

            // Begin searching in root objects
            if (startTransform == null)
            {
                Scene activeScene = SceneManager.GetActiveScene();

                if (activeScene.rootCount == 0)
                {
                    return null;
                }

                GameObject[] rootObjects = activeScene.GetRootGameObjects();

                foreach (var obj in rootObjects)
                {
                    if (traverseTechnique == TraverseTechniques.Breadth)
                    {
                        GameObject result = TraverseBreadth(obj.transform, callback);

                        if (result != null)
                        {
                            return result;
                        }
                    }
                    else
                    {
                        GameObject result = TraverseDepth(obj.transform, callback);

                        if (result != null)
                        {
                            return result;
                        }
                    }

                }
            }
            else
            {
                if (traverseTechnique == TraverseTechniques.Breadth)
                {
                    GameObject result = TraverseBreadth(startTransform, callback);

                    if (result != null)
                    {
                        return result;
                    }
                }
                else
                {
                    GameObject result = TraverseDepth(startTransform, callback);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private static GameObject TraverseBreadth(Transform transform, Predicate<Transform> callback)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(transform);

            while (queue.Count > 0)
            {
                Transform current = queue.Dequeue();

                if (callback(current))
                {
                    return current.gameObject;
                }

                foreach (Transform child in current)
                {
                    queue.Enqueue(child);
                }
            }

            // If no match is found, return null
            return null;
        }

        private static GameObject TraverseDepth(Transform transform, Predicate<Transform> callback)
        {
            if (callback(transform))
            {
                return transform.gameObject;
            }

            foreach (Transform child in transform)
            {
                GameObject result = TraverseDepth(child, callback);

                if (result != null)
                {
                    return result;
                }
            }

            // If no match is found, return null
            return null;
        }

        public static List<GameObject> TraverseAndGetGameObjects(Predicate<GameObject> callback, Transform startTransform = null, TraverseTechniques traverseTechnique = TraverseTechniques.Breadth)
        {
            if (callback == null)
            {
                return new List<GameObject>();
            }

            List<GameObject> resultObjects = new List<GameObject>();

            // Begin searching in root objects
            if (startTransform == null)
            {
                Scene activeScene = SceneManager.GetActiveScene();

                if (activeScene.rootCount == 0)
                {
                    return resultObjects;
                }

                GameObject[] rootObjects = activeScene.GetRootGameObjects();

                foreach (var obj in rootObjects)
                {
                    if (traverseTechnique == TraverseTechniques.Breadth)
                    {
                        TraverseBreadth(obj.transform, callback, resultObjects);
                    }
                    else
                    {
                        TraverseDepth(obj.transform, callback, resultObjects);
                    }
                }
            }
            else
            {
                if (traverseTechnique == TraverseTechniques.Breadth)
                {
                    TraverseBreadth(startTransform, callback, resultObjects);
                }
                else
                {
                    TraverseDepth(startTransform, callback, resultObjects);
                }
            }

            return resultObjects;
        }

        private static void TraverseBreadth(Transform transform, Predicate<GameObject> callback, List<GameObject> resultObjects)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(transform);

            while (queue.Count > 0)
            {
                Transform current = queue.Dequeue();

                if (callback(current.gameObject))
                {
                    resultObjects.Add(current.gameObject);
                }

                foreach (Transform child in current)
                {
                    queue.Enqueue(child);
                }
            }
        }

        private static void TraverseDepth(Transform transform, Predicate<GameObject> callback, List<GameObject> resultObjects)
        {
            if (callback(transform.gameObject))
            {
                resultObjects.Add(transform.gameObject);
            }

            foreach (Transform child in transform)
            {
                TraverseDepth(child, callback, resultObjects);
            }
        }
    }
}
