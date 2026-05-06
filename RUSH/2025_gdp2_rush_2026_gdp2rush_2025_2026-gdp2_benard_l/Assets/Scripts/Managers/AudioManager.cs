using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Data")]
    [SerializeField] private AudioLibrary _Library;
    [Header("Settings")]
    [SerializeField] private AudioMixerGroup _SfxGroup;
    [SerializeField] private AudioSource _AudioSourcePrefab;
    [SerializeField] private int _PoolSize = 20;

    [SerializeField] private AudioMixerGroup _MusicGroup;
    [SerializeField] private AudioSource _MusicSource;

    private List<AudioSource> _Pool = new List<AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializePool();
            if (_Library != null) _Library.Initialize();
        }

        AudioSignals.OnPlaySound += PlaySFX;
        AudioSignals.OnPlayMusic += PlayMusic;
        AudioSignals.OnStopMusic += () => _MusicSource.Stop();
    }

    private void InitializePool()
    {
        for (int i = 0; i < _PoolSize; i++)
        {
            AudioSource lSource = Instantiate(_AudioSourcePrefab, transform);
            lSource.outputAudioMixerGroup = _SfxGroup;
            lSource.playOnAwake = false;
            lSource.gameObject.SetActive(false);
            _Pool.Add(lSource);
        }
    }

    public void PlaySFX(AudioClipsEnum pType, Vector3 pPosition)
    {
        if (pType == AudioClipsEnum.None || _Library == null) return;

        SoundEntry lSoundData = _Library.GetSound(pType);
        if (lSoundData.clip == null) return;

        AudioSource lSource = GetAvailableSource();
        if (lSource != null)
        {
            lSource.transform.position = pPosition;
            lSource.clip = lSoundData.clip;
            lSource.volume = lSoundData.volume;
            lSource.gameObject.SetActive(true);
            lSource.Play();
            StartCoroutine(DisableSourceDelayed(lSource, lSoundData.clip.length));
        }
    }

    public void PlayMusic(AudioClipsEnum pType)
    {
        if (pType == AudioClipsEnum.None || _Library == null)
        {
            _MusicSource.Stop();
            return;
        }

        SoundEntry lSoundData = _Library.GetSound(pType);
        if (lSoundData.clip == null) return;
        if (_MusicSource.clip == lSoundData.clip && _MusicSource.isPlaying) return;

        _MusicSource.clip = lSoundData.clip;
        _MusicSource.volume = lSoundData.volume;
        _MusicSource.Play();
    }

    private AudioSource GetAvailableSource()
    {
        foreach (AudioSource lAudioSource in _Pool)
        {
            if (!lAudioSource.gameObject.activeSelf) return lAudioSource;
        } 
        return null;
    }

    private IEnumerator DisableSourceDelayed(AudioSource pSource, float pDelay)
    {
        yield return new WaitForSeconds(pDelay + 0.1f);
        pSource.gameObject.SetActive(false);
    }
}