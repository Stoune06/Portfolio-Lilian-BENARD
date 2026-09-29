using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Other;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Manager
{

    public class SoundManager : MonoBehaviour
    {
        [Header(Utils.HEADER_PARAMETERS_SOUNDS)]
        [SerializeField] private AudioMixerGroup _SoundGroup;
        [SerializeField] private AudioMixerGroup _MusicGroup;
        [SerializeField] private AudioMixer _Mixer;
        [SerializeField] private GameObject _MusicPlayerParent;
        [SerializeField] private GameObject _SoundPoolParent;
        [SerializeField] private AudioSource _SfxSource;

        [HideInInspector] public AudioMixer Mixer => _Mixer;

        public static SoundManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // POOL
        private readonly List<AudioSource> _InactiveSoundPlayer = new List<AudioSource>();
        private readonly List<AudioSource> _ActiveSoundPlayer = new List<AudioSource>();

        private AudioSource _MusicPlayer;

        private const int NUMBER_OF_SOUNDS_TO_PLAY = 8;

        private const float MAX_SOUND_VALUE = 0f;
        private const float NO_SOUND_VALUE = -80f;

        private void Init()
        {
            Instance = this;

            CreateMusicPlayer(_MusicPlayerParent);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY / AWAKE
        void Awake()
        {
            Init();

            for (int i = 0; i < NUMBER_OF_SOUNDS_TO_PLAY; i++)
            {
                CreateSoundPlayer(_SoundPoolParent);
            }
            DontDestroyOnLoad(this);
        }

        private void CreateSoundPlayer(GameObject pObject)
        {
            AudioSource lInactiveSound = Instantiate(_SfxSource, pObject.transform, true);
            _InactiveSoundPlayer.Add(lInactiveSound);
            lInactiveSound.outputAudioMixerGroup = _SoundGroup;
        }

        private void CreateMusicPlayer(GameObject pObject) => _MusicPlayer = pObject.AddComponent<AudioSource>();

        private void SoundFinish(AudioSource pCurrentSound)
        {
            _ActiveSoundPlayer.Remove(pCurrentSound);
            _InactiveSoundPlayer.Add(pCurrentSound);
        }

        public void PlayMusic(AudioClip pMusic)
        {
            if (pMusic == null) 
                return;
            
            if (_MusicPlayer.clip == pMusic && _MusicPlayer.isPlaying) 
                return; 
            
            _MusicPlayer.clip = pMusic;
            _MusicPlayer.loop = true;
            _MusicPlayer.Play();
            _MusicPlayer.outputAudioMixerGroup = _MusicGroup;
        }

        public void StopMusic() => _MusicPlayer.Stop();

        public void PlaySound(AudioClip pClip, Vector3 pSoundPosition)
        {
            if (_InactiveSoundPlayer.Count == 0) CreateSoundPlayer(_SoundPoolParent);

            AudioSource lSoundPlayer = _InactiveSoundPlayer[0];
            _InactiveSoundPlayer.Remove(lSoundPlayer);
            _ActiveSoundPlayer.Add(lSoundPlayer);

            lSoundPlayer.clip = pClip;
            lSoundPlayer.transform.position = pSoundPosition;
            lSoundPlayer.Play();
            lSoundPlayer.loop = false;

            StartCoroutine(WaitForSoundToEnd(lSoundPlayer));
        }

        private IEnumerator WaitForSoundToEnd(AudioSource pSource)
        {
            yield return new WaitWhile(() => pSource.isPlaying);
            SoundFinish(pSource);
        }

        public void PlayRandomSound(AudioClip[] pSounds, Vector3 pPosition)
        {
            if (pSounds is null || pSounds.Length <= 0) return;

            int lRandomIndex = Random.Range(0, pSounds.Length);
            PlaySound(pSounds[lRandomIndex], pPosition);
        }

        public void ChangeVolume(float pValue, string pBusToChange)
        {
            float lNormalizedValue = Mathf.Clamp01(pValue);
            float dB = Mathf.Lerp(NO_SOUND_VALUE, MAX_SOUND_VALUE, lNormalizedValue);
            
            _Mixer.SetFloat(pBusToChange, dB);
        }

        public float DBToSliderValue(float pDB)
        {
            return Mathf.InverseLerp(NO_SOUND_VALUE, MAX_SOUND_VALUE, pDB);
        }

    }
}