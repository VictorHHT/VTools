using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public class VTSceneObjectCache
    {
        public readonly Transform objTransform;
        public int instanceID;
        public int childCount;
        public int depth;
        public VTHierarchyView.SiblingIndex siblingIndex;
        public bool hasParent;
        public Transform parent;

        public VTSceneObjectCache(Transform objTransform, int instanceID, int childCount, int depth, VTHierarchyView.SiblingIndex siblingIndex, bool hasParent, Transform parent)
        {
            this.objTransform = objTransform;
            this.instanceID = instanceID;
            this.childCount = childCount;
            this.depth = depth;
            this.siblingIndex = siblingIndex;
            this.hasParent = hasParent;
            this.parent = parent;
        }
    }
}