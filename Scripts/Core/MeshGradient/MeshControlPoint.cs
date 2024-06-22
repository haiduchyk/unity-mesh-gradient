namespace Pandora.MeshGradient
{
    using System;
    using UnityEngine;

    [Serializable]
    public class MeshControlPoint
    {
        public Vector2 location;
        public Vector2 uTangent;
        public Vector2 vTangent;
        public Color color;
    }
}