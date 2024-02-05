using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Game.Core.Services.StaticData;
using _Game.Gameplay._Unit.Scripts;
using _Game.Gameplay.Battle.Scripts;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Game.StaticData
{
    public class AssetProvider : IAssetProvider
    {
        private readonly Dictionary<int, UnitAsset> _units = new Dictionary<int, UnitAsset>();
        private readonly Dictionary<int, BattleAsset> _battles = new Dictionary<int, BattleAsset>();
        private readonly Dictionary<int, EnemyAsset> _enemies = new Dictionary<int, EnemyAsset>();
        
        public async Task LoadEnemiesAsync(string assetKey)
        {
            AsyncOperationHandle<BattleEnemyAsset> handle = Addressables.LoadAssetAsync<BattleEnemyAsset>(assetKey);
            
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                BattleEnemyAsset enemyAssets = handle.Result;
                foreach (var asset in enemyAssets.Assets)
                {
                    _enemies[asset.Id] = asset;
                }
                
                Addressables.Release(handle);
            }
            else
            {
                Debug.LogError($"Failed to load unit assets with key {assetKey}");
            }
        }
        
        public async Task LoadUnitsAsync(string assetKey)
        {
            AsyncOperationHandle<AgeUnitAsset> handle = Addressables.LoadAssetAsync<AgeUnitAsset>(assetKey);
            
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AgeUnitAsset unitAssets = handle.Result;
                foreach (var asset in unitAssets.Assets)
                {
                    _units[asset.Id] = asset;
                }
                
                Addressables.Release(handle);
            }
            else
            {
                Debug.LogError($"Failed to load unit assets with key {assetKey}");
            }
        }
        
        
        public async Task LoadBattlesAsync(string assetKey)
        {
            AsyncOperationHandle<GeneralBattleAsset> handle = Addressables.LoadAssetAsync<GeneralBattleAsset>(assetKey);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GeneralBattleAsset battleAssets = handle.Result;
                foreach (var asset in battleAssets.Assets)
                {
                    _battles[asset.Id] = asset;
                }
                
                Addressables.Release(handle);
            }
            else
            {
                Debug.LogError($"Failed to load battle assets with key {assetKey}");
            }
        }
        
        public BattleAsset ForBattle(in int index) =>
            _battles.TryGetValue(index, out BattleAsset battleAsset) 
                ? battleAsset
                : null;
        
        public UnitAsset ForUnit(in int index) =>
            _units.TryGetValue(index, out UnitAsset unitAsset) 
                ? unitAsset
                : null;
        
        public EnemyAsset ForEnemy(in int index) =>
            _enemies.TryGetValue(index, out EnemyAsset enemyAsset) 
                ? enemyAsset
                : null;

        public Sprite[] ForUnitIcons() => _units.Values.Select(asset => asset.Icon).ToArray();

        private void Cleanup()
        {
            _units.Clear();
            _battles.Clear();
            _enemies.Clear();
        }
    }
}