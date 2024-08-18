using _Game.Utils._Addressables;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Repositories.Common
{
    public interface ICommonItemsConfigRepository
    {
        public string GetFoodIconKey(Race race);
        public string GetBaseIconKey();
    }
}