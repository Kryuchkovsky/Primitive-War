using System;
using System.Collections.Generic;
using Logic.Teams;
using UnityEngine;
using UnityEngine.AI;

namespace Logic.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] protected List<UnitRenderer> _unitRenderers;
        [SerializeField] protected NavMeshAgent _navNavMeshAgent;
        [SerializeField] protected Transform _shootPoint;

        public NavMeshAgent NavMeshAgent => _navNavMeshAgent;
        public Transform ShootPoint => _shootPoint;
        public TeamComponent TeamComponent { get; set; }

        public abstract void LookInDirection(Vector3 direction);
        
        public void SetColor(Color color)
        {
            foreach (var renderer in _unitRenderers)
            {
                var materials = renderer.Renderer.materials;

                foreach (var index in renderer.MaterialIndexes)
                {
                    if (index < materials.Length && materials[index])
                    {
                        materials[index].color = color;
                    }
                }
            }
        }
    }
    
    [Serializable]
    public class UnitRenderer
    {
        public int[] MaterialIndexes = {0};
        public Renderer Renderer;
    }
}
