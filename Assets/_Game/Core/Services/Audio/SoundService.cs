using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SoundService : MonoBehaviour, ISoundService 
{
    private IObjectPool<SoundEmitter> soundEmitterPool;
    private readonly List<SoundEmitter> activeSoundEmitters = new List<SoundEmitter>();
    private readonly Queue<SoundEmitter> _frequentSoundEmitters = new Queue<SoundEmitter>();

    [SerializeField] SoundEmitter soundEmitterPrefab;
    [SerializeField] bool collectionCheck = true;
    [SerializeField] int defaultCapacity = 10;
    [SerializeField] int maxPoolSize = 100;
    [SerializeField] int maxSoundInstances = 30;
    [SerializeField] private Transform _transform;

    public Queue<SoundEmitter> FrequentSoundEmitters => _frequentSoundEmitters;

    public Transform Transform
    {
        get => _transform;
        set => _transform = value;
    }

    public void Init() => InitializePool();

    public SoundBuilder CreateSound() => new SoundBuilder(this);
    
    public bool CanPlaySound(SoundData data) {
        if (!data.FrequentSound) return true;

        if (_frequentSoundEmitters.Count >= maxSoundInstances && _frequentSoundEmitters.TryDequeue(out var soundEmitter)) {
            try {
                soundEmitter.Stop();
                return true;
            } catch {
                Debug.Log("SoundEmitter is already released");
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

    public void StopAll() {
        foreach (var soundEmitter in activeSoundEmitters) {
            soundEmitter.Stop();
        }

        _frequentSoundEmitters.Clear();
    }

    void InitializePool() {
        soundEmitterPool = new ObjectPool<SoundEmitter>(
            CreateSoundEmitter,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize);
    }

    SoundEmitter CreateSoundEmitter() {
        var soundEmitter = Instantiate(soundEmitterPrefab);
        soundEmitter.Construct(this);
        soundEmitter.gameObject.SetActive(false);
        return soundEmitter;
    }

    void OnTakeFromPool(SoundEmitter soundEmitter) {
        soundEmitter.gameObject.SetActive(true);
        activeSoundEmitters.Add(soundEmitter);
    }

    void OnReturnedToPool(SoundEmitter soundEmitter) {
        soundEmitter.gameObject.SetActive(false);
        activeSoundEmitters.Remove(soundEmitter);
    }

    void OnDestroyPoolObject(SoundEmitter soundEmitter) {
        Destroy(soundEmitter.gameObject);
    }
}