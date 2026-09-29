using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIAuthManager : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private TMP_InputField _UsernameInput;
    [SerializeField] private TMP_InputField _PasswordInput;
    [SerializeField] private Button _CreateAccountButton;
    [SerializeField] private Button _SignInButton;
    [SerializeField] private Button _OfflineButton;
    [SerializeField] private TextMeshProUGUI _StatusText;

    [Header("Panels")]
    [SerializeField] private GameObject _LoginPanel;
    [SerializeField] private GameObject _UserInfoPanel;
    [SerializeField] private TextMeshProUGUI _UserInfoText;

    private DataManager _DataManager;

    public static event Action OfflinePlayEvent;

    private void Start()
    {
        _DataManager = DataManager.Instance;

        if (_DataManager == null)
        {
            Debug.LogError("Ajoutez le DataManager");
            return;
        }

        DataManager.OnUserSignedIn += OnUserSignedIn;
        DataManager.OnUserSignedOut += OnUserSignedOut;
        DataManager.OnError += OnError;

        if (_CreateAccountButton) _CreateAccountButton.onClick.AddListener(OnCreateAccountClicked);
        if (_SignInButton) _SignInButton.onClick.AddListener(OnSignInClicked);
        if (_OfflineButton) _OfflineButton.onClick.AddListener(PlayOffline);

        UpdateUI(true);
    }

    private async void OnCreateAccountClicked()
    {
        string lUsername = _UsernameInput.text.Trim();
        string lPassword = _PasswordInput.text;

        if (!ValidateInputs(lUsername, lPassword)) return;

        SetButtonsInteractable(false);
        ShowStatus("Création...", Color.yellow);

        bool lSuccess = await _DataManager.CreateAccountWithUsername(lUsername, lPassword);

        if (lSuccess) ShowStatus("Compte créé !", Color.green);
        else SetButtonsInteractable(true);
    }

    private async void OnSignInClicked()
    {
        string lUsername = _UsernameInput.text.Trim();
        string lPassword = _PasswordInput.text;

        if (!ValidateInputs(lUsername, lPassword)) return;

        SetButtonsInteractable(false);
        ShowStatus("Connexion...", Color.yellow);

        bool lSuccess = await _DataManager.SignInWithUsername(lUsername, lPassword);

        if (lSuccess) ShowStatus("Connecté !", Color.green);
        else SetButtonsInteractable(true);
    }

    private void PlayOffline()
    {
        SetButtonsInteractable(false);

        OfflinePlayEvent?.Invoke();

        ShowStatus("Connecté !", Color.green);
    }

    private void OnUserSignedIn(Firebase.Auth.FirebaseUser pUser)
    {
        UpdateUI();
        if (_UserInfoText)
            _UserInfoText.text = $"Bienvenue, {pUser.DisplayName} !\nID: {pUser.UserId}";

        ShowStatus("Prêt à jouer", Color.green);
    }

    private void OnUserSignedOut()
    {
        UpdateUI();
        ShowStatus("Déconnecté", Color.white);
    }

    private void OnError(string pError)
    {
        ShowStatus(pError, Color.red);
        SetButtonsInteractable(true);
    }


    private void UpdateUI(bool pFirstShow = false)
    {
        bool lIsSignedIn = _DataManager != null && _DataManager.IsSignedIn;

        if (_LoginPanel && !pFirstShow) _LoginPanel.SetActive(!lIsSignedIn);
        if (_UserInfoPanel) _UserInfoPanel.SetActive(lIsSignedIn);

        if (!lIsSignedIn)
        {
            SetButtonsInteractable(true);
            if (_UsernameInput) _UsernameInput.text = "";
            if (_PasswordInput) _PasswordInput.text = "";
        }
    }

    private bool ValidateInputs(string pUsername, string pPassword)
    {
        if (string.IsNullOrEmpty(pUsername) || string.IsNullOrEmpty(pPassword))
        {
            ShowStatus("Champs vides", Color.red);
            return false;
        }
        return true;
    }

    private void SetButtonsInteractable(bool pState)
    {
        if (_CreateAccountButton) _CreateAccountButton.interactable = pState;
        if (_SignInButton) _SignInButton.interactable = pState;
        if (_OfflineButton) _OfflineButton.interactable = pState;
    }

    private void ShowStatus(string pMsg, Color pColor)
    {
        if (_StatusText)
        {
            _StatusText.text = pMsg;
            _StatusText.color = pColor;
        }
        Debug.Log($"[UI] {pMsg}");
    }

    private void OnDestroy()
    {
        if (_DataManager != null)
        {
            DataManager.OnUserSignedIn -= OnUserSignedIn;
            DataManager.OnUserSignedOut -= OnUserSignedOut;
            DataManager.OnError -= OnError;
        }
        _CreateAccountButton.onClick.RemoveAllListeners();
        _SignInButton.onClick.RemoveAllListeners();
        _OfflineButton.onClick.RemoveAllListeners();
    }
}