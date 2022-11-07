using UnityEngine;

namespace Logic.Units.Weapon
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;

        public Rigidbody Rigidbody => _rigidbody;

        public void SetBulletColor(Color color)
        {
            _meshRenderer.material.color = color;
            _trailRenderer.startColor = color;
        }
    }
}