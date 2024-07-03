using Assets._Game.Core.Configs.Repositories;
using Assets._Game.Core.Data;
using Assets._Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._Game.Core.DataProviders.Common
{
    public interface ICommonItemsDataProvider
    {
        UniTask<DataPool<Race,Sprite>> LoadFoodIcons(ICommonItemsConfigRepository itemsConfigRepository, int context);
        UniTask<Sprite> LoadTowerIcon(ICommonItemsConfigRepository itemsConfigRepository, int context);
    }
}