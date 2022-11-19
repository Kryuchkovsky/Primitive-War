using System;
using UnityEngine;

namespace Logic.Units.Render
{
    [Serializable]
    public struct UnitRenderData
    {
        public Mesh Mesh;
        public Material Material;
        public Vector3 RotationOffset;
    }
}