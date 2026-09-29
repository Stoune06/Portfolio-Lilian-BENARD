using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [Header("Transition")]
    public GameObject fakeCoverObject;
    public float fakeCoverCloseDuration = 1.0f;

    [Header("Gestion des Cam�ras")]
    public Camera bookCamera;
    public Camera gameCamera;

    [SerializeField]
    private ParticleSystem _PaperParticles;

    [SerializeField]
    private GameObject _Potion;

    [SerializeField]
    private FloatingPotionsManager _PotionAnchor;

    public static GameManager instance { get; private set; }

    private HUDManager _HudManager;
    private AudioManager _AudioManager;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        AudioSignals.TriggerMusic(AudioClipsEnum.ReflectionPhaseMusic);
        _HudManager = HUDManager.instance;
        _AudioManager = AudioManager.instance;

        SubscribeToEvents();
        GameStateManager.canRotateCamera = false;
    }

    private void SubscribeToEvents()
    {
        BookLevelSelector.onLevelSelected += OnNewLevelSelected;
        ReflectionPanel.onButtonStartPressed += StartActionPhase;
        LevelManager.onWin += OnGameWon;
        LevelManager.onGameOver += OnGameLost;

        TilesPlacer.onTilePlaced += _HudManager.UpdateReflectionHUD;

        InputManager.onResetLevel += () => OnRestart(false);
        InputManager.onPause += OnPause;
        GameOverHUD.onClearButtonPressed += () => OnRestart(true);
        GameOverHUD.onRetryButtonPressed += () => OnRestart(false);

        HUDManager.onSpeedChanged += TickManager.UpdateSpeed;
        HUDManager.onPauseButtonPressed += OnPause;

        WinHUD.onReturnToMenu += ReturnToLevelSelector;

        PausePanel.onReturnToMenu += ReturnToLevelSelector;
        PausePanel.onRetryToMenu += () => OnRestart(true);
        PausePanel.onContinue += OnGameResume;

        ReflectionPanel.onClearActionsButtonPressed += OnResetLevel;
    }


    private void UnsubscribeFromEvents()
    {
        BookLevelSelector.onLevelSelected -= OnNewLevelSelected;
        ReflectionPanel.onButtonStartPressed -= StartActionPhase;
        LevelManager.onWin -= OnGameWon;
        LevelManager.onGameOver -= OnGameLost;

        TilesPlacer.onTilePlaced -= _HudManager.UpdateReflectionHUD;

        InputManager.onResetLevel -= () => OnRestart(false);
        GameOverHUD.onClearButtonPressed -= () => OnRestart(true);

        HUDManager.onSpeedChanged -= TickManager.UpdateSpeed;
        HUDManager.onPauseButtonPressed -= OnPause;

        WinHUD.onReturnToMenu -= ReturnToLevelSelector;

        PausePanel.onReturnToMenu -= ReturnToLevelSelector;
        PausePanel.onRetryToMenu -= () => OnRestart(true);
        PausePanel.onContinue -= OnGameResume;
    }

    private void OnNewLevelSelected(LevelData pLevel)
    {
        _Potion.SetActive(true);
        _PaperParticles.gameObject.SetActive(true);

        _PotionAnchor.gameObject.SetActive(false);

        BookLevelSelector.instance.SetupForReturn();

        _HudManager.SetReflectionHUD(pLevel.levelInventory);

        LevelManager.LoadLevel(pLevel);

        _HudManager.ShowReflectionHUD();
        SwitchToGameView();

        GameStateManager.canRotateCamera = true;
        if (SceneTransition.instance != null)
        {
            StartCoroutine(SceneTransition.instance.FadeIn());
        }
    }

    public void StartActionPhase()
    {
        AudioSignals.TriggerMusic(AudioClipsEnum.ActionPhaseMusic);
        TickManager.Resume();
        LevelManager.StartActionPhase();
        _HudManager.ShowActionHUD();
    }

    private void OnResetLevel()
    {
        LevelManager.ClearActions();
    }

    private void ReturnToLevelSelector()
    {
        
        StartCoroutine(ReturnToMenuSequence());
    }

    public void StartReflectionPhase()
    {
        AudioSignals.TriggerMusic(AudioClipsEnum.ReflectionPhaseMusic);
        TickManager.Pause();
        _HudManager.ShowReflectionHUD();
    }

    private void OnPause()
    {
        TickManager.Pause();
        _HudManager.ShowPausePanel();
        LevelManager.SetPause();
    }

    private void OnGameResume()
    {
        if (!GameStateManager.isActionPhase)
        { 
            _HudManager.ShowReflectionHUD();
            LevelManager.SetResume();
        } 
        else
        {
            _HudManager.ShowActionHUD();
            TickManager.Resume();
            GameStateManager.isPaused = false;
        } 
    }

    private void OnGameWon()
    {
        SessionManager.MarkLevelComplete(LevelManager.currentLevel.name);
        TickManager.Pause();
        _HudManager.HideAllPanels();
        AudioSignals.TriggerSound(AudioClipsEnum.UIWin, transform.position);
        StartCoroutine(AutoReturnSequence());
    }

    private IEnumerator AutoReturnSequence()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(ReturnToMenuSequence());
    }

    private void OnGameLost(Cube pCube)
    {
        TickManager.Pause();
        AudioSignals.TriggerSound(AudioClipsEnum.UIGameOver, transform.position);
        AudioSignals.TriggerStopMusic();
        _HudManager.ShowGameOverScreen();
    }

    private void OnRestart(bool pClearAction)
    {
        if (LevelManager.currentLevel != null)
        {
            _HudManager.SetReflectionHUD(LevelManager.currentLevel.levelInventory);
        }
        StartReflectionPhase();
        LevelManager.RestartLevel(pClearAction);
    }

    private IEnumerator ReturnToMenuSequence()
    {
        _HudManager.HideAllPanels(); 
        fakeCoverObject.SetActive(true);
        Animator lCoverAnim = fakeCoverObject.GetComponent<Animator>();
        lCoverAnim.SetTrigger("CloseFakeCover");
        yield return new WaitForSeconds(fakeCoverCloseDuration);
        _PaperParticles.gameObject.SetActive(false);
        _Potion.SetActive(false);
        AudioSignals.TriggerSound(AudioClipsEnum.ClosingBook, transform.position);
        AudioSignals.TriggerStopMusic();


        LevelManager.UnloadCurrentLevel(true);
        GameStateManager.canRotateCamera = false;

        _HudManager.ShowLevelSelector();
        _HudManager.ShowSoundPanel();
        _PotionAnchor.gameObject.SetActive(true);
        _PotionAnchor.RefreshPotions();
        BookLevelSelector.instance.SetupForReturn();

        SwitchToBookView();
        fakeCoverObject.SetActive(false);
        BookLevelSelector.instance.AnimateZoomOut();
    }

    private void SwitchToBookView()
    {
        if (bookCamera != null && gameCamera != null)
        {
            bookCamera.depth = 10;
            gameCamera.depth = 0;
        }
    }

    private void SwitchToGameView()
    {
        if (bookCamera != null && gameCamera != null)
        {
            gameCamera.depth = 10;
            bookCamera.depth = 0;
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
