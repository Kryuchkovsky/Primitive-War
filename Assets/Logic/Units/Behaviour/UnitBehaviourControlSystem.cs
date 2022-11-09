using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Level.Initialization;
using Logic.Level.Map;
using Logic.Units.Weapon;

namespace Logic.Units.Behaviour
{
    public sealed class UnitBehaviourControlSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;

        private EcsPool<MapInformationComponent> _mapInformationComponents;
        private EcsPool<UnitComponent> _unitComponents;
        private EcsPool<MovementComponent> _movementComponents;
        private EcsPool<ShootingComponent> _shootingComponents;
        private EcsPool<KineticWeaponComponent> _kineticWeaponComponents;

        private EcsFilter _mapComponentsFilter;
        private EcsFilter _unitComponentsFilter;
        private MapHolder _map;

        public void Init(IEcsSystems systems)
        {
            _mapInformationComponents = _world.Value.GetPool<MapInformationComponent>();
            _unitComponents = _world.Value.GetPool<UnitComponent>();
            _movementComponents = _world.Value.GetPool<MovementComponent>();
            _shootingComponents = _world.Value.GetPool<ShootingComponent>();
            _kineticWeaponComponents = _world.Value.GetPool<KineticWeaponComponent>();

            _mapComponentsFilter = _world.Value.Filter<MapInformationComponent>().End();
            _unitComponentsFilter = _world.Value
                .Filter<UnitComponent>()
                .Inc<MovementComponent>()
                .Inc<ShootingComponent>()
                .Inc<KineticWeaponComponent>()
                .End();
            
            var mapEntity = _mapComponentsFilter.GetRawEntities().First();
            _map = _mapInformationComponents.Get(mapEntity).Map;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _unitComponentsFilter)
            {
                ref var unitComponent = ref _unitComponents.Get(entity);
                ref var movementComponent = ref _movementComponents.Get(entity);
                ref var shootingComponent = ref _shootingComponents.Get(entity);
                ref var kineticWeaponComponent = ref _kineticWeaponComponents.Get(entity);
                
                var direction = shootingComponent.Target
                    ? shootingComponent.Target.transform.position - unitComponent.Unit.transform.position
                    : unitComponent.Unit.transform.forward;

                unitComponent.Unit.LookInDirection(direction);
                kineticWeaponComponent.IsShooting = shootingComponent.Target;

                if (shootingComponent.Target)
                {
                    movementComponent.TargetPosition = unitComponent.Unit.transform.position;
                }
                else
                {
                    movementComponent.TargetPosition = _map.CaptureZonePosition;
                }
            }
        }
    }
}