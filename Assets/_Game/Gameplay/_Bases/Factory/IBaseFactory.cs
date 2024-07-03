using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Gameplay._Bases.Factory
{
    public interface IBaseFactory
    {
        Base GetBase(Faction faction);
        public void Reclaim(Base @base);
    }
}