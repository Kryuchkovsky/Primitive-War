using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Teams;
using UnityEngine;

namespace Logic.Units.Behaviour
{
    public sealed class UnitShootingControlSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;

        private EcsPool<UnitComponent> _unitComponents;
        private EcsPool<ShootingComponent> _shootingComponents;
        private EcsPool<TeamComponent> _teamComponents;
        private EcsFilter _filter;

        private Collider[] _colliders = new Collider[1];
        
        public void Init(IEcsSystems systems)
        {
            _unitComponents = _world.Value.GetPool<UnitComponent>();
            _shootingComponents = _world.Value.GetPool<ShootingComponent>();
            _teamComponents = _world.Value.GetPool<TeamComponent>();
            _filter = _world.Value.Filter<UnitComponent>().Inc<MovementComponent>().Inc<TeamComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var unitComponent = ref _unitComponents.Get(entity);
                ref var shootingComponent = ref _shootingComponents.Get(entity);
                ref var teamComponent = ref _teamComponents.Get(entity);

                if (shootingComponent.Target)
                {
                    var direction = (shootingComponent.Target.transform.position - unitComponent.Unit.transform.position).normalized;
                    unitComponent.Unit.LookInDirection(direction);
                }
                else
                {
                    var matches = Physics.OverlapSphereNonAlloc(unitComponent.Unit.transform.position, 10, _colliders, teamComponent.EnemiesLayerMask);

                    if (matches > 0 && _colliders[0].TryGetComponent(out Unit unit))
                    {
                        shootingComponent.Target = unit;
                    }
                }
            }
        }
    }
}