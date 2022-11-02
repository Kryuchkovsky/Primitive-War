using System;
using System.Linq;
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
        public void CreatePhysicalBody()
        {
            if (CanCreatePhysicalBody())
            {
                _physicalBody = Instantiate(_animatedBody, _animatedBody.transform.position, _animatedBody.transform.rotation, transform);
                _physicalBody.name = "PhysicalBody";
                _partsOfPhysicalBody = _physicalBody.GetComponentsInChildren<Transform>().Select(x => x.gameObject).ToArray();

                if (_physicalBody.TryGetComponent(out Animator animator))
                {
                    DestroyImmediate(animator);
                }

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
            if (!_animatedBody) return false;

            if (_physicalBody)
            {
                DestroyImmediate(_physicalBody.gameObject);
            }

            _partsOfAnimatedBody = _animatedBody.GetComponentsInChildren<Transform>().Select(x => x.gameObject).ToArray();

            foreach (var part in _partsOfAnimatedBody)
            {
                if (gameObject == part) return false;
            }
            
            return _animatedBody.transform.parent == transform;
        }
    }
}