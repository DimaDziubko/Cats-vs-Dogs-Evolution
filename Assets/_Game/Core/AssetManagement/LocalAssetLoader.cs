using System;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets._Game.Core.AssetManagement
{
    public class LocalAssetLoader
    {
        private GameObject _cacheObject;

        public async UniTask<T> Load<T>(string assetId, Transform parent = null)
        {
            var handle = Addressables.InstantiateAsync(assetId, parent);
            _cacheObject = await handle.Task;
            if (_cacheObject.TryGetComponent(out T component) == false)
            {
                throw new NullReferenceException($"Object of type {typeof(T)} is null on " + 
                                                 "attempt to load it from addressables");
            }

            return component;
        }

        public async UniTask<Disposable<T>> LoadDisposable<T>(string assetId, Transform parent = null)
        {
            var component = await Load<T>(assetId, parent);
            return Disposable<T>.Borrow(component, _ => Unload());
        }

        public void Unload()
        {
            if (_cacheObject == null)
            {
                return;
            }

            _cacheObject.SetActive(false);

            Addressables.ReleaseInstance(_cacheObject);
            _cacheObject = null;
        }
    }
}
