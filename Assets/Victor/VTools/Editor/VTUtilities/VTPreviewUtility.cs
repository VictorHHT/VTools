using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Victor.Tools
{
    public static class VTPreviewUtility
    {
        public static Texture2D GetPreview(GameObject obj, out bool hasModelPreview)
        {
            Texture2D preview;
            hasModelPreview = false;

            if (obj == null || !obj.activeSelf)
            {
                return null;
            }

            Image image = obj.GetComponent<Image>();

            if (image != null && image.sprite != null)
            {
                preview = AssetPreview.GetAssetPreview(image.sprite);
                return preview;
            }

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            bool hasRenderer = renderers.Length != 0;

            if (!hasRenderer)
                return null;

            int validRenderersCount = 0;

            foreach (Renderer renderer in renderers)
            {
                if (renderer is SpriteRenderer && ((SpriteRenderer)renderer).sprite != null)
                {
                    validRenderersCount++;
                }

                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();

                if (renderer is MeshRenderer && meshFilter != null && meshFilter.sharedMesh != null)
                {
                    validRenderersCount++;
                    hasModelPreview = true;
                }

                if (renderer is SkinnedMeshRenderer && ((SkinnedMeshRenderer)renderer).sharedMesh != null)
                {
                    validRenderersCount++;
                    hasModelPreview = true;
                }
            }

            if (validRenderersCount > 1)
            {
                Transform parent = obj.transform.parent;
                Vector3 originalLocalScale = obj.transform.localScale;

                if (parent != null)
                {
                    // AssetPreview.GetAssetPreview is not intelligent enough to consider transform in the full hierarchy and adjust,
                    // so we acquire object's world scale for preview and set back later
                    obj.transform.localScale = obj.transform.lossyScale;
                }

                preview = AssetPreview.GetAssetPreview(obj);

                if (parent != null)
                {
                    obj.transform.localScale = originalLocalScale;
                }

                return preview;
            }

            // Object doens't have multiple renderers in children, simply get from the first array entry
            if (renderers[0] is SpriteRenderer renderer1 && renderer1.sprite != null)
            {
                Transform parent = obj.transform.parent;
                Vector3 originalLocalScale = obj.transform.localScale;

                if (parent != null)
                {
                    obj.transform.localScale = obj.transform.lossyScale;
                }

                preview = AssetPreview.GetAssetPreview(obj);

                if (parent != null)
                {
                    obj.transform.localScale = originalLocalScale;
                }

                return preview;
            }

            MeshFilter meshFilter1 = obj.GetComponent<MeshFilter>();

            if (renderers[0] is MeshRenderer && meshFilter1 != null && meshFilter1.sharedMesh != null)
            {
                Transform parent = obj.transform.parent;
                Vector3 originalLocalScale = obj.transform.localScale;

                if (parent != null)
                {
                    obj.transform.localScale = obj.transform.lossyScale;
                }
               
                preview = AssetPreview.GetAssetPreview(obj);

                if (parent != null)
                {
                    obj.transform.localScale = originalLocalScale;
                }

                hasModelPreview = true;
                return preview;
            }

            if (renderers[0] is SkinnedMeshRenderer renderer2 && renderer2.sharedMesh != null)
            {
                Transform parent = obj.transform.parent;
                Vector3 originalLocalScale = obj.transform.localScale;

                if (parent != null)
                {
                    obj.transform.localScale = obj.transform.parent.TransformVector(obj.transform.localScale);
                }

                preview = AssetPreview.GetAssetPreview(obj);

                if (parent != null)
                {
                    obj.transform.localScale = originalLocalScale;
                }

                hasModelPreview = true;
                return preview;
            }

            return null;
        }
    }

}