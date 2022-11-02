using System;
using UnityEngine;

namespace Logic.Units.Ragdoll
{
    public class PhysicalBodyCreator : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private GameObject[] _partsOfAnimatedBody;
        [SerializeField] [HideInInspector] private GameObject[] _partsOfPhysicalBody;
        
        [SerializeField] private GameObject _animatedBody;
        [SerializeField] private GameObject _physicalBody;

        [ContextMenu("Create physical body")]
        public void CreatePhysicalBody(GameObject body)
        {
            if (_physicalBody)
            {
                DestroyImmediate(_physicalBody.gameObject);
            }
            
            if (CanCreatePhysicalBody())
            {
                _physicalBody = Instantiate(_animatedBody, transform);
                _partsOfPhysicalBody = _physicalBody.GetComponentsInChildren<GameObject>();

                for (int i = 0; i < _partsOfAnimatedBody.Length && i < _partsOfPhysicalBody.Length; i++)
                {
                    var physicalBodyPart = _partsOfPhysicalBody[i].AddComponent<PhysicalBodyPart>();
                    physicalBodyPart.FindComponents();
                    physicalBodyPart.TargetBodyPart = _partsOfAnimatedBody[i].transform;
                }
            }
            else
            {
                throw new Exception("Failed to create physical body.");
            }
        }

        private bool CanCreatePhysicalBody()
        {
            if (!_animatedBody || _physicalBody) return false;

            _partsOfAnimatedBody = _animatedBody.GetComponentsInChildren<GameObject>();

            foreach (var part in _partsOfAnimatedBody)
            {
                if (gameObject == part) return false;
            }
            
            return _animatedBody.transform.parent == transform;
        }
    }
}