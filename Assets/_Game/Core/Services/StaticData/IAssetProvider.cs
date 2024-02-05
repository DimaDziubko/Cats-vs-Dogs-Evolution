using System.Threading.Tasks;
using _Game.Gameplay._Unit.Scripts;
using _Game.Gameplay.Battle.Scripts;
using UnityEngine;

namespace _Game.Core.Services.StaticData
{
    public interface IAssetProvider
    {
        Task LoadEnemiesAsync(string assetKey);
        Task LoadBattlesAsync(string assetKey);
        Task LoadUnitsAsync(string assetKey);
        BattleAsset ForBattle(in int currentBattleIndex);
        UnitAsset ForUnit(in int index);
        EnemyAsset ForEnemy(in int index);
        Sprite[] ForUnitIcons();
    }
}