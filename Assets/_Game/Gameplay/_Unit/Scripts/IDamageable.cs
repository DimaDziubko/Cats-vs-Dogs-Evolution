namespace _Game.Gameplay._Unit.Scripts
{
    public interface IDamageable
    {
        bool IsDead { get; }
        void GetDamage(float damage);
    }
}