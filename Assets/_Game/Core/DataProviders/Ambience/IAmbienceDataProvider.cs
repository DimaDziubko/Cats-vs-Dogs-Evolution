using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._Game.Core.DataProviders.Ambience
{
    public interface IAmbienceDataProvider
    {
        UniTask<AudioClip> Load(string key, int cacheContext);
    }
}