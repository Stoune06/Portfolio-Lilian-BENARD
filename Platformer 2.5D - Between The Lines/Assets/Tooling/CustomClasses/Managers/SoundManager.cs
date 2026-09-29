using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Rush.Managers
{
    [Obsolete]
    public class SoundManager : MonoBehaviour
    {
        [Header("Source Number")]
        [SerializeField]
        private int _MusicSourceNumber = 3;
        [SerializeField] private int _SfxSourceNumber = 10;

        [Header("Mixer Group")]
        [SerializeField] private AudioMixerGroup _SfxGroup;
        [SerializeField] private AudioMixerGroup _MusicGroup;
        [SerializeField] private AudioMixer _Master;

        [Header("Volume")]
        [field: SerializeField, Range(MIN_VOLUME, MAX_VOLUME)] private float _SfxVolume = 0;
        [field: SerializeField, Range(MIN_VOLUME, MAX_VOLUME)] private float _MusicVolume = 0;
        public float SfxVolume { get => _SfxVolume; set { SetVolume(_SfxGroup, value); _SfxVolume = value; } }
        public float MusicVolume { get => _MusicVolume; set { SetVolume(_MusicGroup, value); _MusicVolume = value; } }

        [field: SerializeField] public static bool SoundHavePosition { get; private set; } = false;

        public const string SFX_PATH = "Sound/Sfx", MUSIC_PATH = "Sound/Music", PROPERTY = "MasterVolume";
        public const float MIN_VOLUME = -80f, MAX_VOLUME = 20f;

        private static Dictionary<string, AudioClip> _KeyToAudio = new Dictionary<string, AudioClip>();
        private static AudioSource[] _MusicSources, _SfxSources, _AudioSources;

        private static SoundManager instance;

        public static bool Pause { get => _Pause; private set => SetPause(value); }
        private static bool _Pause = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else Destroy(gameObject);
        }

        private void Init()
        {
            SetDictionnary();

            CreateAudioSource(ref _MusicSources, _MusicSourceNumber, _MusicGroup);
            CreateAudioSource(ref _SfxSources, _SfxSourceNumber, _SfxGroup);
            _AudioSources = _MusicSources.Concat(_SfxSources).ToArray();
        }

        private void SetDictionnary()
        {
            string[] lPathArray = new string[2] { SFX_PATH, MUSIC_PATH };
            AudioClip[] lAudioArray;
            foreach (string lPath in lPathArray)
            {
                lAudioArray = Resources.LoadAll<AudioClip>(lPath);
                foreach (AudioClip lClip in lAudioArray)
                {
                    _KeyToAudio.Add(lClip.name, lClip);
                }
            }
        }

        private void CreateAudioSource(ref AudioSource[] pAudioSourceList, int pNumber, AudioMixerGroup pGroup)
        {
            pAudioSourceList = new AudioSource[pNumber];
            AudioSource lAudio;

            for (int i = 0; i < pNumber; i++)
            {
                lAudio = gameObject.AddComponent<AudioSource>();
                pAudioSourceList[i] = lAudio;
                lAudio.playOnAwake = false;
                lAudio.outputAudioMixerGroup = pGroup;
            }
        }

        public static void PlaySfx(string pKey, bool pIsPauseAffected = true, bool pIsLooping = false)
            => Play(pKey, _SfxSources, pIsPauseAffected, pIsLooping);

        public static void PlaySfx(AudioClip pClip, bool pIsPauseAffected = true, bool pIsLooping = false)
            => Play(pClip, _SfxSources, pIsPauseAffected, pIsLooping);

        public static void PlayMusic(string pKey, bool pIsPauseAffected = true, bool pIsLooping = true)
            => Play(pKey, _MusicSources, pIsPauseAffected, pIsLooping);

        public static void PlayMusic(AudioClip pClip, bool pIsPauseAffected = true, bool pIsLooping = false)
            => Play(pClip, _MusicSources, pIsPauseAffected, pIsLooping);

        private static void Play(string pKey, AudioSource[] pSourceList, bool pIsPauseAffected, bool pIsLooping)
        {
            if (_KeyToAudio.ContainsKey(pKey))
            {
                foreach (AudioSource item in pSourceList)
                {
                    if (!item.isPlaying)
                    {
                        item.clip = _KeyToAudio[pKey];
                        item.loop = pIsLooping;
                        item.ignoreListenerPause = pIsPauseAffected;
                        item.Play();
                        return;
                    }
                }
            }
            else throw new System.NotImplementedException($"The key : {pKey} wasn't found in your files." +
                    $"Please Make sure that the pass is either \"Sound/Sfx/{pKey}\" or \"Music" +
                    $"/Sfx/{pKey}\". Or there is not enough AudioSource");
        }

        private static void Play(AudioClip pClip, AudioSource[] pSourceList, bool pIsPauseAffected, bool pIsLooping)
        {
            foreach (AudioSource item in pSourceList)
            {
                if (!item.isPlaying)
                {
                    item.clip = pClip;
                    item.loop = pIsLooping;
                    item.ignoreListenerPause = pIsPauseAffected;
                    item.Play();
                    return;
                }
            }
            throw new System.NotImplementedException($"There is not enough AudioSource" +
                $", you can add some within the SoundManager's inspector");
        }

        private void SetVolume(AudioMixerGroup pMixerGroup, float pVolume)
        {
            //pMixerGroup.audio("Volume", pVolume);
            pMixerGroup.audioMixer.SetFloat(PROPERTY, pVolume);
            Debug.Log("f");
        }

        private static void SetPause(bool pPause)
        {
            _Pause = pPause;
            AudioSettings l = new AudioSettings();
            if(pPause)
                foreach (AudioSource item in _AudioSources) item.Pause();
            else
                foreach (AudioSource item in _AudioSources) item.UnPause();
        }

        private static void ClearSources(AudioSource[] pSources)
        {
            foreach (AudioSource item in pSources)
                item.clip = null;
        }

        public static void ClearMusic() => ClearSources(_MusicSources);
        public static void ClearSfx() => ClearSources(_SfxSources);
        public static void ClearAudio() => ClearSources(_AudioSources);

        private static void MuteSources(bool pBool, AudioSource[] pSources)
        {
            foreach (AudioSource item in pSources)
                item.mute = pBool;
        }

        public static void MuteMusic(bool pBool = true) => MuteSources(pBool, _MusicSources);
        public static void MuteSfx(bool pBool = true) => MuteSources(pBool, _SfxSources);
        public static void MuteAudio(bool pBool = true) => MuteSources(pBool, _AudioSources);
    }
}