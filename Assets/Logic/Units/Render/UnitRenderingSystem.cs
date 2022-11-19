using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
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
}