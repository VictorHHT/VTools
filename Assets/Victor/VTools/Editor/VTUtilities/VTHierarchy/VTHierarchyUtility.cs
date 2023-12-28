using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Victor.Tools.VTHierarchyView;

namespace Victor.Tools
{
    public static class VTHierarchyUtility
    {
        public static SiblingIndex GetObjectSelfSiblingIndex(Transform transform)
        {
            var parent = transform.parent;
            var parentChildCount = (parent != null) ? parent.childCount : transform.gameObject.scene.rootCount;

            if (parentChildCount == 1)
                return SiblingIndex.Last;

            var siblingIndex = transform.GetSiblingIndex();

            if (siblingIndex == parentChildCount - 1)
                return SiblingIndex.Last;

            return SiblingIndex.Middle;
        }
    }
}
