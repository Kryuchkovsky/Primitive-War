using Leopotam.EcsLite;

namespace Logic.Units.Behaviour
{
    public struct HealthComponent
    {
        public float HealthPoints;
    }

    public struct DamageComponent
    {
        public EcsPackedEntity DamagedEntity;
        public float Damage;
    }
}