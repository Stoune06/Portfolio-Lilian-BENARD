using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class LevelManager : OnTickBasedObject
{

    private static LevelData _CurrentLevel;
    
    public static LevelData currentLevel
    {
        get => _CurrentLevel;
        private set
        {
            if (_CurrentLevel != value)
            {
                _CurrentLevel = value;
                onCurrentLevelChanged?.Invoke(value);
            }
        }
    }
    
    public static event Action<LevelData> onCurrentLevelChanged;
    private static GameObject _CurrentLevelGameObject;

    private static int _CubesSpawned = 0;
    private static int _CubesTargetReached = 0;
    public static LevelManager instance { get; private set; }

    public static event Action onWin;
    public static event Action<Cube> onGameOver;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        TickManager.onTick += OnTickEvent;
        SpawnPoint.onSpawn += OnCubeSpawn;
        Target.onTargetReached += OnTargetReached;

        Cube.onCollisionEvent += GameOver;
        Cube.onOutOfLevel += GameOver;
    }

    private void GameOver(Cube pCube)
    {
        onGameOver?.Invoke(pCube);
        //TickManager.Pause();
    }

    private void OnTargetReached()
    {
        _CubesTargetReached++;
    }

    private void OnCubeSpawn()
    {
        _CubesSpawned++;
    }

    protected override void OnTickEvent()
    {
        base.OnTickEvent();
        CheckWin();
    }

    private void CheckWin()
    {
        if(_CubesSpawned == _CubesTargetReached && _CubesSpawned > 0)
        {
            onWin?.Invoke();
            //TickManager.Pause();
        }
    }

    public static void UnloadCurrentLevel(bool pClearAction)
    {
        Destroy(_CurrentLevelGameObject);
        CubeManager.ClearAllCubes();
        if(pClearAction)TilesPlacer.ClearAllTiles();
        else SwitchState.ResetRotations();
        _CubesSpawned = 0;
        _CubesTargetReached = 0;
    }

    public static void LoadLevel(LevelData pLevel)
    {
        if (_CurrentLevelGameObject != null)
        {
            UnloadCurrentLevel(true);
        }
        _CurrentLevelGameObject = Instantiate(pLevel.levelPrefab);
        currentLevel = pLevel;
        FallState.levelYLimit = pLevel.YBound;
        TilesInventoryManager.availableTiles = currentLevel.levelInventory;
        TilesPlacer.Init();

        GameStateManager.isActionPhase = false;
        TilesPlacer.SetResumeMod();
        GameStateManager.isPaused = false;
    }

    private static void ReloadCurrentLevel()
    {
        Destroy(_CurrentLevelGameObject);
        _CurrentLevelGameObject = Instantiate(currentLevel.levelPrefab);
        GameStateManager.isActionPhase = false;
        TilesPlacer.Init();
        TilesPlacer.SetResumeMod();
        GameStateManager.isPaused = false;
    }

    public static void StartActionPhase()
    {
        GameStateManager.isActionPhase = true;
    }


    public static void RestartLevel(bool pClearAction)
    {
        UnloadCurrentLevel(pClearAction);
        TickManager.Restart();
        ReloadCurrentLevel();
        _CubesSpawned = 0;
        _CubesTargetReached = 0;
        ClearAllCubes();
    }

    public static void SetPause()
    {
        TilesPlacer.SetPauseMod();
        GameStateManager.isPaused = true;
    }

    public static void ClearActions()
    {
        TilesPlacer.ClearAllTiles();
        LoadLevel(currentLevel);
        if (GameStateManager.isActionPhase) GameStateManager.isActionPhase = false;
    }

    public static void SetResume()
    {
        TilesPlacer.SetResumeMod();
        GameStateManager.isPaused = false;
    }

    private static void ClearAllCubes()
    {
        CubeManager.ClearAllCubes();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
