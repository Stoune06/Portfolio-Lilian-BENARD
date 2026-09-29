using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PageLogic : MonoBehaviour
{
    [Header("UI & Visuel")]
    public TextMeshPro levelNameText;
    public Renderer pageRenderer;

    [Header("Interaction & Boutons")]
    [SerializeField] private Canvas _Canvas;
    public Button playButton;
    public Button nextButton;
    public Button backButton;

    [Header("Animation")]
    public Animator pageAnimator;

    private LevelData _data;
    private GameObject _previewInstance;
    private int _myPageIndex;

    public LevelPreviewSetup previewSetup;

    private static float _spawnOffsetY = -1000f;
    [SerializeField] private float _RotationSpeed = 10f;

    public void Setup(LevelData level, int pageIndex)
    {
        _data = level;
        _myPageIndex = pageIndex;

        if (levelNameText != null) levelNameText.text = level.name;
        if (pageAnimator == null) pageAnimator = GetComponent<Animator>();

        SetupButtons();
        SetupLevelPreview(level);

        // Assigne la caméra pour les clics UI
        if (_Canvas != null) _Canvas.worldCamera = Camera.main;
    }

    private void SetupButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(LaunchLevel);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => BookLevelSelector.instance.OnNextButtonPressed());
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => BookLevelSelector.instance.OnBackButtonPressed());
        }
    }

    private void Update()
    {
        // Rotation du niveau preview (optionnel, pour l'effet visuel sur la page)
        if (_previewInstance != null)
        {
            foreach (Transform child in _previewInstance.transform)
            {
                if (child.GetComponent<Camera>() == null && child.GetComponent<LevelPreviewSetup>() == null)
                {
                    child.RotateAround(_previewInstance.transform.position, Vector3.up, _RotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void SetupLevelPreview(LevelData level)
    {
        if (level.levelPrefab != null && pageRenderer != null)
        {
            _spawnOffsetY -= 100f;
            Vector3 hiddenPosition = new Vector3(0, _spawnOffsetY, 0);

            _previewInstance = Instantiate(level.levelPrefab, hiddenPosition, Quaternion.identity);
            _previewInstance.transform.localScale = Vector3.one * level.previewScale;
            SetLayerRecursively(_previewInstance, LayerMask.NameToLayer("LevelPreviewLayer"));

            LevelPreviewSetup previewSetup = _previewInstance.GetComponentInChildren<LevelPreviewSetup>();

            if (previewSetup != null)
            {
                previewSetup.GeneratePreview(pageRenderer);

                if (previewSetup.previewTexture != null)
                {
                    pageRenderer.material.mainTexture = previewSetup.previewTexture;
                }
            }
        }
    }
    private void LaunchLevel()
    {
        if (_data != null)
        {
            Transform targetZoom = pageRenderer != null ? pageRenderer.transform : this.transform;

            BookLevelSelector.instance.SelectLevel(_data, targetZoom);
        }
    }

    public void SetPreviewActive(bool isActive)
    {
        if (_previewInstance != null)
        {
            _previewInstance.SetActive(isActive);
        }
    }

    void OnMouseDown() { LaunchLevel(); }

    public void AnimateTurnToLeft() { if (pageAnimator != null) pageAnimator.SetTrigger("ClosePage"); }
    public void AnimateTurnToRight() { if (pageAnimator != null) pageAnimator.SetTrigger("OpenPage"); }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }

    private void OnDestroy()
    {
        if (_previewInstance != null) Destroy(_previewInstance);
    }
}