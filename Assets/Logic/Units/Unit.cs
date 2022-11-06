using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Logic.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] protected List<UnitRenderer> _unitRenderers;
        [SerializeField] protected NavMeshAgent _navNavMeshAgent;

        public NavMeshAgent NavMeshAgent => _navNavMeshAgent;

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
