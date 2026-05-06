using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Data/AudioLibrary")]
public class AudioLibrary : ScriptableObject
{
    [SerializeField]
    private List<SoundEntry> _Sounds;

    private Dictionary<AudioClipsEnum, SoundEntry> _SoundDictionary;

    public void Initialize()
    {
        _SoundDictionary = new Dictionary<AudioClipsEnum, SoundEntry>();
        foreach (SoundEntry lSound in _Sounds)
        {
            if (!_SoundDictionary.ContainsKey(lSound.type))
            {
                _SoundDictionary.Add(lSound.type, lSound);
            }
        }
    }

    public SoundEntry GetSound(AudioClipsEnum pAudioType)
    {
        if (_SoundDictionary == null) Initialize();

        if (_SoundDictionary.TryGetValue(pAudioType, out SoundEntry lFoundEntry))
        {
            return lFoundEntry;
        }
        return default;
    }
}

[System.Serializable]
public struct SoundEntry
{
    public AudioClipsEnum type;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
}