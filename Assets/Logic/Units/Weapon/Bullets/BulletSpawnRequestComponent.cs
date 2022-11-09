using UnityEngine;

namespace Logic.Units.Weapon.Bullets
{
    public struct BulletSpawnRequestComponent
    {
        public Bullet Prefab;
        public Transform ShotPoint;
        public BulletData Data;
    }
}