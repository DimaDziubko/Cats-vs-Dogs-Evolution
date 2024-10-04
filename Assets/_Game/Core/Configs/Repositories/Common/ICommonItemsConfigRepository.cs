using _Game.Gameplay._Boosts.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine;

namespace _Game.Core.Configs.Repositories.Common
{
    public interface ICommonItemsConfigRepository
    {
        public string GetFoodIconKey(Race race);
        public string GetBaseIconKey();
        Sprite ForFoodIcon(Race race);
        Sprite ForBaseIcon();
        Sprite ForBoostIcon(BoostType boostType);
        Sprite GetUnitAttackIconFor(Race race);
        Sprite GetUnitHealthIconFor(Race unitDataRace);
    }
}