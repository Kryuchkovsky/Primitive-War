using System;
using UnityEngine;

namespace Logic.Units.Weapon.Bullets
{
    public class Bullet : MonoBehaviour
    {
        public event Action<Collision> OnCollide; 

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;

        public Rigidbody Rigidbody => _rigidbody;

        public void SetBulletColor(Color color)
        {
            _meshRenderer.material.color = color;
            _trailRenderer.startColor = color;
        }

        public void OnCollisionEnter(Collision collision)
        {
            OnCollide?.Invoke(collision);
        }
    }
}