using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Victor.EditorTween;

namespace Victor.Tools
{
    public class VTHighlighter
    {
        public float fadeInDuration = 0.1f;
        public float fadeOutDuration = 0.75f;
        public float fadeOutDelay = 0.2f;
        public bool autoRemove = true;
        public Color highlightSelfColor;
        public Color highlightChildrenColor;
        public EaseType fadeInEaseType = EaseType.EaseOutQuint;
        public EaseType fadeOutEaseType = EaseType.EaseOutQuad;

        private const CameraEvent k_HighlightCameraEvent = CameraEvent.BeforeImageEffects;
        private static int s_InstanceCount = 0;

        private string m_HighlightCommandBufferName;
        private bool m_UnobscuredHighlight = true;
        private HashSet<HighlightedObjectInfo> m_HighlightedObjctInfos = new HashSet<HighlightedObjectInfo>();
        private Material m_HighlightMaskMaterial;

        /// <summary>
        /// If true, root and children highlights are not obscured by other objects. 
        /// Root highlight is not obscured by children highlights, but children highlights are obscured by the root highlight for clarity.
        /// <para>
        /// If false, root and children highlights are obscured by other objects. 
        /// Children highlights are obscured by the root highlight, and the root highlight is also obscured by children highlights.
        /// </para>
        /// </summary>
        public bool unobscuredHighlight
        {
            get => m_UnobscuredHighlight;
            set
            {
                // If the desired value is indeed different from current (Prevent unnecessary garbage collection and allocation)
                if (value != m_UnobscuredHighlight)
                {
                    if (m_HighlightMaskMaterial != null)
                    {
                        Object.DestroyImmediate(m_HighlightMaskMaterial);
                    }

                    if (value)
                    {
                        ApplyUnobscuredHighlightMask();
                    }
                    else
                    {
                        ApplyHighlightMask();
                    }

                    m_UnobscuredHighlight = value;
                }
            }
        }

        public VTHighlighter()
        {
            SceneView.duringSceneGui += DoHighlightObject;
            highlightSelfColor = Color.red.NewS(0.9f).NewV(0.8f).NewA(0.4f);
            highlightChildrenColor = Color.blue.NewS(0.8f).NewV(0.9f).NewA(0.35f);
            s_InstanceCount++;
            m_HighlightCommandBufferName = "VTools.VTHighlighter." + s_InstanceCount;
        }

        private class HighlightedObjectInfo
        {
            public HighlightedObjectInfo(int instanceID)
            {
                objInstanceID = instanceID;
                tweenedSelfColor = Color.white.NewA(0);
                tweenedChildColor = Color.white.NewA(0);
            }

            public int objInstanceID;
            // If fading in or fading out, we don't want to interrupt the process with a tween of the same kind
            public bool fadingIn = false;
            public Color tweenedSelfColor;
            public Color tweenedChildColor;
            public Material highlightEffectMat;
            public VTweenCore selfColorTween;
            public VTweenCore childColorTween;
        }

        public void HighlightSceneObject(GameObject sceneObjectToHighlight)
        {
            HighlightedObjectInfo highlightedObjectInfo = GetOrAddHighlightedObjectInfo(sceneObjectToHighlight.GetInstanceID());

            // Make sure fade in doesn't being started again during existing fade in tween
            if (highlightedObjectInfo != null && !highlightedObjectInfo.fadingIn)
            {
                float previousDuration = highlightedObjectInfo.selfColorTween == null ? 0 : highlightedObjectInfo.selfColorTween.m_Duration;
                float previousProgress = highlightedObjectInfo.selfColorTween == null ? 0 : highlightedObjectInfo.selfColorTween.m_Progress;

                float duration;

                // Try to start a tween based on previous tween duration
                // Note: If previous tween is null, the first condition evaluates to true, so the later clamp won't apply for the fadein tween that has really short duration (less than 0.2 for example)
                if (previousDuration == 0)
                {
                    duration = fadeInDuration;
                }
                else
                {
                    duration = previousProgress * fadeInDuration;

                    // If the previous tween duration is too short, the next tween will also be very short in time and the animation will be incredibly fast, so safe guard this
                    if (duration < 0.2f)
                    {
                        duration = 0.21f;
                    }
                }

                VTweenConfig fadeInHighlightConfig = new VTweenConfig().SetDuration(duration).SetEaseType(fadeInEaseType);

                highlightedObjectInfo.selfColorTween?.Remove();
                highlightedObjectInfo.selfColorTween = VTweenCreator.TweenColor(highlightedObjectInfo.tweenedSelfColor, newColor => highlightedObjectInfo.tweenedSelfColor = newColor, highlightSelfColor).SetConfig(fadeInHighlightConfig).OnValueChanged(() =>
                {
                    SceneView.RepaintAll();
                }).OnStart(() =>
                {
                    highlightedObjectInfo.fadingIn = true;
                }).OnComplete(() =>
                {
                    if (autoRemove)
                    {
                        RemoveHighlightedSceneObject(sceneObjectToHighlight);
                    }
                });

                highlightedObjectInfo.childColorTween?.Remove();
                highlightedObjectInfo.childColorTween = VTweenCreator.TweenColor(highlightedObjectInfo.tweenedChildColor, newColor => highlightedObjectInfo.tweenedChildColor = newColor, highlightChildrenColor).SetConfig(fadeInHighlightConfig).OnValueChanged(() =>
                {
                    SceneView.RepaintAll();
                });
            }
        }

        public void RemoveHighlightedSceneObject(GameObject highlightedSceneObject)
        {
            HighlightedObjectInfo highlightedObjectInfo = GetHighlightedObjectInfo(highlightedSceneObject.GetInstanceID());

            // Make sure we don't start fade out again during existing fade out tween
            if (highlightedObjectInfo != null && highlightedObjectInfo.fadingIn)
            {
                float previousDuration = highlightedObjectInfo.selfColorTween == null ? 0 : highlightedObjectInfo.selfColorTween.m_Duration;
                float previousProgress = highlightedObjectInfo.selfColorTween == null ? 0 : highlightedObjectInfo.selfColorTween.m_Progress;

                float duration;

                if (previousDuration == 0)
                {
                    duration = fadeOutDuration;
                }
                else
                {
                    duration = previousProgress * fadeOutDuration;

                    if (duration < 0.2f)
                    {
                        duration = 0.21f;
                    }
                }

                // Only if previous tween is null or completed do we apply delay to fadeout tween
                bool shouldHaveDelay = (highlightedObjectInfo.selfColorTween == null || highlightedObjectInfo.selfColorTween.m_Completed);
                VTweenConfig fadeOutTweenConfig = new VTweenConfig().SetDuration(duration).SetEaseType(fadeOutEaseType).SetInitialDelay(shouldHaveDelay ? fadeOutDelay : 0);

                highlightedObjectInfo.selfColorTween?.Remove();
                highlightedObjectInfo.selfColorTween = VTweenCreator.TweenColor(highlightedObjectInfo.tweenedSelfColor, newColor => highlightedObjectInfo.tweenedSelfColor = newColor, highlightSelfColor.NewA(0)).SetConfig(fadeOutTweenConfig).OnValueChanged(() =>
                {
                    SceneView.RepaintAll();
                }).OnStart(() =>
                {
                    highlightedObjectInfo.fadingIn = false;
                }).OnComplete(() =>
                {
                    Object.DestroyImmediate(highlightedObjectInfo.highlightEffectMat);
                    m_HighlightedObjctInfos.Remove(highlightedObjectInfo);
                });

                highlightedObjectInfo.childColorTween?.Remove();
                highlightedObjectInfo.childColorTween = VTweenCreator.TweenColor(highlightedObjectInfo.tweenedChildColor, newColor => highlightedObjectInfo.tweenedChildColor = newColor, highlightChildrenColor.NewA(0)).SetConfig(fadeOutTweenConfig).OnValueChanged(() =>
                {
                    SceneView.RepaintAll();
                });
            }
        }

        private HighlightedObjectInfo GetOrAddHighlightedObjectInfo(int instanceID)
        {
            HighlightedObjectInfo highlightedObjectInfo;
            highlightedObjectInfo = m_HighlightedObjctInfos.FirstOrDefault(highlightInfo => highlightInfo.objInstanceID == instanceID);

            if (highlightedObjectInfo != null)
            {
                return highlightedObjectInfo;
            }
            else
            {
                highlightedObjectInfo = new HighlightedObjectInfo(instanceID);

                m_HighlightedObjctInfos.Add(highlightedObjectInfo);

                return highlightedObjectInfo;
            }
        }

        private HighlightedObjectInfo GetHighlightedObjectInfo(int instanceID)
        {
            var highlightedObjectInfo = m_HighlightedObjctInfos.FirstOrDefault(highlightInfo => highlightInfo.objInstanceID == instanceID);

            return highlightedObjectInfo;
        }

        private CommandBuffer GetOrAddHighlightCommandBuffer(SceneView sceneView)
        {
            CommandBuffer highlightCommandBuffer = null;

            foreach (var commandBuffer in sceneView.camera.GetCommandBuffers(k_HighlightCameraEvent))
            {
                if (commandBuffer.name == m_HighlightCommandBufferName)
                {
                    highlightCommandBuffer = commandBuffer;
                }
            }

            if (highlightCommandBuffer == null)
            {
                highlightCommandBuffer = new CommandBuffer() { name = m_HighlightCommandBufferName };
                sceneView.camera.AddCommandBuffer(k_HighlightCameraEvent, highlightCommandBuffer);
            }

            return highlightCommandBuffer;
        }

        private void DoHighlightObject(SceneView sceneView)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            if (m_HighlightMaskMaterial == null)
            {
                if (m_UnobscuredHighlight)
                {
                    ApplyUnobscuredHighlightMask();
                }
                else
                {
                    ApplyHighlightMask();
                }
            }

            var graphics = GetOrAddHighlightCommandBuffer(sceneView);
            graphics.Clear();

            var maskId = Shader.PropertyToID("_VTHighlightMask");
            var resultId = Shader.PropertyToID("_VTHighlightResult");
            var roootId = Shader.PropertyToID("_VTHighlightRoot");

            foreach (var highlightedInfo in m_HighlightedObjctInfos)
            {
                GameObject highlightedObj = (GameObject)EditorUtility.InstanceIDToObject(highlightedInfo.objInstanceID);

                graphics.GetTemporaryRT(maskId, -1, -1);
                graphics.SetRenderTarget(maskId, BuiltinRenderTextureType.CameraTarget);
                graphics.ClearRenderTarget(false, true, new Color(0, 0, 0, 0));

                if (highlightedInfo.highlightEffectMat == null)
                {
                    highlightedInfo.highlightEffectMat = new Material(Shader.Find("VTools/Editor/VTHighlighterEffect"));
                }

                highlightedInfo.highlightEffectMat.SetColor("_Color", highlightedInfo.tweenedSelfColor);
                highlightedInfo.highlightEffectMat.SetColor("_ChildColor", highlightedInfo.tweenedChildColor);

                var renderers = highlightedObj.GetComponentsInChildren<Renderer>();
                Renderer rootRenderer = null;

                foreach (var renderer in renderers)
                {
                    // Check if the renderer belongs to the root object
                    // Note: If the root object doesn't have a renderer, rootRenderer will remain null
                    var isRoot = renderer.gameObject == highlightedObj;

                    if (isRoot)
                    {
                        rootRenderer = renderer;
                        continue;
                    }

                    // The object can't be root if reach here, so we set id to 0
                    graphics.SetGlobalFloat(roootId, 0);

                    if (renderer is MeshRenderer)
                    {
                        var mesh = renderer.GetComponent<MeshFilter>().ConvertPseudoNull()?.sharedMesh;

                        if (mesh == null)
                        {
                            continue;
                        }

                        graphics.DrawRenderer(renderer, m_HighlightMaskMaterial);
                    }
                    else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                    {
                        var mesh = skinnedMeshRenderer.sharedMesh;

                        if (mesh == null)
                        {
                            continue;
                        }

                        graphics.DrawRenderer(renderer, m_HighlightMaskMaterial);
                    }
                    else
                    {
                        graphics.DrawRenderer(renderer, m_HighlightMaskMaterial);
                    }
                }

                if (rootRenderer != null)
                {
                    graphics.SetGlobalFloat(roootId, 1);

                    if (rootRenderer is MeshRenderer)
                    {
                        var mesh = rootRenderer.GetComponent<MeshFilter>().ConvertPseudoNull()?.sharedMesh;

                        if (mesh != null)
                        {
                            graphics.DrawRenderer(rootRenderer, m_HighlightMaskMaterial);
                        }
                    }
                    else if (rootRenderer is SkinnedMeshRenderer skinnedMeshRenderer)
                    {
                        var mesh = skinnedMeshRenderer.sharedMesh;

                        if (mesh != null)
                        {
                            graphics.DrawRenderer(rootRenderer, m_HighlightMaskMaterial);
                        }
                    }
                    else
                    {
                        graphics.DrawRenderer(rootRenderer, m_HighlightMaskMaterial);
                    }
                }

                graphics.SetGlobalTexture(maskId, maskId);
                graphics.GetTemporaryRT(resultId, -1, -1);
                graphics.Blit(BuiltinRenderTextureType.CameraTarget, resultId, highlightedInfo.highlightEffectMat);
                graphics.Blit(resultId, BuiltinRenderTextureType.CameraTarget);
            }
        }

        private void ApplyUnobscuredHighlightMask()
        {
            m_HighlightMaskMaterial = new Material(Shader.Find("VTools/Editor/VTHighlighterMaskUnobscured"));
        }

        private void ApplyHighlightMask()
        {
            m_HighlightMaskMaterial = new Material(Shader.Find("VTools/Editor/VTHighlighterMask"));
        }
    }
}
