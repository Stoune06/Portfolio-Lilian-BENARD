using System;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField]
    private Button _RetryButton;

    [SerializeField]
    private Button _ReturnToMenuButton;

    [SerializeField]
    private Button _ContinueButton;

    public static event Action onReturnToMenu;
    public static event Action onRetryToMenu;
    public static event Action onContinue;


    private void Start()
    {
        _RetryButton.onClick.AddListener(() => onRetryToMenu?.Invoke());
        _ReturnToMenuButton.onClick.AddListener(()=> onReturnToMenu?.Invoke());
        _ContinueButton.onClick.AddListener(()=> onContinue?.Invoke());
    }

}
