using Logic.Units.Weapon;
using UnityEngine;

namespace Logic.Units
{
    public class SoldierUnit : Unit
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _aimPivot;
        [SerializeField] private Transform _rightPoint;
        [SerializeField] private Transform _leftPoint;

        private Transform _shoulder;
        private Quaternion _leftHandRotation;
        private Vector3 _lookDirection;
        
        public override KineticWeaponType KineticWeaponType => KineticWeaponType.AK74;

        private void Start()
        {
            _shoulder = _animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        }

        private void OnAnimatorIK()
        {
            _animator.SetLookAtWeight(1, 0, 1);
            _animator.SetLookAtPosition(_lookDirection);
            
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftPoint.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandRotation);

            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightPoint.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightPoint.rotation); 
        }

        public override void LookInDirection(Vector3 direction)
        {
            _lookDirection = direction;
            _leftHandRotation = _leftPoint.rotation;
            _leftPoint.position = _manualWeapon.PointForLeftHand.position;
            _aimPivot.position = _shoulder.position;
            _aimPivot.rotation = Quaternion.LookRotation(_lookDirection);
        }
    }
}