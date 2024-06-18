using UnityEngine;

public class SoundBuilder
{
    private readonly ISoundService _soundService;
    private SoundData _soundData;
    Vector3 _position = Vector3.zero;
    private bool _randomPitch;

    public SoundBuilder(ISoundService audioService) => 
        _soundService = audioService;

    public SoundBuilder WithSoundData(SoundData soundData)
    {
        _soundData = soundData;
        return this;
    }

    public SoundBuilder WithPosition(Vector3 position)
    {
        _position = position;
        return this;
    }
    
    public SoundBuilder WithRandomPitch() {
        _randomPitch = true;
        return this;
    }
    
    public void Play() {
        if (!_soundService.CanPlaySound(_soundData)) return;
            
        SoundEmitter soundEmitter = _soundService.Get();
        soundEmitter.Initialize(_soundData);
        soundEmitter.Transform.position = _position;
        soundEmitter.Transform.parent = _soundService.Transform;

        if (_randomPitch) {
            soundEmitter.WithRandomPitch();
        }
            
        if (_soundData.FrequentSound) {
            _soundService.FrequentSoundEmitters.Enqueue(soundEmitter);
        }
            
        soundEmitter.Play();
    }
}