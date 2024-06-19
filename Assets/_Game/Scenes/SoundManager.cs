using System.Collections.Generic;
using _Game.Core.Services.Audio;
using AudioSystem;
using UnityEngine;
using UnityEngine.Pool;

public class SoundManager : PersistentSingleton<SoundManager>, ISoundService
{
        private IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitters = new List<SoundEmitter>();
        public readonly Queue<SoundEmitter> _frequentSoundEmitters = new Queue<SoundEmitter>();

        [SerializeField] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;

        void Start() {
            InitializePool();
        }

        public SoundBuilder CreateSound() => new SoundBuilder(this);

        public Transform Transform => gameObject.transform;

        public bool CanPlaySound(SoundData data) {
            if (!data.FrequentSound) return true;

            if (_frequentSoundEmitters.Count >= maxSoundInstances && _frequentSoundEmitters.TryDequeue(out var soundEmitter)) {
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

        public SoundEmitter Get() {
            return soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter) {
            soundEmitterPool.Release(soundEmitter);
        }

        public Queue<SoundEmitter> FrequentSoundEmitters => _frequentSoundEmitters;

        public void StopAll() {
            foreach (var soundEmitter in activeSoundEmitters) {
                soundEmitter.Stop();
            }

            _frequentSoundEmitters.Clear();
        }

        private void InitializePool() {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize);
        }

        private SoundEmitter CreateSoundEmitter() 
        {
            var soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        private void OnTakeFromPool(SoundEmitter soundEmitter) 
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }

        private void OnReturnedToPool(SoundEmitter soundEmitter) 
        {
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        private void OnDestroyPoolObject(SoundEmitter soundEmitter) => 
            Destroy(soundEmitter.gameObject);
    }
