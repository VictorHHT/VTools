using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Victor.Tools
{
    [InitializeOnLoad]
    public static class VTOverlapRevealer
    {
        public const int DefaultLimit = 20;

        static VTOverlapRevealer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private struct UISortInfo
        {
            public GameObject uiObject;
            public int parentCanvasSortingOrder;
            public int depth;
        }

        static void OnSceneGUI(SceneView sceneView)
        {
            Event evt = Event.current;
            EventModifiers actionKeyModifier = VTShortcutModifiers.ToEventModifiers(ShortcutModifiers.Action | ShortcutModifiers.Shift);

            if (evt.type == EventType.MouseDown && evt.button == 1 && evt.modifiers == actionKeyModifier)
            {
                VTRevealerWindow.Init();
                // Prevent this event grab the focus of the window on Windows 10
                evt.Use();
            }

            VTRevealerWindow.DrawObjectOutline();
        }

        public static RevealerHit[] PickAll(RevealerFilter filter, SceneView sceneView, Vector2 guiPosition, out int objOverlapCount, int limit = DefaultLimit)
        {
            var hitResults = PickAllNonAlloc(filter, sceneView, guiPosition, out objOverlapCount, limit);
            return hitResults;
        }

        public static RevealerHit[] PickAllNonAlloc(RevealerFilter filter, SceneView sceneView, Vector2 guiPosition, out int objOverlapCount, int limit = DefaultLimit)
        {
            objOverlapCount = 0;
            var hits = new List<RevealerHit>();
            var screenPosition = HandleUtility.GUIPointToScreenPixelCoordinate(guiPosition);
            var ray3D = HandleUtility.GUIPointToWorldRay(guiPosition);
            var worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);
            var layerMask = Physics.DefaultRaycastLayers;

            var gameObjectHits = new Dictionary<GameObject, RevealerHit>();

            // Raycast (3D)
            if (filter.raycast)
            {
                RaycastHit[] raycastHits = new RaycastHit[limit];
                var raycastHitCount = Physics.RaycastNonAlloc(ray3D, raycastHits, Mathf.Infinity, layerMask);

                for (var i = 0; i < raycastHitCount; i++)
                {
                    var raycastHit = raycastHits[i];

                    // If the object is hidden in the hierarchy, continue and don't add it to the final gameObjectHits dictionary
                    if (SceneVisibilityManager.instance.IsHidden(raycastHit.transform.gameObject))
                    {
                        continue;
                    }

                    var gameObject = raycastHit.transform.gameObject;

                    if (!gameObjectHits.TryGetValue(gameObject, out var hit))
                    {
                        hit = new RevealerHit(gameObject);
                    }

                    hit.point = raycastHit.point;
                    hit.distance = raycastHit.distance;

                    gameObjectHits[gameObject] = hit;
                }
            }

            // Overlap (2D)
            if (filter.overlap)
            {
                var overlap2DHits = new Collider2D[limit];
                var overlapHitCount = Physics2D.OverlapPointNonAlloc(worldPosition, overlap2DHits, layerMask);

                for (var i = 0; i < overlapHitCount; i++)
                {
                    var overlapHit = overlap2DHits[i];

                    if (SceneVisibilityManager.instance.IsHidden(overlapHit.transform.gameObject))
                    {
                        continue;
                    }

                    var gameObject = overlapHit.transform.gameObject;

                    if (!gameObjectHits.TryGetValue(gameObject, out var hit))
                    {
                        hit = new RevealerHit(gameObject);
                    }

                    hit.distance = hit.distance ?? Vector3.Distance(overlapHit.transform.position, worldPosition);

                    gameObjectHits[gameObject] = hit;
                }
            }

            // Handles (Remaining and UI)
            List<UISortInfo> uiSortInfos = null;

            if (filter.handles)
            {
                var handleHits = new HashSet<GameObject>();
                PickAllHandles(handleHits, guiPosition, limit);

                uiSortInfos = new List<UISortInfo>();
                List<GameObject> remainingObjects = new List<GameObject>();

                foreach (var handleHit in handleHits)
                {
                    Canvas canvas = handleHit.GetComponentInParent<Canvas>();

                    // Check if the GameObject is an UI item and not a canvas
                    if (canvas != null && handleHit.GetComponent<Canvas>() == null)
                    {
                        Graphic graphic = handleHit.GetComponent<Graphic>();

                        // If the UI item picked is indeed an UI item, not its empty parent
                        if (graphic)
                        {
                            UISortInfo uiSortInfo = new UISortInfo
                            {
                                uiObject = handleHit,
                                parentCanvasSortingOrder = canvas.sortingOrder,
                                depth = graphic.depth
                            };

                            uiSortInfos.Add(uiSortInfo);
                        }
                    }
                    else if (canvas == null) // Ensure hit object is not an UI item
                    {
                        remainingObjects.Add(handleHit);
                    }
                }

                // Canvas has higher sorting order will draw first which should has higher precedence in the displayed items
                // similarly, UI elements which has deeper depth (lower in the hierarchy)
                uiSortInfos = uiSortInfos
                    .OrderByDescending(sortInfo => sortInfo.parentCanvasSortingOrder)
                    .ThenByDescending(sortInfo => sortInfo.depth)
                    .ToList();

                foreach (var remainingHit in remainingObjects)
                {
                    var gameObject = remainingHit;

                    if (!gameObjectHits.TryGetValue(gameObject, out var hit))
                    {
                        hit = new RevealerHit(gameObject);
                    }

                    hit.distance = hit.distance ?? Vector3.Distance(remainingHit.transform.position, worldPosition);
                    gameObjectHits[gameObject] = hit;
                }
            }

            // Add hits
            foreach (var gameObjectHit in gameObjectHits.Values)
            {
                hits.Add(gameObjectHit);
            }

            // Sort by distance
            hits.Sort(distanceComparison);
            objOverlapCount = hits.Count;

            // Add UIs to hit infos
            foreach (var sortInfo in uiSortInfos)
            {
                RevealerHit revealerHit = new RevealerHit(sortInfo.uiObject);
                hits.Add(revealerHit);
            }

            return hits.ToArray();
        }

        private static void PickAllHandles(HashSet<GameObject> results, Vector2 position, int limit = DefaultLimit)
        {
            GameObject result;
            int count = 0;

            do
            {
                var ignored = results.ToArray();

                result = HandleUtility.PickGameObject(position, false, ignored);

                // The "ignored" list may not always be completely effective. In certain cases, an item that is included
                // in the "ignored" list can still be returned. This situation serves as an indication that the search process
                // should be terminated.
                if (results.Contains(result))
                {
                    result = null;
                }

                if (result != null)
                {
                    results.Add(result);
                }

            } while (result != null && count++ < limit);
        }

        private static readonly Comparison<RevealerHit> distanceComparison = CompareHits;

        private static int CompareHits(RevealerHit a, RevealerHit b)
        {
            float distanceA = a.distance ?? Mathf.Infinity;
            float distanceB = b.distance ?? Mathf.Infinity;
            return distanceA.CompareTo(distanceB);
        }
    }
}
