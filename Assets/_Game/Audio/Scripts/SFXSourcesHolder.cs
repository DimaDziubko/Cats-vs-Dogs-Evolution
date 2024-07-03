using UnityEngine;
using UnityEngine.Serialization;

namespace Assets._Game.Audio.Scripts
{
    public class SFXSourcesHolder : MonoBehaviour
    {
        [FormerlySerializedAs("_sfxSources")] public AudioSource[] SfxSources;
    }
}