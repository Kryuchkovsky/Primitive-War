using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Units.Spawn
{
    public sealed class SpawnRequestCreationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const int NumberOfCommands = 2;
        
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<UnitList> _unitList;

        private EcsPool<SpawnInformationComponent> _spawnInformationComponents;
        private EcsPool<SpawnRequestQueueComponent> _requestQueueComponents;

        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _filter = _world.Value.Filter<SpawnInformationComponent>().Inc<SpawnRequestQueueComponent>().End();
            _spawnInformationComponents = _world.Value.GetPool<SpawnInformationComponent>();
            _requestQueueComponents = _world.Value.GetPool<SpawnRequestQueueComponent>();

            for (int i = 0; i < NumberOfCommands; i++)
            {
                var entity = _world.Value.NewEntity();
                
                _spawnInformationComponents.Add(entity);
                ref var spawnInformationComponent = ref _spawnInformationComponents.Get(entity);
                spawnInformationComponent.TeamId = i;
                
                _requestQueueComponents.Add(entity);
                ref var requestQueueComponent = ref _requestQueueComponents.Get(entity);
                requestQueueComponent.TeamId = i;
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var spawnInformationComponent = ref _spawnInformationComponents.Get(entity);
                spawnInformationComponent.TimeBeforeSpawn -= Time.deltaTime;

                if (spawnInformationComponent.TimeBeforeSpawn <= 0)
                {
                    ref var requestQueueComponent = ref _requestQueueComponents.Get(entity);
                    var unitType = (UnitType) Random.Range(0, Enum.GetNames(typeof(UnitType)).Length);
                    var unitData = _unitList.Value.GetUnitByType(unitType);
                    requestQueueComponent.UnitPrefabs.Enqueue(unitData.Prefab);
                    spawnInformationComponent.TimeBeforeSpawn = unitData.SpawnInterval;
                }
            }
        }
    }

    public struct SpawnRequestQueueComponent
    {
        public Queue<Unit> UnitPrefabs;
        public int TeamId;
    }
    
    public struct SpawnInformationComponent
    {
        public int TeamId;
        [Min(0)] public float TimeBeforeSpawn;
    }
}