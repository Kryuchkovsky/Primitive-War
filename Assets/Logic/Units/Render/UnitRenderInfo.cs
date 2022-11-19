using System.Collections.Generic;
using UnityEngine;

namespace Logic.Units.Render
{
    public class UnitRenderInfo
    {
        public List<Vector4> Positions;
        public List<Matrix4x4> Matrices;
        public Quaternion RotationOffset;
        public int Count;
    }
}