using UnityEngine;

namespace Logic.Units
{
    public class TankUnit : Unit
    {
        [SerializeField] private Transform _tower;
        [SerializeField] private Transform _gun;

        public override void LookInDirection(Vector3 direction)
        {
            var towerRotation = Quaternion.LookRotation(direction);
            towerRotation.y = 0;
            _tower.rotation = Quaternion.RotateTowards(_tower.rotation, towerRotation, 15);
            
            var gunRotation = Quaternion.LookRotation(direction);
            gunRotation.x = 0;
            gunRotation.z = 0;
            _gun.rotation = Quaternion.RotateTowards(_gun.rotation, gunRotation, 15);
        }
    }
}