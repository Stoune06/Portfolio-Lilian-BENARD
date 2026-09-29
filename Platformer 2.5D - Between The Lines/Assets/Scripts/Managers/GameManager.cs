using FMOD.Studio;
using FMODUnity;
using Platformer.Areas;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tooling;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

//Author : Sales Noé
namespace Platformer.Managers
{
	public class GameManager : Singleton<GameManager>
	{
        public static event System.Action LevelTransitionStartEvent;

        private List<CheckPoint> _CheckPoints = new List<CheckPoint>();

        private SaveData CurrentSave = null;
        private GameObject _CurrentCinematicObject;
        private VideoManager _VideoManager;

        [Header("Settings")]
        [SerializeField] private bool _IsOffline = false;
        [Header("Prefabs")]
        [SerializeField] private GameObject _CinematicPrefab;
        [SerializeField] private GameObject _CinematicScreen;

        [SerializeField] private EventReference introEvent;

        public static EventInstance introInstance;
        public static EventInstance OUTROInstance;

        public static bool IsOffline { get; private set; }

        private void Start()
        {
            _VideoManager = GetComponent<VideoManager>();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            IsOffline = _IsOffline;
        }

        #region Get Set Fonctions
        public void PlayOffline(bool pIsOffline) => IsOffline = pIsOffline;

        public SaveData GetCurrentData() => CurrentSave;

        private CheckPoint GetCheckPoint(int pIndex) => _CheckPoints[pIndex];

        #endregion

        #region Set Fonctions
        public void SetCurrentData(SaveData pData)
        {
            CurrentSave = pData;
        }

        public void SetTransitionScreenState(bool pState)
        {
            _CinematicScreen.SetActive(pState);
        }

        public void SetCheckPoint()
        {
            _CheckPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None).ToList();
            if (_CheckPoints.Count <= 0) return;

            _CheckPoints.Sort((pA, pB) => pA.transform.position.x.CompareTo(pB.transform.position.x));

            for (int i = 0; i < _CheckPoints.Count; i++)
            {
                _CheckPoints[i].SetIndex(i);
                if (i <= CurrentSave.lastCheckpoint) _CheckPoints[i].ActiveCheckPoint();
            }
        }
        #endregion

        #region LevelTransition
        public void StartLevelTransition()
        {
            if (Player.Player.Instance != null)
            {
                Player.Player.Instance.gameObject.SetActive(false);
                Player.Player.Instance.SetVelocityX(0);
            }

            if (CurrentSave.currentLevel == LoadManager.Instance.LevelsCount())
            {
                _VideoManager.PlayOutroVideo();
                OnOutroVideoStart();
            }
            else
            {
                _CurrentCinematicObject = Instantiate(_CinematicPrefab, null);
                PlayableDirector lCinematic = _CurrentCinematicObject.GetComponentInChildren<PlayableDirector>();

                lCinematic.Play();

                OnLevelTranstionCinematicStart();
            }
        }

        public void EndLevelTransition()
        {
            OnLevelTranstionCinematicEnd();
            SetTransitionScreenState(true);
            if (Player.Player.Instance != null) Player.Player.Instance.gameObject.SetActive(true);
        }
        public void InitializeLevel()
        {
            Player.Player lPlayer = Player.Player.Instance;

            if (_CheckPoints == null || _CheckPoints.Count <= 0)
            {
                Debug.LogWarning("No checkpoints found in this scene");
                return;
            }

            int lIndex = CurrentSave.lastCheckpoint;
            if (lIndex < 0 || lIndex >= _CheckPoints.Count) lIndex = 0;

            CheckPoint lLastCheckPoint = GetCheckPoint(lIndex);

            if (lPlayer != null && lLastCheckPoint != null)
            {
                lPlayer.transform.position = new Vector3(lLastCheckPoint.transform.position.x, lLastCheckPoint.transform.position.y, lPlayer.transform.position.z);
            }

            _VideoManager.PlayIntroVideo(LoadManager.Instance.IsDebug());
            if(CurrentSave.currentLevel == 0)OnIntroVideoStart();
        }

        public void GoToCheckPoint(int pAddIndex)
        {
            int lNextCheckPoint = CurrentSave.lastCheckpoint + pAddIndex;

            if (lNextCheckPoint >= _CheckPoints.Count - 1) return;

            Vector3 lPos = GetCheckPoint(lNextCheckPoint).transform.position;

            Player.Player.Instance.transform.position = new Vector3(lPos.x, lPos.y, Player.Player.Instance.transform.position.z);

            _VideoManager.PlayIntroVideo(LoadManager.Instance.IsDebug());
        }
        #endregion

        #region FMOD Fonction

        private void OnLevelTranstionCinematicStart()
        {
            LevelOpening.musicInstance1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            LevelOpening.ambInstance1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        private void OnLevelTranstionCinematicEnd()
        {

        }
        private void OnOutroVideoStart()
        {
            LevelOpening.musicInstance2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            LevelOpening.ambInstance2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            OUTROInstance = RuntimeManager.CreateInstance("event:/Music/MUS_Outro");
            OUTROInstance.start();
        }
        private void OnIntroVideoStart()
        {
           
            
                introInstance = RuntimeManager.CreateInstance("event:/Music/MUS_Intro");
                introInstance.start();
            
        }

        #endregion

        #region Signals Connect

        private void OnEnable()
        {
            Player.Player.OnPlayerRespawn += GoToCheckPoint;
            PauseManage.onReturnCheckpoint += GoToCheckPoint;
        }
        private void OnDisable()
        {
            Player.Player.OnPlayerRespawn -= GoToCheckPoint;
            PauseManage.onReturnCheckpoint -= GoToCheckPoint;
        }
        #endregion
    }
}