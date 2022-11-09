using UnityEngine;

namespace Logic.Units.Weapon
{
    public struct KineticWeaponComponent
    {
        public Transform ShotPoint;
        public KineticWeaponData Data;
        public bool IsShooting;
    }
}