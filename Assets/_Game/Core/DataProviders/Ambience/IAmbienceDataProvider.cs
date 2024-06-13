using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Ambience
{
    public interface IAmbienceDataProvider
    {
        UniTask<AudioClip> Load(string key, int cacheContext);
    }
}