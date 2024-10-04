using _Game.Core.DataProviders.Facade;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Ambience
{
    public interface IAmbienceDataLoader
    {
        UniTask<AudioClip> Load(string key, LoadContext cacheContext);
    }
}