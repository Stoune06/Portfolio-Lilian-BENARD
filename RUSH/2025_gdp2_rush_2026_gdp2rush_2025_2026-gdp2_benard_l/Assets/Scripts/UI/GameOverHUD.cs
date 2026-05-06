using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverHUD : MonoBehaviour
{
    [SerializeField]
    private Button _ClearButton;

    [SerializeField]
    private Button _RetryButton;

    public static event Action onClearButtonPressed;
    public static event Action onRetryButtonPressed;

    void Start()
    {
        _ClearButton.onClick.AddListener(() => onClearButtonPressed?.Invoke());
        _RetryButton.onClick.AddListener(() => onRetryButtonPressed?.Invoke());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
