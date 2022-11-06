using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Level.Initialization;
using Logic.Teams;
using Logic.Units.Behaviour;
using UnityEngine;

namespace Logic.Units.Spawn
{
    public sealed class UnitSpawnSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;

        private EcsPool<UnitComponent> _unitComponents;
        private EcsPool<MovementComponent> _movementComponents;
        private EcsPool<ShootingComponent> _shootingComponents;
        private EcsPool<MapInformationComponent> _mapInformationComponents;
        private EcsPool<SpawnRequestQueueComponent> _requestQueueComponents;
        private EcsPool<TeamComponent> _teamComponents;
        
        private EcsFilter _mapInformationComponentFilter;
        private EcsFilter _requestQueueComponentFilter;
        
        public void Init(IEcsSystems systems)
        {
            _unitComponents = _world.Value.GetPool<UnitComponent>();
            _movementComponents = _world.Value.GetPool<MovementComponent>();
            _shootingComponents = _world.Value.GetPool<ShootingComponent>();
            _mapInformationComponents = _world.Value.GetPool<MapInformationComponent>();
            _requestQueueComponents = _world.Value.GetPool<SpawnRequestQueueComponent>();
            _teamComponents = _world.Value.GetPool<TeamComponent>();
            
            _mapInformationComponentFilter = _world.Value.Filter<MapInformationComponent>().End();
            _requestQueueComponentFilter = _world.Value.Filter<SpawnRequestQueueComponent>().Inc<TeamComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var mapEntity in _mapInformationComponentFilter)
            {
                ref var mapInformationComponent = ref _mapInformationComponents.Get(mapEntity);
                
                foreach (var entity in _requestQueueComponentFilter)
                {
                    ref var requestQueueComponent = ref _requestQueueComponents.Get(entity);
                    ref var teamComponent = ref _teamComponents.Get(entity);

                    if (requestQueueComponent.UnitPrefabs.Count > 0)
                    {
                        var unitPrefab = requestQueueComponent.UnitPrefabs.Dequeue();
                        var teamId = teamComponent.TeamId;
                        var spawnPlace = mapInformationComponent.Map.SpawnPlaces.First(place => place.Number == teamId);
                        var position = spawnPlace.SpawnPoints[Random.Range(0, spawnPlace.SpawnPoints.Count)].position;
                        var unit = Object.Instantiate(unitPrefab, position, Quaternion.identity, mapInformationComponent.Map.transform);
                        unit.TeamComponent = teamComponent;
                        unit.gameObject.layer = teamComponent.LayerIndex;
                        
                        var unitEntity = _world.Value.NewEntity();
                        
                        ref var unitComponent = ref _unitComponents.Add(unitEntity);
                        unitComponent.Unit = unit;
                        
                        ref var movementComponent = ref _movementComponents.Add(unitEntity);
                        movementComponent.TargetPosition = mapInformationComponent.Map.CaptureZonePosition;

                        _shootingComponents.Add(unitEntity);
                        
                        ref var unitTeamComponent = ref _teamComponents.Add(unitEntity);
                        unitTeamComponent = teamComponent;

                    }
                }
            }
        }
    }
}