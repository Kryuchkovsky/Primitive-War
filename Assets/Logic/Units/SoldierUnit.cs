using UnityEngine;

namespace Logic.Units
{
    public class SoldierUnit : Unit
    {
        [SerializeField] private Transform _model;
        
        public override void LookInDirection(Vector3 direction)
        {
            var rotation = Quaternion.LookRotation(direction);
            _model.rotation = Quaternion.Lerp(_model.rotation, rotation, Time.deltaTime * 5);
        }
    }
}