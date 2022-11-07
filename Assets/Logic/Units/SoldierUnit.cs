using System;
using Logic.Units.Weapon;
using UnityEngine;

namespace Logic.Units
{
    public class SoldierUnit : Unit
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _model;
        [SerializeField] private WeaponHolder _weapon;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Transform _rightHand;

        private Transform _shoulder;
        private Transform _aimPivot;
        private Transform _right;
        private Transform _left;

        private Quaternion _leftHandRotation;

        private void Start()
        {
            _shoulder = _animator.GetBoneTransform(HumanBodyBones.RightShoulder);

            _aimPivot = new GameObject().transform;
            _aimPivot.name = "aim_pivot";
            _aimPivot.parent = transform;

            _right = new GameObject().transform;
            _right.name = "right";
            _right.transform.parent = _aimPivot;
            
            _left = new GameObject().transform;
            _left.name = "left";
            _left.transform.parent = _aimPivot;

            _right.localPosition = _weapon.PointForRightHand.localPosition;
            var rotation = Quaternion.Euler(_weapon.PointForRightHand.position);
            _right.localRotation = rotation;
        }

        private void Update()
        {
            _leftHandRotation = _left.rotation;
            _left.position = _weapon.PointForLeftHand.position;

            _aimPivot.position = _shoulder.position;
            
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _left.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandRotation);

            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _right.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _right.rotation); 


            // var direction = _leftHand.position - _rightHand.position;
            // _weapon.transform.position = _rightHand.transform.position;
            // _weapon.transform.rotation = Quaternion.LookRotation(direction);
        }

        public override void LookInDirection(Vector3 direction)
        {
            var rotation = Quaternion.LookRotation(direction);
            _model.rotation = Quaternion.Lerp(_model.rotation, rotation, Time.deltaTime * 5);
        }
    }
}