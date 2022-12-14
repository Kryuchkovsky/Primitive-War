using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Mechanics.Explosions;
using UnityEngine;

namespace Logic.Units.Weapon.Bullets
{
    public sealed class BulletsDestructionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;
        
        private EcsPool<BulletComponent> _bulletComponents;
        private EcsPool<BulletCollisionComponent> _bulletCollisionComponents;
        private EcsPool<ExplosionComponent> _explosionComponents;

        private EcsFilter _bulletComponentsFilter;
        private EcsFilter _bulletCollisionComponentsFilter;

        public void Init(IEcsSystems systems)
        {
            _bulletComponents = _world.Value.GetPool<BulletComponent>();
            _bulletCollisionComponents = _world.Value.GetPool<BulletCollisionComponent>();
            _explosionComponents = _world.Value.GetPool<ExplosionComponent>();
            
            _bulletComponentsFilter = _world.Value.Filter<BulletComponent>().Exc<BulletCollisionComponent>().End();
            _bulletCollisionComponentsFilter = _world.Value.Filter<BulletComponent>().Inc<BulletCollisionComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
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

            foreach (var entity in _bulletCollisionComponentsFilter)
            {
                ref var bulletComponent = ref _bulletComponents.Get(entity);
                ref var bulletCollisionComponent = ref _bulletCollisionComponents.Get(entity);

                if (bulletComponent.Data.ExplosionRange > 0)
                {
                    var explosionEntity = _world.Value.NewEntity();
                    ref var explosionComponent =  ref _explosionComponents.Add(explosionEntity);
                    explosionComponent.LayerMask = bulletComponent.Data.LayerMask;
                    explosionComponent.Position = bulletComponent.Bullet.transform.position;
                    explosionComponent.Radius = bulletComponent.Data.ExplosionRange;
                    explosionComponent.Force = bulletComponent.Data.ExplosionForce;
                    explosionComponent.Damage = bulletComponent.Data.Damage;
                }
                else
                {
                    if (bulletCollisionComponent.Collision != null && bulletCollisionComponent.Collision.collider.TryGetComponent(out Unit unit))
                    {
                        unit.TakeDamage(bulletComponent.Data.Damage);
                    }
                }
                
                Object.Destroy(bulletComponent.Bullet.gameObject);
                _world.Value.DelEntity(entity);
            }
        }
    }
}