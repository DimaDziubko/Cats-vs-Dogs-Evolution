using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Vfx.Factory;
using Assets._Game.Gameplay._Bases.Factory;

namespace Assets._Game.Core.Factory
{
    public class FactoriesHolder : IFactoriesHolder
    {
        public IUnitFactory UnitFactory { get; }
        public ICoinFactory CoinFactory { get; }
        public IVfxFactory VfxFactory { get; }
        public IBaseFactory BaseFactory { get; }
        public IProjectileFactory ProjectileFactory { get; }
        public IEnumerable<GameObjectFactory> Factories { get; }

        public FactoriesHolder(
            IUnitFactory unitFactory,
            ICoinFactory coinFactory,
            IVfxFactory vfxFactory,
            IBaseFactory baseFactory,
            IProjectileFactory projectileFactory)
        {
            UnitFactory = unitFactory;
            CoinFactory = coinFactory;
            VfxFactory = vfxFactory;
            BaseFactory = baseFactory;
            ProjectileFactory = projectileFactory;
            
            Factories = new[]
            {
                unitFactory as GameObjectFactory, 
                baseFactory as GameObjectFactory, 
                projectileFactory as GameObjectFactory, 
                coinFactory as GameObjectFactory, 
                vfxFactory as GameObjectFactory, 
            };
        }
    }
}
