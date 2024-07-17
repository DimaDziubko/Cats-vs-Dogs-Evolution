using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace _Game.Core.Services.Audio
{
    public class SoundService : MonoBehaviour, ISoundService
    {
        [ShowInInspector]
        private Dictionary<AudioClip, IObjectPool<SoundEmitter>> _soundEmitterPools =
            new Dictionary<AudioClip, IObjectPool<SoundEmitter>>();

        [ShowInInspector]
        private Dictionary<AudioClip, List<SoundEmitter>> _activeSoundEmitters =
            new Dictionary<AudioClip, List<SoundEmitter>>();

        [ShowInInspector]
        private Dictionary<AudioClip, Queue<SoundEmitter>> _frequentSoundEmitters =
            new Dictionary<AudioClip, Queue<SoundEmitter>>();

        [SerializeField] SoundEmitter _soundEmitterPrefab;
        [SerializeField] bool _collectionCheck = true;
        [SerializeField] int _defaultCapacity = 10;
        [SerializeField] int _maxPoolSize = 100;
        [SerializeField] int _maxSoundInstances = 30;

        [SerializeField] private Transform _transform;

        public Dictionary<AudioClip, Queue<SoundEmitter>> FrequentSoundEmitters => _frequentSoundEmitters;

        public Transform Transform
        {
            get => _transform;
            set => _transform = value;
        }

        public SoundBuilder CreateSound() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData data)
        {
            if (!data.FrequentSound) return true;

            if (_frequentSoundEmitters.TryGetValue(data.Clip, out var queue) && queue.Count >= _maxSoundInstances)
            {
                if (queue.TryDequeue(out var soundEmitter))
                {
                    try
                    {
                        if(soundEmitter.gameObject.activeInHierarchy)
                            soundEmitter.Stop();
                        return true;
                    }
                    catch
                    {
                        Debug.LogWarning("SoundEmitter is already released");
                    }

                    return false;
                }
            }

            return true;
        }


        public SoundEmitter Get(AudioClip clip)
        {
            if (!_soundEmitterPools.TryGetValue(clip, out var pool))
            {
                pool = InitializePool(clip);
                _soundEmitterPools[clip] = pool;
            }

            return pool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            if (_soundEmitterPools.TryGetValue(soundEmitter.Data.Clip, out var pool))
            {
                pool.Release(soundEmitter);
            }
        }

        public void StopAll()
        {
            foreach (var keyValuePair in _activeSoundEmitters)
            {
                foreach (var soundEmitter in keyValuePair.Value)
                {
                    soundEmitter.Stop();
                }
            }

            foreach (var queue in _frequentSoundEmitters.Values)
            {
                queue.Clear();
            }
        }

        public void Cleanup()
        {
            foreach (var queue in _frequentSoundEmitters.Values)
            {
                queue.Clear();
            }

            foreach (var keyValuePair in _soundEmitterPools)
            {
                var pool = keyValuePair.Value;
                pool.Clear();
            }

            _soundEmitterPools.Clear();
            _frequentSoundEmitters.Clear();
            _activeSoundEmitters.Clear();
        }

        private IObjectPool<SoundEmitter> InitializePool(AudioClip clip)
        {
            return new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                soundEmitter => OnTakeFromPool(soundEmitter, clip),
                soundEmitter => OnReturnedToPool(soundEmitter, clip),
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize);
        }

        private SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(_soundEmitterPrefab);
            soundEmitter.Construct(this);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        private void OnTakeFromPool(SoundEmitter soundEmitter, AudioClip clip)
        {
            if (soundEmitter == null)
            {
                //Debug.LogError("SoundEmitter is null in OnTakeFromPool");
                return;
            }

            if (clip == null)
            {
                //Debug.LogError("AudioClip is null in OnTakeFromPool");
                return;
            }

            soundEmitter.gameObject.SetActive(true);

            if (!_activeSoundEmitters.ContainsKey(clip))
            {
                _activeSoundEmitters[clip] = new List<SoundEmitter>();
            }
            _activeSoundEmitters[clip].Add(soundEmitter);
        }

        private void OnReturnedToPool(SoundEmitter soundEmitter, AudioClip clip)
        {
            if (soundEmitter == null)
            {
                //Debug.LogError("SoundEmitter is null in OnReturnedToPool");
                return;
            }

            if (clip == null)
            {
                //Debug.LogError("AudioClip is null in OnReturnedToPool");
                return;
            }

            soundEmitter.gameObject.SetActive(false);

            if (_activeSoundEmitters.ContainsKey(clip))
            {
                _activeSoundEmitters[clip].Remove(soundEmitter);

                if (_activeSoundEmitters[clip].Count == 0)
                {
                    _activeSoundEmitters.Remove(clip);
                }
            }
        }

        private void OnDestroyPoolObject(SoundEmitter soundEmitter) =>
            Destroy(soundEmitter.gameObject);
    }
}