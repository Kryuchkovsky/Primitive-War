using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Level.Initialization;
using Logic.Level.Map;
using Logic.Teams;
using Logic.Units.Behaviour;
using Logic.Units.Weapon;
using UnityEngine;

namespace Logic.Units.Spawn
{
    public sealed class UnitSpawnSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<KineticWeaponConfiguration> _kineticWeaponConfiguration;

        private EcsPool<MapInformationComponent> _mapInformationComponents;
        private EcsPool<SpawnRequestQueueComponent> _requestQueueComponents;
        private EcsPool<UnitComponent> _unitComponents;
        private EcsPool<HealthComponent> _healthComponents;
        private EcsPool<MovementComponent> _movementComponents;
        private EcsPool<ShootingComponent> _shootingComponents;
        private EcsPool<KineticWeaponComponent> _kineticWeaponComponents;
        private EcsPool<WeaponReloadComponent> _weaponReloadComponents;
        private EcsPool<TeamComponent> _teamComponents;
        private EcsPool<DamageComponent> _damageComponents;

        private EcsFilter _mapComponentsFilter;
        private EcsFilter _requestQueueComponentsFilter;
        
        private MapHolder _map;

        public void Init(IEcsSystems systems)
        {
            _mapInformationComponents = _world.Value.GetPool<MapInformationComponent>();
            _requestQueueComponents = _world.Value.GetPool<SpawnRequestQueueComponent>();
            _unitComponents = _world.Value.GetPool<UnitComponent>();
            _healthComponents = _world.Value.GetPool<HealthComponent>();
            _movementComponents = _world.Value.GetPool<MovementComponent>();
            _shootingComponents = _world.Value.GetPool<ShootingComponent>();
            _kineticWeaponComponents = _world.Value.GetPool<KineticWeaponComponent>();
            _weaponReloadComponents = _world.Value.GetPool<WeaponReloadComponent>();
            _teamComponents = _world.Value.GetPool<TeamComponent>();
            _damageComponents = _world.Value.GetPool<DamageComponent>();

            _mapComponentsFilter = _world.Value.Filter<MapInformationComponent>().End();
            _requestQueueComponentsFilter = _world.Value.Filter<SpawnRequestQueueComponent>().Inc<TeamComponent>().End();

            var mapEntity = _mapComponentsFilter.GetRawEntities().First();
            _map = _mapInformationComponents.Get(mapEntity).Map;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _requestQueueComponentsFilter)
            {
                ref var requestQueueComponent = ref _requestQueueComponents.Get(entity);
                ref var teamComponent = ref _teamComponents.Get(entity);

                if (requestQueueComponent.Requests.Count > 0)
                {
                    var data = requestQueueComponent.Requests.Dequeue();
                    var teamId = teamComponent.TeamId;
                    var spawnPlace = _map.SpawnPlaces.First(place => place.Number == teamId);
                    var position = spawnPlace.SpawnPoints[Random.Range(0, spawnPlace.SpawnPoints.Count)].position;
                    var unit = Object.Instantiate(data.Prefab, position, Quaternion.identity, _map.UnitsContainer);
                    unit.SetColor(teamComponent.Color);
                    unit.TeamComponent = teamComponent;
                    unit.gameObject.layer = teamComponent.LayerIndex;

                    var unitEntity = _world.Value.NewEntity();

                    ref var unitComponent = ref _unitComponents.Add(unitEntity);
                    unitComponent.Unit = unit;
                    unitComponent.Data = data;

                    ref var healthComponent = ref _healthComponents.Add(unitEntity);
                    healthComponent.HealthPoints = data.InitialHealthPoints;

                    _movementComponents.Add(unitEntity);
                    _shootingComponents.Add(unitEntity);

                    ref var kineticWeaponComponent = ref _kineticWeaponComponents.Add(unitEntity);
                    kineticWeaponComponent.ShotPoint = unit.ManualWeaponHolder.ShotPoint;
                    kineticWeaponComponent.Data = _kineticWeaponConfiguration.Value.GetDataByType(unit.KineticWeaponType);

                    _weaponReloadComponents.Add(unitEntity);

                    ref var unitTeamComponent = ref _teamComponents.Add(unitEntity);
                    unitTeamComponent = teamComponent;

                    unit.OnTakeDamage += damage =>
                    {
                        var damageEntity = _world.Value.NewEntity();
                        ref var damageComponent = ref _damageComponents.Add(damageEntity);
                        damageComponent.DamagedEntity = _world.Value.PackEntity(unitEntity);
                        damageComponent.Damage = damage;
                    };
                }
            }
        }
    }
}