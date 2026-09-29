using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Tooling;

[AutoLoad]
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    private const string API_BASE_URL = "https://betweenthelineapi.onrender.com";

    private FirebaseAuth _Auth;
    private FirebaseUser _CurrentUser;
    private bool _IsFirebaseInitialized;
    public bool IsSignedIn => _CurrentUser != null;

    public static event Action<FirebaseUser> OnUserSignedIn;
    public static event Action OnUserSignedOut;
    public static event Action<string> OnError;

    private bool _IsSaving;
    private bool _IsLoading;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(lTask =>
        {
            DependencyStatus lDependencyStatus = lTask.Result;
            if (lDependencyStatus == DependencyStatus.Available)
            {
                _Auth = FirebaseAuth.DefaultInstance;
                _Auth.StateChanged += AuthStateChanged;

                AuthStateChanged(this, null);

                _IsFirebaseInitialized = true;
                Debug.Log("[DataManager] Firebase initialized");
            }
            else
            {
                string lError = $"Impossible to resolve Firebase dependencies: {lDependencyStatus}";
                Debug.LogError($"[DataManager] {lError}");
                OnError?.Invoke(lError);
            }
        });
    }

    private void AuthStateChanged(object pSender, EventArgs pEventArgs)
    {
        if (_Auth.CurrentUser != _CurrentUser)
        {
            bool lSignedIn = _CurrentUser != _Auth.CurrentUser && _Auth.CurrentUser != null;
            if (!lSignedIn && _CurrentUser != null)
            {
                Debug.Log("[DataManager] User disconnected");
                OnUserSignedOut?.Invoke();
            }
            _CurrentUser = _Auth.CurrentUser;
            if (lSignedIn)
            {
                Debug.Log($"[DataManager] User connected : {_CurrentUser.UserId}");
                OnUserSignedIn?.Invoke(_CurrentUser);
            }
        }
    }


    public async Task<bool> CreateAccountWithUsername(string pUsername, string pPassword)
    {
        if (!_IsFirebaseInitialized) { OnError?.Invoke("Firebase not initialized"); return false; }

        string lEmail = GenerateFakeEmail(pUsername);
        if (lEmail == null) return false;

        try
        {
            AuthResult lResult = await _Auth.CreateUserWithEmailAndPasswordAsync(lEmail, pPassword);
            if (lResult.User != null)
            {
                UserProfile lProfile = new UserProfile { DisplayName = pUsername };
                SaveData lNewUserSave = new SaveData();
                lNewUserSave.username = pUsername;
                await SaveData(lNewUserSave);
                await lResult.User.UpdateUserProfileAsync(lProfile);
                OnUserSignedIn?.Invoke(_CurrentUser);
                return true;
            }
            return false;
        }
        catch (Exception lEx) { HandleAuthError(lEx, pUsername); return false; }
    }

    public async Task<bool> SignInWithUsername(string pUsername, string pPassword)
    {
        if (!_IsFirebaseInitialized) { OnError?.Invoke("Firebase not initialized"); return false; }

        string lEmail = GenerateFakeEmail(pUsername);
        if (lEmail == null) return false;
        Debug.Log($"[DEBUG] Connexion attempt with email : '{lEmail}' and password : '{pPassword}'");
        try
        {
            await _Auth.SignInWithEmailAndPasswordAsync(lEmail, pPassword);
            return true;
        }
        catch (Exception lEx)
        {
            if (lEx.GetBaseException() is FirebaseException lFbEx)
            {
                AuthError lErrorCode = (AuthError)lFbEx.ErrorCode;
                Debug.LogError($"[FIREBASE ERROR] Code: {lErrorCode} | Message: {lFbEx.Message}");
                OnError?.Invoke($"Error : {lErrorCode}");
            }
            else
            {
                Debug.LogError($"[OTHER ERROR] {lEx.Message}");
                OnError?.Invoke(lEx.Message);
            }
            return false;
        }

    }

    public void SignOut()
    {
        _Auth?.SignOut();
    }

    private string GenerateFakeEmail(string pUsername)
    {
        if (string.IsNullOrEmpty(pUsername) || pUsername.Length < 3)
        {
            OnError?.Invoke("Username is too short");
            return null;
        }
        string lClean = System.Text.RegularExpressions.Regex.Replace(pUsername.ToLower(), @"[^a-z0-9._-]", "");
        return $"{lClean}@firebaseauth.local";
    }

    private void HandleAuthError(Exception pEx, string pUsername)
    {
        string lMsg = pEx.Message;
        if (pEx is FirebaseException lFEx) lMsg = $"Firebase error ({lFEx.ErrorCode}): {lFEx.Message}";
        Debug.LogError($"[Auth Error] {lMsg}");
        OnError?.Invoke(lMsg);
    }

    public async Task<bool> SaveData(object pData)
    {
        if (_IsSaving || !IsSignedIn) return false;
        _IsSaving = true;
        try
        {
            string lIdToken = await GetFirebaseIdToken();
            string lJsonData = JsonUtility.ToJson(pData);

            bool lSuccess = await SendRequest($"{API_BASE_URL}/api/save", "POST", lJsonData, lIdToken);
            return lSuccess;
        }
        catch (Exception lEx)
        {
            return false;
        }
        finally { _IsSaving = false; }
    }

    public async Task<T> LoadData<T>() where T : class
    {
        if (_IsLoading || !IsSignedIn) return null;

        _IsLoading = true;
        try
        {
            string lIdToken = await GetFirebaseIdToken();
            string lJsonResponse = await SendRequestGet($"{API_BASE_URL}/api/load", lIdToken);

            if (string.IsNullOrEmpty(lJsonResponse))
            {
                return null;
            }

            T lData = JsonUtility.FromJson<T>(lJsonResponse);
            return lData;
        }
        catch (Exception lEx)
        {
            return null;
        }
        finally { _IsLoading = false; }
    }

    public async Task<SaveData[]> GetLeaderboard()
    {
        if (_IsLoading) return null;

        try
        {
            string lIdToken = await GetFirebaseIdToken();
            string lJsonResponse = await SendRequestGet($"{API_BASE_URL}/api/leaderboard", lIdToken);

            if (string.IsNullOrEmpty(lJsonResponse)) return null;
            JsonArrayWrapper lData = JsonUtility.FromJson<JsonArrayWrapper>(lJsonResponse);

            if (lData == null || lData.entries == null)
            {
                Debug.LogError("Error : empty wrapper");
                return null;
            }

            return lData.entries;
        }
        catch (Exception lEx)
        {
            Debug.LogError($"Error : {lEx.Message}");
            return null;
        }
    }

    private async Task<string> GetFirebaseIdToken()
    {
        if (_CurrentUser == null) throw new Exception("User null");
        return await _CurrentUser.TokenAsync(true); 
    }

    private async Task<bool> SendRequest(string pUrl, string pMethod, string pJsonData, string pToken)
    {
        using (UnityWebRequest lRequest = new UnityWebRequest(pUrl, pMethod))
        {
            byte[] lBody = Encoding.UTF8.GetBytes(pJsonData);
            lRequest.uploadHandler = new UploadHandlerRaw(lBody);
            lRequest.downloadHandler = new DownloadHandlerBuffer();
            lRequest.SetRequestHeader("Content-Type", "application/json");
            lRequest.SetRequestHeader("Authorization", "Bearer " + pToken);

            UnityWebRequestAsyncOperation lOperation = lRequest.SendWebRequest();
            while (!lOperation.isDone) await Task.Yield();

            if (lRequest.result == UnityWebRequest.Result.Success) return true;

            Debug.LogError($"API Error: {lRequest.error} : {lRequest.downloadHandler.text}");
            return false;
        }
    }

    private async Task<string> SendRequestGet(string pUrl, string pToken)
    {
        using (UnityWebRequest lRequest = UnityWebRequest.Get(pUrl))
        {
            lRequest.SetRequestHeader("Authorization", "Bearer " + pToken);
            UnityWebRequestAsyncOperation lOperation = lRequest.SendWebRequest();
            while (!lOperation.isDone) await Task.Yield();

            if (lRequest.result == UnityWebRequest.Result.Success) return lRequest.downloadHandler.text;

            Debug.LogError($"API Error: {lRequest.error} : {lRequest.downloadHandler.text}");
            throw new Exception(lRequest.error);
        }
    }
    
    
}


