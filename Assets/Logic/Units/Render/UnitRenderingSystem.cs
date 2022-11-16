using System;
using System.Collections.Generic;
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

        private EcsPool<UnitRenderRequest> _renderRequests;
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _renderRequests = _world.Value.GetPool<UnitRenderRequest>();
            _filter = _world.Value.Filter<UnitRenderRequest>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var renderRequest = ref _renderRequests.Get(entity);

                if (renderRequest.CashedInstanceCount != renderRequest.InstanceCount ||
                    renderRequest.CachedSubMeshIndex != renderRequest.SubMeshIndex)
                {
                    UpdateBuffers(ref renderRequest);
                }
                
                Graphics.DrawMeshInstanced(
                    renderRequest.Mesh,
                    renderRequest.SubMeshIndex,
                    renderRequest.Material,
                    renderRequest.Matrices);
                
                // Graphics.DrawMeshInstancedIndirect(
                //     renderRequest.Mesh,
                //     renderRequest.SubMeshIndex,
                //     renderRequest.Material,
                //     new Bounds(Vector3.zero, new Vector3(100, 100, 100)),
                //     renderRequest.ArgsBuffer);
            }
        }

        public void Destroy(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var renderRequest = ref _renderRequests.Get(entity);
                renderRequest.PositionBuffer?.Release();
                renderRequest.PositionBuffer = null;
                renderRequest.ArgsBuffer?.Release();
                renderRequest.ArgsBuffer = null;
            }
        }

        private void UpdateBuffers(ref UnitRenderRequest request)
        {
            if (request.Mesh)
            {
                request.SubMeshIndex = Mathf.Clamp(request.SubMeshIndex, 0, request.Mesh.subMeshCount - 1);
            }

            request.PositionBuffer?.Release();
            request.PositionBuffer = new ComputeBuffer(request.InstanceCount, 16);
            request.PositionBuffer.SetData(request.Positions);
            request.Material.SetBuffer("positionBuffer", request.PositionBuffer);
            
            if (request.Mesh)
            {
                request.Args[0] = request.Mesh.GetIndexCount(request.SubMeshIndex);
                request.Args[1] = (uint)request.InstanceCount;
                request.Args[2] = request.Mesh.GetIndexStart(request.SubMeshIndex);
                request.Args[3] = request.Mesh.GetBaseVertex(request.SubMeshIndex);
            }
            else
            {
                request.Args[0] = request.Args[1] = request.Args[2] = request.Args[3] = 0;
            }

            request.ArgsBuffer.SetData(request.Args);
            request.CashedInstanceCount = request.InstanceCount;
            request.CachedSubMeshIndex = request.SubMeshIndex;
        }
    }
    
     public sealed class UnitRenderRequestCreationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const int ARGS_BUFFER_SIZE = 5;
        
        private readonly EcsCustomInject<UnitRenderDataList> _dataList;
        private readonly EcsWorldInject _world;

        private Dictionary<UnitType, UnitRenderInfo> _unitRenderInfo;

        private EcsPool<UnitRenderRequest> _renderRequests;
        private EcsPool<UnitComponent> _unitComponents;
        private EcsPool<UnitRenderComponent> _renderComponents;

        private EcsFilter _renderRequestsFilter;
        private EcsFilter _renderComponentsFilter;

        public void Init(IEcsSystems systems)
        {
            _unitRenderInfo = new Dictionary<UnitType, UnitRenderInfo>();

            _renderRequests = _world.Value.GetPool<UnitRenderRequest>();
            _unitComponents = _world.Value.GetPool<UnitComponent>();
            _renderComponents = _world.Value.GetPool<UnitRenderComponent>();
            
            _renderRequestsFilter = _world.Value.Filter<UnitRenderRequest>().End();
            _renderComponentsFilter = _world.Value.Filter<UnitComponent>().Inc<UnitRenderComponent>().End();

            foreach (var type in _dataList.Value.GetTypes())
            {
                var data = _dataList.Value.GetDataByType(type);
                var entity = _world.Value.NewEntity();
                ref var renderRequest = ref _renderRequests.Add(entity);
                renderRequest.Mesh = data.Mesh;
                renderRequest.Material = data.Material;
                renderRequest.ArgsBuffer = new ComputeBuffer(1, ARGS_BUFFER_SIZE * sizeof(uint), ComputeBufferType.IndirectArguments);
                renderRequest.Args = new uint[ARGS_BUFFER_SIZE];
                renderRequest.Type = type;
                var renderInfo = new UnitRenderInfo();
                renderInfo.Positions = new List<Vector4>();
                renderInfo.Matrices = new List<Matrix4x4>();
                renderInfo.RotationOffset = Quaternion.Euler(data.RotationOffset);
                _unitRenderInfo.Add(type, renderInfo);
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var key in _unitRenderInfo.Keys)
            {
                _unitRenderInfo[key].Positions.Clear();
                _unitRenderInfo[key].Matrices.Clear();
                _unitRenderInfo[key].Count = 0;
            }
            
            foreach (var entity in _renderComponentsFilter)
            {
                ref var unitComponent = ref _unitComponents.Get(entity);
                ref var renderComponent = ref _renderComponents.Get(entity);
                var info = _unitRenderInfo[renderComponent.Type];
                var unitPosition = unitComponent.Unit.transform.position;
                var position = new Vector4(unitPosition.x, unitPosition.y, unitPosition.z, 1);
                var rotation = unitComponent.Unit.transform.rotation * info.RotationOffset;
                info.Positions.Add(position);
                info.Matrices.Add(Matrix4x4.TRS(unitPosition, rotation, unitComponent.Unit.transform.localScale));
                info.Count += 1;
            }
            
            foreach (var entity in _renderRequestsFilter)
            {
                ref var renderRequest = ref _renderRequests.Get(entity);
                renderRequest.InstanceCount = _unitRenderInfo[renderRequest.Type].Count;
                renderRequest.Positions = _unitRenderInfo[renderRequest.Type].Positions.ToArray();
                renderRequest.Matrices = _unitRenderInfo[renderRequest.Type].Matrices;
            }
        }
    }

     public class UnitRenderInfo
     {
         public List<Vector4> Positions;
         public List<Matrix4x4> Matrices;
         public Quaternion RotationOffset;
         public int Count;
     }
    
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

    [Serializable]
    public struct UnitRenderData
    {
        public Mesh Mesh;
        public Material Material;
        public UnitType Type;
        public Vector3 RotationOffset;
    }

    public struct UnitRenderComponent
    {
        public UnitType Type;
    }
}