using System.Collections.Generic;
using UnityEngine;

namespace Logic.Units
{
    public class RagdollController : MonoBehaviour
    {
        [SerializeField] private List<Rigidbody> _ragdollRigidbodies;
        [SerializeField] private List<Collider> _ragdollColliders;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _hipsTransform;
        [SerializeField] private Animator _animator;

        private Vector3 _defaultHipsPosition;
        private bool _inRagdollState;
        
        private void Awake()
        {
            _defaultHipsPosition = _hipsTransform.localPosition;
        }

        public void SetRagdollStatus(bool isEnabled)
        {
            if (_inRagdollState && !isEnabled)
            {
                var position = _hipsTransform.position;
                transform.position = position;
                _hipsTransform.localPosition = _defaultHipsPosition;
            }
            
            _rigidbody.isKinematic = isEnabled;
            _collider.enabled = !isEnabled;
            _animator.enabled = !isEnabled;
            
            for (int i = 0; i < _ragdollRigidbodies.Count && i < _ragdollColliders.Count; i++)
            {
                _ragdollRigidbodies[i].isKinematic = !isEnabled;
                _ragdollColliders[i].enabled = isEnabled;
            }
            
            _inRagdollState = isEnabled;
        }
    }
}