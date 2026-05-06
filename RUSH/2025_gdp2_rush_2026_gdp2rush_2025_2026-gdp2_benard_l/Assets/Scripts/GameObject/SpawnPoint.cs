using System;
using UnityEngine;

public class SpawnPoint : OnTickBasedObject
{
    [Header("Settings")]
    [SerializeField] private Cube _CubePrefab;
    [SerializeField] private int _Delay = 0;
    [SerializeField] private int _TicksBetweenSpawn = 1;
    [SerializeField] private int _NCubeToSpawn = 1;
    [SerializeField] private int _CubeSpeed = 1;

    [Header("Magic Visuals")]
    [SerializeField] private MeshRenderer _PlateRenderer;
    [SerializeField] private ParticleSystem _MagicVFX;   
    [Header("Color Settings")]
    [SerializeField] private bool _UseCustomColor = false;
    [SerializeField] private Color _LiquidColor = Color.white;

    private Vector3 _SpawnPosition;
    private int _CubesAlreadySpawn = 0;
    private int _TickSinceLastSpawn = 0;

    private Color _FinalColor;

    private float _ElapsedTime = 0f;
    private float _RealTickDuration = 1f;

    public static event Action onSpawn;

    protected override void Start()
    {
        base.Start();
        _SpawnPosition = transform.position;

        // Logique de jeu existante (Calcul du "crédit" de temps initial)
        _TickSinceLastSpawn = _TicksBetweenSpawn - _Delay;

        // Récupération de la durée réelle d'un tick
        _RealTickDuration = TickManager._DurationBetweenTicks;

        InitializeVisuals();
    }

    private void Update()
    {
        if (!GameStateManager.isActionPhase)
        {
            HandleReflectionPreview();
        }
    }

    protected override void OnTickEvent()
    {
        base.OnTickEvent();
        if(gameObject.activeInHierarchy)
        {
            if (_TickSinceLastSpawn >= _TicksBetweenSpawn && _CubesAlreadySpawn < _NCubeToSpawn)
            {
                Debug.Log("Spawn");
                SpawnCube();
                _CubesAlreadySpawn += 1;
                _TickSinceLastSpawn = 0;
                return;
            }

            if (_CubesAlreadySpawn < _NCubeToSpawn)
            {
                _TickSinceLastSpawn += 1;
            }
        }
    }

    private void InitializeVisuals()
    {
        if (_UseCustomColor)
        {
            _FinalColor = _LiquidColor;
        }
        else
        {
            _FinalColor = GetComponentInChildren<MeshRenderer>().material.color;
        }
            _PlateRenderer.material.color = _FinalColor;

        // 3. Appliquer aux particules magiques
        if (_MagicVFX != null)
        {
            var main = _MagicVFX.main;
            main.startColor = _FinalColor;
            float distanceCible = Cube.cubeSide;
            float dureeDuVol = 0.6f;

            main.startLifetime = dureeDuVol;
            main.startSpeed = distanceCible / dureeDuVol;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
        }
    }

    private void HandleReflectionPreview()
    {
        _ElapsedTime += Time.deltaTime;

        float totalInterval = _TicksBetweenSpawn * _RealTickDuration;

        if (_ElapsedTime - _Delay * _RealTickDuration >= totalInterval)
        {
            PlayMagicPulse();
            _ElapsedTime = 0;
        }
    }

    private void PlayMagicPulse()
    {
        if (_MagicVFX != null)
        {
            _MagicVFX.Play();
        }
    }

    private void SpawnCube()
    {
        Debug.Log($"SPAWN déclenché par : {gameObject.name} (Parent: {transform.parent.name}) à la position {transform.position}");
        Cube lCube = Instantiate(_CubePrefab).GetComponent<Cube>();
        lCube.transform.position = _SpawnPosition + Vector3.up * Cube.cubeSide * 0.5f;
        lCube.movementDirection = transform.forward;
        lCube.actionsPerTick = _CubeSpeed;
        lCube.SetColor(_FinalColor);

        PlayMagicPulse();

        AudioSignals.TriggerSound(AudioClipsEnum.CubeSpawn, transform.position);
        onSpawn?.Invoke();
    }

    public int GetTotalCubesToSpawn()
    {
        return _NCubeToSpawn;
    }
}