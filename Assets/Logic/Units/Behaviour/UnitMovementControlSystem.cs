using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Logic.Units.Behaviour
{
    public sealed class UnitMovementControlSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;

        private EcsPool<UnitComponent> _unitComponents;
        private EcsPool<MovementComponent> _movementComponents;
        private EcsFilter _filter;
        
        public void Init(IEcsSystems systems)
        {
            _unitComponents = _world.Value.GetPool<UnitComponent>();
            _movementComponents = _world.Value.GetPool<MovementComponent>();
            _filter = _world.Value.Filter<UnitComponent>().Inc<MovementComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var unitComponent = ref _unitComponents.Get(entity);
                ref var movementComponent = ref _movementComponents.Get(entity);
                unitComponent.Unit.NavMeshAgent.SetDestination(movementComponent.TargetPosition);
            }
        }
    }
}