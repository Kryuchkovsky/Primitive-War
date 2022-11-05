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

                ref var spawnInformationComponent = ref _spawnInformationComponents.Add(entity);
                spawnInformationComponent.TeamId = i;
                
                ref var requestQueueComponent = ref _requestQueueComponents.Add(entity);
                requestQueueComponent.UnitPrefabs = new Queue<Unit>();
                requestQueueComponent.TeamId = i;
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var spawnInformationComponent = ref _spawnInformationComponents.Get(entity);

                if (spawnInformationComponent.UnitIsSpawning)
                {
                    spawnInformationComponent.TimeBeforeSpawn -= Time.deltaTime;
                    
                    if (spawnInformationComponent.TimeBeforeSpawn <= 0)
                    {
                        RequestUnitSpawn(entity, ref spawnInformationComponent);
                    }
                }
                else
                {
                    StartUnitSpawn(ref spawnInformationComponent);
                }
            }
        }

        private void RequestUnitSpawn(int entity, ref SpawnInformationComponent spawnInformationComponent)
        {
            ref var requestQueueComponent = ref _requestQueueComponents.Get(entity);
            requestQueueComponent.UnitPrefabs.Enqueue(spawnInformationComponent.SpawningUnitData.Prefab);
            requestQueueComponent.TeamId = spawnInformationComponent.TeamId;
            spawnInformationComponent.UnitIsSpawning = false;
        }

        private void StartUnitSpawn(ref SpawnInformationComponent spawnInformationComponent)
        {
            var unitType = (UnitType)Random.Range(0, Enum.GetNames(typeof(UnitType)).Length);
            var unitData = _unitList.Value.GetUnitByType(unitType);
            spawnInformationComponent.SpawningUnitData = unitData;
            spawnInformationComponent.TimeBeforeSpawn = unitData.SpawningTime;
            spawnInformationComponent.UnitIsSpawning = true;
        }
    }
}