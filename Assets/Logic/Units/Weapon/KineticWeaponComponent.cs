using UnityEngine;

namespace Logic.Units.Weapon
{
    public struct KineticWeaponComponent
    {
        public Transform ShotPoint;
        public KineticWeaponData Data;
        public bool IsShooting;
    }

    public struct WeaponReloadComponent
    {
        public float ShotsBeforeReload;
        public float ReloadTime;
    }

    public struct BulletComponent
    {
        public Bullet Bullet;
        public BulletData Data;
        public float TraveledDistance;
    }
}