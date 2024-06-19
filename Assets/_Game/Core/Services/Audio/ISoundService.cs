using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Services.Audio
{
    public interface ISoundService
    {
        Transform Transform { get; }
        bool CanPlaySound(SoundData soundData);
        void ReturnToPool(SoundEmitter soundEmitter);
        Queue<SoundEmitter> FrequentSoundEmitters { get;}
        SoundEmitter Get();
        SoundBuilder CreateSound();
    }
}