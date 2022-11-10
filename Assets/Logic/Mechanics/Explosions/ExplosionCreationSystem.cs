using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Units;
using UnityEngine;

namespace Logic.Mechanics.Explosions
{
    public sealed class ExplosionCreationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;

        private Collider[] _colliders = new Collider[1000];
        
        private EcsPool<ExplosionComponent> _explosionComponents;
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _explosionComponents = _world.Value.GetPool<ExplosionComponent>();
            _filter = _world.Value.Filter<ExplosionComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var explosionComponent = ref _explosionComponents.Get(entity);

                var colliders = Physics.OverlapSphereNonAlloc(
                    explosionComponent.Position, 
                    explosionComponent.Radius, 
                    _colliders, 
                    explosionComponent.LayerMask);

                for (int i = 0; i < colliders; i++)
                {
                    if (_colliders[i].TryGetComponent(out Unit unit))
                    {
                        var distance = (explosionComponent.Position - unit.transform.position).magnitude;
                        var damage = explosionComponent.Damage / (distance * distance);
                        unit.TakeDamage(damage);
                    }

                    _colliders[i].attachedRigidbody.AddExplosionForce(explosionComponent.Force, explosionComponent.Position, explosionComponent.Radius);
                }
                
                _world.Value.DelEntity(entity);
            }
        }
    }

    public struct ExplosionComponent
    {
        public LayerMask LayerMask;
        public Vector3 Position;
        public float Radius;
        public float Force;
        public float Damage;
    }
}