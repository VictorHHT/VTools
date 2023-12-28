using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    [InitializeOnLoad]
    public static class VTHierarchyView
    {
        public static Dictionary<int, VTSceneObjectCache> sceneObjCacheDict;
        private const float k_PreviewWindowWidth = 190;
        private const float k_PreviewWindowHeight = 120;
        private static readonly Color s_RowShadingBackgroundColor = new Color(0, 0, 0, 0.075f);
        private static readonly Color s_RowShadingSeperatorColor = new Color(0, 0, 0, 0.05f);
        private static float s_LastCacheClearTimeStamp;
        private static float s_CurrentCacheClearTimeStamp;
        private static bool s_ClearTempPreviewResultedChange;
        private static bool s_PreviewWindowOpenOrClose;
        private static VTPreviewWindow s_ObjectPreviewWindowContent;

        public enum SiblingIndex
        {
            Middle,
            Last
        }

        static VTHierarchyView()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            EditorApplication.hierarchyChanged += HierarchyChanged;
            // When we undo/redo while mouse is inside the Hierarchy, make sure parent previews are being updated
            // We don't call ClearTemporaryPreview because we want to recache objects
            Undo.undoRedoPerformed += UnityEditorDynamic.AssetPreview.ClearTemporaryAssetPreviews();
            sceneObjCacheDict = new Dictionary<int, VTSceneObjectCache>();
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (obj == null)
                return;

            VTSceneObjectCache sceneObjectCache;

            if (sceneObjCacheDict.ContainsKey(instanceID))
            {
                sceneObjectCache = sceneObjCacheDict[instanceID];
            }
            else
            {
                int childCount = obj.transform.childCount;
                bool hasParent = obj.transform.parent == null ? false : true;
                int depth = obj.transform.GetHierarchyDepth();
                SiblingIndex siblingIndex = VTHierarchyUtility.GetObjectSelfSiblingIndex(obj.transform);
                Transform parent = obj.transform.parent;
                sceneObjectCache = new VTSceneObjectCache(obj.transform, instanceID, childCount, depth, siblingIndex, hasParent, parent);
                sceneObjCacheDict[instanceID] = sceneObjectCache;
            }

            // Update previews as mouse entering Hierarchy window
            // Use handles in the scene view to modify object's position and scale doesn't set hierarchyChanged to true, so we need to manually set it
            if (Event.current.type == EventType.MouseEnterWindow)
            {
                ClearTemporaryPreview();
            }

            DrawRowShading(selectionRect);
            DrawObjectPreviewButton(sceneObjectCache, selectionRect);

            if (IsSearching())
                return;

            DrawFoldouts(sceneObjectCache, selectionRect);
        }

        /// <summary>
        /// Hierarchy changed is set whenever an object's position or scale is changed in the hierarchy,
        /// temporary previews are cleared, or Undo/Redo that changes scene objects is performed
        /// </summary>
        private static void HierarchyChanged()
        {
            // Ensure an infinite hierarchy changed loop is not created
            // hierarchy changed is set when ClearTemporaryPreview() is called
            if (s_ClearTempPreviewResultedChange)
            {
                s_ClearTempPreviewResultedChange = false;
            }
            else if (s_PreviewWindowOpenOrClose)
            {
                // Object is created or destroyed when preview window open or close, which will set hierarchy changed to true,
                // but nothing really changed, so skip rest of the code
                s_PreviewWindowOpenOrClose = false;
            }
            else
            {
                for (int i = 0; i < sceneObjCacheDict.Values.Count; i++)
                {
                    VTSceneObjectCache objectInfo = sceneObjCacheDict.Values.ToList()[i];
                    CacheSceneObject(objectInfo);
                }

                ClearTemporaryPreview();
            }
        }

        private static void ClearTemporaryPreview()
        {
            s_ClearTempPreviewResultedChange = true;
            UnityEditorDynamic.AssetPreview.ClearTemporaryAssetPreviews();
        }

        private static void DrawRowShading(Rect selectionRect)
        {
            var isOdd = Mathf.FloorToInt(((selectionRect.y - 4) / 16) % 2) != 0;

            if (isOdd) return;

            var foldoutRect = new Rect(selectionRect);
            foldoutRect.width += selectionRect.x + 16f;
            foldoutRect.height += 1f;
            foldoutRect.x = VTHierarchyStyles.k_DrawOffset;

            // Background
            EditorGUI.DrawRect(foldoutRect, s_RowShadingBackgroundColor);
            // Top line
            foldoutRect.height = 1f;
            EditorGUI.DrawRect(foldoutRect, s_RowShadingSeperatorColor);
            // Bottom line
            foldoutRect.y += 15f;
            EditorGUI.DrawRect(foldoutRect, s_RowShadingSeperatorColor);
        }

        private static void DrawFoldouts(VTSceneObjectCache objectCache, Rect selectionRect)
        {
            Rect foldoutRect = new Rect(selectionRect);

            if (objectCache.depth >= 1)
            {
                for (int i = 1; i <= objectCache.depth; i++)
                {
                    Rect rect = new Rect(selectionRect);
                    rect.height = 16;
                    rect.width = 15;
                    rect.x = rect.xMin - VTHierarchyStyles.k_IndentationPerDepth * i - VTHierarchyStyles.k_FixedIndentation - 5f;
                    GUI.DrawTexture(rect, VTHierarchyTextures.levelTexture);
                }
            }

            var transform = objectCache.objTransform;
            if (transform.childCount > 0) return;

            var index = objectCache.siblingIndex;
            foldoutRect.width = 16f;

            foldoutRect.x = selectionRect.x - 16f;

            GUI.DrawTexture(foldoutRect, GetFoldoutIcon(index));
        }

        private static void DrawObjectPreviewButton(VTSceneObjectCache objectCache, Rect selectionRect)
        {
            Rect previewIconRect = new Rect(selectionRect);
            previewIconRect.x -= 1f;
            previewIconRect.width = previewIconRect.height = 16f;

            Texture2D texture = VTPreviewUtility.GetPreview(objectCache.objTransform.gameObject, out bool hasModelPreview);

            if (texture == null)
                return;

            VTGUI.StoreGUIColor();
            bool isOnEvenRow = Mathf.FloorToInt(((selectionRect.y - 4) / 16) % 2) == 0;

            if (isOnEvenRow)
                GUI.color = Color.white.NewV(0.8f);
            else
                GUI.color = Color.white.NewV(0.9f);

            GUI.Box(previewIconRect, VTHierarchyTextures.levelTexture, VTStyles.evenBackgroundTight);
            VTGUI.RevertGUIColor();
            GUI.DrawTexture(previewIconRect, texture, ScaleMode.ScaleToFit);

            if (Event.current.type == EventType.MouseDown)
            {
                if (previewIconRect.Contains(Event.current.mousePosition))
                {
                    Rect showRect = new Rect(previewIconRect);
                    showRect.x -= 40;
                    showRect.y += 2;

                    s_ObjectPreviewWindowContent = new VTPreviewWindow(objectCache.objTransform.gameObject, k_PreviewWindowWidth, k_PreviewWindowHeight, hasModelPreview);
                    s_ObjectPreviewWindowContent.onOpenCallback += PreviewWindowOpenOrClose;
                    s_ObjectPreviewWindowContent.onCloseCallback += PreviewWindowOpenOrClose;
                    PopupWindow.Show(showRect, s_ObjectPreviewWindowContent);
                    Event.current.Use();
                }
            }

            if (Event.current.type == EventType.ScrollWheel && s_ObjectPreviewWindowContent != null && s_ObjectPreviewWindowContent.editorWindow != null)
            {
                GUIUtility.hotControl = 0;
                s_ObjectPreviewWindowContent.editorWindow.Close();
                s_ObjectPreviewWindowContent = null;
            }
        }

        private static void CacheSceneObject(VTSceneObjectCache objectCache)
        {
            Transform objTransform = objectCache.objTransform;

            // If the object has been removed from the scene, it needs to be removed from the dictionary as well
            if (objTransform == null)
            {
                sceneObjCacheDict.Remove(objectCache.instanceID);
                return;
            }

            objectCache.childCount = objTransform.childCount;
            objectCache.hasParent = objTransform.parent == null ? false : true;
            objectCache.depth = objTransform.transform.GetHierarchyDepth();
            objectCache.siblingIndex = VTHierarchyUtility.GetObjectSelfSiblingIndex(objTransform);
            objectCache.parent = objTransform.parent;
        }

        private static Texture2D GetFoldoutIcon(SiblingIndex index)
        {
            switch (index)
            {
                case SiblingIndex.Middle:
                    return VTHierarchyTextures.middleSiblingTexture;
                case SiblingIndex.Last:
                    return VTHierarchyTextures.lastSiblingTexture;
                default:
                    return VTHierarchyTextures.middleSiblingTexture;
            }
        }

        private static bool IsSearching()
        {
            bool searching = UnityEditorDynamic.SceneHierarchyWindow.lastInteractedHierarchyWindow.sceneHierarchy.hasSearchFilter;
            return searching;
        }

        private static void PreviewWindowOpenOrClose()
        {
            s_PreviewWindowOpenOrClose = true;
        }
    }
}
