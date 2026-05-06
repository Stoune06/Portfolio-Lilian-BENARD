using Firebase.Auth;
using Platformer.Areas;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tooling;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

//Author : Sales Noé
namespace Platformer.Managers
{
    public class LoadManager : Singleton<LoadManager>
    {
        [Header("Debug")]
        [SerializeField] private bool _Debug = false;
        [SerializeField] private int _DebugLevel = 0;
        [SerializeField] private int _DebugCheckPoints = 0;
        [Header("Scenes")]
        [SerializeField] private string _MainMenuPath;
        [SerializeField] private List<string> _InGamePaths = new List<string>();

        private DataManager _DataManager;
        private GameManager _GameManager;

        private bool _IsLoading = false;
        private string _DebugScenePath = default;

        void Start()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            _DataManager = DataManager.Instance;
            _GameManager = GameManager.Instance;

#if UNITY_EDITOR
            if (_Debug && !string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
            {
                _DebugScenePath = EditorSceneManager.GetActiveScene().path;
                InitializeDebugData();
            }
#endif

            if (GameManager.IsOffline) GoToMainMenu(false);
        }

        private void InitializeDebugData()
        {
            SaveData lCurrentSave = new SaveData { currentLevel = _DebugLevel, lastCheckpoint = _DebugCheckPoints };
            _GameManager.SetCurrentData(lCurrentSave);
            _GameManager.SetCheckPoint();
            _GameManager.InitializeLevel();
        }

        public bool IsDebug() => _Debug;
        public int LevelsCount() => _InGamePaths.Count - 1;

        public void LoadLastSave()
        {
            SaveData lCurrentSave = _GameManager.GetCurrentData();
            if (lCurrentSave == null) lCurrentSave = new SaveData();

            StartCoroutine(LoadInGame());
        }

        private void OnUserSignedIn(FirebaseUser pUser)
        {
            _GameManager.PlayOffline(false);
            GoToMainMenu(false);
        }

        private void PlayOffline()
        {
            _GameManager.PlayOffline(true);
            GoToMainMenu(false);
        }

        public void GoToMainMenu(bool pIsReturn = true, bool pEndGame = false)
        {
            _ = LoadMainMenu(pIsReturn, pEndGame);
        }

        private async Task LoadMainMenu(bool pIsReturn = true, bool pEndGame = false)
        {
            if (_GameManager == null) return;

            SaveData lCurrentSave;
            if (!pIsReturn)
            {
                if (GameManager.IsOffline)
                {
                    if (_Debug) lCurrentSave = new SaveData { currentLevel = _DebugLevel, lastCheckpoint = _DebugCheckPoints };
                    else lCurrentSave = new SaveData { currentLevel = 0, lastCheckpoint = 0 };
                }
                else lCurrentSave = await _DataManager.LoadData<SaveData>() ?? new SaveData();
                _GameManager.SetCurrentData(lCurrentSave);
            }

            AsyncOperation lAsyncLoad = null;

            if (!_Debug)
            {
                lAsyncLoad = SceneManager.LoadSceneAsync(_MainMenuPath);

                while (!lAsyncLoad.isDone)
                    await Task.Yield();
            }
            else PlayInGame();

            LevelOpening.musicInstance1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            LevelOpening.ambInstance1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            LevelOpening.musicInstance2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            LevelOpening.ambInstance2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            if (pEndGame)
            {
                GameObject lCreditButton = GameObject.Find("Credits");
                LoadPrefab lLoader = lCreditButton.GetComponent<LoadPrefab>();
                HideUi lHider = lCreditButton.GetComponent<HideUi>();
                lLoader.Load();
                lHider.Hide();
            }
        }

        public void LoadNextLevel()
        {
            SaveData lCurrentSave = _GameManager.GetCurrentData();
            lCurrentSave.currentLevel++;
            lCurrentSave.lastCheckpoint = 0;
            _GameManager.SetCurrentData(lCurrentSave);

            if (_Debug) _DebugScenePath = _InGamePaths[lCurrentSave.currentLevel];

            StartCoroutine(LoadInGame(lCurrentSave.currentLevel));
        }

        private async void PlayInGame()
        {
            if (_GameManager == null) _GameManager = GameManager.Instance;

            if (_GameManager == null) return;

            if (SceneManager.sceneCount > 2)
            {
                AsyncOperation lAsyncUnload = SceneManager.UnloadSceneAsync(_MainMenuPath);
                while (!lAsyncUnload.isDone) await Task.Yield();
            }

            SaveData lSave = _GameManager.GetCurrentData();

            if (lSave == null)
            {
                Debug.LogWarning("Save not ready yet");
                return;
            }

            int lTargetLevel = lSave.currentLevel;

            StartCoroutine(LoadInGame(lTargetLevel));
        }

        private IEnumerator LoadInGame(int pLevel = 0)
        {
            if (_IsLoading) yield break;
            if (pLevel < 0 || pLevel >= _InGamePaths.Count) yield break;

            _IsLoading = true;

            string lScenePath = _InGamePaths[pLevel];

            if (_Debug) lScenePath = _DebugScenePath;

            AsyncOperation lAsyncLoad = SceneManager.LoadSceneAsync(lScenePath);

            while (!lAsyncLoad.isDone)
            {
                yield return null;
            }

            yield return new WaitForEndOfFrame();

            _GameManager.SetCheckPoint();
            _GameManager.InitializeLevel();

            _IsLoading = false;
        }
        private void ResetManagerData()
        {
            _IsLoading = false;
            if (GameManager.Instance != null) GameManager.Instance.SetCurrentData(null);
        }
        private void OnEnable()
        {
            DataManager.OnUserSignedIn += OnUserSignedIn;
            DataManager.OnUserSignedOut += ResetManagerData;
            UIAuthManager.OfflinePlayEvent += PlayOffline;
            MainMenuButton.BackToMainMenuEvent += () => GoToMainMenu();
            PlayButton.OnPlay += PlayInGame;
        }
        private void OnDisable()
        {
            DataManager.OnUserSignedIn -= OnUserSignedIn;
            DataManager.OnUserSignedOut -= ResetManagerData;
            UIAuthManager.OfflinePlayEvent -= PlayOffline;
            MainMenuButton.BackToMainMenuEvent -= () => GoToMainMenu();
            PlayButton.OnPlay -= PlayInGame;
        }
    }
}