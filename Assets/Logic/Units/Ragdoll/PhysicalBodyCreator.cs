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
                _partsOfAnimatedBody = _animatedBody.GetComponentsInChildren<Transform>().Select(x => x.gameObject).ToArray();
                _partsOfPhysicalBody = _physicalBody.GetComponentsInChildren<Transform>().Select(x => x.gameObject).ToArray();

                if (_physicalBody.TryGetComponent(out Animator animator))
                {
                    DestroyImmediate(animator);
                }

                for (int i = 0; i < _partsOfAnimatedBody.Length && i < _partsOfPhysicalBody.Length; i++)
                {
                    var components = _partsOfPhysicalBody[i].GetComponents<PhysicalBodyPart>();
                        
                    for (int j = 0; j < components.Length; j++)
                    {
                        DestroyImmediate(components[j]);
                    }
                    
                    if (_partsOfPhysicalBody[i].TryGetComponent(out CharacterJoint joint))
                    {
                        DestroyImmediate(joint);
                    }

                    if (_partsOfPhysicalBody[i].TryGetComponent(out Rigidbody _))
                    {
                        var physicalBodyPart = _partsOfPhysicalBody[i].AddComponent<PhysicalBodyPart>();
                        physicalBodyPart.FindComponents();
                        physicalBodyPart.TargetBodyPart = _partsOfAnimatedBody[i].transform;
                    }
                }
            }
            else
            {
                throw new Exception("Failed to create physical body.");
            }
        }

        private bool CanCreatePhysicalBody()
        {
            if (!_animatedBody || !_physicalBody) return false;

            return _animatedBody.transform.parent == transform && _physicalBody.transform.parent == transform;
        }
    }
}