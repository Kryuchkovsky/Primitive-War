using System;
using Logic.Units.Weapon;
using UnityEngine;

namespace Logic.Units
{
    public class SoldierUnit : Unit
    {
        [SerializeField] private RagdollController _ragdollController;
        [SerializeField] private Transform _shoulder;
        [SerializeField] private Vector3 _offset;

        public RagdollController RagdollController => _ragdollController;
        public override KineticWeaponType KineticWeaponType => KineticWeaponType.AK74;

        public override void LookInDirection(Vector3 direction)
        {
            _shoulder.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(_offset);
        }
    }
}