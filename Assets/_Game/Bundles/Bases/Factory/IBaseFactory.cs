using _Game.Bundles.Bases.Scripts;

namespace _Game.Bundles.Bases.Factory
{
    public interface IBaseFactory
    {
        Base GetPlayerBase();
        Base GetEnemyBase();
        public void Reclaim(Base @base);
    }
}