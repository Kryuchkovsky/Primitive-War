using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Teams;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Units.Spawn
{
    public sealed class SpawnRequestCreationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<UnitList> _unitList;
        private readonly EcsCustomInject<TeamsConfiguration> _teamsConfiguration;

        private EcsPool<SpawnInformationComponent> _spawnInformationComponents;
        private EcsPool<SpawnRequestQueueComponent> _requestQueueComponents;
        private EcsPool<TeamComponent> _teamComponents;

        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _filter = _world.Value.Filter<SpawnInformationComponent>().Inc<SpawnRequestQueueComponent>().End();
            _spawnInformationComponents = _world.Value.GetPool<SpawnInformationComponent>();
            _requestQueueComponents = _world.Value.GetPool<SpawnRequestQueueComponent>();
            _teamComponents = _world.Value.GetPool<TeamComponent>();

            for (int i = 0; i < _teamsConfiguration.Value.TotalTeams; i++)
            {
                var entity = _world.Value.NewEntity();

               _spawnInformationComponents.Add(entity);

                ref var requestQueueComponent = ref _requestQueueComponents.Add(entity);
                requestQueueComponent.Requests = new Queue<UnitData>();

                ref var teamComponent = ref _teamComponents.Add(entity);
                teamComponent = _teamsConfiguration.Value.GetDataByIndex(i);
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
            requestQueueComponent.Requests.Enqueue(spawnInformationComponent.SpawningUnitData);
            spawnInformationComponent.UnitIsSpawning = false;
        }

        private void StartUnitSpawn(ref SpawnInformationComponent spawnInformationComponent)
        {
            var unitType = (UnitType)Random.Range(0, Enum.GetNames(typeof(UnitType)).Length);
            var unitData = _unitList.Value.GetUnitByType(UnitType.Solider);
            spawnInformationComponent.SpawningUnitData = unitData;
            spawnInformationComponent.TimeBeforeSpawn = unitData.SpawningTime;
            spawnInformationComponent.UnitIsSpawning = true;
        }
    }
}