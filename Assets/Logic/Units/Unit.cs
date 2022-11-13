using System;
using System.Collections.Generic;
using Logic.Teams;
using Logic.Units.Weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Logic.Units
{
    public abstract class Unit : MonoBehaviour
    {
        public event Action<float> OnTakeDamage; 

        [SerializeField] protected List<UnitRenderer> _unitRenderers;
        [SerializeField] protected Renderer _renderer;
        [SerializeField] protected NavMeshAgent _navNavMeshAgent;
        [SerializeField] protected ManualWeaponHolder _manualWeapon;

        public NavMeshAgent NavMeshAgent => _navNavMeshAgent;
        public ManualWeaponHolder ManualWeaponHolder => _manualWeapon;
        public TeamComponent TeamComponent { get; set; }
        public abstract KineticWeaponType KineticWeaponType { get; }

        public abstract void LookInDirection(Vector3 direction);
        
        public void SetColor(Color color)
        {
            return;
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

        public void TakeDamage(float damage) => OnTakeDamage?.Invoke(damage);
    }
    
    [Serializable]
    public class UnitRenderer
    {
        public int[] MaterialIndexes = {0};
        public Renderer Renderer;
    }
}
