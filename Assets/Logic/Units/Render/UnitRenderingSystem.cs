using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Units.Behaviour;
using UnityEngine;

namespace Logic.Units.Render
{
    public sealed class UnitRenderingSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsCustomInject<UnitList> _unitList;
        private readonly EcsWorldInject _world;

        private int _instanceCount = 100000;
        private Mesh _instanceMesh;
        private Material _instanceMaterial;
        private int _subMeshIndex = 0;

        private int _cachedInstanceCount = -1;
        private int _cachedSubMeshIndex = -1;
        private ComputeBuffer _positionBuffer;
        private ComputeBuffer _argsBuffer;
        private readonly uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };

        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _filter = _world.Value.Filter<UnitRenderComponent>().End();
            
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            UpdateBuffers();
        }

        public void Run(IEcsSystems systems)
        {
            // Update starting position buffer
            if (_cachedInstanceCount != _instanceCount || _cachedSubMeshIndex != _subMeshIndex)
                UpdateBuffers();

            // Render
            Graphics.DrawMeshInstancedIndirect(_instanceMesh, _subMeshIndex, _instanceMaterial,
                new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), _argsBuffer);
        }

        public void Destroy(IEcsSystems systems)
        {
            if (_positionBuffer != null)
                _positionBuffer.Release();
            _positionBuffer = null;

            if (_argsBuffer != null)
                _argsBuffer.Release();
            _argsBuffer = null;
        }

        private void UpdateBuffers()
        {
            // Ensure submesh index is in range
            if (_instanceMesh != null)
                _subMeshIndex = Mathf.Clamp(_subMeshIndex, 0, _instanceMesh.subMeshCount - 1);

            // Positions
            if (_positionBuffer != null)
                _positionBuffer.Release();
            _positionBuffer = new ComputeBuffer(_instanceCount, 16);
            Vector4[] positions = new Vector4[_instanceCount];
            for (int i = 0; i < _instanceCount; i++)
            {
                float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
                float distance = Random.Range(20.0f, 100.0f);
                float height = Random.Range(-2.0f, 2.0f);
                float size = Random.Range(0.05f, 0.25f);
                positions[i] = new Vector4(Mathf.Sin(angle) * distance, height, Mathf.Cos(angle) * distance, size);
            }

            _positionBuffer.SetData(positions);
            _instanceMaterial.SetBuffer("positionBuffer", _positionBuffer);

            // Indirect args
            if (_instanceMesh != null)
            {
                _args[0] = (uint)_instanceMesh.GetIndexCount(_subMeshIndex);
                _args[1] = (uint)_instanceCount;
                _args[2] = (uint)_instanceMesh.GetIndexStart(_subMeshIndex);
                _args[3] = (uint)_instanceMesh.GetBaseVertex(_subMeshIndex);
            }
            else
            {
                _args[0] = _args[1] = _args[2] = _args[3] = 0;
            }

            _argsBuffer.SetData(_args);

            _cachedInstanceCount = _instanceCount;
            _cachedSubMeshIndex = _subMeshIndex;
        }
    }

    public struct UnitRenderComponent
    {
        public Mesh Mesh;
        public UnitType Type;
    }

    public struct UnitRenderRequest
    {
        public UnitType Type;
    }
}