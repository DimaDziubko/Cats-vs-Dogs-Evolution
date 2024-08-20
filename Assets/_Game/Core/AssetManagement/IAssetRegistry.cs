using Cysharp.Threading.Tasks;
    using UnityEngine.AddressableAssets;

    namespace _Game.Core.AssetManagement
{
    public interface IAssetRegistry
    {
        UniTask<T> LoadAsset<T>(AssetReference assetReference, int timeline, int context) where T : class;
        UniTask<T> LoadAsset<T>(string key, int timeline, int context) where T : class;
        void ClearContext(int timeline, int context);
        void ClearTimeline(int timeline);
        UniTask Warmup<T>(AssetReference configCatIconAtlas) where T : class;
        UniTask Warmup<T>(string address) where T : class;
    }
}