using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Logic.Units.Weapon.Bullets
{
    public sealed class BulletsDestructionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;
        
        private EcsPool<BulletComponent> _bulletComponents;
        private EcsPool<BulletCollisionComponent> _bulletCollisionComponents;

        private EcsFilter _bulletComponentsFilter;
        private EcsFilter _bulletCollisionComponentsFilter;

        public void Init(IEcsSystems systems)
        {
            _bulletComponents = _world.Value.GetPool<BulletComponent>();
            _bulletCollisionComponents = _world.Value.GetPool<BulletCollisionComponent>();
            
            _bulletComponentsFilter = _world.Value.Filter<BulletComponent>().Exc<BulletCollisionComponent>().End();
            _bulletCollisionComponentsFilter = _world.Value.Filter<BulletCollisionComponent>().Inc<BulletCollisionComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _bulletCollisionComponentsFilter)
            {
                ref var bulletComponent = ref _bulletComponents.Get(entity);
                ref var bulletCollisionComponent = ref _bulletCollisionComponents.Get(entity);

                if (bulletCollisionComponent.Collision.collider.TryGetComponent(out Unit unit))
                {
                    unit.TakeDamage(bulletComponent.Data.Damage);
                }
                
                Object.Destroy(bulletComponent.Bullet.gameObject);
                _world.Value.DelEntity(entity);
            }
            
            foreach (var entity in _bulletComponentsFilter)
            {
                ref var bulletComponent = ref _bulletComponents.Get(entity);

                if (bulletComponent.TraveledDistance >= bulletComponent.Data.Distance)
                {
                    _bulletCollisionComponents.Add(entity);
                }
                else
                {
                    var delta = bulletComponent.Bullet.Rigidbody.velocity.magnitude * Time.deltaTime;
                    bulletComponent.TraveledDistance += delta;
                }
            }
        }
    }
}