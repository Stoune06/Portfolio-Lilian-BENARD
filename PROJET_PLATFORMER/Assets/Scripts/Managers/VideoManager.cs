using Tooling;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

//Author : Noé SALES
namespace Platformer.Managers
{
    public class VideoManager : Singleton<VideoManager>
    {
        [Header("Settings")]
        [SerializeField] private float _HoldDuration = 1.5f;
        [Header("Videos")]
        [SerializeField] private VideoClip _IntroVideoClip;
        [SerializeField] private VideoClip _OutroVideoClip;
        [Header("Objects")]
        [SerializeField] private VideoPlayer _CinematicVideo;
        [SerializeField] private Image _SkipCircle;
        [Header("Prefabs")]
        [SerializeField] private GameObject _LevelOpeningPrefab;
        private LevelOpening _CurrentOpening;

        private float _HoldTimer = 0f;

        private GameManager _GameManager;
        private Coroutine CinematicCoroutine;

        private VideoClip _CurrentVideo = null;

        private FMOD.Studio.EventInstance introInstance;
        private void Start()
        {
            _GameManager = GetComponent<GameManager>();
        }

        private void Update()
        {
            if (CinematicCoroutine == null) return;

            bool lHoldInput = false;

#if UNITY_EDITOR || UNITY_STANDALONE
            lHoldInput = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
#endif

#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                Touch lTouch = Input.GetTouch(0);
                lHoldInput = lTouch.phase == TouchPhase.Stationary || lTouch.phase == TouchPhase.Moved;
            }
#endif

            if (lHoldInput)
            {
                _HoldTimer += Time.unscaledDeltaTime;

                if (_HoldTimer >= _HoldDuration)
                {
                    SkipCinematic();
                    _HoldTimer = 0f;
                }
            }
            else
            {
                _HoldTimer = 0f;
            }
            _SkipCircle.fillAmount = _HoldTimer / _HoldDuration;
        }

        private void SkipCinematic()
        {
            if (CinematicCoroutine == null) return;

            StopCoroutine(CinematicCoroutine);
            _CinematicVideo.Stop();
            CinematicEnd();
        }

        public void PlayIntroVideo(bool pIsDebug)
        {
            CreateLevelOpening();

            if (pIsDebug)
            {
                _CurrentOpening.OpenTransitionScreen();
                return;
            }

            if (_GameManager.GetCurrentData().currentLevel == 0 && _GameManager.GetCurrentData().lastCheckpoint == 0 && CinematicCoroutine == null) 
                CinematicCoroutine = StartCoroutine(PlayCinematic(_IntroVideoClip));
            else _CurrentOpening.OpenTransitionScreen();
        }
        public void PlayOutroVideo()
        {
            if (CinematicCoroutine != null) return;
            CinematicCoroutine = StartCoroutine(PlayCinematic(_OutroVideoClip));
        }


        private System.Collections.IEnumerator PlayCinematic(VideoClip pVideo)
        {
            _SkipCircle.fillAmount = 0f;
            _HoldTimer = 0f;
            _CurrentVideo = pVideo;

            _CinematicVideo.gameObject.SetActive(true);
            InputManager.EnabledInputControl = false;

            _CinematicVideo.clip = pVideo;

            _CinematicVideo.Play();
            while (_CinematicVideo.isPlaying) yield return null;

            CinematicEnd();
        }

        private void CinematicEnd()
        {
            if (_CurrentOpening != null) _CurrentOpening.OpenTransitionScreen();
            _CinematicVideo.clip = null;
            _CinematicVideo.gameObject.SetActive(false);

            InputManager.EnabledInputControl = true;
            CinematicCoroutine = null;


            if (_CurrentVideo == _OutroVideoClip) LoadManager.Instance.GoToMainMenu(true, true);


            GameManager.introInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            GameManager.OUTROInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

        private void CreateLevelOpening()
        {
            if (_CurrentOpening != null) return;

            GameObject lObj = Instantiate(_LevelOpeningPrefab, transform);
            _CurrentOpening = lObj.GetComponent<LevelOpening>();

        }
    }
}