using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine.AddressableAssets;

namespace _Game.Core.DataProviders.BaseDataProvider
{
    public class BaseLoadOptions
    {
        public Faction Faction;
        public int Timeline;
        public int CacheContext;
        public AssetReferenceGameObject BasePrefab;
        public float CoinsAmount;
    }
}