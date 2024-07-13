using _Game.Core.DataProviders.Facade;
using Assets._Game.Core.Configs.Repositories;
using Assets._Game.Core.Data;
using Assets._Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Common
{
    public interface ICommonItemsDataProvider
    {
        UniTask<DataPool<Race,Sprite>> LoadFoodIcons(ICommonItemsConfigRepository itemsConfigRepository, LoadContext context);
        UniTask<Sprite> LoadBaseIcon(ICommonItemsConfigRepository itemsConfigRepository, LoadContext context);
    }
}