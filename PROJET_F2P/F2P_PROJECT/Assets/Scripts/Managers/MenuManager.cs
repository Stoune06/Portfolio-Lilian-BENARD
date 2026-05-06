using System;
using Com.IsartDigital.HealerSurvivor.Manager;
using DG.Tweening;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Menus
{

    public class MenuManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private PlayerSpawner _Spawner;
        [SerializeField] private AudioClip _MenuMusic;
        [SerializeField] private AudioClip _GameMusic;
        [SerializeField] private AudioClip _ClickSFX;
        
        private const float TWEEN_SCALE = 1f;
        private const float TWEEN_DURATION = .5f;

        private readonly Dictionary<EMenuType, GameObject> _Menus = new Dictionary<EMenuType, GameObject>();
        
        private GameManager _GameManager => GameManager.Instance;

        private bool _IsGamePlaying;
        
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            RegisterMenus();
            RegisterButtons();

            ShowMenu(EMenuType.TITLE_CARD);
        }

        private void Start()
        {
            _GameManager.onLevelFinished += ShowMenu;
            _GameManager.onGameOver += ShowMenu;
        }

        private void RegisterMenus()
        {
            _Menus.Clear();

            // Include inactive menus: the WIN_SCREEN / GAME_OVER / PAUSE prefabs are typically disabled in the
            // scene at boot and would otherwise be missing from _Menus, so ShowMenu(WIN_SCREEN) would silently no-op.
            foreach (MenuType lMenu in FindObjectsByType<MenuType>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                _Menus[lMenu.type] = lMenu.gameObject;
        }

        private void RegisterButtons()
        {
            MenuButton[] lButtons = FindObjectsByType<MenuButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (MenuButton lButton in lButtons)
            {
                Button lUIButton = lButton.GetComponent<Button>();
                EMenuType lTarget = lButton.TargetMenu;

                lUIButton.onClick.AddListener(() => ShowMenu(lTarget));
            }
        }

        public void ShowMenu(EMenuType pType)
        {
            // Null-guard the click SFX: if SoundManager is missing or _ClickSFX not assigned, we don't want
            // an exception to abort the rest of ShowMenu (which would leave the WIN_SCREEN invisible).
            if (SoundManager.Instance != null && _ClickSFX != null)
                SoundManager.Instance.PlaySound(_ClickSFX, transform.position);

            CheckType(pType);

            foreach (GameObject lMenu in _Menus.Values)
                if (lMenu != null) lMenu.SetActive(false);

            if (!_Menus.TryGetValue(pType, out GameObject lActiveMenu) || lActiveMenu == null)
            {
                Debug.LogError($"[MenuManager] Menu not registered: {pType}. Registered: {string.Join(", ", _Menus.Keys)}");
                return;
            }

            lActiveMenu.SetActive(true);

            // Pop-in tween on the inner content panel. Skip gracefully if the menu prefab doesn't have the
            // expected structure (Canvas with 2+ children) so a misconfigured menu still displays.
            Canvas lCanvas = lActiveMenu.transform.GetComponentInChildren<Canvas>();
            if (lCanvas == null || lCanvas.transform.childCount < 2) return;

            Transform lTransform = lCanvas.transform.GetChild(1);
            lTransform.DOKill();
            // Set the final scale first so the panel is visible even if DOTween fails silently on mobile
            // (IL2CPP stripping, uninitialized tween pool, etc.). The pop-in is a nice-to-have, not a requirement.
            lTransform.localScale = Vector3.one * TWEEN_SCALE;
            lTransform.DOScale(Vector3.one * TWEEN_SCALE, TWEEN_DURATION).From(Vector3.zero).SetEase(Ease.OutBack).SetUpdate(true);
        }

        private void CheckType(EMenuType pType)
        {
            switch (pType)
            {
                case EMenuType.PLAY when !_IsGamePlaying:
                    _GameManager.ResetValues();
                    _Spawner.SpawnPlayer();
                    _IsGamePlaying = true;
                    _GameManager.onSwitchPauseGame?.Invoke(false);
                    // StartRun fires onGameStart (so FTUEManager can block wave spawning before anything else)
                    // then triggers wave 1 only when the FTUE has already been played.
                    _GameManager.StartRun();
                    SoundManager.Instance.PlayMusic(_GameMusic);
                    break;
                case EMenuType.PLAY when _IsGamePlaying:
                    _GameManager.onSwitchPauseGame?.Invoke(false);
                    _GameManager.onGameStart?.Invoke();
                    break;
                case EMenuType.PAUSE:
                    _GameManager.onSwitchPauseGame?.Invoke(true);
                    break;
                case EMenuType.MAIN:
                    _GameManager.onSwitchPauseGame?.Invoke(false);
                    _IsGamePlaying = false;
                    _GameManager.onLeaveGame?.Invoke();
                    _GameManager.onBackToMenu?.Invoke();
                    break;
                case EMenuType.WIN_SCREEN or EMenuType.GAME_OVER:
                    _GameManager.onSwitchPauseGame?.Invoke(true);
                    // Don't re-invoke onGameOver here: ShowMenu is already subscribed to it,
                    // so re-invoking would recurse infinitely.
                    break;
            }

        }
    }
}