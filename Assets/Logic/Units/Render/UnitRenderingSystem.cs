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
        
        private ComputeBuffer _positionBuffer;
        private ComputeBuffer _argsBuffer;
        private readonly uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };

        public void Init(IEcsSystems systems)
        {
            _renderRequests = _world.Value.GetPool<UnitRenderRequest>();
            _filter = _world.Value.Filter<UnitRenderRequest>().End();
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
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

                Graphics.DrawMeshInstancedIndirect(
                    renderRequest.Mesh, 
                    renderRequest.SubMeshIndex, 
                    renderRequest.Material,
                    new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), 
                    _argsBuffer);
            }
        }

        public void Destroy(IEcsSystems systems)
        {
            _positionBuffer?.Release();
            _positionBuffer = null;
            _argsBuffer?.Release();
            _argsBuffer = null;
        }

        private void UpdateBuffers(ref UnitRenderRequest request)
        {
            if (request.Mesh)
            {
                request.SubMeshIndex = Mathf.Clamp(request.SubMeshIndex, 0, request.Mesh.subMeshCount - 1);
            }

            _positionBuffer?.Release();
            _positionBuffer = new ComputeBuffer(request.InstanceCount, 16);
            _positionBuffer.SetData(request.Positions);
            request.Material.SetBuffer("positionBuffer", _positionBuffer);
            
            if (request.Mesh)
            {
                _args[0] = (uint)request.Mesh.GetIndexCount(request.SubMeshIndex);
                _args[1] = (uint)request.InstanceCount;
                _args[2] = (uint)request.Mesh.GetIndexStart(request.SubMeshIndex);
                _args[3] = (uint)request.Mesh.GetBaseVertex(request.SubMeshIndex);
            }
            else
            {
                _args[0] = _args[1] = _args[2] = _args[3] = 0;
            }

            _argsBuffer.SetData(_args);

            request.CashedInstanceCount = request.InstanceCount;
            request.CachedSubMeshIndex = request.SubMeshIndex;
        }
    }
    
     public sealed class UnitRenderRequestCreationSystem : IEcsInitSystem, IEcsRunSystem
    {
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
            _renderComponentsFilter = _world.Value.Filter<UnitComponent>().Exc<UnitRenderComponent>().End();

            foreach (var type in _dataList.Value.GetTypes())
            {
                var data = _dataList.Value.GetDataByType(type);
                var entity = _world.Value.NewEntity();
                ref var renderRequest = ref _renderRequests.Add(entity);
                renderRequest.Mesh = data.Mesh;
                renderRequest.Material = data.Material;
                renderRequest.Type = type;
                _unitRenderInfo.Add(type, new UnitRenderInfo());
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var key in _unitRenderInfo.Keys)
            {
                _unitRenderInfo[key].Positions.Clear();
                _unitRenderInfo[key].Count = 0;
            }
            
            foreach (var entity in _renderComponentsFilter)
            {
                ref var unitComponent = ref _unitComponents.Get(entity);
                ref var renderComponent = ref _renderComponents.Get(entity);
                _unitRenderInfo[renderComponent.Type].Positions.Add(unitComponent.Unit.transform.position);
                _unitRenderInfo[renderComponent.Type].Count += 1;
            }
            
            foreach (var entity in _renderRequestsFilter)
            {
                ref var renderRequest = ref _renderRequests.Get(entity);
                renderRequest.InstanceCount = _unitRenderInfo[renderRequest.Type].Count;
                renderRequest.Positions = _unitRenderInfo[renderRequest.Type].Positions.ToArray();
            }
        }
    }

     public class UnitRenderInfo
     {
         public List<Vector4> Positions;
         public int Count;
     }

    public struct UnitRenderRequest
    {
        public Mesh Mesh;
        public Material Material;
        public Vector4[] Positions;
        public int InstanceCount;
        public int CashedInstanceCount;
        public int SubMeshIndex;
        public int CachedSubMeshIndex;
        public UnitType Type;
    }

    public struct UnitRenderData
    {
        public Mesh Mesh;
        public Material Material;
        public UnitType Type;
    }

    public struct UnitRenderComponent
    {
        public UnitType Type;
    }
}