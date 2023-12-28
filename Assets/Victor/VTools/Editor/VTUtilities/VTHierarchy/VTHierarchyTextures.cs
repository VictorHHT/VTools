using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTHierarchyTextures
    {
        public static Texture2D firstSiblingTexture;
        public static Texture2D middleSiblingTexture;
        public static Texture2D lastSiblingTexture;
        public static Texture2D levelTexture;

        static VTHierarchyTextures()
        {
            firstSiblingTexture = Resources.Load<Texture2D>("FirstSibling");
            middleSiblingTexture = Resources.Load<Texture2D>("MiddleSiblings");
            lastSiblingTexture = Resources.Load<Texture2D>("LastSibling");
            levelTexture = Resources.Load<Texture2D>("Levels");
        }  
    }
}