using _Game.Core.Services.Audio;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private SoundData _soundData;
    [SerializeField] private SoundService _manager;

    public float Speed = 0.2f; // Інтервал між пострілами
    private float _lastShootTime;

    public void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= _lastShootTime + Speed)
        {
            _lastShootTime = Time.time;

            _manager.CreateSound()
                .WithSoundData(_soundData)
                .WithRandomPitch()
                .WithPosition(Vector3.zero)
                .Play();
        }
    }
}
