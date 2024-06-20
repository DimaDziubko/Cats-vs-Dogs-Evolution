using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace _Game.Core.Services.Audio
{
    public class SoundService : MonoBehaviour, ISoundService
    {
        [ShowInInspector]
        private IObjectPool<SoundEmitter> _soundEmitterPool;
        [ShowInInspector]
        private readonly List<SoundEmitter> _activeSoundEmitters = new List<SoundEmitter>();
        [ShowInInspector]
        private readonly Queue<SoundEmitter> _frequentSoundEmitters = new Queue<SoundEmitter>();

        [SerializeField] SoundEmitter _soundEmitterPrefab;
        [SerializeField] bool _collectionCheck = true;
        [SerializeField] int _defaultCapacity = 10;
        [SerializeField] int _maxPoolSize = 100;
        [SerializeField] int _maxSoundInstances = 30;
    
        [SerializeField] private Transform _transform;

        public Queue<SoundEmitter> FrequentSoundEmitters => _frequentSoundEmitters;

        public Transform Transform
        {
            get => _transform;
            set => _transform = value;
        }
    
        public void Init() => InitializePool();

        public SoundBuilder CreateSound() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData data)
        {
            if (!data.FrequentSound) return true;

            if (_frequentSoundEmitters.Count >= _maxSoundInstances && _frequentSoundEmitters.TryDequeue(out var soundEmitter)) 
            {
                try {
                    soundEmitter.Stop();
                    return true;
                } catch {
                    //Debug.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get() => _soundEmitterPool.Get();

        public void ReturnToPool(SoundEmitter soundEmitter) {
            _soundEmitterPool.Release(soundEmitter);
        }

        public void StopAll() 
        {
            foreach (var soundEmitter in _activeSoundEmitters) {
                soundEmitter.Stop();
            }
            
            _frequentSoundEmitters.Clear();
        }

        public void Cleanup()
        {
            _frequentSoundEmitters.Clear();
        }

        void InitializePool() {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
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

        private void OnTakeFromPool(SoundEmitter soundEmitter) 
        {
            soundEmitter.gameObject.SetActive(true);
            _activeSoundEmitters.Add(soundEmitter);
        }

        private void OnReturnedToPool(SoundEmitter soundEmitter) 
        {
            soundEmitter.gameObject.SetActive(false);
            _activeSoundEmitters.Remove(soundEmitter);
        }

        private void OnDestroyPoolObject(SoundEmitter soundEmitter) => 
            Destroy(soundEmitter.gameObject);
    }
}