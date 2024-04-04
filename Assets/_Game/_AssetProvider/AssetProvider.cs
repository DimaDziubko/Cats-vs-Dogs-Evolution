﻿using System.Collections.Generic;
using _Game.Core.Services.AssetProvider;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Game._AssetProvider
{
    public class AssetProvider : IAssetProvider
    {
        
        private readonly Dictionary<string, AsyncOperationHandle> _completeCache = new Dictionary<string, AsyncOperationHandle>();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new Dictionary<string, List<AsyncOperationHandle>>();

        public void Init()
        {
            Addressables.InitializeAsync();
        }
        
        public UniTask<GameObject> Instantiate(string address) => 
            Addressables.InstantiateAsync(address).ToUniTask();

        public UniTask<GameObject> Instantiate(string address, Vector3 at) => 
            Addressables.InstantiateAsync(address, at, Quaternion.identity).ToUniTask();
        
        public UniTask<GameObject> Instantiate(string address, Transform under) => 
            Addressables.InstantiateAsync(address, under).ToUniTask();
        public async UniTask<T> Load<T>(AssetReference assetReference) where T : class
        {
            if (_completeCache.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle completeHandle))
                return completeHandle.Result as T;

            return await RunWithCacheOnComplete(
                Addressables.LoadAssetAsync<T>(assetReference),
                cacheKey: assetReference.AssetGUID);
        }

        public async UniTask<T> Load<T>(string address) where T : class
        {
            if (_completeCache.TryGetValue(address, out AsyncOperationHandle completeHandle))
                return completeHandle.Result as T;

            return await RunWithCacheOnComplete(
                Addressables.LoadAssetAsync<T>(address),
                cacheKey: address);
        }

        public void Release(string key)
        {
            if (_completeCache.TryGetValue(key, out AsyncOperationHandle handle) && handle.IsValid())
            {
                Addressables.Release(handle);
                _completeCache.Remove(key);
            }

            if (_handles.TryGetValue(key, out List<AsyncOperationHandle> handlesList))
            {
                foreach (var opHandle in handlesList)
                {
                    if(opHandle.IsValid())
                    {
                        Addressables.Release(opHandle);
                    }
                }
                _handles.Remove(key);
            }
        }
        
        
        //TODO Choose place
        public void CleanUp()
        {
            foreach (List<AsyncOperationHandle> resourcesHandles in _handles.Values)
            foreach (AsyncOperationHandle handle in resourcesHandles)
            {
                Addressables.Release(handle);
            }  
            
            _completeCache.Clear();
            _handles.Clear();
        }

        private void AddHandler<T>(string key, AsyncOperationHandle<T> handle) where T : class
        {
            if(!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
            {
                resourceHandles = new List<AsyncOperationHandle>();
                _handles[key] = resourceHandles;
            }
            
            resourceHandles.Add(handle);
        }

        private async UniTask<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey) where T : class
        {
            handle.Completed += completeHandle =>
            {
                _completeCache[cacheKey] = completeHandle;
            };

            AddHandler(cacheKey, handle);
            return await handle.Task;
        }
    }
}