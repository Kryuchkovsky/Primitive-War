using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Units.Weapon.Bullets;
using UnityEngine;

namespace Logic.Units.Weapon
{
    public sealed class WeaponControlSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;
        
        private EcsPool<KineticWeaponComponent> _kineticWeaponComponents;
        private EcsPool<WeaponReloadComponent> _weaponReloadComponents;
        private EcsPool<BulletSpawnRequestComponent> _bulletSpawnRequestComponents;
        
        private EcsFilter _weaponComponentsfilter;

        public void Init(IEcsSystems systems)
        {
            _kineticWeaponComponents = _world.Value.GetPool<KineticWeaponComponent>();
            _weaponReloadComponents = _world.Value.GetPool<WeaponReloadComponent>();
            _bulletSpawnRequestComponents = _world.Value.GetPool<BulletSpawnRequestComponent>();

            _weaponComponentsfilter = _world.Value.Filter<KineticWeaponComponent>().Inc<WeaponReloadComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _weaponComponentsfilter)
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
            weaponReloadComponent.ShotsBeforeReload -= 1;
            weaponReloadComponent.ReloadTime = weaponReloadComponent.ShotsBeforeReload > 0
                ? kineticWeaponComponent.Data.ReloadTimeBetweenShots
                : kineticWeaponComponent.Data.MainReloadTime;

            var entity = _world.Value.NewEntity();
            ref var bulletSpawnRequestComponent = ref _bulletSpawnRequestComponents.Add(entity);
            bulletSpawnRequestComponent.Prefab = kineticWeaponComponent.Data.Bullet;
            bulletSpawnRequestComponent.ShotPoint = kineticWeaponComponent.ShotPoint;
            bulletSpawnRequestComponent.Data = kineticWeaponComponent.Data.BulletData;
        }
    }
}