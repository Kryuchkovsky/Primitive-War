using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Level.Map;
using UnityEngine;

namespace Logic.Level.Initialization
{
    public sealed class MapCreationSystem : IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;

        private EcsPool<MapInformationComponent> _mapInformationComponents;
        private EcsCustomInject<MapHolder> _map;

        public void Init(IEcsSystems systems)
        {
            _mapInformationComponents = _world.Value.GetPool<MapInformationComponent>();
            var map = Object.Instantiate(_map.Value);
            var mapEntity = _world.Value.NewEntity();
            ref var mapInformationComponent = ref _mapInformationComponents.Add(mapEntity);
            mapInformationComponent.Map = map;
        }
    }
}