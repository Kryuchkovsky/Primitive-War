using Logic.Units.Weapon;
using UnityEngine;

namespace Logic.Units
{
    public class SoldierUnit : Unit
    {
        [SerializeField] private RagdollController _ragdollController;
        [SerializeField] private Animator _animator;

        private Transform _shoulder;
        private Quaternion _gunTargetRotation;
        private Vector3 _lookDirection;
        private Vector3 _gunTargetPosition;

        public RagdollController RagdollController => _ragdollController;
        public override KineticWeaponType KineticWeaponType => KineticWeaponType.AK74;

        public override void LookInDirection(Vector3 direction)
        {
            _lookDirection = direction;
            _shoulder ??= _animator.GetBoneTransform(HumanBodyBones.RightShoulder);
            _gunTargetPosition = _shoulder.position + _lookDirection;
            _gunTargetRotation = Quaternion.LookRotation(_lookDirection);
        }
        
        private void OnAnimatorIK()
        {
            _animator.SetLookAtWeight(1, 0, 1);
            _animator.SetLookAtPosition(_lookDirection);
            
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _gunTargetPosition);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _gunTargetRotation);
        }
    }
}