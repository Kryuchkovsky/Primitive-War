using UnityEngine;

namespace Logic.Units.Weapon.Bullets
{
    public struct BulletComponent
    {
        public Bullet Bullet;
        public BulletData Data;
        public float TraveledDistance;
    }

    public struct BulletCollisionComponent
    {
        public Collision Collision;
    }
}