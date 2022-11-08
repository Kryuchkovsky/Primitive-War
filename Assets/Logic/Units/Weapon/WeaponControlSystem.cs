using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Logic.Units.Weapon
{
    public sealed class WeaponControlSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;
        
        private EcsPool<KineticWeaponComponent> _kineticWeaponComponents;
        private EcsPool<WeaponReloadComponent> _weaponReloadComponents;
        private EcsPool<BulletComponent> _bulletComponents;

        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _kineticWeaponComponents = _world.Value.GetPool<KineticWeaponComponent>();
            _weaponReloadComponents = _world.Value.GetPool<WeaponReloadComponent>();
            _bulletComponents = _world.Value.GetPool<BulletComponent>();
            _filter = _world.Value.Filter<KineticWeaponComponent>().Inc<WeaponReloadComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var kineticWeaponComponent = ref _kineticWeaponComponents.Get(entity);
                ref var weaponReloadComponent = ref _weaponReloadComponents.Get(entity);

                if (kineticWeaponComponent.IsShooting && weaponReloadComponent.ShotsBeforeReload == 0)
                {
                    weaponReloadComponent.ShotsBeforeReload = kineticWeaponComponent.Data.Shots;
                }
                
                if (weaponReloadComponent.ShotsBeforeReload > 0)
                {
                    if (weaponReloadComponent.ReloadTime > 0)
                    {
                        weaponReloadComponent.ReloadTime -= Time.deltaTime;
                    }
                    else
                    {
                        MakeShot(kineticWeaponComponent, ref weaponReloadComponent);
                    }
                }
            }
        }

        private void MakeShot(KineticWeaponComponent kineticWeaponComponent, ref WeaponReloadComponent weaponReloadComponent)
        {
            var rotation = Quaternion.LookRotation(kineticWeaponComponent.ShotPoint.forward);
            var bullet = Object.Instantiate(
                kineticWeaponComponent.Data.Bullet,
                kineticWeaponComponent.ShotPoint.position,
                rotation);
            weaponReloadComponent.ShotsBeforeReload -= 1;
            weaponReloadComponent.ReloadTime = weaponReloadComponent.ShotsBeforeReload > 0
                ? kineticWeaponComponent.Data.ReloadTimeBetweenShots
                : kineticWeaponComponent.Data.MainReloadTime;

            var bulletEntity = _world.Value.NewEntity();
            ref var bulletComponent = ref _bulletComponents.Add(bulletEntity);
            bulletComponent.Bullet = bullet;
            bulletComponent.Data = kineticWeaponComponent.Data.BulletData;
        }
    }
}