using System.Collections.Generic;
using System.Threading.Tasks;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Gameplay.Battle.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Services.StaticData
{
    public interface IAssetProvider
    {
        UniTask<GameObject> Instantiate(string path);
        UniTask<GameObject> Instantiate(string path, Vector3 at);
        UniTask<T> Load<T>(AssetReference assetReference) where T : class;
        UniTask<T> Load<T>(string address) where T : class;
        void CleanUp();
        void Initialize();
        UniTask<GameObject> Instantiate(string address, Transform under);
    }
}