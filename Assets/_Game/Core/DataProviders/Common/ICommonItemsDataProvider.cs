using _Game.Core.Configs.Repositories;
using _Game.Core.Data;
using _Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Common
{
    public interface ICommonItemsDataProvider
    {
        UniTask<DataPool<Race,Sprite>> LoadFoodIcons(ICommonItemsConfigRepository itemsConfigRepository, int context);
        UniTask<Sprite> LoadTowerIcon(ICommonItemsConfigRepository itemsConfigRepository, int context);
    }
}