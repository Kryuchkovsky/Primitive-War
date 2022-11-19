using System.Collections.Generic;
using UnityEngine;

namespace Logic.Units.Render
{
    public struct UnitRenderRequest
    {
        public Mesh Mesh;
        public Material Material;
        public ComputeBuffer ArgsBuffer;
        public ComputeBuffer PositionBuffer;
        public List<Matrix4x4> Matrices;
        public Vector4[] Positions;
        public uint[] Args;
        public int InstanceCount;
        public int CashedInstanceCount;
        public int SubMeshIndex;
        public int CachedSubMeshIndex;
        public UnitType Type;
    }
}