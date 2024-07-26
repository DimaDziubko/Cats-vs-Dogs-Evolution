using Assets._Game.Gameplay.Common.Scripts;

namespace _Game.Core.Configs.Repositories.Common
{
    public interface ICommonItemsConfigRepository
    {
        public string GetFoodIconKey(Race race);
        public string GetBaseIconKey();
    }
}