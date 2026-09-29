using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private ReflectionPanel _ReflectionPanel;
    [SerializeField] private GameObject _ActionPanel;
    [SerializeField] private WinHUD _WinScreenPanel;
    [SerializeField] private GameOverHUD _GameOverPanel;
    [SerializeField] private GameObject _LevelSelector;
    [SerializeField] private PausePanel _PausePanel;
    [SerializeField] private GameObject _SoundPanel;


    [SerializeField] private Slider _SpeedSlider;
    [SerializeField] private Button _PauseButton;

    public static event Action<float> onSpeedChanged;
    public static event Action onPauseButtonPressed;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        _SpeedSlider.onValueChanged.AddListener(UpdateSpeed);
        _PauseButton.onClick.AddListener(() => onPauseButtonPressed?.Invoke());
        BookLevelSelector.onBookOpened += () => _SoundPanel.SetActive(false);
    }

    public void HideAllPanels()
    {
        _SoundPanel.SetActive(false);
        _SpeedSlider.gameObject.SetActive(false);
        _ReflectionPanel.gameObject.SetActive(false);
        _ActionPanel.SetActive(false);
        _WinScreenPanel.gameObject.SetActive(false);
        _GameOverPanel.gameObject.SetActive(false);
        _LevelSelector.gameObject.SetActive(false);

        _PausePanel.gameObject.SetActive(false);
        _PauseButton.gameObject.SetActive(false);
    }

    public void ShowReflectionHUD()
    {
        HideAllPanels();
        _ReflectionPanel.gameObject.SetActive(true);
        _SpeedSlider.gameObject.SetActive(true);
        _PauseButton.gameObject.SetActive(true);
    }

    public void ShowSoundPanel()
    {
        _SoundPanel.SetActive(true);
    }

    public void SetReflectionHUD(List<InventoryEntry> pInventoryEntry)
    {
        _ReflectionPanel.SetTilesInventory(pInventoryEntry);
    }

    public void UpdateReflectionHUD(List<int> pQuantityList)
    {
        _ReflectionPanel.UpdateInventoryCount(pQuantityList);
    }

    public void UpdateSpeed(float pSpeed)
    {
        onSpeedChanged?.Invoke(pSpeed);
    }

    public void ShowLevelSelector()
    {
        HideAllPanels();
        _LevelSelector.gameObject.SetActive(true);
    }

    public void ShowActionHUD()
    {
        HideAllPanels();
        _ActionPanel.SetActive(true);
        _SpeedSlider.gameObject.SetActive(true);
        _PauseButton.gameObject.SetActive(true);
    }

    public void ShowWinScreen()
    {
        HideAllPanels();
        _WinScreenPanel.gameObject.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        HideAllPanels();
        _GameOverPanel.gameObject.SetActive(true);
    }

    public void ShowPausePanel()
    {
        HideAllPanels();
        _PausePanel.gameObject.SetActive(true);
        _SoundPanel.SetActive(true);
    }
}
