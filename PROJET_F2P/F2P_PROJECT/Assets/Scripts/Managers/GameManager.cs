using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.Menus;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    
    public class GameManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private AudioClip _MenuMusic;
        [SerializeField] private AudioClip _GameMusic;
        [SerializeField] private AudioClip _WinSound;
        [SerializeField] private AudioClip _LoseSound;
        
        public Action<int> onCallWaveAndIncreaseIndex;
        public Action onGainingCoins;
        public Action onMidNight;
        public Action onSwitchCardFromDeck;
        public Action<bool> onSwitchPauseGame;
        public Action onLeaveGame;
        public Action onGameStart;
        public Action<EMenuType> onGameOver;
        public Action<EMenuType> onLevelFinished;
        public Action onBackToMenu;

        private int _CurrentIndexWave = 0;

        public int CurrentWave => _CurrentIndexWave;

        public static GameManager Instance { get; private set; }

        // Flag consumed by FTUEManager to suppress the first wave during the tutorial.
        public bool IsWaveSpawningBlocked { get; set; } = false;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            #region SINGLETON
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            #endregion
        }
        
        private void Start()
        {
            onSwitchPauseGame += SetPause;

            onGameStart += PlayMusic;
            onBackToMenu += BackToMenuMusic;
            onGameOver += PlaySoundOnWinOrLose;
            onLevelFinished += PlaySoundOnWinOrLose;

            onLeaveGame += CleanupRun;
        }

        private void CleanupRun()
        {
            // Destroy everything with the "Ally" layer: both the Player and companion allies share it.
            // Player.OnDestroy resets Player.instance to null, so PlayerSpawner.SpawnPlayer spawns a fresh one next PLAY.
            int lAllyLayer = LayerMask.NameToLayer("Ally");
            Character[] lCharacters = FindObjectsByType<Character>(FindObjectsSortMode.None);
            foreach (Character lCharacter in lCharacters)
            {
                if (lCharacter != null && lCharacter.gameObject.layer == lAllyLayer)
                    Destroy(lCharacter.gameObject);
            }
        }
        
        public void HandleNewWave()
        {
            if (IsWaveSpawningBlocked) return;
            onCallWaveAndIncreaseIndex?.Invoke(_CurrentIndexWave);
            Player.instance.InvokeWaveStart(_CurrentIndexWave);
            _CurrentIndexWave++;
        }
        
        public void ResetValues()
        {
            _CurrentIndexWave = 0;
            IsWaveSpawningBlocked = false;
            Time.timeScale = 1f;
            Debug.Log("GameManager: Data has been reset for a new run.");
        }

        private void SetPause(bool pStatus) => Time.timeScale = pStatus ? 0f : 1f;

        private void OnDisable()
        {
            onSwitchPauseGame -= SetPause;
            onLeaveGame -= CleanupRun;
        }
        
        public void StartRun()
        {
            onGameStart?.Invoke();

            // Skip FTUE hand-off only when every FTUE run has been completed.
            if (FTUEManager.HasCompletedAllRuns)
            {
                SpawnActiveDeck();
                HandleNewWave();
            }
        }
        
        public void SpawnActiveDeck()
        {
            List<CardCharacterSO> lDeck = DeckBuilderManager.Instance.activeCurrentCards;
            Vector3 lPlayerPos = Player.instance.transform.position;

            foreach (CardCharacterSO lCard in lDeck)
            {
                if (lCard.PrefabCharacter != null)
                {
                    Vector3 lSpawnOffset = UnityEngine.Random.insideUnitSphere * 2f;
                    lSpawnOffset.y = 0; 
            
                    Instantiate(lCard.PrefabCharacter, lPlayerPos + lSpawnOffset, Quaternion.identity);
                }
            }
        }

        private void PlayMusic() => SoundManager.Instance.PlayMusic(_GameMusic);
        
        private void BackToMenuMusic() => SoundManager.Instance.PlayMusic(_MenuMusic);

        private void PlaySoundOnWinOrLose(EMenuType pType)
        {
            // Null-guard everything: if this subscriber throws (SoundManager missing on mobile, clip not
            // assigned, etc.) the exception interrupts the multicast dispatch of onLevelFinished and
            // MenuManager.ShowMenu never fires → no WIN_SCREEN. Silent safe path is the lesser evil.
            if (SoundManager.Instance == null) return;

            switch (pType)
            {
                case EMenuType.WIN_SCREEN:
                    if (_WinSound != null) SoundManager.Instance.PlaySound(_WinSound, transform.position);
                    break;
                case EMenuType.GAME_OVER:
                    if (_LoseSound != null) SoundManager.Instance.PlaySound(_LoseSound, transform.position);
                    break;
            }
        }
        
    }
}