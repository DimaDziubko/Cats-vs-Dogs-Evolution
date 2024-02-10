namespace _Game.Bundles.Units.Common.Scripts
{
    public interface IDamageable
    {
        bool IsDead { get; }
        void GetDamage(float damage);
    }
}