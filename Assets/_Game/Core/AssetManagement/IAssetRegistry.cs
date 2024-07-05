    using Cysharp.Threading.Tasks;

    namespace Assets._Game.Core.AssetManagement
{
    public interface IAssetRegistry
    {
        UniTask<T> LoadAsset<T>(string key, int context) where T : class;
        void ClearContext(int context);
    }
}