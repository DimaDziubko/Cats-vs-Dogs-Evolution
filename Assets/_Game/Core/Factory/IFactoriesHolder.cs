using System.Collections.Generic;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Vfx.Factory;
using Assets._Game.Gameplay._Bases.Factory;
using Assets._Game.Gameplay._Coins.Factory;
using Assets._Game.Gameplay._Units.Factory;
using Assets._Game.Gameplay.Vfx.Factory;

namespace Assets._Game.Core.Factory
{
    public interface IFactoriesHolder
    {
        public IUnitFactory UnitFactory { get; }
        public ICoinFactory CoinFactory { get; }
        public IVfxFactory VfxFactory { get; }
        public IBaseFactory BaseFactory { get; }
        public IProjectileFactory ProjectileFactory { get; }
        public IEnumerable<GameObjectFactory> Factories { get; }
    }
}