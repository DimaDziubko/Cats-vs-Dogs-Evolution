using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Bases.Factory
{
    public interface IBaseFactory
    {
        Base GetBase(Faction faction);
        public void Reclaim(Base @base);
    }
}