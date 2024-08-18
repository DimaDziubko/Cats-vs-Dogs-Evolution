using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.UnitDataProviders;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Assets._Game.Utils.Extensions;

namespace _Game.Core.DataProviders.UnitUpgradeDataProvider
{
    public class UnitDataProvider : IUnitDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public UnitDataProvider(IMyLogger logger)
        {
            _logger = logger;
        }

        public UnitData LoadUnitData(UnitLoadOptions options)
        {
            Race race;
            switch (options.Faction)
            {
                case Faction.Player:
                    race = options.CurrentRace;
                    break;
                case Faction.Enemy:
                    race = options.CurrentRace == Race.Cat ? Race.Dog : Race.Cat;
                    break;
                default:
                    race = options.CurrentRace;
                    break;
            }

            _logger.Log($"Unit with id {options.Config.Id} load successfully");
            
            return new UnitData
            {
                Config = options.Config,
                Race = race,
                UnitLayer = options.Config.GetUnitLayerForFaction(options.Faction),
                AggroLayer = options.Config.GetAggroLayerForFaction(options.Faction),
                AttackLayer = options.Config.GetAttackLayerForFaction(options.Faction),
            };
        }
    }
}