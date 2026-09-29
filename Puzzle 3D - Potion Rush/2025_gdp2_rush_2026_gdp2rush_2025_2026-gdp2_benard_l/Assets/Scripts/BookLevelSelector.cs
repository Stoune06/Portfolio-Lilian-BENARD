using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookLevelSelector : MonoBehaviour
{
    public static BookLevelSelector instance;

    [Header("UI Menu")]
    public Button playButton;
    public Animator bookAnimator;
    public string openTriggerName = "OpenBook";

    [Header("Configuration Pages")]
    public Transform pagesParent;
    public GameObject pagePrefab;
    public LevelData[] levels;
    public float pageThickness = 0.01f;

    [Header("Transition Cam�ra (Zoom)")]
    [SerializeField] private float _ZoomDuration = 1.0f;
    [SerializeField] private float _StopDistance = 2.0f;
    [SerializeField] private AnimationCurve _ZoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Transition Retour")]
    public Transform coverTarget;

    public static event Action<LevelData> onLevelSelected;
    public static event Action onBookOpened;

    private List<PageLogic> _instantiatedPages = new List<PageLogic>();
    private int _currentTopPageIndex = 0;
    private Vector3 _OriginalCamPos;
    private Quaternion _OriginalCamRot;
    private bool _IsAnimating = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (Camera.main != null)
        {
            _OriginalCamPos = Camera.main.transform.position;
            _OriginalCamRot = Camera.main.transform.rotation;
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OpenBook);
        }

        GeneratePages();

        UpdateActivePreviews();
    }

    public void SelectLevel(LevelData level, Transform targetPage)
    {
        if (_IsAnimating) return;
        StartCoroutine(ZoomRoutine(level, targetPage));
    }

    private IEnumerator ZoomRoutine(LevelData pLevel, Transform pTarget)
    {
        _IsAnimating = true;
        Camera lCam = Camera.main;

        if (lCam != null && pTarget != null)
        {
            Vector3 lStartPos = lCam.transform.position;
            Vector3 lEndPos = pTarget.position - (lCam.transform.forward * _StopDistance);

            float lElapsed = 0f;
            while (lElapsed < _ZoomDuration)
            {
                float lRatio = lElapsed / _ZoomDuration;
                lCam.transform.position = Vector3.Lerp(lStartPos, lEndPos, _ZoomCurve.Evaluate(lRatio));
                lElapsed += Time.deltaTime;
                yield return null;
            }
            lCam.transform.position = lEndPos;
        }
        if (SceneTransition.instance != null)
        {
            yield return StartCoroutine(SceneTransition.instance.FadeOut());
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }
        onLevelSelected?.Invoke(pLevel);

        if (lCam != null)
        {
            lCam.transform.position = _OriginalCamPos;
            lCam.transform.rotation = _OriginalCamRot;
        }

        _IsAnimating = false;
    }

    public void OpenBook()
    {
        if (bookAnimator != null) bookAnimator.SetTrigger(openTriggerName);
        onBookOpened?.Invoke();
    }

    void GeneratePages()
    {
        foreach (Transform lChild in pagesParent) Destroy(lChild.gameObject);
        _instantiatedPages.Clear();

        for (int i = levels.Length-1 ; i >= 0; i--)
        {
            GameObject pageObj = Instantiate(pagePrefab, pagesParent);
            pageObj.name = "Page_" + (i + 1);
            pageObj.transform.localPosition = new Vector3(-0.25f, i * pageThickness + 0.1f, 0);

            PageLogic logic = pageObj.GetComponent<PageLogic>();
            if (logic != null)
            {
                logic.Setup(levels[i], i);
                if(i == levels.Length-1) logic.backButton.gameObject.SetActive(false);
                else if (i == 0) logic.nextButton.gameObject.SetActive(false);
                _instantiatedPages.Add(logic);
            }
            pageObj.SetActive(true);
        }
    }

    public void OnNextButtonPressed()
    {
        if (_currentTopPageIndex < _instantiatedPages.Count)
        {
            _instantiatedPages[_currentTopPageIndex].AnimateTurnToLeft();
            _currentTopPageIndex++;

            UpdateActivePreviews();
        }
    }

    public void OnBackButtonPressed()
    {
        if (_currentTopPageIndex > 0)
        {
            _currentTopPageIndex--;
            _instantiatedPages[_currentTopPageIndex].AnimateTurnToRight();
            UpdateActivePreviews();
        }
    }

    private void OnEnable()
    {
        UpdateActivePreviews();
    }
    private void OnDisable()
    {
        if (_instantiatedPages != null)
        {
            foreach (var page in _instantiatedPages)
            {
                if (page != null) page.SetPreviewActive(false);
            }
        }
    }

    private void ToggleAllPreviews(bool isActive)
    {
        if (_instantiatedPages != null)
        {
            foreach (var page in _instantiatedPages)
            {
                if (page != null)
                {
                    page.SetPreviewActive(isActive);
                }
            }
        }
    }

    public void SetupForReturn()
    {
        if (bookAnimator != null)
        {
            bookAnimator.Play("Closed", 0, 0f);
            bookAnimator.Update(0f);
        }

        Camera lCam = Camera.main;
        if (lCam != null && coverTarget != null)
        {
            Vector3 lClosePos = coverTarget.position - (lCam.transform.forward * _StopDistance);
            lCam.transform.position = lClosePos;
            lCam.transform.rotation = _OriginalCamRot;
        }

        _currentTopPageIndex = 0;

        if (_instantiatedPages != null)
        {
            foreach (var lPage in _instantiatedPages)
            {
                if (lPage != null && lPage.pageAnimator != null)
                {
                    lPage.pageAnimator.Rebind();
                    lPage.pageAnimator.Update(0f);
                }
            }
        }
        UpdateActivePreviews();
    }

    private void UpdateActivePreviews()
    {
        if (_instantiatedPages == null) return;

        for (int i = 0; i < _instantiatedPages.Count; i++)
        {
            if (_instantiatedPages[i] == null) continue;

            bool shouldBeActive = (i == _currentTopPageIndex) || (i == _currentTopPageIndex + 1);

            _instantiatedPages[i].SetPreviewActive(shouldBeActive);
        }
    }

    public void AnimateZoomOut()
    {
        StartCoroutine(ZoomOutRoutine());
    }

    private IEnumerator ZoomOutRoutine()
    {
        _IsAnimating = true;
        Camera lCam = Camera.main;

        if (lCam != null)
        {
            Vector3 lStartPos = lCam.transform.position;
            float lElapsed = 0f;
            while (lElapsed < _ZoomDuration)
            {
                float lRatio = lElapsed / _ZoomDuration;
                lCam.transform.position = Vector3.Lerp(lStartPos, _OriginalCamPos, _ZoomCurve.Evaluate(lRatio));
                lElapsed += Time.deltaTime;
                yield return null;
            }
            lCam.transform.position = _OriginalCamPos;
        }
        if (playButton != null) playButton.gameObject.SetActive(true);
        _IsAnimating = false;
        AudioSignals.TriggerMusic(AudioClipsEnum.ReflectionPhaseMusic);
    }
}