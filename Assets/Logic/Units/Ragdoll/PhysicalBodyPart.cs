using System;
using UnityEngine;

namespace Logic.Units.Ragdoll
{
    [RequireComponent(typeof(ConfigurableJoint))]
    public class PhysicalBodyPart : MonoBehaviour
    {
        [SerializeField] private Transform _targetBodyPart;
        [SerializeField] private ConfigurableJoint _joint;

        private Quaternion _startRotation;

        public Transform TargetBodyPart
        {
            get => _targetBodyPart;
            set => _targetBodyPart = value;
        }

        private void Awake()
        {
            _startRotation = transform.localRotation;

            if (!_joint)
            {
                FindComponents();
            }
        }

        private void FixedUpdate()
        {
            _joint.targetRotation = Quaternion.Inverse(_targetBodyPart.localRotation) * _startRotation;
        }

        public void FindComponents()
        {
            _joint = GetComponent<ConfigurableJoint>();
            _joint.xMotion = ConfigurableJointMotion.Locked;
            _joint.yMotion = ConfigurableJointMotion.Locked;
            _joint.zMotion = ConfigurableJointMotion.Locked;
        }
    }
}