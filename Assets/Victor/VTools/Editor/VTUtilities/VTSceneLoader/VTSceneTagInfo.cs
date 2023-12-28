using System;
using UnityEngine;
using UnityEditor;

namespace Victor.Tools
{
    [Serializable]
    public class SceneTagInfo
    {
        public Texture2D tagTexture;
        public bool textureMissing;
        public Color tagColor;
    }
}

