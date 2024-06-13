using System.Collections.Generic;
using UnityEngine;

public interface ISoundService
{
    Transform Transform { get; }

    bool CanPlaySound(SoundData soundData);
    void ReturnToPool(SoundEmitter soundEmitter);
    Queue<SoundEmitter> FrequentSoundEmitters { get;}
    SoundEmitter Get();
    SoundBuilder CreateSound();
}