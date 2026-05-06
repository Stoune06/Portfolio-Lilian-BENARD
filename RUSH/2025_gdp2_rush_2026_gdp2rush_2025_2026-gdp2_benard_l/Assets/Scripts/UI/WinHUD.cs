using System;
using UnityEngine;
using UnityEngine.UI;

public class WinHUD : MonoBehaviour
{
    [SerializeField]
    private Button _ReturnToMenuButton;

    public static event Action onReturnToMenu;

    private void Start()
    {
        _ReturnToMenuButton.onClick.AddListener(() => onReturnToMenu?.Invoke());
    }

}
